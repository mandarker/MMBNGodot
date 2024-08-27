using Godot;
using System;
using MMBN.Gameplay.Entities;
using System.Collections.Generic;
using MMBN.Gameplay.Entities.EntityGroupedBehaviours;
using MMBN.Gameplay.Chips;
using MMBN.VFX;
using System.Diagnostics;
using MMBN.UI;

namespace MMBN.Gameplay.Battle
{
	public partial class BattleSession : Node2D
	{
		private BattleWinCondition _winCondition;

        [Export]
		private BattleGrid _battleGrid;
        public BattleGrid BattleGrid { get { return _battleGrid; } }

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

        [Export]
        private BattleCustomScreenUIController _customScreenUIController;

        private bool _isPaused = false;
        public bool IsPaused { get { return _isPaused; } }
        public Action<bool> OnPaused;

        private HashSet<AnimatedVFXController> _animatedVFXControllers;

        public void Init(BattleWinCondition winCondition, Camera2D camera)
        {
            _battleGrid.Init();

            // temporary, we will need to put this somewhere
            _playerBattleEntity = BattleEntityGeneratorHelper.GenerateEntity(BattleEntityGeneratorHelper.PlayerEntityID);
            AddChild(_playerBattleEntity);
            _playerBattleEntity.Init(_battleGrid, new Vector2(1, 1));

            BattleEntity enemyEntity = BattleEntityGeneratorHelper.GenerateEntity(BattleEntityGeneratorHelper.MettaurEntityID);
            AddChild(enemyEntity);
            enemyEntity.Init(_battleGrid, new Vector2(3, 0));

            BattleEntity enemyEntity2 = BattleEntityGeneratorHelper.GenerateEntity(BattleEntityGeneratorHelper.MettaurEntityID);
            AddChild(enemyEntity2);
            enemyEntity2.Init(_battleGrid, new Vector2(4, 1));

            BattleEntity enemyEntity3 = BattleEntityGeneratorHelper.GenerateEntity(BattleEntityGeneratorHelper.MettaurEntityID);
            AddChild(enemyEntity3);
            enemyEntity3.Init(_battleGrid, new Vector2(5, 2));

            BattleEntity[] enemyEntities = new BattleEntity[3];
            enemyEntities[0] = enemyEntity;
            enemyEntities[1] = enemyEntity2;
            enemyEntities[2] = enemyEntity3;

            _winCondition = winCondition;
            
            _entities = new List<BattleEntity>();
			_queuedAddEntities = new List<BattleEntity>();
			_queuedRemoveEntities = new List<BattleEntity>();

			_mettaurGroupedBehaviour = new MettaurGroupedBehaviour();

			// spawn the player and enemies in the appropriate spot
			_entities.Add(_playerBattleEntity);
			_enemyCount = 0;

            foreach (BattleEntity entity in enemyEntities)
            {
                _entities.Add(entity);

                _mettaurGroupedBehaviour.SubmitMettaur(entity);

                if (entity.EntityType == BattleEntity.BattleEntityType.ENEMY)
                {
                    ++_enemyCount;
                    entity.HealthController.OnHealthReachedZero += () => OnEnemyDied?.Invoke();
                }
            }

            if (_chipsController == null)
			{
				PackedScene chipsControllerPackedScene = GD.Load<PackedScene>("res://_scenePrefabs/ChipsController.tscn");
				_chipsController = chipsControllerPackedScene.Instantiate<ChipsController>();
				_chipsController.Init(_playerBattleEntity);
                _chipsController.DisplayChipsOnPlayer();
			}

            _animatedVFXControllers = new HashSet<AnimatedVFXController>();

            // set up chips
            Game.Instance.BattleChipsManager.ResetAvailableBattleChips();

            // set up custom screen
            _customScreenUIController.Initialize();
            _customScreenUIController.ResetUI();


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

        public void SetPaused(bool paused)
        {
            _isPaused = paused;

            // pause all entity animations
            foreach (var entity in _entities)
            {
                entity.AnimationController.SetAnimationPaused(_isPaused);
            }

            // pause all vfx
            foreach (var vfxController in _animatedVFXControllers)
            {
                if (paused)
                    vfxController.Pause();
                else
                    vfxController.Unpause();
            }

            OnPaused?.Invoke(paused);
        }

        public void SubscribeVFXController(AnimatedVFXController animatedVFXController)
        {
            _animatedVFXControllers.Add(animatedVFXController);

            animatedVFXController.OnVFXFinished += () => _animatedVFXControllers.Remove(animatedVFXController);
        }

        public void UnsubscribeVFXController(AnimatedVFXController animatedVFXController)
        {
            if (_animatedVFXControllers.Contains(animatedVFXController))
            {
                _animatedVFXControllers.Remove(animatedVFXController);
            }
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
