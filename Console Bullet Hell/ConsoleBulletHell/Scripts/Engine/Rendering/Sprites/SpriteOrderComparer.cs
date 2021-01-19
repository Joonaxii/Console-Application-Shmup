using System.Collections.Generic;

namespace Joonaxii.ConsoleBulletHell
{
    public class SpriteOrderComparer : IComparer<Entity>
    {
        public int Compare(Entity first, Entity second)
        {
            int orderA;
            int orderB;

            bool notNullA;
            bool notNullB;

            if ((notNullA = first.Sprite != null) & (notNullB = second.Sprite != null))
            {
                orderA = first.Sprite.Order + first.renderingOffset;
                orderB = second.Sprite.Order + second.renderingOffset;

                return -orderA.CompareTo(orderB);
            }

            if (!notNullA & !notNullB)
            {
                return 0;
            }

            return notNullA ? 1 : -1;
        }
    }
}