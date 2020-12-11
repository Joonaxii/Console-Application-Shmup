namespace Joonaxii.ConsoleBulletHell
{
    public class EffectPool : ObjectPool
    {
        private int _order;
        private Animation _anim;

        public EffectPool(int order, string anim, int initialCount) : base(initialCount)
        {
            _order = order;
            _anim = SpriteBank.GetAnimation(anim);
            GenerateInitial(initialCount);
        }

        public override IPoolable GetNew()
        {
            return new Effect(_order, _anim);
        }
    }
}