using Godot;
using MMBN.Utility;
using System;

namespace MMBN.Gameplay.Battle.BattleStateMachine
{
    public partial class BattleBeginState : GeneralState
    {
        [Export]
        private AnimationPlayer _battleStartUIAnimationPlayer;

        [Export]
        private GeneralState _nextState;

        public override void EndState()
        {
        }

        public override void StartState()
        {
            _battleStartUIAnimationPlayer.Play("BattleStart");
        }

        public override void UpdateState(float deltaTime)
        {
            if (!_battleStartUIAnimationPlayer.IsPlaying())
            {
                _parentStateMachine.SetState(_nextState);
            }
        }
    }
}
