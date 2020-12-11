namespace Joonaxii.ConsoleBulletHell
{
    public class PlayerMissile : Bullet
    {
        private BulletPattern _patternToUse;
        private float _activationDuration;
        private float _turingDuration;
        private float _turingRotationAngle;

        private float _timeActivation;
        private float _timeTurning;
        private float _speedDuringActivation;
        private float _speedDuringTurning;
        private bool _isX;
        private BulletPool _pooler;

        private Bullet[] _bullets = new Bullet[0];
        private Vector2[] _dirs = new Vector2[0];
        private Vector2[] _poss = new Vector2[0];

        public PlayerMissile(string pool, float activationDuration, float turingDuration, float turingRotationAngle, float speedDuringActivation, float speedDuringTurning, bool isXAxis, BulletPattern pattern, EntityType type, Sprite sprt, int order, Animation anim, float radius, Vector2 colliderOffset) : base(type, sprt, order, anim, radius, colliderOffset)
        {
            _activationDuration = activationDuration;
            _turingDuration = turingDuration;

            _turingRotationAngle = turingRotationAngle;
            _speedDuringActivation = speedDuringActivation;
            _speedDuringTurning = speedDuringTurning;

            _isX = isXAxis;

            _pooler = Program.PoolManager.GetPool<BulletPool>(pool);
            _patternToUse = pattern;
        }

        private float _actualRotationDir;
        public override void Spawn(Vector2 pos, Vector2 dir)
        {
            if (IsAlive) { return; }
            base.Spawn(pos, dir);
            _timeActivation = 0;
            _timeTurning = 0;
            _actualRotationDir = _turingRotationAngle *  Maths.Sign(_isX ? dir.x : dir.y);
            _patternToUse.GetControllableBullets(_owner, Damage, renderingOffset, 0.15f, _lifeTime, _pooler, out _bullets, out _poss, out _dirs);
        }

        public override void Despawn(bool silent)
        {
            if (!IsAlive) { return; }
            for (int i = 0; i < _bullets.Length; i++)
            {
                _bullets[i].Despawn(silent);
            }
            _bullets = new Bullet[0];

            if (!silent)
            {
                _patternToUse.SpawnBullets<Bullet>(_owner, true, Position, Direction, _pooler, (int)(Damage * 0.25f), 0.05f, 1.5f, _lifeTime * 0.25f, renderingOffset + 10, 0.575f);
            }
            base.Despawn(silent);
        }

        public override void Movement(float delta)
        {
            if(_timeActivation < _activationDuration)
            {
                _timeActivation += delta;
                HandleMovement(delta, _speedDuringActivation);
                return;
            }

            if (_timeTurning < _turingDuration)
            {
                _timeTurning += delta;
                Direction = Direction.Rotate(_actualRotationDir * delta);
                HandleMovement(delta, _speedDuringTurning);
                return;
            }

            HandleMovement(delta, 1.0f);
        }

        private void HandleMovement(float delta, float spdMult)
        {
            Position += Direction * delta * _speed * spdMult;

            float angle = Vector2.up.SignedAngle(Direction);
            for (int i = 0; i < _bullets.Length; i++)
            {
                Vector2 p = _poss[i].Rotate(angle);
                Vector2 d = _dirs[i].Rotate(angle);

                //Make it longer
                p.x *= 1.6725f;
                d.x *= 1.6725f;

                //Make it less thinner
                p.y *= 0.6725f;
                d.y *= 0.6725f;

                _bullets[i].Position = Position + p;
                _bullets[i].Direction = d;
                _bullets[i].UpdateAnimation(delta);
            }
        }

    }
}