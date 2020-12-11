using System.Collections;
using System.Collections.Generic;

namespace Joonaxii.ConsoleBulletHell
{
    public class LevelManager
    {   
        public int LevelPhase { get; private set; }
        public Player Player { get; private set; }

        private MotherShip _boss;
        private Background _background;

        private Entity _level;

        private bool _active;

        public LevelManager()
        {
            _background = new Background();
            Player = new Player(SpriteBank.GetSprite("Player"), 10, 200, 1.0f, null);
            _boss = new MotherShip(EntityID.BOSS_1, EntityType.ENEMY, 10000, true, 2, new BoxCollider(new Vector2(5, 0), new Vector2(10, 20)), new BoxCollider(new Vector2(-0, -15), new Vector2(25, 10)), new BoxCollider(new Vector2(-0, 15), new Vector2(25, 10)));

            _level = new Entity(EntityID.NONE, EntityType.NONE, 0, 0, false, 0, null, null, Vector2.zero);
            _active = false;
        }

        public void Load()
        {
            LevelPhase = 0;
            _level.Spawn(Vector2.zero, Vector2.zero);

            _background.Start(2.5f);

            _level.StopCoroutine(_levelLoop);
            switch (Program.Difficulty)
            {
                case Difficulty.EASY:
                    Player.Setup(20, 4, 4);
                    _levelLoop = EasyDifficulty();
                    _boss.Setup(Program.PoolManager.SpawnObject<StateMachine>("Boss_1_E"), 10000, 1);
                    break;
                case Difficulty.MEDIUM:
                    Player.Setup(10, 3, 3);
                    _levelLoop = EasyDifficulty();
                    _boss.Setup(Program.PoolManager.SpawnObject<StateMachine>("Boss_1_E"), 12000, 1);
                    break;
                case Difficulty.HARD:
                    Player.Setup(5, 2, 2);
                    _levelLoop = EasyDifficulty();
                    _boss.Setup(Program.PoolManager.SpawnObject<StateMachine>("Boss_1_E"), 16000, 1);
                    break;
                case Difficulty.YOURE_DEAD:
                    Player.Setup(3, 2, 0);
                    _levelLoop = EasyDifficulty();
                    _boss.Setup(Program.PoolManager.SpawnObject<StateMachine>("Boss_1_E"), 22000, 1);
                    break;
            }

            Player.Spawn(Player.DEFAULT_SPAWN_POSITION, Vector2.zero);
            _level.StartCoroutine(_levelLoop);

            _active = true;
        }

        private IEnumerator _levelLoop;
        public int Update(float delta)
        {
            if (!_active) { return 0; }
            _background.Update(delta);
            Player.Update(delta);

            if (_level.UpdateCoroutines()) { return 0; }
            return 2;
        }

        public void UnLoad()
        {
            _active = false;
            _background.Stop();
            _level.Despawn(true);
            Player.Despawn(true);

            if (_boss.IsAlive)
            {
                _boss.Despawn(true);
            }
        }

