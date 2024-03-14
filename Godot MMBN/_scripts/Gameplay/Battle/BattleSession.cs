using Godot;
using System;
using MMBN.Gameplay.Entities;
using System.Collections.Generic;
using MMBN.Gameplay.Entities.EntityGroupedBehaviours;
using MMBN.Gameplay.Chips;

namespace MMBN.Gameplay.Battle
{
	public sealed class BattleSession
	{
		private BattleWinCondition _winCondition;
		private BattleGrid _battleGrid;

		public Action OnEnemyDied;

		private List<BattleEntity> _entities;
		private List<BattleEntity> _queuedAddEntities;
		private List<BattleEntity> _queuedRemoveEntities;

		private MettaurGroupedBehaviour _mettaurGroupedBehaviour;

		private int _enemyCount;

		private ChipsController _chipsController;
		public ChipsController ChipsController { get { return _chipsController; }}

        private BattleEntity _playerBattleEntity;
        public BattleEntity PlayerBattleEntity { get { return _playerBattleEntity; }}

		public BattleSession(BattleGrid grid, BattleEntity playerEntity, BattleEntity[] entities, BattleWinCondition winCondition)
		{
			_winCondition = winCondition;

			_battleGrid = grid;
			grid.Init();
			_entities = new List<BattleEntity>();
			_queuedAddEntities = new List<BattleEntity>();
			_queuedRemoveEntities = new List<BattleEntity>();
            _playerBattleEntity = playerEntity;

			_mettaurGroupedBehaviour = new MettaurGroupedBehaviour();

			// spawn the player and enemies in the appropriate spot
			_battleGrid.OccupyTile(playerEntity);
			_entities.Add(playerEntity);
			_enemyCount = 0;

			if (_chipsController == null)
			{
				PackedScene chipsControllerPackedScene = GD.Load<PackedScene>("res://_scenePrefabs/ChipsController.tscn");
				_chipsController = chipsControllerPackedScene.Instantiate<ChipsController>();
				_chipsController.Init(playerEntity);
			}

			foreach (BattleEntity entity in entities)
			{
				_battleGrid.OccupyTile(entity);
				_entities.Add(entity);

				_mettaurGroupedBehaviour.SubmitMettaur(entity);

				if (entity.EntityType == BattleEntity.BattleEntityType.ENEMY)
				{
					++_enemyCount;
					entity.HealthController.OnHealthReachedZero += () => OnEnemyDied?.Invoke();
				}
			}

			OnEnemyDied += CheckIfAllEnemiesDied;
		}

		public void CheckIfAllEnemiesDied()
		{
			--_enemyCount;

			if (_enemyCount <= 0)
			{
				GD.Print("killed all enemies");
			}
		}

		public void SpawnEntity(BattleEntity entity, Vector2 gridPosition)
		{
			entity.Init(_battleGrid, gridPosition);
			_queuedAddEntities.Add(entity);
		}

		public void RemoveEntity(BattleEntity entity)
		{
			_queuedRemoveEntities.Add(entity);
		}

		public List<BattleEntity> GetEntitiesAtPosition(Vector2 tilePosition)
		{
			List<BattleEntity> entities = new List<BattleEntity>();

			if (_battleGrid.TryGetTile(tilePosition, out BattleGridTile tile))
			{
				entities.AddRange(tile.GetEntities());
			}

			return entities;
		}

		public void HighlightGridTile(Vector2 gridPosition)
		{
			_battleGrid.SetHighlightAt(gridPosition, true);
		}

		public void UnhighlightGridTile(Vector2 gridPosition)
		{
			_battleGrid.SetHighlightAt(gridPosition, false);
		}

        private bool _isPaused = false;
        public bool IsPaused { get { return _isPaused; } }
        public Action<bool> OnPaused;

        public void SetPaused(bool paused)
        {
            _isPaused = paused;

            // pause all entity animations
            foreach (var entity in _entities)
            {
                entity.AnimationController.SetAnimationPaused(_isPaused);
            }

            OnPaused?.Invoke(paused);
        }

		public void Update(float deltaTime)
		{
            if (_isPaused)
                return;

			for (int i = 0; i < _queuedAddEntities.Count; ++i)
			{
				_entities.Add(_queuedAddEntities[i]);
				Game.Instance.AddChild(_queuedAddEntities[i]);
			}
			_queuedAddEntities.Clear();

			for (int i = 0; i < _entities.Count; ++i)
			{
				_entities[i].StateMachine.UpdateStateMachine(deltaTime);
				_entities[i].AnimationController.Update(deltaTime);
			}

			for (int i = 0; i < _queuedRemoveEntities.Count; ++i)
			{
				_entities.Remove(_queuedRemoveEntities[i]);
				_battleGrid.UnoccupyTile(_queuedRemoveEntities[i]);
			}
			_queuedRemoveEntities.Clear();
		}
	}
}
