using Godot;
using MMBN.Utility;
using System;

namespace MMBN.Gameplay.Entities.StatusEffects
{
    public sealed class InvincibleStatusEffect : IStatusEffect
    {
        private float _duration;
        private string _sourceID;

        public float Duration { get { return _duration; } }

        private DelayedEventHandler _statusEffectUpdateHandler;
        public Action OnStatusEnded;

        private StatusManager _statusManager; 

        public InvincibleStatusEffect(float duration, string sourceID, StatusManager statusManager)
        {
            _duration = duration;
            _sourceID = sourceID;
            _statusManager = statusManager;

            _statusEffectUpdateHandler = new DelayedEventHandler(
                _duration,
                () => OnStatusEnded?.Invoke(),
                false
                ); 
        }

        public string GetStatusID()
        {
            return StatusEffectHelper.INVINCIBLE_STATUS_ID;
        }

        public string GetSourceID()
        {
            return _sourceID;
        }

        public bool IsStatusEffectApplicable()
        {
            return true;
        }

        public void ApplyStatusEffect()
        {
            
        }

        public void UpdateStatusEffect(float deltaTime)
        {
            _statusEffectUpdateHandler.Update(deltaTime);
        }

        public void RemoveStatusEffect()
        {

        }
    }
}
