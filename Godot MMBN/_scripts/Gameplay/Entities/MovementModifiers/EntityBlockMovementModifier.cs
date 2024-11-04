using Godot;
using MMBN.Gameplay.Battle;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;

namespace MMBN.Gameplay.Entities
{
    public partial class EntityBlockMovementModifier : MovementModifier
    {
        private const string MODIFIER_ID = "MOVEMENT_MODIFIER_ENTITY_BLOCK";

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
                List<BattleEntity> entities = tile.GetEntities().ToList().Where(entity => entity.Interactable == true).ToList();

                if (entities.Count == 0)
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
