using System;
using System.Collections;

namespace Joonaxii.ConsoleBulletHell
{
    public class MotherShip : Boss
    {
        public bool followPlayer = false;

        public int CurrentAttack { get; set; }

        private Entity _warning;
        private Laser[] _lasers;
        private Vector2[] _laserOffsets = new Vector2[3] { new Vector2(0f, -0), new Vector2(-0f, -17), new Vector2(-0f, 17) };

        private float _smoothTime = 10;
        private BulletPool _smallBullet;

        private Animation _animWarn;
        private Animation _animWon;
        private Animation _animWonHI;

        public MotherShip(EntityID id, EntityType type, int maxHp, bool autoUpdate, int order, params ColliderBase[] colliders) : base(id, type, 0, maxHp, autoUpdate, order, SpriteBank.GetSprite("Boss_1_Damage"), SpriteBank.GetSprite("Boss_1"), null, Vector2.zero, colliders)
        {
            _animWarn = SpriteBank.GetAnimation("Boss Warning");

            _animWon = SpriteBank.GetAnimation("Won");
            _animWonHI = SpriteBank.GetAnimation("HI-Won");

            _warning = new Entity(EntityID.PROP, EntityType.NONE, 0, 0, false, 99999, null, _animWarn, Vector2.zero);
            _lasers = new Laser[3]
            {
                new Laser(this, 800),
                new Laser(this, 800),
                new Laser(this, 800),
            };

            _smallBullet = Program.PoolManager.GetPool<BulletPool>("Bullet_NMY");
            Sprite = _defaultSprite;
        }

        public override void Despawn(bool silent)
        {
            StopLaser();
            StateMachine.Stop();
            base.Despawn(silent);

            StopCoroutine(_warningRoutine);
            _warning.Despawn(silent);

            for (int i = 0; i < _lasers.Length; i++)
            {
                _lasers[i].Despawn(silent);
            }

            Player.MAX_X = 77;
        }

        public override void Spawn(Vector2 pos, Vector2 dir)
        {
            base.Spawn(pos, dir);
        }

        public void StartIntroVS()
        {
            StopCoroutine(_warningRoutine);
            _warningRoutine = WarningScreen(_animWarn);
            StartCoroutine(_warningRoutine);
        }

        private Vector2 _velocity;
        public override void Update(float deltaTime)
        {
            base.Update(deltaTime);

            if (_isDying) { return; }
            if (followPlayer)
            {
                float plrPos = Maths.InverseLerp(Player.MIN_Y, Player.MAX_Y, Program.Player.Position.y);
                Position = Vector2.SmoothDamp(Position, new Vector2(55, Maths.Lerp(-6, 6, plrPos)), ref _velocity, _smoothTime, deltaTime);
            }

            Player.MAX_X = CanCollide ? Position.x - 20.0f : 77;
            UpdateLasers(deltaTime);
        }

