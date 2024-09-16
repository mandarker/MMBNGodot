using Godot;
using MMBN.Gameplay.Chips;
using MMBN.UI.Displayers;
using MMBN.Utility;
using System;

namespace MMBN.Gameplay.Battle.BattleStateMachine
{
    public partial class BattleFreezeChipState : GeneralState
    {
        private FreezeChipBase _freezeChip;

        private enum State { BEGINNING, RUNCHIP, END }

        [Export]
        private GeneralState _nextState;

        [Export]
        private AudioStream _freezeAudioStream;

        [Export]
        private FreezeChipPopupDisplayer _freezeChipPopupDisplayer;
        [Export]
        private AnimationPlayer _popupAnimationPlayer;

        [Export]
        private float _dimTime;
        [Export]
        private float _dimStrength;
        [Export]
        private Sprite2D _dimSprite;

        private State _currentState;

        private InterpolatedEventHandler<float> _dimInterpolatedEventHandler;
        private InterpolatedEventHandler<float> _dimOutInterpolatedEventHandler;


        public static readonly string STATE_ID = "BATTLE_FREEZECHIP_STATE";
        public override string GetStateID()
        {
            return STATE_ID;
        }

        public void SetFreezeChip(FreezeChipBase freezeChip)
        {
            _freezeChip = freezeChip;
        }

        public override void EndState()
        {
            _freezeChip.EndChip();
        }

        public override void StartState()
        {
            _currentState = State.BEGINNING;

            Game.Instance.SFXManager.PlaySFX(_freezeAudioStream);
            _freezeChipPopupDisplayer.SetText(_freezeChip);
            _popupAnimationPlayer.Play("BeginFreeze");

            _dimInterpolatedEventHandler = new InterpolatedEventHandler<float>(
                0,
                _dimStrength,
                InterpolationFunctions.FloatLinearFunction,
                _dimTime
                );
            _dimInterpolatedEventHandler.OnInterpolationAction += OnDimInterpolation;

            _dimOutInterpolatedEventHandler = new InterpolatedEventHandler<float>(
                _dimStrength,
                0,
                InterpolationFunctions.FloatLinearFunction,
                _dimTime
                );
            _dimOutInterpolatedEventHandler.OnInterpolationAction += OnDimInterpolation;
            _dimOutInterpolatedEventHandler.OnInterpolationEndedAction += OnDimEndedInterpolation;

            _freezeChip.OnChipFinished += OnChipFinished;
        }

        private void OnDimInterpolation(float value)
        {
            ((ShaderMaterial)_dimSprite.Material).SetShaderParameter("_transparency", value);
        }

        private void OnDimEndedInterpolation()
        {
            _parentStateMachine.SetState(_nextState);
        }

        private void OnChipFinished()
        {
            _freezeChip.OnChipFinished -= OnChipFinished;
            _currentState = State.END;
        }

        public override void UpdateState(float deltaTime)
        {
            switch (_currentState)
            {
                case State.BEGINNING:
                    _dimInterpolatedEventHandler.Update(deltaTime);
                    if (!_popupAnimationPlayer.IsPlaying())
                    {
                        _freezeChip.BeforeRunChip();
                        _currentState = State.RUNCHIP;
                    }
                    break;
                case State.RUNCHIP:
                    _freezeChip.RunChip(deltaTime);
                    break;
                default:
                case State.END:
                    _dimOutInterpolatedEventHandler.Update(deltaTime);
                    break;
            }

        }
    }
}
