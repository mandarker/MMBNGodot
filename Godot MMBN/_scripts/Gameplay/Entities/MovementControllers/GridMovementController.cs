using Godot;
using MMBN.Gameplay.Battle;
using MMBN.Utility;
using System;
using System.Runtime.CompilerServices;

namespace MMBN.Gameplay.Entities
{
	public partial class GridMovementController : MovementControllerBase
	{
        public override void Init(BattleGrid battleGrid, Vector2 initialPosition, BattleEntity entity)
        {
            base.Init(battleGrid, initialPosition, entity);
        }

		public override void TryMoveInDirection(Vector2 direction)
		{
			if (TryRunMovementModifiers(direction, out Vector2 finalizedMovement))
			{
				Vector2 finalizedPosition = finalizedMovement + _tilePosition;

				// move it in data for the grid
				_battleGrid.MoveEntity(_entity, _tilePosition, finalizedPosition);

				// change the tile position locally
				_tilePosition = finalizedMovement + _tilePosition;

				// for now, just move it immediately
				// TODO: make coroutines?
				_node.Position = _battleGrid.TilePositionToWorldPosition(_tilePosition);
				_entity.ZIndex = _battleGrid.GetZindex(_tilePosition);
			}
		}

		public override void UpdateMovement(float deltaTime)
		{

		}
	}
}
