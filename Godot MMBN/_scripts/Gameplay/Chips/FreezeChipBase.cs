using Godot;
using MMBN.Gameplay.Entities.Animation;
using MMBN.Gameplay.Entities;
using System;

namespace MMBN.Gameplay.Chips
{
    public abstract partial class FreezeChipBase : ChipBase
    {
        public override void StartChip(EntityAnimationController playerAnimationController, BattleEntity playerEntity)
        {
            base.StartChip(playerAnimationController, playerEntity);

            Game.Instance.BattleSession.RunFreezeChip(this);
        }
    }
}
