using System.Collections;

namespace Joonaxii.ConsoleBulletHell
{
    public class Enemy : Entity
    {
        public StateMachine StateMachine { get; protected set; }
        protected IEnumerator _dying;
        protected EffectPool _deathFX;
        protected bool _isDying;

        protected Sprite _defaultSprite;
        protected Sprite _damageSprite;

        protected float _hurtTimer;
        protected float _hurtTime = 0.15f;
        protected bool _shouldFlash;

        public Enemy(EntityID id, EntityType type, EffectPool fx, int damage, int maxHP, bool autoUpdate, int order, Sprite hurt, Sprite sprt, Animation anim, Vector2 colliderOffset, params ColliderBase[] colliders) : base(id, type, damage, maxHP, autoUpdate, order, sprt, anim, colliderOffset, colliders)
        {
            _deathFX = fx;

            _defaultSprite = sprt;
            _damageSprite = hurt;

            _hurtTimer = 0;
            _shouldFlash = false;
        }

        public override void OnCollisionEnter(ICollideable other)
        {
            Entity ent = other.GetEntity();
            if (ent.Type != EntityType.WORLD && ent.Type != EntityType.PLAYER)
            {
                ent.Despawn(false);
                switch (TakeDamage(ent.Damage))
                {
                    case 2:
                        if(ent.Type == EntityType.BULLET_PLR)
                        {
                            int multiplier = Type == EntityType.ENEMY | Type == EntityType.ENEMY_MELEE ? 1 : 10;

                            Player plr = Program.Player;
                            plr.SpecialMeter += 0.00025f * MaxHealth;

                            switch (Program.Difficulty)
                            {
                                case Difficulty.EASY:
                                    plr.Score += 10 * MaxHealth  * multiplier;
                                    break;
                                case Difficulty.MEDIUM:
                                    plr.Score += 25 * MaxHealth  * multiplier;
                                    break;
                                case Difficulty.HARD:
                                    plr.Score += 50 * MaxHealth  * multiplier;
                                    break;
                                case Difficulty.YOURE_DEAD:
                                    plr.Score += 100 * MaxHealth  * multiplier;
                                    break;
                            }
                        }
                        Die();
                        break;
                    case 1:
                        _hurtTimer = 0;
                        _shouldFlash = true;
                        Sprite = _damageSprite;
                        break;
                }
            }
        }

        public virtual void Setup(StateMachine stateMachine, int maxHP, int damage)
        {
            Damage = damage;
            MaxHealth = maxHP;

            StateMachine = stateMachine;
            StateMachine.Setup(this);
        }

        public override void Spawn(Vector2 pos, Vector2 dir)
        {
            base.Spawn(pos, dir);
            _isDying = false;
            _hurtTimer = 0;
            _shouldFlash = false;
            StopCoroutine(_dying);
            StateMachine.Start();
        }

        public override void Update(float deltaTime)
        {
            base.Update(deltaTime);
            if (_isDying) { return; }

            if (_shouldFlash)
            {
                if (_hurtTimer >= _hurtTime)
                {
                    _hurtTimer = 0;
                    _shouldFlash = false;
                    Sprite = _defaultSprite;
                    return;
                }
                _hurtTimer += deltaTime;
            }

            StateMachine?.Update(deltaTime);
        }

        public void Die()
        {
            if (_isDying) { return; }
            _isDying = true;

            StopCoroutine(_dying);
            StartCoroutine(_dying = DyingSequence());
        }

        protected virtual IEnumerator DyingSequence()
        {
            Effect fx = _deathFX?.Get() as Effect;
            fx?.Spawn(Position, Vector2.zero);
            yield return null;
            Despawn(false);
        }
    }
}