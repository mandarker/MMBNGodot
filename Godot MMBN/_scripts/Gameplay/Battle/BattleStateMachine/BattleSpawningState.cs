using Godot;
using System;
using MMBN.Utility;
using System.Collections.Generic;
using MMBN.Gameplay.Entities;

namespace MMBN.Gameplay.Battle.BattleStateMachine
{
    public partial class BattleSpawningState : GeneralState
    {
        [Export]
        private float _spawningTime;

        [Export]
        private BattleSession _battleSession;

        [Export]
        private GeneralState _nextState;

        [Export]
        private AudioStream _spawningAudioStream;

        [Export]
        private GeneralState _actionState;

        private InterpolatedEventHandler<float> _pixelationInterpolatedEventHandler;

        private int _entityCounter;
        private List<BattleEntity> _enemyEntities;


        public static readonly string STATE_ID = "BATTLE_SPAWNING_STATE";
        public override string GetStateID()
        {
            return STATE_ID;
        }

        public override void EndState()
        {
            
        }

        public override void StartState()
        {
            _entityCounter = 0;

            _enemyEntities = _battleSession.GetEnemyEntities();

            _pixelationInterpolatedEventHandler = new InterpolatedEventHandler<float>(1, 0, InterpolationFunctions.FloatLinearFunction, _spawningTime);
            _pixelationInterpolatedEventHandler.OnInterpolationEndedAction += OnPixelInterpolationEnded;
            _pixelationInterpolatedEventHandler.OnInterpolationAction += OnPixelInterpolation;


            foreach (var entity in _enemyEntities)
            {
                entity.AnimationController.SetSpriteTransparency(0);
            }

            foreach (var entity in _battleSession.GetAllEntities())
            {
                entity.StateMachine.PauseStateMachine(_actionState);
            }

            Game.Instance.SFXManager.PlaySFX(_spawningAudioStream);
        }

        public override void UpdateState(float deltaTime)
        {
            _pixelationInterpolatedEventHandler.Update(deltaTime);
        }

        private void OnPixelInterpolation(float interpolation)
        {
            _enemyEntities[_entityCounter].AnimationController.SetSpritePixelation(interpolation);
            _enemyEntities[_entityCounter].AnimationController.SetSpriteTransparency(1 - interpolation);
        }

        private void OnPixelInterpolationEnded()
        {
            _entityCounter++;
            _pixelationInterpolatedEventHandler.Reset();

            if (_entityCounter >= _enemyEntities.Count)
            {
                _parentStateMachine.SetState(_nextState);
            }
            else
            {
                Game.Instance.SFXManager.PlaySFX(_spawningAudioStream);
            }
        }
    }
}
