namespace Joonaxii.ConsoleBulletHell
{
    public class PatternInstance
    {
        private float _waitTime;

        private int _order;
        private float _lifeTime;
        private float _timeOffset;
        private float _speedModifier;
        private float _bulletSize;
        private float _directionLerp;

        private Waypoint _point;
        private Vector2 _direction;
        private bool _directionToPlayer;

        private string _poolName;
        private string _patternName;

        private BulletPool _pool;
        private BulletPattern _pattern;

        private float _time;
        private float _timeWave;
        private bool _complete;

        private int _damage;
        private bool _correctY;
        private Entity _owner;
        private float _rotation;
        private WaveModifier _rotationPerTick;

        public PatternInstance(bool correctY, int damage, float timeOffset, float waitTime, float lifeTime, string pattern, string pool, int order, float speedMod, float bulletSize, Waypoint point, Vector2 direction, bool directionToPlayer, float rotationPerTick, float directionLerp = 0.0f) : this(correctY, damage, timeOffset, waitTime, lifeTime, pattern, pool, order, speedMod, bulletSize, point, direction, directionToPlayer, new WaveModifier(FadingType.LINEAR, WaveType.LINEAR, 1, rotationPerTick, 0 ,0), directionLerp) { }

        public PatternInstance(bool correctY, int damage, float timeOffset, float waitTime, float lifeTime, string pattern, string pool, int order, float speedMod, float bulletSize, Waypoint point, Vector2 direction, bool directionToPlayer, WaveModifier rotationPerTick = null, float directionLerp = 0.0f)
        {
            _rotationPerTick = rotationPerTick == null ? WaveModifier.zero : rotationPerTick;

            _directionLerp = directionLerp;

            _correctY = correctY & !directionToPlayer;
            _damage = damage;
            _waitTime = waitTime;
            _timeOffset = timeOffset;

            _lifeTime = lifeTime;

            _patternName = pattern;
            _poolName = pool;

            _order = order;

            _speedModifier = speedMod;
            _bulletSize = bulletSize;

            _point = point;

            _direction = direction;
            _directionToPlayer = directionToPlayer;
        }

        public void Load(Entity owner)
        {
            _owner = owner;
            _pool = Program.PoolManager.GetPool<BulletPool>(_poolName);
            Program.BulletPatternManager.TryGetPattern(_patternName, out _pattern);
        }

        public void Clear()
        {
            _rotation = 0;
            _timeWave = 0;
        }

        public void Trigger(Vector2 pos)
        {
            _point.BakePosition(pos);
            Vector2 direction = _directionToPlayer ? (_point.GetPosition() - Program.Player.Position).Normalized : _direction;
            direction = direction.Rotate(_rotation);

            if (_directionToPlayer)
            {
                direction.y = -direction.y;
                direction *= _direction;
            }

            _pattern.SpawnBullets<Bullet>(_owner, _correctY, _point.GetPosition(), direction, _pool, _damage, _bulletSize, _speedModifier, _lifeTime, _order, _directionLerp);
            _complete = true;
        }

        public void Reset()
        {
            _time = _timeOffset;
            _complete = false;
        }

        public bool Update(float deltaTime, Vector2 pos)
        {
            if (_complete) { return true; }
            _time += deltaTime;

            _rotation = _rotationPerTick.Update(_timeWave, deltaTime);
            _timeWave += deltaTime;

            if (_time >= _waitTime)
            {
                Trigger(pos);
                return true;
            }
            return false;
        }

        public PatternInstance Clone()
        {
            return new PatternInstance(_correctY, _damage, _timeOffset, _waitTime, _lifeTime, _patternName, _poolName, _order, _speedModifier, _bulletSize, _point, _direction, _directionToPlayer, _rotationPerTick.Clone(), _directionLerp);
        }
    }
}