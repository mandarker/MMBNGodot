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

        private PlayerController _playerController;

        private bool _isPaused = false;

        public override void EndState()
        {
            _playerController.ClearInputs();
        }

        public override void StartState()
        {
            _playerController = Game.Instance.PlayerController;
            _playerController.ClearInputs();
            SetStatePaused(false);
            _playerController.OnStartButtonPressed += () => SetStatePaused(!_isPaused);

            _isPaused = false;
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

        public override void UpdateState(float deltaTime)
        {
            if (_isPaused)
                return;

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
