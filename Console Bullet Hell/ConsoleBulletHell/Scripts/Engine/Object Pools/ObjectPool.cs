using System;
using System.Collections.Generic;

namespace Joonaxii.ConsoleBulletHell
{
    public abstract class ObjectPool
    {
        private readonly int ALLOCATION_SIZE;
        private Queue<IPoolable> _pool;

        public ObjectPool(int initialCount)
        {
            ALLOCATION_SIZE = Math.Max(initialCount / 2, 1);
        }

        protected void GenerateInitial(int count)
        {
            _pool = new Queue<IPoolable>(count * 2);
            for (int i = 0; i < count; i++)
            {
                Return(GetNew());
            }
        }

        public abstract IPoolable GetNew();

        public void Return(IPoolable input)
        {
            if(input == null) { return; }
            _pool.Enqueue(input);
        }

        public IPoolable Get()
        {
            IPoolable temp;
            if(_pool.Count < 1)
            {
                for (int i = 0; i < ALLOCATION_SIZE-1; i++)
                {
                    Return(GetNew());
                }

                temp = GetNew();
                return temp;
            }

            temp = _pool.Dequeue();
            return temp;
        }
    }
}