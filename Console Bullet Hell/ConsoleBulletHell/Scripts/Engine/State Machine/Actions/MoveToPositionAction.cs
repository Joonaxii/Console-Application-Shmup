namespace Joonaxii.ConsoleBulletHell
{
    public class MoveToPositionAction : StateAction
    {
        private Waypoint[] _points;
     
        private bool _smooth;
        private bool _relative;
        private float _duration;
        private float _smoothingTime;
        private TranslationType _type;
        private Vector2 _initialPosition;
        private Vector2 _velocity;

        private WaveModifier[] _modifiers;

        public MoveToPositionAction(Waypoint targetPosition) : base(false, false)
        {
            _type = TranslationType.SET;
            _points = new Waypoint[] { targetPosition };
            _modifiers = null;

            _duration = 0;
            _smooth = false;
        }

        public MoveToPositionAction(float smoothingTime, bool relative, WaveModifier modifierX, WaveModifier modifierY, float duration, bool mustWait) : base(true, mustWait)
        {
            _type = TranslationType.SET;

            _points = new Waypoint[0];
            _modifiers = new WaveModifier[] { modifierX, modifierY };

            _duration = duration;
            _smoothingTime = smoothingTime;
            _smooth = _smoothingTime > 0;
            _relative = relative;
        }

        public MoveToPositionAction(Waypoint positionFrom, Waypoint positionTo, float duration, bool smooth, bool _mustWait) : base(true, _mustWait)
        {         
            _type = TranslationType.LERP;
            _points = new Waypoint[] { positionFrom, positionTo };
            _modifiers = null;

            _duration = duration;
            _smooth = smooth;    
        }

        public MoveToPositionAction(Waypoint pointA, Waypoint pointB, Waypoint pointC, float duration, bool smooth, bool _mustWait) : base(true, _mustWait)
        {
            _type = TranslationType.LERP_QUADRATIC_BEZIER;
            _points = new Waypoint[] { pointA, pointB, pointC };
            _modifiers = null;
            _duration = duration;
            _smooth = smooth;
        }

        public MoveToPositionAction(Waypoint pointA, Waypoint pointB, Waypoint pointC, Waypoint pointD, float duration, bool smooth, bool _mustWait) : base(true, _mustWait)
        {
            _type = TranslationType.LERP_CUBIC_BEZIER;
            _points = new Waypoint[] { pointA, pointB, pointC, pointD };
            _duration = duration;
            _smooth = smooth;
        }

        public override void Reset()
        {
            base.Reset();
            if(_modifiers != null)
            {
                for (int i = 0; i < _modifiers.Length; i++)
                {
                    _modifiers[i].Reset();
                }
            }
            _velocity = Vector2.zero;
        }

        public override bool OnEnter()
        {
            bool exit = base.OnEnter();
            _initialPosition = _hostEntity.Position;

            if(_points.Length > 0)
            {
                for (int i = 0; i < _points.Length; i++)
                {
                    _points[i].BakePosition(_initialPosition);
                }

                _hostEntity.Position = _points[0].GetPosition();

            }
            return exit;
        }

        public override bool OnUpdate(float deltaTime)
        {
            time += deltaTime;
            if(_type == TranslationType.SET & _modifiers != null)
            {
                if (_duration > 0.0f & time >= _duration)
                {
                    return true;
                }

                float x = _modifiers[0].Update(time, deltaTime);
                float y = _modifiers[1].Update(time, deltaTime);

                Vector2 pos = _relative ? _initialPosition + new Vector2(x, y) : new Vector2(x, y);

                _hostEntity.Position = _smooth ? Vector2.SmoothDamp(_hostEntity.Position, pos, ref _velocity, _smoothingTime, deltaTime) : pos;
                return false;
            }

            float t = _smooth ? Maths.SmoothStep(0.0f, 1.0f, time / _duration) : time / _duration;
            switch (_type)
            {
                case TranslationType.LERP:
                    if (time >= _duration)
                    {
                        _hostEntity.Position = _points[1].GetPosition();
                        return true;
                    }

                    _hostEntity.Position = Vector2.Lerp(_points[0].GetPosition(), _points[1].GetPosition(), t);
                    return false;

                case TranslationType.LERP_QUADRATIC_BEZIER:
                    if (time >= _duration)
                    {
                        _hostEntity.Position = _points[2].GetPosition();
                        return true;
                    }

                    _hostEntity.Position = Maths.QuadraticBezierCurve(_points[0].GetPosition(), _points[1].GetPosition(), _points[2].GetPosition(), t);
                    return false;

                case TranslationType.LERP_CUBIC_BEZIER:
                    if (time >= _duration)
                    {
                        _hostEntity.Position = _points[3].GetPosition();
                        return true;
                    }

                    _hostEntity.Position = Maths.CubicBezierCurve(_points[0].GetPosition(), _points[1].GetPosition(), _points[2].GetPosition(), _points[3].GetPosition(), t);
                    return false;
            }
            return true;
        }

        public override StateAction Clone()
        {
            switch (_type)
            {
                default:
                    return _modifiers != null ? new MoveToPositionAction(_smoothingTime, _relative, _modifiers[0].Clone(), _modifiers[1].Clone(), _duration, mustWait) : new MoveToPositionAction(_points[0]);
                case TranslationType.LERP:
                    return new MoveToPositionAction(_points[0], _points[1], _duration, _smooth, mustWait);
                case TranslationType.LERP_QUADRATIC_BEZIER:
                    return new MoveToPositionAction(_points[0], _points[1], _points[2], _duration, _smooth, mustWait);
                case TranslationType.LERP_CUBIC_BEZIER:
                    return new MoveToPositionAction(_points[0], _points[1], _points[2], _points[3], _duration, _smooth, mustWait);
            }
        }

        public enum TranslationType
        {
            SET,

            LERP,
            LERP_QUADRATIC_BEZIER,
            LERP_CUBIC_BEZIER,
        }
    }
}