        protected override IEnumerator DyingSequence()
        {
            for (int i = 0; i < _lasers.Length; i++)
            {
                _lasers[i].Despawn(true);
            }

            StopCoroutine(_bulletPatterns);
            CanCollide = false;
            Player.MAX_X = 77;

            int maxExplosions = 48;
            int explosionsPerTickMin = 4;
            int explosionsPerTickMax = 16;
            float explosionInterval = 0.125f;
            float explosionIntervalTo = 0.0015f;

            Vector2 min = Collider.bounds.min - Vector2.one * 10f;
            Vector2 max = Collider.bounds.max + Vector2.one * 10f;

            Random rng = new Random();
            for (int i = 0; i < maxExplosions; i++)
            {
                for (int j = 0; j < rng.Next(explosionsPerTickMin, explosionsPerTickMax + 1); j++)
                {
                    Effect fx = _deathFX.Get() as Effect;
                    fx.Spawn(new Vector2(Maths.Lerp(min.x, max.x, (float)rng.NextDouble()), Maths.Lerp(min.y, max.y, (float)rng.NextDouble())), Vector2.zero);
                    fx.Setup(0, 1000 + i + j, 1.5f, true);
                }
                yield return Program.UpdateTime.WaitForSeconds(Maths.Lerp(explosionInterval, explosionIntervalTo, Maths.EaseIn(i / (maxExplosions - 1.0f))));
            }

            for (int j = 0; j < 48; j++)
            {
                Effect fx = _deathFX.Get() as Effect;
                fx.Spawn(new Vector2(Maths.Lerp(min.x, max.x, (float)rng.NextDouble()), Maths.Lerp(min.y, max.y, (float)rng.NextDouble())), Vector2.zero);
                fx.Setup(0, 1500 + j, 1.25f, true);
            }

            _isVisible = false;
            yield return Program.UpdateTime.WaitForSeconds(2.0f);

            bool newHigh = Player.ValidateScore(Program.Difficulty, Program.Player.Score);

            yield return WarningScreen(newHigh ? _animWonHI : _animWon);
            yield return Program.UpdateTime.WaitForSeconds(2.0f);
            Despawn(true);
        }

        private IEnumerator _warningRoutine;
        private IEnumerator WarningScreen(Animation anim)
        {
            Vector2 startPos = new Vector2(0, -80);
            Vector2 endPos = new Vector2(0, -2);

            _warning.Animation = anim;
            _warning.Spawn(startPos, Vector2.zero);
            yield return Program.UpdateTime.WaitForSeconds(2.0f);

            float t = 0;
            float duration = 0.5f;
            float delta;

            while (t < duration)
            {
                delta = Program.UpdateTime.DeltaTime;
                float n = t / duration;

                _warning.Update(delta);
                _warning.Position = Vector2.Lerp(startPos, endPos, Maths.SmoothStep(0.0f, 1.0f, n));
                t += delta;
                yield return null;
            }

            startPos = endPos;
            _warning.Position = startPos;
            endPos = new Vector2(0, 10);

            duration = 2.5f;
            t = 0;
            while (t < duration)
            {
                delta = Program.UpdateTime.DeltaTime;
                float n = t / duration;

                _warning.Update(delta);
                _warning.Position = Vector2.Lerp(startPos, endPos, n);
                t += delta;
                yield return null;
            }


            startPos = endPos;
            _warning.Position = startPos;
            endPos = new Vector2(0, 90);

            duration = 2.0f;
            t = 0;

            float multiplier = 1.0f;
            while (t < duration)
            {
                delta = Program.UpdateTime.DeltaTime;
                float n = t / duration;

                _warning.Update(delta);
                _warning.Position = Vector2.Lerp(startPos, endPos, n);
                t += delta * multiplier;

                multiplier = Maths.Lerp(1.0f, 5.0f, n);
                yield return null;
            }

            _warning.Despawn(false);
        }

        private void UpdateLasers(float deltaTime)
        {
            for (int i = 0; i < _lasers.Length; i++)
            {
                _lasers[i].Position = Position + _laserOffsets[i];
                _lasers[i].Update(deltaTime);
            }
        }

