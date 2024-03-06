using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MMBN.Gameplay.Entities.StatusEffects
{
    public class StatusManager
    {
        private List<IStatusEffect> _statusEffects;

        private Queue<IStatusEffect> _removalQueue;

        public StatusManager()
        {
            _statusEffects = new List<IStatusEffect>();

            _removalQueue = new Queue<IStatusEffect>();
        }

	    public void ApplyStatus(IStatusEffect statusEffect)
        {
            if (statusEffect.IsStatusEffectApplicable())
            {
                _statusEffects.Add(statusEffect);
                statusEffect.ApplyStatusEffect();
            }
        }

        public void UpdateStatusEffects(float deltaTime)
        {
            foreach (IStatusEffect statusEffect in _statusEffects)
            {
                statusEffect.UpdateStatusEffect(deltaTime);
            }

            while (_removalQueue.TryDequeue(out IStatusEffect statusEffect))
            {
                _statusEffects.Remove(statusEffect);
            }
        }

        public bool HasStatus(string statusID)
        {
            return _statusEffects.Any(statusEffect => statusEffect.GetStatusID().Equals(statusID));
        }

        public void RemoveStatus(IStatusEffect statusEffect)
        {
            if (_statusEffects.Contains(statusEffect))
            {
                statusEffect.RemoveStatusEffect();
                _removalQueue.Enqueue(statusEffect);
            }
        }

        public void RemoveStatus(string statusID)
        {
            IEnumerable<IStatusEffect> matchedStatusEffects = _statusEffects.Where(statusEffect => statusEffect.GetStatusID().Equals(statusID));

            foreach (IStatusEffect statusEffect in matchedStatusEffects)
            {
                _removalQueue.Enqueue(statusEffect);
            }
        }

        public void RemoveStatus(string statusID, string sourceID)
        {
            // check if there's a duplicate
            List<IStatusEffect> sameEffects = _statusEffects.Where(statusEffect => statusEffect.GetStatusID()
                .Equals(statusID)).ToList();

            for (int i = 0; i < sameEffects.Count; ++i)
            {
                // if there is
                if (((InvincibleStatusEffect)sameEffects[i]).GetSourceID().Equals(sourceID))
                {
                    // remove the original status effect
                    _statusEffects.Remove(sameEffects[i]);
                    return;
                }
            }
        }
    }
}
