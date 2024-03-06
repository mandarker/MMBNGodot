using Godot;
using MMBN.Gameplay.Chips;
using MMBN.Gameplay.Entities.Animation;
using MMBN.Gameplay.Entities.EntityStateMachine;
using MMBN.Gameplay.Entities.StatusEffects;
using System;

namespace MMBN.Gameplay.Entities.OnDamageDealtModifiers
{
    public partial class OnDamageDealtInvincibleModifier : OnDamageDealtModifier
    {
        [Export]
        private HealthController _healthController;

        [Export]
        private EntityStateController _entityStateController;

        [Export]
        private EntityState _damagedState;

        private float _invincibilityDuration = 2;

        public override string GetDamageDealtModifierID()
        {
            return OnDamageDealtModifier.INVINCIBLE_DAMAGE_DEALT_MODIFIER_ID;
        }

        public override int ApplyModifier(int damage, BattleEntity damagedEntity, AttackData attackData)
        {
            if (_healthController != null)
            {
                if (_healthController.StatusManager.HasStatus(StatusEffectHelper.INVINCIBLE_STATUS_ID))
                {
                    return 0;
                }
                else
                {
                    _entityStateController.SetState(_damagedState);
                }
            }

            return base.ApplyModifier(damage, damagedEntity, attackData);
        }
    }
}
