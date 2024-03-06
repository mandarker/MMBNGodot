using Godot;
using MMBN.Gameplay.Battle;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace MMBN.Gameplay.Entities
{
	public abstract partial class MovementControllerBase : Node
	{
		protected Vector2 _tilePosition;

		public Vector2 TilePosition { get{ return _tilePosition; } }

		[Export]
		protected Node2D _node;
		protected BattleGrid _battleGrid;
		protected BattleEntity _entity;

		protected List<MovementModifier> _movementModifiers;

		public virtual void Init(BattleGrid battleGrid, Vector2 initialPosition, BattleEntity entity)
		{
			_battleGrid = battleGrid;
			_tilePosition = initialPosition;
			_entity = entity;
			_node = entity;

			_movementModifiers = new List<MovementModifier>();

			entity.ZIndex = battleGrid.GetZindex(_tilePosition);

			int childCount = GetChildCount();
			for (int i = 0; i < childCount; ++i)
			{
				_movementModifiers.Add(GetChild<MovementModifier>(i));
			}

			_battleGrid.OccupyTile(entity);
			_entity.Position = _battleGrid.TilePositionToWorldPosition(initialPosition);
		}

		public abstract void UpdateMovement(float deltaTime);
		
		public void AddMovementModifier(MovementModifier movementModifier)
		{
			if (_movementModifiers.Where(mod => mod.GetType().Equals(movementModifier.GetType())).Count() > 0)
			{
				GD.PrintErr($"Movement modifier of type {movementModifier.GetType()} has already been added.");
				return;
			}

			_movementModifiers.Add(movementModifier);
			
			// sort by priority after adding
			_movementModifiers.Sort(new ModifierSorter());
		}

		public void RemoveMovementModifier(MovementModifier movementModifier)
		{
			if (!_movementModifiers.Contains(movementModifier))
			{
				GD.PrintErr($"Movement modifier of type {movementModifier.GetType()} is not added.");
				return;
			}

			_movementModifiers.Remove(movementModifier);
		}

		public bool TryRunMovementModifiers(Vector2 inputMovement, out Vector2 outputMovement)
		{
			outputMovement = Vector2.Zero;

			for (int i = 0; i < _movementModifiers.Count; ++i)
			{
				MovementModifier movementModifier = _movementModifiers[i];

				if (movementModifier.ApplyModifier(inputMovement, out Vector2 calculatedMovement))
				{
					inputMovement = calculatedMovement;
				}
				else
				{
					// cannot move
					return false;
				}
			}

			outputMovement = inputMovement;
			return true;
		}

		public bool CanMoveInDirection(Vector2 direction)
		{
			return TryRunMovementModifiers(direction, out Vector2 outputMovement);
		}

		public abstract void TryMoveInDirection(Vector2 direction);

        public class ModifierSorter : IComparer<MovementModifier>
        {
            public int Compare(MovementModifier x, MovementModifier y)
            {
                return x.GetPriority().CompareTo(y.GetPriority());
            }
        }
    }
}
