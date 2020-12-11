namespace Joonaxii.ConsoleBulletHell
{
    public class WeightedObject<T>
    {
        public T item;
        public int weight;

        public WeightedObject(T itm, int wei)
        {
            weight = wei;
            item = itm;
        }
    }
}