        private IEnumerator EasyDifficulty()
        {
            EnemyPool _enemyNorm = Program.PoolManager.GetPool<EnemyPool>("Enemy_1");

            StateMachinePool poolNmy1Up = Program.PoolManager.GetPool<StateMachinePool>("Enemy_1_1_Up");
            StateMachinePool poolNmy1Down = Program.PoolManager.GetPool<StateMachinePool>("Enemy_1_1_Down");


            StateMachinePool poolNmy2Up = Program.PoolManager.GetPool<StateMachinePool>("Enemy_1_Sine_Up");
            StateMachinePool poolNmy2Down = Program.PoolManager.GetPool<StateMachinePool>("Enemy_1_Sine_Down");


            StateMachinePool poolNmyFinalUp = Program.PoolManager.GetPool<StateMachinePool>("Enemy_1_Final_Up");
            StateMachinePool poolNmyFinalDown = Program.PoolManager.GetPool<StateMachinePool>("Enemy_1_Final_Down");

            yield return Program.UpdateTime.WaitForSeconds(2.0f);
            float time = 0;
            float spawnInterval = 0.15f;

            Enemy nmy;

            int squareX = 4;
            int squareY = 2;

            List<Enemy> enemies = new List<Enemy>();
            List<Vector2> positions = new List<Vector2>();

            for (int y = 0; y < squareY; y++)
            {
                for (int x = 0; x < squareX; x++)
                {
                    Vector2 pos = new Vector2(80 + x * 24.0f, (y + 0.65f) * 8.0f);

                    nmy = _enemyNorm.Get() as Enemy;
                    nmy.Setup(poolNmy1Up.Get() as StateMachine, 40, 1);
                    nmy.renderingOffset = 200 + (y + x);
                    enemies.Add(nmy);
                    positions.Add(pos);

                    pos.y = -pos.y;
                    nmy = _enemyNorm.Get() as Enemy;
                    nmy.Setup(poolNmy1Down.Get() as StateMachine, 40, 1);
                    nmy.renderingOffset = 200 - (y + x);
                    enemies.Add(nmy);
                    positions.Add(pos);

                }
            }

            for (int i = 0; i < enemies.Count; i+=2)
            {
                enemies[i].Spawn(positions[i], Vector2.zero);
                enemies[i+1].Spawn(positions[i+1], Vector2.zero);
                yield return Program.UpdateTime.WaitForSeconds(spawnInterval);
            }

            while (true)
            {
                bool breakThis = true;
                for (int i = enemies.Count - 1; i >= 0; i--)
                {
                    if (enemies[i].IsAlive) { breakThis = false; break; }
                }

                if (breakThis) { break; }
                yield return null;
            }

            enemies.Clear();
            positions.Clear();

            yield return Program.UpdateTime.WaitForSeconds(1.0f);

            spawnInterval = 0.25f;

            squareY *= 2;
            squareX *= 2;

            for (int y = 0; y < squareY; y++)
            {
                for (int x = 0; x < squareX; x++)
                {
                    Vector2 pos = new Vector2(80 + x * 8.0f, (y + 0.65f) * 4.0f);

                    nmy = _enemyNorm.Get() as Enemy;
                    nmy.Setup(poolNmy2Up.Get() as StateMachine, 40, 1);
                    nmy.renderingOffset = 200 + (y + x);
                    enemies.Add(nmy);
                    positions.Add(pos);

                    pos.y = -pos.y;
                    nmy = _enemyNorm.Get() as Enemy;
                    nmy.Setup(poolNmy2Down.Get() as StateMachine, 40, 1);
                    nmy.renderingOffset = 200 - (y + x);
                    enemies.Add(nmy);
                    positions.Add(pos);

                }
            }

            for (int i = 0; i < enemies.Count; i += 2)
            {
                enemies[i].Spawn(positions[i], Vector2.zero);
                enemies[i + 1].Spawn(positions[i + 1], Vector2.zero);
                yield return Program.UpdateTime.WaitForSeconds(spawnInterval);
            }

            while (true)
            {
                bool breakThis = true;
                for (int i = enemies.Count - 1; i >= 0; i--)
                {
                    if (enemies[i].IsAlive) { breakThis = false; break; }
                }

                if (breakThis) { break; }
                yield return null;
            }

            enemies.Clear();
            positions.Clear();

            yield return Program.UpdateTime.WaitForSeconds(1.0f);


            //Final Wave
            time = 0;
            float attackInterval = 0.05f;
            float attackFrequency = 2.0f;
            float attackAmplitudeY = 8;
            float attackAmplitudeX = 24f;
            int enemyCount = 16;
            int countS = 0;

            while(countS <= enemyCount)
            {
                if(time < attackInterval)
                {
                    time += Program.UpdateTime.DeltaTime;
                    yield return null;
                    continue;
                }

                float nn = (countS / (float)enemyCount) * attackFrequency * Maths.PI;
                float x = Maths.Cos(nn * 0.5f) * attackAmplitudeX * 2;
                float y = Maths.Sin(nn) * attackAmplitudeY;

                Vector2 pos = new Vector2(100 + x, y);

                nmy = _enemyNorm.Get() as Enemy;
                nmy.Setup((pos. y < 0 ? poolNmyFinalDown.Get() : poolNmyFinalUp.Get()) as StateMachine, 20, 1);
                nmy.renderingOffset = 200 + (int)y;
                nmy.Spawn(pos, Vector2.zero);
                enemies.Add(nmy);

                pos = new Vector2(100 + x, -y);

                nmy = _enemyNorm.Get() as Enemy;
                nmy.Setup((pos. y < 0 ? poolNmyFinalDown.Get() : poolNmyFinalUp.Get()) as StateMachine, 20, 1);
                nmy.renderingOffset = 200 + (int)y;
                nmy.Spawn(pos, Vector2.zero);
                enemies.Add(nmy);

                countS++;
                time = 0;
                yield return null;
            }

            while (true)
            {
                bool breakThis = true;
                for (int i = enemies.Count - 1; i >= 0; i--)
                {
                    if (enemies[i].IsAlive) { breakThis = false; break; }
                }

                if (breakThis) { break; }
                yield return null;
            }

            enemies.Clear();

            yield return Program.UpdateTime.WaitForSeconds(1.0f);
            _boss.Spawn(new Vector2(100, 0), Vector2.zero);
            yield return Program.UpdateTime.WaitForSeconds(13.0f);
            _background.StartScrollSpeedFade(0.35f, 3.0f, FadingType.FADE_IN);

            while (true)
            {
                if (!_boss.IsAlive) { break; }
                yield return null;
            }
        }
    }
}