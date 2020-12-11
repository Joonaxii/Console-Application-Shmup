using System;

namespace Joonaxii.ConsoleBulletHell
{
    public class BulletPool : ObjectPool
    {
        private EntityType _type;
        private Sprite _sprt;
        private Animation _animation;
        private Vector2 _collOffset;
        private float _radius;

        private Func<EntityType, Sprite, Animation, float, Vector2, Bullet> _createNewBullet;
 
        public BulletPool(int initialCount) : base(initialCount) { }

        public BulletPool(EntityType type, int initialCount, float radius, Vector2 colliderOffset, string sprite, string animation,  Func<EntityType, Sprite, Animation, float, Vector2, Bullet> bullet) : base(initialCount)
        {
            _createNewBullet = bullet;
            _radius = radius;
            _type = type;

            _collOffset = colliderOffset;

            _sprt = SpriteBank.GetSprite(sprite);
            _animation = SpriteBank.GetAnimation(animation);
            GenerateInitial(initialCount);
        }

      
        public override IPoolable GetNew()
        {
            Bullet bullet =  _createNewBullet.Invoke(_type, _sprt, _animation, _radius, _collOffset);
            bullet.Create(this);
            return bullet; 
        }
    }
}