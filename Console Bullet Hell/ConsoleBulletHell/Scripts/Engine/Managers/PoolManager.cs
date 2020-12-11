using System.Collections.Generic;
using System.Linq;

namespace Joonaxii.ConsoleBulletHell
{
    public class PoolManager
    {
        private Dictionary<string, ObjectPool> _pools;
        public void Load()
        {
            _pools = new Dictionary<string, ObjectPool>();
            _pools.Add("Bullet_PLR", new BulletPool(EntityType.BULLET_PLR, 500, 0.01f, Vector2.zero, "Bullet_0", string.Empty, CreateNewDefaultBullet));

            _pools.Add("Bullet_NMY", new BulletPool(EntityType.BULLET_NMY, 800, 0.05f, Vector2.zero, "Bullet_0", string.Empty, CreateNewDefaultBullet));
            _pools.Add("Bullet_Small", new BulletPool(EntityType.BULLET_NMY, 800, 0.05f, Vector2.zero, "Bullet_Small", string.Empty, CreateNewDefaultBullet));
            _pools.Add("Bullet_Medium", new BulletPool(EntityType.BULLET_NMY, 512, 1.5f, Vector2.zero, "Bullet_Medium", string.Empty, CreateNewDefaultBullet));
            _pools.Add("Bullet_Big", new BulletPool(EntityType.BULLET_NMY, 512, 4, Vector2.zero, string.Empty, "Bullet_Big", CreateNewDefaultBullet));

            _pools.Add("Boss_1_E", new StateMachinePool(StateMachinePool.GetEntityStateMachine(EntityID.BOSS_1, Difficulty.EASY), 1));
            _pools.Add("Boss_1_M", new StateMachinePool(StateMachinePool.GetEntityStateMachine(EntityID.BOSS_1, Difficulty.EASY), 1));
            _pools.Add("Boss_1_H", new StateMachinePool(StateMachinePool.GetEntityStateMachine(EntityID.BOSS_1, Difficulty.EASY), 1));
            _pools.Add("Boss_1_DED", new StateMachinePool(StateMachinePool.GetEntityStateMachine(EntityID.BOSS_1, Difficulty.EASY), 1));

            _pools.Add("Enemy_1_1_Up", new StateMachinePool(StateMachinePool.GetEntityStateMachine(EntityID.ENEMY, Difficulty.EASY, 0), 1));
            _pools.Add("Enemy_1_1_Down", new StateMachinePool(StateMachinePool.GetEntityStateMachine(EntityID.ENEMY, Difficulty.EASY, 1), 1));

            _pools.Add("Enemy_1_Sine_Up", new StateMachinePool(StateMachinePool.GetEntityStateMachine(EntityID.ENEMY, Difficulty.EASY, 2), 1));
            _pools.Add("Enemy_1_Sine_Down", new StateMachinePool(StateMachinePool.GetEntityStateMachine(EntityID.ENEMY, Difficulty.EASY, 3), 1));

            _pools.Add("Enemy_1_Final_Up", new StateMachinePool(StateMachinePool.GetEntityStateMachine(EntityID.ENEMY, Difficulty.EASY, 998), 1));
            _pools.Add("Enemy_1_Final_Down", new StateMachinePool(StateMachinePool.GetEntityStateMachine(EntityID.ENEMY, Difficulty.EASY, 999), 1));


            _pools.Add("Player_Life", new EffectPool(500, "Player_Life", 4));
            _pools.Add("Player_Death", new EffectPool(500, "Player_Death", 4));
            _pools.Add("Player_Respawn", new EffectPool(500, "Player_Respawn", 4));

            _pools.Add("Enemy_1", new EnemyPool(1, 5, 400, "Enemy_1", "", "Player_Death", Vector2.zero, new ColliderBase[] { new CircleCollider(Vector2.zero, 5.5f) }, 16));
        }

        public T GetPool<T>(string name) where T : ObjectPool
        {
            return _pools.TryGetValue(name, out ObjectPool pool) ? pool as T : null;
        }

        public T SpawnObject<T>(string poolName) where T : IPoolable
        {
            ObjectPool pool = GetPool<ObjectPool>(poolName);
            return pool != null ? (T)pool.Get(): default(T);
        }

        private Bullet CreateNewDefaultBullet(EntityType type, Sprite sprt, Animation anim, float rad, Vector2 colliderOffset)
        {
            return new Bullet(type, sprt, 100, anim, rad, colliderOffset);
        }

    }
}