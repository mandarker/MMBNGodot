using Godot;
using MMBN.Gameplay.Battle;
using MMBN.Utility;
using System;
using System.Diagnostics;

namespace MMBN.Gameplay.Entities.EntityStateMachine
{
    public partial class TrackPlayerState : EntityState
    {
		[Export]
		private EntityState _attackState;
        [Export]
        private MovementControllerBase _movementController;

        private BattleGrid _battleGrid;

        private DelayedEventHandler _delayedMovementHandler;
		private const float MOVEMENT_SPEED = 1f;

        private bool _isPaused = false;

        public override void BeginState()
        {
            
        }

        public override void UpdateState(float deltaTime)
        {
            if (_isPaused)
            {
                return;
            }

            _delayedMovementHandler.Update(deltaTime);
        }

        private void DelayedCheckMovement()
        {
            BattleEntity playerEntity = _battleGrid.GetNearestPlayerEntity(_movementController.TilePosition);

			Vector2 direction = new Vector2(
				0, 
				Mathf.Clamp(playerEntity.MovementController.TilePosition.Y - _movementController.TilePosition.Y, -1, 1)
				);

            if (direction == Vector2.Zero)
            {
                _entityStateController.SetState(_attackState);
            }
            else
            {
			    _movementController.TryMoveInDirection(direction);
            }

            _delayedMovementHandler.Reset();
        }

        public override void PauseState()
        {
            _isPaused = true;
        }

        public override void ContinueState()
        {
            _isPaused = false;
        }

        public override void EndState()
        {

        }

        public override string GetStateID()
        {
			return EntityState.TRACK_PLAYER_STATE_ID;
        }

        public override void _Ready()
        {
            // oh god this is so bad
            _battleGrid = GetNode<BattleGrid>("/root/MainScene/BattleGrid");

            _delayedMovementHandler = new DelayedEventHandler(
                MOVEMENT_SPEED,
                () => DelayedCheckMovement(),
                false
            );
        }
    }
}
