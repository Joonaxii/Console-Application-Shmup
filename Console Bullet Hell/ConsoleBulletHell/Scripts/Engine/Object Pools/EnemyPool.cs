namespace Joonaxii.ConsoleBulletHell
{
    public class EnemyPool : ObjectPool
    {
        private int _maxHP;
        private int _damage;
        private int _order;

        private Sprite _sprt;
        private Animation _anim;

        private Vector2 _collOffset;

        private ColliderBase[] _colliders;

        private EffectPool _effect;


        public EnemyPool(int damage, int maxHP, int order, string sprt, string anim, string deathEffect, Vector2 colliderOffset, ColliderBase[] colliders, int initialCount) : base(initialCount)
        {
            _damage = damage;
            _maxHP = maxHP;
            _order = order;

            _sprt = SpriteBank.GetSprite(sprt);
            _anim = SpriteBank.GetAnimation(anim);

            _collOffset = colliderOffset;

            _colliders = colliders;

            _effect = Program.PoolManager.GetPool<EffectPool>(deathEffect);
            GenerateInitial(initialCount);
        }

        public override IPoolable GetNew()
        {
            ColliderBase[] colliders = _colliders == null ? null : new ColliderBase[_colliders.Length];
            for (int i = 0; i < colliders.Length; i++)
            {
                colliders[i] = _colliders[i].Clone();
            }
            return new Enemy(EntityID.ENEMY, EntityType.ENEMY, _effect, _damage, _maxHP, true, _order, _sprt, _sprt, _anim, _collOffset, colliders);
        }
    }
}