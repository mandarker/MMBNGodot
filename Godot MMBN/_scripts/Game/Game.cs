using Godot;
using MMBN.Gameplay.Battle;
using MMBN.Gameplay.Chips;
using MMBN.Gameplay.Entities;
using MMBN.Gameplay.Entities.StatusEffects;
using MMBN.SFX;
using System;
using System.Diagnostics;

namespace MMBN {
    public partial class Game : Node2D
    {
        public static Game Instance { get; private set; }

        private GlobalVariables _globalVariables;

        public GlobalVariables GlobalVariables { get { return _globalVariables; } }

        private BattleChipsManager _battleChipsManager;
        public BattleChipsManager BattleChipsManager { get { return _battleChipsManager; } }

        [Export]
		private PlayerController _playerController;
        public PlayerController PlayerController { get { return _playerController; } }

        [Export]
		private BattleSession _battleSession;
        public BattleSession BattleSession { get { return _battleSession; } }

        private SFXManager _sfxManager;
        public SFXManager SFXManager { get { return _sfxManager; } }

        [Export]
        private Camera2D _camera2D;

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

            _battleChipsManager = new BattleChipsManager();

            _sfxManager = new SFXManager(this);

            _battleSession.Init(null, _camera2D);


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
