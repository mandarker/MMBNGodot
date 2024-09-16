using Godot;
using MMBN.Gameplay.Entities;
using System;
using System.Collections.ObjectModel;

namespace MMBN.Gameplay.Battle
{
	public sealed partial class BattleGrid : Node
	{
		public BattleGridTile[] _gridTiles;

		public void Init()
		{
			_gridTiles = new BattleGridTile[18];

			for (int i = 0; i < _gridTiles.Length; ++i)
			{
				_gridTiles[i] = new BattleGridTile(GetChild<Node2D>(i));
				_gridTiles[i].SetPlayerTile((i % 6 < 3));
			}
		}

		public bool TryGetTile(Vector2 tilePosition, out BattleGridTile tile)
		{
			if (TilePositionToArrayPosition(tilePosition, out int arrayPosition))
			{
				tile = _gridTiles[arrayPosition];
				return true;
			}

			tile = null;
			return false;
		}

		public void SetHighlightAt(Vector2 tilePosition, bool highlighted, object obj)
		{
			if (TilePositionToArrayPosition(tilePosition, out int arrayPosition))
			{
				_gridTiles[arrayPosition].SetTileHighlighted(highlighted, obj);
			}
		}

		public void OccupyTile(BattleEntity entity)
		{
			Vector2 tilePosition = entity.MovementController.TilePosition;
			if (TilePositionToArrayPosition(tilePosition, out int arrayPosition))
			{
				_gridTiles[arrayPosition].AddEntity(entity);
			}
		}

		public void UnoccupyTile(BattleEntity entity)
		{
			Vector2 tilePosition = entity.MovementController.TilePosition;
			if (TilePositionToArrayPosition(tilePosition, out int arrayPosition))
			{
				_gridTiles[arrayPosition].RemoveEntity(entity);
			}
		}

		public int GetZindex(Vector2 tilePosition)
		{
			return (int)tilePosition.Y;
		}

		public BattleEntity GetNearestPlayerEntity(Vector2 tilePosition)
		{
			float distance = 10000;
			BattleEntity closestEntity = null;

			foreach (BattleGridTile tile in _gridTiles)
			{
				ReadOnlyCollection<BattleEntity> tileEntities = tile.GetEntities();
				foreach (BattleEntity entity in tileEntities)
				{
					if (entity.EntityType == BattleEntity.BattleEntityType.PLAYER)
					{
						if (entity.MovementController.TilePosition.DistanceTo(tilePosition) < distance)
						{
							distance = entity.MovementController.TilePosition.DistanceTo(tilePosition);
							closestEntity = entity;
						}
					}
				}
			}

			return closestEntity;
		}

		private bool TilePositionToArrayPosition(Vector2 tilePosition, out int arrayPosition)
		{
			/*
				(0, 0) top left corner

				0, 1, 2, 3, 4, 5, 6
				7, 8, 9, 10, 11, 12, 13
				14, 15, 16, 17, 18, 19, 20
			*/
			arrayPosition = -1;

			if (!IsTilePositionInBounds(tilePosition))
			{
				return false;
			}

			arrayPosition = (int)tilePosition.X + ((int)tilePosition.Y * 6);
			return true;
		}

		public Vector2 TilePositionToWorldPosition(Vector2 tilePosition)
		{
			if (TilePositionToArrayPosition(tilePosition, out int arrayPosition))
			{
				return _gridTiles[arrayPosition].GetWorldPosition();
			}

			return Vector2.Zero;
		}

		public bool IsTilePositionInBounds(Vector2 tilePosition)
		{
			if (tilePosition.X < 0 || tilePosition.Y < 0)
			{
				//GD.PrintErr("Tile position is negative.");
				return false;
			}

			if (tilePosition.X >= 6 || tilePosition.Y >= 3)
			{
				//GD.PrintErr("Tile position is OOB positive.");
				return false;
			}

			return true;
		}

		public void MoveEntity(BattleEntity entity, Vector2 initialPosition, Vector2 endPosition)
		{
			if (TryGetTile(initialPosition, out BattleGridTile initialTile) && 
				TryGetTile(endPosition, out BattleGridTile endTile))
				{
					if (initialTile.RemoveEntity(entity))
					{
						endTile.AddEntity(entity);
					}
				}
		}
	}
}
