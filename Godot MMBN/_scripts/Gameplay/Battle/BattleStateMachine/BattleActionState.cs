using Godot;
using MMBN.Gameplay.Entities;
using MMBN.UI;
using MMBN.Utility;
using MMBN.VFX;
using System;
using System.Collections.Generic;

namespace MMBN.Gameplay.Battle.BattleStateMachine
{
    public partial class BattleActionState : GeneralState
    {
        [Export]
        private BattleSession _battleSession;

        [Export]
        private PauseUIController _pauseUIController;

        [Export]
        private BattleCustomBarUIController _battleCustomBarUIController;

        [Export]
        private AudioStream _custBarFullSFX;

        [Export]
        private GeneralState _customState;

        private PlayerController _playerController;

        private bool _isPaused = false;

        private float _battleCustomAmount;
        [Export]
        private float _battleCustomSpeed;
        private bool _wasCustomFull;

        public override void EndState()
        {
            _playerController.ClearInputs(this);
            _battleCustomBarUIController.SetVisible(false);
        }

        public override void StartState()
        {
            _playerController = Game.Instance.PlayerController;
            _playerController.ClearInputs(this);
            SetStatePaused(false);
            _playerController.SubscribeInput(
                this, 
                PlayerController.ButtonDictionaryEnum.START_BUTTON_PRESSED, 
                () => SetStatePaused(!_isPaused)
                );
            _playerController.SubscribeInput(
                this,
                PlayerController.ButtonDictionaryEnum.L_BUTTON_PRESSED,
                TryOpenCustom
                );
            _playerController.SubscribeInput(
                this,
                PlayerController.ButtonDictionaryEnum.R_BUTTON_PRESSED,
                TryOpenCustom
                );

            _battleCustomAmount = 0;
            _battleCustomBarUIController.ResetUI();
            _battleCustomBarUIController.SetVisible(true);

            _isPaused = false;
            _wasCustomFull = false;
        }

        private void SetStatePaused(bool isPaused)
        {
            _isPaused = isPaused;

            _pauseUIController.SetPauseScreen(isPaused);

            // pause all entity animations
            foreach (var entity in _battleSession.GetAllEntities())
            {
                entity.AnimationController.SetAnimationPaused(_isPaused);

                if (_isPaused)
                {
                    entity.StateMachine.PauseStateMachine(this);
                }
                else
                {
                    entity.StateMachine.UnpauseStateMachine(this);
                }
            }

            // pause all vfx
            if (isPaused)
            {
                _battleSession.PauseVFXControllers();
            }
            else
            {
                _battleSession.UnpauseVFXControllers();
            }
        }

        private void TryOpenCustom()
        {
            GD.Print("what");
            if (_battleCustomAmount >= 1)
            {
                _parentStateMachine.SetState(_customState);
            }
        }


        public override void UpdateState(float deltaTime)
        {
            if (_isPaused)
                return;

            _battleCustomAmount += deltaTime * _battleCustomSpeed;
            if (_battleCustomAmount >= 1)
            {
                if (!_wasCustomFull)
                {
                    _wasCustomFull = true;
                    Game.Instance.SFXManager.PlaySFX(_custBarFullSFX);
                }

                _battleCustomAmount = 1;
            }

            _battleCustomBarUIController.SetBarFill(_battleCustomAmount);

            List<BattleEntity> queuedAddEntities = _battleSession.GetQueuedAddedEntities();
            List<BattleEntity> allEntities = _battleSession.GetAllEntities();
            List<BattleEntity> queuedRemoveEntities = _battleSession.GetQueuedRemovedEntities();

            for (int i = 0; i < queuedAddEntities.Count; ++i)
            {
                allEntities.Add(queuedAddEntities[i]);
                _battleSession.AddChild(queuedAddEntities[i]);
            }
            queuedAddEntities.Clear();

            for (int i = 0; i < allEntities.Count; ++i)
            {
                allEntities[i].StateMachine.UpdateStateMachine(deltaTime);
                allEntities[i].AnimationController.Update(deltaTime);
            }

            for (int i = 0; i < queuedRemoveEntities.Count; ++i)
            {
                allEntities.Remove(queuedRemoveEntities[i]);
                _battleSession.BattleGrid.UnoccupyTile(queuedRemoveEntities[i]);
            }
            queuedRemoveEntities.Clear();
        }
    }
}
