using Godot;
using MMBN.Gameplay.Chips;
using MMBN.Utility;
using System;

namespace MMBN.Gameplay.Battle.BattleStateMachine
{
    public partial class BattleFreezeChipState : GeneralState
    {
        private FreezeChipBase _freezeChip;

        private DelayedEventHandler _dimScreenDelayedEventHandler;

        public void SetFreezeChip(FreezeChipBase freezeChip)
        {
            _freezeChip = freezeChip;
        }

        public override void EndState()
        {
            _freezeChip.EndChip();
        }

        public override void StartState()
        {
            
        }

        public override void UpdateState(float deltaTime)
        {
            _freezeChip.RunChip(deltaTime);
        }
    }
}
