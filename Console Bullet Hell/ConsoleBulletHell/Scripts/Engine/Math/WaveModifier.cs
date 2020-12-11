namespace Joonaxii.ConsoleBulletHell
{
    public class WaveModifier
    {
        public static WaveModifier zero = new WaveModifier(FadingType.LINEAR, WaveType.LINEAR, 0, 0, 0, 0);
        private WaveType _type;
        private FadingType _smoothing;

        private float _frequency;
        private float _amplitudeMin;
        private float _amplitudeMax;

        private float _phaseShift;

        private float _phase;
        private float _time;

        private WaveModifier _modFrequency;
        private WaveModifier _modAmplitude;

        public WaveModifier(FadingType smoothing, WaveType type, float frequency, float minAmplitude, float maxAmplitude, float phase, float phaseShift)
        {
            _smoothing = smoothing;
            _type = type;

            _frequency = frequency;
            _amplitudeMin = minAmplitude;
            _amplitudeMax = maxAmplitude;

            _modFrequency = null;
            _modAmplitude = null;

            _phase = phase * Maths.Deg2Rad;
            _phaseShift = phaseShift * Maths.Deg2Rad;
        }

        public WaveModifier(FadingType smoothing, WaveType type, float frequency, float amplitude, float phase, float phaseShift)
        {
            _smoothing = smoothing;
            _type = type;

            _frequency = frequency;
            _amplitudeMin = -amplitude;
            _amplitudeMax = amplitude;

            _modFrequency = null;
            _modAmplitude = null;

            _phase = phase * Maths.Deg2Rad;
            _phaseShift = phaseShift * Maths.Deg2Rad;
        }

        public WaveModifier(FadingType smoothing, WaveType type, WaveModifier frequency, WaveModifier amplitude, float phase, float phaseShift)
        {
            _smoothing = smoothing;
            _type = type;

            _modFrequency = frequency;
            _modAmplitude = amplitude;

            _phase = phase * Maths.Deg2Rad;
            _phaseShift = phaseShift * Maths.Deg2Rad;
        }

        public void Reset()
        {
            _time = 0.0f;
            _modFrequency?.Reset();
            _modAmplitude?.Reset();

        }

        public float Update(float time, float delta)
        {
            float wave = 0.0f;
            if (_type == WaveType.LINEAR)
            {
                wave = (time * (_modFrequency != null ? _modFrequency.Update(time, delta) : _frequency) + _time);
                switch (_smoothing)
                {
                    case FadingType.FADE_IN:
                        wave = Maths.EaseIn(wave);
                        break;
                    case FadingType.FADE_OUT:
                        wave = Maths.EaseOut(wave);
                        break;
                }
                _time += delta * _phaseShift;
                return (_modAmplitude != null ? wave * _modAmplitude.Update(time, delta) : Maths.Lerp(_amplitudeMin, _amplitudeMax, ((wave + 1.0f) * 0.5f)));
            }

            float phase = _phase + (Maths.PI * time * (_modFrequency != null ? _modFrequency.Update(time, delta) : _frequency)) + (_time * Maths.PI);
            _time += delta * _phaseShift;

            switch (_type)
            {
                case WaveType.COSINE:
                    wave = Maths.Cos(phase);
                    break;
                case WaveType.SINE:
                    wave = Maths.Sin(phase);
                    break;

                case WaveType.TRIANGLE:
                    wave = Maths.Triangle(phase);
                    break;

                case WaveType.SAWTOOTH:
                    wave = Maths.Sawtooth(phase);
                    break;

                case WaveType.COSINE_SQR:
                    wave = Maths.Square(phase, false);
                    break;
                case WaveType.SINE_SQR:
                    wave = Maths.Square(phase, true);
                    break;
            }

            switch (_smoothing)
            {
                case FadingType.FADE_IN:
                    wave = Maths.EaseIn(wave);
                    break;
                case FadingType.FADE_OUT:
                    wave = Maths.EaseOut(wave);
                    break;
                case FadingType.SMOOTH_STEP:
                    wave = Maths.SmoothStep(-1, 1, wave);
                    break;
            }

            return  (_modAmplitude != null ? wave * _modAmplitude.Update(time, delta) : Maths.Lerp(_amplitudeMin, _amplitudeMax, ((wave + 1.0f) * 0.5f)));
        }

        public WaveModifier Clone()
        {
            return _modAmplitude != null ? new WaveModifier(_smoothing, _type, _modFrequency.Clone(), _modAmplitude.Clone(), _phase * Maths.Rad2Deg, _phaseShift * Maths.Rad2Deg) :
                                           new WaveModifier(_smoothing, _type, _frequency, _amplitudeMin, _amplitudeMax, _phase * Maths.Rad2Deg, _phaseShift * Maths.Rad2Deg);
        }
    }
}