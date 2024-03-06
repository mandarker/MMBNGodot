using Godot;
using MMBN.Gameplay.Chips;
using MMBN.Gameplay.Entities.OnDamageDealtModifiers;
using MMBN.Gameplay.Entities.StatusEffects;
using System;
using System.Collections.Generic;

namespace MMBN.Gameplay.Entities
{
    public partial class HealthController : Node
    {
	    [Export]
	    private int _health;
	    public int Health { get {return _health;}}

        [Export]
        private bool _invincibleOnDamage;

	    public Action OnHealthReachedZero;
	    public Action OnDamageDealt;

        public StatusManager StatusManager;

        [Export]
        private OnDamageDealtModifier[] _onDamageDealtModifiers;
        private List<OnDamageDealtModifier> _onDamageDealtModifierList;
        private BattleEntity _sourceEntity;

        public void Init(BattleEntity entity)
        {
            StatusManager = new StatusManager();

            _onDamageDealtModifierList = new List<OnDamageDealtModifier>();

            // apply initial modifiers first
            foreach (OnDamageDealtModifier modifier in _onDamageDealtModifiers)
            {
                _onDamageDealtModifierList.Add(modifier);
            }

            _sourceEntity = entity;
        }

        public override void _Process(double delta)
        {
            StatusManager.UpdateStatusEffects((float)delta);
        }

        public void AddDamageDealtModifier(OnDamageDealtModifier damageDealtModifier)
        {
            _onDamageDealtModifierList.Add(damageDealtModifier); 
        }

        public void RemoveDamageDealtModifier(string damageDealtModifierID)
        {
            _onDamageDealtModifierList.RemoveAll(damageDealtModifier => damageDealtModifier.GetDamageDealtModifierID().Equals(damageDealtModifierID));
        }

	    public void DealDamage(AttackData attackData)
	    {
            int damage = (int)attackData.Damage;

            if (_onDamageDealtModifierList.Count > 0)
            {
                damage = _onDamageDealtModifierList[0].ApplyModifier(damage, _sourceEntity, attackData);
            }

            if (damage > 0)
            {
                _health -= damage;

                OnDamageDealt?.Invoke();

                if (_health <= 0)
                {
                    OnHealthReachedZero?.Invoke();
                }
            }
	    }
    }
}
