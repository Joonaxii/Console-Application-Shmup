using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Joonaxii.ConsoleBulletHell
{
    public class Player : Entity
    {
        public static Vector2 DEFAULT_SPAWN_POSITION { get => new Vector2(-60, 0); }

        public static float MAX_Y = 19;
        public static float MIN_Y = -20;

        public static float MAX_X = 77;
        public static float MIN_X = -78;

        public override int CurrentHealth
        {
            get => base.CurrentHealth;
            set
            {
                base.CurrentHealth = value;
                Program.HealthBarManager.UpdatePlayerHealthBar(base.CurrentHealth, MaxHealth, CurrentLives, MaxLives, _specialNormalized);
            }
        }

        public int MaxLives { get; private set; } 
        public int CurrentLives { get; private set; }
        

        public long Score
        {
            get => _score;
            set
            {
                long prev = _score;
                _score = Math.Max(value, 0);

                if(prev != _score)
                {
                    long hi = HighScore[(int)Program.Difficulty];
                    Program.HealthBarManager.DrawScore(_score, Math.Max(hi, _score), hi < _score);
                }
            }
        }
        private long _score;
        public static long[] HighScore { get; private set; }

        private static string _highScoreSave;

        public float speed = 35.0f;
        public float fireRate = 0.1f;
        public float specialCooldown = 5.0f;
        public float SpecialMeter
        {
            get => _special;

            set
            {
                float specialPrev = _special;
                _special = value;

                if(_special == specialPrev) { return; }

                Program.HealthBarManager.UpdatePlayerHealthBar(base.CurrentHealth, MaxHealth, CurrentLives, MaxLives, _specialNormalized);
            }
        }

        private float _specialNormalized { get => Math.Min(_special / specialCooldown, 1.0f); }

        private float _special;

        private bool _canShoot;
        private float _shootTimer;
        private float _immunity;
        private float _immunityTimer;
        private float _flashingInterval = 0.065f;
        private bool _isAlt;

        private Entity _colliderVisual;

        private BulletPattern _patternMain;
        private BulletPattern _patternAlt;

        private BulletPattern _patternSpecial;

        private BulletPool _poolBullet;

        private EffectPool _death;
        private EffectPool _loseLife;
        private EffectPool _respawn;

        private bool _respawning;
        private Entity _gameOver;

        private PlayerMissilePool _poolMissiles;
        private float _timeNotHit;
        private float _timeNotHitMaxTime = 20.0f;
        private float _specialOverTimeMin = 0.00001f;
        private float _specialOverTimeMax = 0.25f;
        private int _cumulativeSocreReduction = 0;
        private int _cumulativeScoreReductionAmount = 50;

        public Player(Sprite sprt, int damage, int maxHp, float immunityTime = 1.0f, Animation anim = null) : base(EntityID.PLAYER, EntityType.PLAYER, damage, maxHp, false, 0, sprt, anim, Vector2.zero, new CircleCollider(Vector2.zero, 0.5f))
        {
            _highScoreSave = Path.Combine(System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Console Bullethell/Saves/");

            _poolMissiles = new PlayerMissilePool("Bullet_PLR", "Player_Special_Alt", 0.15f, 0.5f, -345.0f, 1.55f, 0.55f, false, SpriteBank.GetSprite("Bullet_0"), null, 900, 64);

            _immunity = immunityTime;
            _colliderVisual = new Entity(EntityID.PROP, EntityType.NONE, 0, 0, false, 0, SpriteBank.GetSprite("Player_Hit_Box"), null, Vector2.zero);

            Program.BulletPatternManager.TryGetPattern("Player_Main", out _patternMain);
            Program.BulletPatternManager.TryGetPattern("Player_Alt", out _patternAlt);
            Program.BulletPatternManager.TryGetPattern("Player_Special", out _patternSpecial);

            _gameOver = new Entity(EntityID.PROP, EntityType.NONE, 0, 0, false, 99999, SpriteBank.GetSprite("GameOver"), null, Vector2.zero);

            _death = Program.PoolManager.GetPool<EffectPool>("Player_Death");
            _loseLife = Program.PoolManager.GetPool<EffectPool>("Player_Life");
            _respawn = Program.PoolManager.GetPool<EffectPool>("Player_Respawn");

            _poolBullet = Program.PoolManager.GetPool<BulletPool>("Bullet_PLR");
        }

        public static bool ValidateScore(Difficulty diff, long score)
        {
            int difficulty = (int)diff;
            if (HighScore[difficulty] >= score) { return false; }
            HighScore[difficulty] = score;

            if (!Directory.Exists(_highScoreSave))
            {
                Directory.CreateDirectory(_highScoreSave);
            }

            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < HighScore.Length; i++)
            {
                sb.AppendLine(HighScore[i].ToString());
            }

            File.WriteAllText(Path.Combine(_highScoreSave, "High.score"), sb.ToString());
            return true;
        }

        public static void LoadHighScore()
        {
            string path = Path.Combine(_highScoreSave, "High.score");
            string[] strs = File.Exists(path) ? File.ReadAllLines(path) : new string[] { };

            if(HighScore == null || HighScore.Length != 4)
            {
                HighScore = new long[4];
            }

            for (int i = 0; i < HighScore.Length; i++)
            {
                if(i >= strs.Length)
                {
                    HighScore[i] = 0;
                    continue;
                }

                HighScore[i] = long.TryParse(strs[i].Trim(), out long val) ? val : 0;
            }
        }

        public void Setup(int maxHP, int damage, int maxLives)
        {
            MaxHealth = maxHP;
            Damage = damage;
            MaxLives = maxLives;
        }

        public override void Spawn(Vector2 pos, Vector2 dir)
        {
            _gameOver.Despawn(true);
            CurrentLives = MaxLives;
            base.Spawn(pos, dir);
            _colliderVisual.Spawn(pos, dir);
            _canShoot = true;
            _respawning = false;
            SpecialMeter = specialCooldown;
            _timeNotHit = 0;

            LoadHighScore();
            Score = 0;
            _cumulativeSocreReduction = 0;

            Program.HealthBarManager.DrawScore(Score, HighScore[(int)Program.Difficulty], false);
        }

        public override void Despawn(bool silent)
        {
            base.Despawn(silent);
            _colliderVisual.Despawn(silent);

            if (!silent)
            {
                Effect fx = _death?.Get() as Effect;
                fx?.Spawn(Position, Vector2.zero);
                fx?.Setup(0, 1000);
            }
        }

        public void Respawn()
        {
            Position = DEFAULT_SPAWN_POSITION;
            _colliderVisual.SetPosition(Position);

            _respawning = false;
            CurrentHealth = MaxHealth;
            _canShoot = true;

            CanCollide = true;
            StartImmunity();

            Effect fx = _respawn?.Get() as Effect;
            fx?.Spawn(Position, Vector2.zero);
            fx?.Setup(0, 1000);
        }

        public override void OnCollisionEnter(ICollideable other)
        {
            if (!_isImmune)
            {
                Entity ent = other.GetEntity();
                if(ent.Type == EntityType.ENEMY_MELEE) { return; }

                if (ent.Type != EntityType.WORLD & ent.Type != EntityType.ENEMY)
                {
                    ent.Despawn(false);
                }

                TakeDamage(ent.Damage);
            }
        }

        public override int TakeDamage(int damage)
        {
            switch (base.TakeDamage(damage))
            {
                case 2:
                    CurrentLives--;
                    _timeNotHit = 0;

                    switch (Program.Difficulty)
                    {
                        case Difficulty.EASY:
                            Score -= 1200 * damage;
                            break;
                        case Difficulty.MEDIUM:
                            Score -= 4800 * damage;
                            break;
                        case Difficulty.HARD:
                            Score -= 6400 * damage;
                            break;
                        case Difficulty.YOURE_DEAD:
                            Score -= 16000 * damage;
                            break;
                    }
                    if (CurrentLives < 0)
                    {
                        SpecialMeter = 0;
                        CurrentLives = 0;
                        CurrentHealth = 0;
                        _isVisible = false;
                        CanCollide = false;
                        _respawning = true;
                        _colliderVisual.Despawn(false);
                        StopCoroutine(_gameOverRoutine);
                        _gameOverRoutine = Gameovercreen();
                        StartCoroutine(_gameOverRoutine);
                    }
                    else
                    {
                        CurrentHealth = 0;
                        _isVisible = false;
                        CanCollide = false;
                        _isImmune = true;

                        Effect fx = _loseLife?.Get() as Effect;
                        fx?.Spawn(Position, Vector2.zero);
                        fx?.Setup(0, 1000);

                        StopCoroutine(_move);
                        _move = MoveToStart();
                        StartCoroutine(_move);

                    }
                    return 2;
                case 1:

                    switch (Program.Difficulty)
                    {
                        case Difficulty.EASY:
                            Score -= 120 * damage;
                            break;
                        case Difficulty.MEDIUM:
                            Score -= 480 * damage;
                            break;
                        case Difficulty.HARD:
                            Score -= 640 * damage;
                            break;
                        case Difficulty.YOURE_DEAD:
                            Score -= 1600 * damage;
                            break;
                    }

                    CurrentHealth += 0;
                    _timeNotHit = 0;
                    StartImmunity();
                    return 1;
            }
            return 0;
        }

        public override void Reset()
        {
            base.Reset();
            _immunityTimer = 0;
            _flashingTimer = 0;
            _isVisible = true;
        }

        private IEnumerator _move;
        private IEnumerator MoveToStart()
        {
            _respawning = true;
            yield return Program.UpdateTime.WaitForSeconds(0.5f);
            _respawnTimer = 0;

            Vector2 pos = Position;
            while (_respawnTimer < _respawnTime)
            {
                Position = Vector2.Lerp(pos, DEFAULT_SPAWN_POSITION, Maths.EaseOut(_respawnTimer / _respawnTime));
                _colliderVisual.Position = Position;
                _respawnTimer += Program.UpdateTime.DeltaTime;
                yield return null;
            }

            Respawn();
        }

        private float _respawnTimer = 0;
        private float _respawnTime = 1.0f;
        public override void Update(float deltaTime)
        {
            if (!IsAlive) { return; }
            base.Update(deltaTime);

            if (_respawning)
            {
                return;
            }

            Vector2 vec = new Vector2();
            if (Input.GetKey(KeyCode.Up))
            {
                vec.y -= 1;
            }

            if (Input.GetKey(KeyCode.Down))
            {
                vec.y += 1;
            }

            if (Input.GetKey(KeyCode.Right))
            {
                vec.x += 1;
            }

            if (Input.GetKey(KeyCode.Left))
            {
                vec.x -= 1;
            }

            _isAlt = Input.GetKey(KeyCode.X);

            Direction = vec;
            Position += Direction * deltaTime * (_isAlt ? speed * 0.5f : speed);

            Position.x = Maths.Clamp(Position.x, MIN_X, MAX_X);
            Position.y = Maths.Clamp(Position.y, MIN_Y, MAX_Y);

            HandleSpecial(deltaTime);
            HandleShooting(deltaTime);
            UpdateImmune(deltaTime);

            _colliderVisual.SetPosition(Position);
        }

        private void HandleSpecial(float delta)
        {
            _timeNotHit += delta;

            if(SpecialMeter < specialCooldown)
            {
                SpecialMeter += delta * Maths.Lerp(_specialOverTimeMin, _specialOverTimeMax,  Math.Min(_timeNotHit / _timeNotHitMaxTime, 1.0f));
                return;
            }

            if (Input.GetKeyDown(KeyCode.C))
            {
  
                SpecialMeter = 0;
                StopCoroutine(_specialRoutine);

                _specialRoutine = SpecialRoutine();
                StartCoroutine(_specialRoutine);
            }
        }

        private IEnumerator _specialRoutine;

        private IEnumerator SpecialRoutine()
        {
            Vector2 shootDir = Vector2.right;
            int multiplier = 10;
            switch (Program.Difficulty)
            {
                case Difficulty.MEDIUM:
                    multiplier = 5;
                    break;
                case Difficulty.HARD:
                    multiplier = 1;
                    break;
                case Difficulty.YOURE_DEAD:
                    multiplier = 0;
                    break;
            }

       
            int times = 10;
            float interval = 0.085f;
            for (int i = 0; i < times; i++)
            {
                Score -= _cumulativeSocreReduction;

                Vector2 point = Position + new Vector2(-3, 2.05f);
                _patternSpecial.SpawnBullets<Bullet>(this, true, point, shootDir.Rotate(-15.0f), _poolMissiles, 2, 2f, 1.25f, 8.5f, 5000);

                point = Position + new Vector2(-3, -2.05f);
                _patternSpecial.SpawnBullets<Bullet>(this, true, point, shootDir.Rotate(15.0f), _poolMissiles, 2, 2f, 1.25f, 8.5f, 5000);
                yield return Program.UpdateTime.WaitForSeconds(interval);
                _cumulativeSocreReduction += _cumulativeScoreReductionAmount * multiplier;
            }

        }

        private void HandleShooting(float delta)
        {
            _shootTimer += delta;
            _canShoot = _shootTimer >= fireRate ? true : _canShoot;
          
            if (_canShoot && (Input.GetKey(KeyCode.Z)))
            {
                Shoot(_isAlt);

                _canShoot = false;
                _shootTimer = 0;
                return;
            }
        }

        private void StartImmunity()
        {
            _isImmune = true;
            _isVisible = false;

            _immunityTimer = 0;
            _flashingTimer = 0;
        }

        private float _flashingTimer;
        private void UpdateImmune(float delta)
        {
            if (_isImmune)
            {
                _immunityTimer += delta;
                _flashingTimer += delta;

                if(_immunityTimer >= _immunity)
                {
                    _immunityTimer = 0;
                    _flashingTimer = 0;
                    _isImmune = false;
                    _isVisible = true;
                    return;
                }

                if(_flashingTimer >= _flashingInterval)
                {
                    _flashingTimer = _flashingTimer - _flashingInterval;
                    _isVisible = !_isVisible;
                }
            }          
        }

        private void Shoot(bool isAlt)
        {
            BulletPattern _pattern = isAlt ? _patternAlt : _patternMain;

            Vector2 shootDir = -Vector2.right;
            Vector2 point = Position + new Vector2(3, 2.05f);
            _pattern.SpawnBullets<Bullet>(this, false, point, shootDir, _poolBullet, 0, 4.0f, 1.0f, 3.5f, 5000);

            point = Position + new Vector2(3, -2.05f);
            _pattern.SpawnBullets<Bullet>(this, false, point, shootDir, _poolBullet, 0, 4.0f, 1.0f, 3.5f, 5000);
        }

        private IEnumerator _gameOverRoutine;
        private IEnumerator Gameovercreen()
        {
            Vector2 startPos = new Vector2(200, 0);
            Vector2 endPos = new Vector2(-5, 0);

            _gameOver.Spawn(startPos, Vector2.zero);
            yield return Program.UpdateTime.WaitForSeconds(2.0f);

            float t = 0;
            float duration = 0.5f;
            float delta;

            while (t < duration)
            {
                delta = Program.UpdateTime.DeltaTime;
                float n = t / duration;

                _gameOver.Update(delta);
                _gameOver.Position = Vector2.Lerp(startPos, endPos, Maths.SmoothStep(0.0f, 1.0f, n));
                t += delta;
                yield return null;
            }

            startPos = endPos;
            _gameOver.Position = startPos;
            endPos = new Vector2(5, 0);

            duration = 2.5f;
            t = 0;
            while (t < duration)
            {
                delta = Program.UpdateTime.DeltaTime;
                float n = t / duration;

                _gameOver.Update(delta);
                _gameOver.Position = Vector2.Lerp(startPos, endPos, n);
                t += delta;
                yield return null;
            }


            startPos = endPos;
            _gameOver.Position = startPos;
            endPos = new Vector2(-205, 0);

            duration = 2.0f;
            t = 0;

            float multiplier = 1.0f;
            while (t < duration)
            {
                delta = Program.UpdateTime.DeltaTime;
                float n = t / duration;

                _gameOver.Update(delta);
                _gameOver.Position = Vector2.Lerp(startPos, endPos, n);
                t += delta * multiplier;

                multiplier = Maths.Lerp(1.0f, 5.0f, n);
                yield return null;
            }

            _gameOver.Despawn(false);
            yield return Program.UpdateTime.WaitForSeconds(1.0f);
            Program.StopRun();
            Despawn(true);
        }

        private class PlayerMissilePool : BulletPool
        {
            private string _poolName;
            private BulletPattern _pattern;

            private float _activationD;
            private float _turningD;
            private float _angle;

            private float _spdActivate;
            private float _spdTurn;

            private bool _isX;
            private Sprite _sprt;
            private Animation _anim;
            private int _order;

            public PlayerMissilePool(string poolName, string pattern, float activationD, float turningD, float angle, float spdActivate, float spdTurn, bool isX, Sprite sprt, Animation anim, int order, int initialCount) : base(initialCount)
            {
                _poolName = poolName;
                Program.BulletPatternManager.TryGetPattern(pattern, out _pattern);
                _activationD = activationD;
                _turningD = turningD;
                _angle = angle;
                _spdActivate = spdActivate;
                _spdTurn = spdTurn;
                _isX = isX;
                _sprt = sprt;
                _anim = anim;
                _order = order;

                GenerateInitial(initialCount);
            }

            public override IPoolable GetNew()
            {
                PlayerMissile bullet = new PlayerMissile(_poolName, _activationD, _turningD, _angle, _spdActivate, _spdTurn, _isX, _pattern, EntityType.BULLET_PLR, _sprt, _order, _anim, 1.0f, Vector2.zero);
                bullet.Create(this);
                return bullet;
            }
        }
    }
}