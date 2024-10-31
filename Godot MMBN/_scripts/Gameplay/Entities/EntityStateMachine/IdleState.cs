using Godot;
using MMBN.Gameplay.Entities.Animation;
using MMBN.Utility;
using System;

namespace MMBN.Gameplay.Entities.EntityStateMachine
{
    public partial class IdleState : EntityState
    {
		[Export]
		private float _idleTime;
		private DelayedEventHandler _delayedEvent;
        [Export]
        private EntityAnimationController _animationController;

		[Export]
		private EntityState nextState;

        private bool _isPaused = false;

        public override void BeginState()
        {
			_delayedEvent = new DelayedEventHandler(
				_idleTime, 
				() => _entityStateController.SetState(nextState),
				false
				);
            
            _animationController.PlayAnimation(AnimationID.IDLE_ANIMATION_ID);
        }

        public override void EndState()
        {

        }

        public override void PauseState()
        {
            _isPaused = true;

            _animationController.SetAnimationPaused(_isPaused);
        }

        public override void ContinueState()
        {
            _isPaused = false;

            _animationController.SetAnimationPaused(_isPaused);
        }

        public override string GetStateID()
        {
			return EntityState.IDLE_STATE_ID;
        }

        public override void UpdateState(float deltaTime)
        {
            if (!_isPaused)
            {
                _delayedEvent.Update(deltaTime);
            }
        }
    }
}