        private IEnumerator _bulletPatterns;
        private IEnumerator LaserBullets()
        {
            float interval;
            float intervalHalf;
            int count;
            switch (CurrentAttack)
            {
                case 0:
                    Program.BulletPatternManager.TryGetPattern("Boss_Laser_Up_Easy", out BulletPattern patternUp);
                    Program.BulletPatternManager.TryGetPattern("Boss_Laser_Up_Easy_Rev", out BulletPattern patternDown);

                    yield return Program.UpdateTime.WaitForSeconds(1.0f);

                    count = 6;

                    interval = 0.35f;
                    intervalHalf = interval * 0.5f;

                    bool revUp = false;
                    bool revDown = false;

                    int upC = 0;
                    int downC = 0;

                    float offsetPerLoop = (1.0f / count) * 2.0f;
                    float offsetUp = 0;
                    float offsetDown = offsetPerLoop;

                    float angleMax = 15.0f;
                    float frequency = 0.5f;

                    Vector2 dirUp = Vector2.up;
                    Vector2 dirDown = -Vector2.up;

                    while (true)
                    {
                        Vector2 root = _laserOffsets[0] + Position;
                        root.x -= 32;
                        float sin = Maths.Sin((Program.UpdateTime.RunTime / frequency) * Maths.PI) * angleMax;

                        Vector2 from = root;
                        Vector2 target = new Vector2(-70, root.y);
                        float n = upC / (float)count;
                        patternUp?.SpawnBullets<Bullet>(this, true, Vector2.Lerp(from, target, n) + new Vector2(offsetUp, 0), dirUp.Rotate(sin), _smallBullet, 1, 0.075f, 1, 3.5f, 50);

                        upC += (revUp ? -1 : 1);
                        if (revUp ? upC <= 0 : upC >= count)
                        {
                            revUp = !revUp;
                            if (!revUp)
                            {
                                offsetUp += offsetPerLoop;
                            }
                        }
                        yield return Program.UpdateTime.WaitForSeconds(intervalHalf);

                        root = _laserOffsets[0] + Position;
                        n = downC / (float)count;
                        patternDown?.SpawnBullets<Bullet>(this, true, Vector2.Lerp(from, target, n) + new Vector2(offsetDown, 0), dirDown.Rotate(-sin), _smallBullet, 1, 0.075f, 1, 3.5f, 50);

                        downC += (revDown ? -1 : 1);
                        if (revDown ? downC <= 0 : downC >= count)
                        {
                            revDown = !revDown;
                            if (!revDown)
                            {
                                offsetDown += offsetPerLoop;
                            }
                        }
                        yield return Program.UpdateTime.WaitForSeconds(intervalHalf);
                    }
                case 2:
                    Program.BulletPatternManager.TryGetPattern("Boss_Attack_Laser_2", out patternUp);
                    //Program.BulletPatternManager.TryGetPattern("Boss_Laser_Up_Easy_Rev", out patternDown);

                    yield return Program.UpdateTime.WaitForSeconds(1.0f);

                    count = 3;

                    interval = 0.15f;

                    intervalHalf = interval * 0.75f;

                    revUp = false;
                    revDown = false;

                    upC = 0;
                    downC = 0;

                    offsetPerLoop = 1.25f;
                    offsetUp = 0;
                    offsetDown = offsetPerLoop;

                    frequency = 0.25f;

                    dirUp = Vector2.up;
                    dirDown = -Vector2.up;

                    while (true)
                    {
                        Vector2 root = _laserOffsets[1] + Position;
                        root.x -= 32;

                        Vector2 from = root;
                        Vector2 target = new Vector2(-70, root.y);
                        float n = upC / (float)count;
                        patternUp?.SpawnBullets<Bullet>(this, true, Vector2.Lerp(from, target, n) + new Vector2(offsetUp, 0), dirUp, _smallBullet, 1, 0.075f, 1, 3.5f, 50, 1);

                        upC += (revUp ? -1 : 1);
                        if (revUp ? upC <= 0 : upC >= count)
                        {
                            revUp = !revUp;
                            offsetUp += offsetPerLoop;
                            offsetUp = offsetUp > 15 ? -5.0f : offsetUp;
                        }

                        yield return Program.UpdateTime.WaitForSeconds(intervalHalf);
                        from = root = _laserOffsets[2] + Position;
                        target = new Vector2(-70, root.y);
                        n = 1.0f - (downC / (float)count);
                        patternUp?.SpawnBullets<Bullet>(this, true, Vector2.Lerp(from, target, n) + new Vector2(offsetDown, 0), dirDown, _smallBullet, 1, 0.075f, 1, 3.5f, 50, 1);
                        downC += (revDown ? -1 : 1);
                        if (revDown ? downC <= 0 : downC >= count)
                        {
                            revDown = !revDown;
                            offsetDown -= offsetPerLoop;
                            offsetDown = offsetDown < 15 ? 5.0f : offsetDown;
                        }
                        yield return Program.UpdateTime.WaitForSeconds(intervalHalf);
                    }

                case 1:
                case 3:

                    Program.BulletPatternManager.TryGetPattern("Boss_Attack_Laser_2", out patternUp);
                    //Program.BulletPatternManager.TryGetPattern("Boss_Laser_Up_Easy_Rev", out patternDown);

                    yield return Program.UpdateTime.WaitForSeconds(1.0f);

                    count = CurrentAttack == 3 ? 10 : 8;

                    interval = 0.4f;

                    if (CurrentAttack == 3)
                    {
                        interval *= 0.35f;
                    }

                    intervalHalf = interval * 0.25f;

                    revUp = false;
                    revDown = false;

                    upC = 0;
                    downC = 0;

                    offsetPerLoop = 2.25f;
                    offsetUp = 0;
                    offsetDown = offsetPerLoop;

                    angleMax = 25.0f;
                    frequency = 0.25f;

                    dirUp = Vector2.up;
                    dirDown = -Vector2.up;

                    while (true)
                    {
                        Vector2 root = _laserOffsets[1] + Position;
                        root.x -= 32;

                        Vector2 from = root;
                        Vector2 target = new Vector2(-70, root.y);
                        float n = upC / (float)count;
                        patternUp?.SpawnBullets<Bullet>(this, true, Vector2.Lerp(from, target, n) + new Vector2(offsetUp, 0), dirUp, _smallBullet, 1, 0.075f, 1, 3.5f, 50, 1);

                        upC += (revUp ? -1 : 1);
                        if (revUp ? upC <= 0 : upC >= count)
                        {
                            revUp = !revUp;
                            offsetUp += offsetPerLoop;
                            offsetUp = offsetUp > 15 ? -5.0f : offsetUp;
                        }

                        yield return Program.UpdateTime.WaitForSeconds(intervalHalf);
                        from = root = _laserOffsets[2] + Position;
                        target = new Vector2(-70, root.y);
                        n = 1.0f - (downC / (float)count);
                        patternUp?.SpawnBullets<Bullet>(this, true, Vector2.Lerp(from, target, n) + new Vector2(offsetDown, 0), dirDown, _smallBullet, 1, 0.075f, 1, 3.5f, 50, 1);
                        downC += (revDown ? -1 : 1);
                        if (revDown ? downC <= 0 : downC >= count)
                        {
                            revDown = !revDown;
                            offsetDown -= offsetPerLoop;
                            offsetDown = offsetDown < 15 ? 5.0f : offsetDown;
                        }
                        yield return Program.UpdateTime.WaitForSeconds(intervalHalf);
                    }
            }

        }

        public void StartLaser()
        {
            StopCoroutine(_bulletPatterns);
            switch (CurrentAttack)
            {
                case 0:
                    _lasers[0].Spawn(Position + _laserOffsets[0], -Vector2.right);
                    _bulletPatterns = LaserBullets();
                    StartCoroutine(_bulletPatterns);
                    break;
                case 1:
                case 3:

                    for (int i = 1; i < _lasers.Length; i++)
                    {
                        _lasers[i].Spawn(Position + _laserOffsets[i], -Vector2.right);
                    }
                    _bulletPatterns = LaserBullets();
                    StartCoroutine(_bulletPatterns);
                    break;

                case 2:
                    for (int i = 0; i < _lasers.Length; i++)
                    {
                        _lasers[i].Spawn(Position + _laserOffsets[i], -Vector2.right);
                    }
                    _bulletPatterns = LaserBullets();
                    StartCoroutine(_bulletPatterns);
                    break;
            }
        }

        public void StopLaser()
        {
            StopCoroutine(_bulletPatterns);
            for (int i = 0; i < _lasers.Length; i++)
            {
                _lasers[i].Stop();
            }
        }
    }
}