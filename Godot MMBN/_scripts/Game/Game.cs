using Godot;
using MMBN.Gameplay.Battle;
using MMBN.Gameplay.Chips;
using MMBN.Gameplay.Entities;
using MMBN.Gameplay.Entities.StatusEffects;
using MMBN.SFX;
using System;
using System.Diagnostics;

namespace MMBN {
    public partial class Game : Node
    {
        public static Game Instance { get; private set; }

        private GlobalVariables _globalVariables;

        public GlobalVariables GlobalVariables { get { return _globalVariables; } }

        private BattleChipsManager _battleChipsManager;
        public BattleChipsManager BattleChipsManager { get { return _battleChipsManager; } }

		private PlayerController _playerController;
        public PlayerController PlayerController { get { return _playerController; } }

		public BattleSession BattleSession;

        private SFXManager _sfxManager;
        public SFXManager SFXManager { get { return _sfxManager; } }

		public override void _Ready()
		{
			// initialize variables
			Init();
		}

		private void Init()
		{
			if (Instance != null && Instance != this)
			{
				Free();
			}
			else
			{
				Instance = this;
			}

			_globalVariables = new GlobalVariables();
			_globalVariables.SubmitFloat(GlobalVariableIDs.BUSTER_CHARGETIME_ID, 1.5f);

            _sfxManager = new SFXManager(this);

			BattleGrid battleGrid = GetNode<BattleGrid>("/root/MainScene/BattleGrid");
			battleGrid.Init();

			BattleEntity playerBattleEntity = BattleEntityGeneratorHelper.GenerateEntity(BattleEntityGeneratorHelper.PlayerEntityID);
			AddChild(playerBattleEntity);
			playerBattleEntity.Init(battleGrid, new Vector2(1, 1));
			
			BattleEntity enemyEntity = BattleEntityGeneratorHelper.GenerateEntity(BattleEntityGeneratorHelper.MettaurEntityID);
			AddChild(enemyEntity);
			enemyEntity.Init(battleGrid, new Vector2(3, 0));

			BattleEntity enemyEntity2 = BattleEntityGeneratorHelper.GenerateEntity(BattleEntityGeneratorHelper.MettaurEntityID);
			AddChild(enemyEntity2);
			enemyEntity2.Init(battleGrid, new Vector2(4, 1));

			BattleEntity enemyEntity3 = BattleEntityGeneratorHelper.GenerateEntity(BattleEntityGeneratorHelper.MettaurEntityID);
			AddChild(enemyEntity3);
			enemyEntity3.Init(battleGrid, new Vector2(5, 2));

			_playerController = GetNode<PlayerController>("/root/MainScene/PlayerController");

			BattleEntity[] enemyEntities = new BattleEntity[3];
			enemyEntities[0] = enemyEntity;
			enemyEntities[1] = enemyEntity2;
			enemyEntities[2] = enemyEntity3;

			BattleSession = new BattleSession(
				battleGrid,
				playerBattleEntity,
				enemyEntities,
				null
			);


            /*
            BattleSession.ChipsController.EnqueueChip(cannonChip);
            BattleSession.ChipsController.EnqueueChip(swordChip);
            BattleSession.ChipsController.EnqueueChip(cannonChip);
			BattleSession.ChipsController.EnqueueChip(cannonChip);
            BattleSession.ChipsController.EnqueueChip(swordChip);
            BattleSession.ChipsController.DisplayChipsOnPlayer();
            */

            _playerController.OnStartButtonPressed += () => SetBattleSessionPaused(!BattleSession.IsPaused);
		}

        private void SetBattleSessionPaused(bool paused)
        {
            BattleSession.SetPaused(paused);
            /*
            _playerController.SetAButtonDisabled(paused);
            _playerController.SetBButtonDisabled(paused);
            */
        }

        public override void _Process(double delta)
        {
            SFXManager.Update();
			BattleSession.Update((float) delta);
        }
    }
}
