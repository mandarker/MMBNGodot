using Godot;
using MMBN.Gameplay.Entities;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

public sealed class BattleGridTile
{
	private bool _isPlayerTile;
	public bool IsPlayerTile { get{ return _isPlayerTile; } }

	// do this later
	//private bool _isCracked;

	private HashSet<BattleEntity> _entities;

	private Node2D _node;

	public BattleGridTile(Node2D node)
	{
		_node = node;
		_isPlayerTile = false;

		_entities = new HashSet<BattleEntity>();
	}

	public Vector2 GetWorldPosition()
	{
		return _node.Position;
	}

	public void SetPlayerTile(bool isPlayerTile)
	{
		_isPlayerTile = isPlayerTile;
	}

	public void SetTileHighlighted(bool highlighted)
	{
		_node.GetChild<CanvasItem>(0).Visible = highlighted;
	}

	public bool AddEntity(BattleEntity entity)
	{
		if (_entities.Contains(entity))
		{
			GD.PrintErr("Entity already exists at that tile.");
			return false;
		}

		_entities.Add(entity);
		return true;
	}

	public bool RemoveEntity(BattleEntity entity)
	{
		if (!_entities.Contains(entity))
		{
			GD.PrintErr("Entity does not exist at that tile.");
			return false;
		}

		_entities.Remove(entity);
		return true;
	}

	public ReadOnlyCollection<BattleEntity> GetEntities()
	{
		return new ReadOnlyCollection<BattleEntity>(_entities.ToArray<BattleEntity>());
	}
}
