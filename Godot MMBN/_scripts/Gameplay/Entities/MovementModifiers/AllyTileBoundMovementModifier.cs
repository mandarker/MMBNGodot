using Godot;
using MMBN.Gameplay.Battle;
using System;
using System.Security.Cryptography.X509Certificates;

namespace MMBN.Gameplay.Entities
{
    public partial class AllyTileBoundMovementModifier : MovementModifier
    {
		private const string MODIFIER_ID = "MOVEMENT_MODIFIER_ALLY_TILE_BOUND";

		[Export]
		private BattleEntity _currentEntity;
		private BattleGrid _battleGrid;

        public override void _Ready()
        {
            base._Ready();
            _battleGrid = Game.Instance.BattleSession.BattleGrid;
        }

        public override bool ApplyModifier(Vector2 movementVector, out Vector2 outputMovement)
        {
            Vector2 expectedPosition = movementVector + _currentEntity.MovementController.TilePosition;

			if (_battleGrid.TryGetTile(expectedPosition, out BattleGridTile tile))
			{
				if (tile.IsPlayerTile && _currentEntity.EntityType == BattleEntity.BattleEntityType.PLAYER ||
					!tile.IsPlayerTile && _currentEntity.EntityType == BattleEntity.BattleEntityType.ENEMY)
					{
						outputMovement = movementVector;
						return true;
					}
			}

			outputMovement = Vector2.Zero;
			return false;
        }

        public override string GetModifierID()
        {
            return MODIFIER_ID;
        }

		public override int GetPriority()
		{
			return MovementModifier.MOVEMENT_MODIFIER_PRIORITY_2;
		}
    }
}
