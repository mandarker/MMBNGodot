using Godot;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace MMBN.Gameplay.Entities.StatusEffects
{
    public interface IStatusEffect
    {
        public string GetStatusID();
        public string GetSourceID();
        public bool IsStatusEffectApplicable();
        public void ApplyStatusEffect();
        public void UpdateStatusEffect(float deltaTime);
        public void RemoveStatusEffect();
    }
}
