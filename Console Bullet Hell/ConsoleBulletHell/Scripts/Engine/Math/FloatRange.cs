using System;

namespace Joonaxii.ConsoleBulletHell
{
    public struct FloatRange
    {
        public float Value => Maths.Lerp(_min, _max, (float)_random.NextDouble());
        public bool IsNull => _min == 0 & _max == 0;

        private float _min;
        private float _max;

        private Random _random;

        public FloatRange(float min, float max)
        {
            _random = new Random();
            _min = min; 
            _max = max; 
        }

        public FloatRange(float min, float max, int seed)
        {
            _random = new Random(seed);
            _min = min;
            _max = max;
        }
    }
}