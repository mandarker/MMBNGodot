using Godot;
using MMBN.Gameplay.Chips;
using System;

namespace MMBN.Gameplay.Entities.OnDamageDealtModifiers
{
    public partial class OnDamageDealtModifier : Node
    {
        protected OnDamageDealtModifier _nextModifier;

        public static readonly string INVINCIBLE_DAMAGE_DEALT_MODIFIER_ID = "INVINCIBLE_DAMAGE_DEALT_MODIFIER_ID";

        public virtual string GetDamageDealtModifierID() { return ""; }

        [Export]
        private HealthController _healthController;

        public void SetNextModifier(OnDamageDealtModifier nextModifier)
        {
            _nextModifier = nextModifier;
        }

        public virtual int ApplyModifier(int damage, BattleEntity damagedEntity, AttackData attackData)
        {
            if (_nextModifier != null)
            {
                return _nextModifier.ApplyModifier(damage, damagedEntity, attackData);
            }
            else
            {
                return damage;
            }
        }
    }
}
