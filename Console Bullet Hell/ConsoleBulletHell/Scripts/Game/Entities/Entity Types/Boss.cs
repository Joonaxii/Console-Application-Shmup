using System;

namespace Joonaxii.ConsoleBulletHell
{
    public class Boss : Enemy
    {
        public override int CurrentHealth
        {
            get => base.CurrentHealth;
            set
            {
                base.CurrentHealth = Math.Max(value, 0);
                Program.HealthBarManager.UpdateBossHealthBar(base.CurrentHealth, MaxHealth);
            }
        }

        public Boss(EntityID id, EntityType type, int damage, int maxHp, bool autoUpdate,  int order, Sprite hurt, Sprite sprt, Animation anim, Vector2 colliderOffset, params ColliderBase[] colliders) : base(id, type, Program.PoolManager.GetPool<EffectPool>("Player_Respawn"), damage, maxHp, autoUpdate, order, hurt, sprt, anim, colliderOffset, colliders)
        {
        }
    }
}