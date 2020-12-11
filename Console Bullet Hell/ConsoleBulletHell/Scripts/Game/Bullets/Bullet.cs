namespace Joonaxii.ConsoleBulletHell
{
    public class Bullet : Entity
    {
        public override int Damage { get => _owner != null ? base.Damage + _owner.Damage : base.Damage; protected set => base.Damage = value; }
        protected bool _overrideUpdate;

        protected Entity _owner;

        protected float _speed = 0.25f;
        protected float _lifeTime = 2.0f;

        private int _waitFrames;
        private int _currentWaitFrame;
        private CircleCollider _circle;

        public Bullet(EntityType type, Sprite sprt, int order, Animation anim, float radius, Vector2 colliderOffset) : base(EntityID.BULLET, type, 0, 0, true, order, sprt, anim, colliderOffset, new CircleCollider(Vector2.zero, radius))
        {
            _circle = Collider.CollisionShapes[0] as CircleCollider;
        }

        public override void Spawn(Vector2 pos, Vector2 dir)
        {
            _currentWaitFrame = 0;
            base.Spawn(pos, dir);
            _canCollide = !_overrideUpdate;
        }

        public override void Despawn(bool silent)
        {
            base.Despawn(silent);
        }

        public override void Update(float deltaTime)
        {
            if (_overrideUpdate) { return; }
            if (_isOutOfBounds) { Despawn(true); return; }
            base.Update(deltaTime);

            if(_currentWaitFrame < _waitFrames)
            {
                _currentWaitFrame++;
                return;
            }

            if (_lifeTime > 0 && _time >= _lifeTime)
            {
                Despawn(false);
                return;
            }

            Movement(deltaTime);
        }

        public virtual void Movement(float delta)
        {
            Position += Direction * delta * _speed;
        }

        public void SetupBullet(Entity owner, int damage, float radius, float speed, float lifeTime, int orderOffset, bool overrideUpdate = false, int waitFrames = 1)
        {
            _owner = owner;
            Damage = damage;

            _waitFrames = waitFrames;

            renderingOffset = orderOffset;
            _speed = speed;
            _lifeTime = lifeTime;

            _circle.SetRadius(radius);

            _overrideUpdate = overrideUpdate;

            if (_overrideUpdate)
            {
                _pool = null;
                _canCollide = false;
            }
        }
    }
}