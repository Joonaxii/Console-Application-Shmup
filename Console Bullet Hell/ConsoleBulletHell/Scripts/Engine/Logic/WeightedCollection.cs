using System;
using System.Collections.Generic;

namespace Joonaxii.ConsoleBulletHell
{
    public class WeightedCollection<T>
    {
        private List<T> _instances;
        private Random _rnd;

        public WeightedCollection(bool shuffle, params WeightedObject<T>[] weighted)
        {
            _rnd = new Random();
            _instances = new List<T>();

            for (int i = 0; i < weighted.Length; i++)
            {
                for (int j = 0; j < weighted[i].weight; j++)
                {
                    _instances.Add(weighted[i].item);
                }
            }

            if (shuffle)
            {
                Shuffle();
            }
        }

        public T SelectRandom()
        {
            return _instances[_rnd.Next(0, _instances.Count)];
        }

        private void Shuffle()
        {
            List<T> _temp = new List<T>();
            int count = _instances.Count;

            for (int i = 0; i < count; i++)
            {
                int rnd = _rnd.Next(0, _instances.Count);

                _temp.Add(_instances[rnd]);
                _instances.RemoveAt(rnd);
            }

            _instances = _temp;
        }
    }

}