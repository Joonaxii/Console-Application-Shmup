namespace Joonaxii.ConsoleBulletHell
{
    public class Effect : Entity
    {
        private bool _reverse = false;
        private float _speed = 1.0f;

        public Effect(int order, Animation anim, bool autoUpdate = true) : base(EntityID.PROP, EntityType.NONE, 0, 0, autoUpdate, order, null, anim, Vector2.zero, null)
        {
        }

        public void Setup(int frameOffset, int order, float speed = 1.0f, bool reverse = false)
        {
            _speed = speed;
            _reverse = reverse;

            
            renderingOffset = order;

            if (Animation != null)
            {
                _frameAnim = _reverse ? Animation.Frames.Length - 1 - frameOffset : frameOffset;
                Sprite = Animation.Frames[_frameAnim];
            }
        }

        public override void Update(float deltaTime)
        {
            if(Animation != null)
            {
                if(Animation.Animate(this, ref _timeAnim, ref _frameAnim, deltaTime * _speed, _reverse))
                {
                    Despawn(false);
                }
                return;
            }
            Despawn(false);
        }
    }
}