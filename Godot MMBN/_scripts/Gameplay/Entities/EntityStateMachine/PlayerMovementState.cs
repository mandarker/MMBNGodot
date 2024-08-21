using Godot;
using MMBN.Gameplay.Entities.Animation;
using MMBN.Utility;
using MMBN.VFX;
using System;

namespace MMBN.Gameplay.Entities.EntityStateMachine
{
    public partial class PlayerMovementState : EntityState
    {
		private PlayerController _playerController;
		private Vector2 _sanitizedDirectionalInput;
		private Vector2 _lastSanitizedDirectionalInput;

		[Export]
		private MovementControllerBase _movementController;
		[Export]
		private EntityAnimationController _playerAnimationController;
		[Export]
		private EntityState _busterShotState;
		[Export]
		private EntityState _chipState;
		[Export]
		private BattleEntity _playerEntity;
        [Export]
        private AudioStreamPlayer _chargingAudioStreamPlayer;
        [Export]
        private AudioStreamPlayer _chargedAudioStreamPlayer;

        [Export]
		private float _movementSpeed = 0.15f;
		private bool _isMoving = false;
		private bool _isPaused = false;
        private bool _isBeforeCharging = false;
		private bool _isCharging = false;
        private bool _chargingSFXPlayed = false;
        private bool _chargedSFXPlayed = false;

		[Export]
		private float _chargeVFXDelay = 0.15f;
		private float _chargeTime = 0;
		private AnimatedVFXController _chargeVFXController;
		private bool _chargeVFXSpawned = false;

		private ThresholdedDelayedEventHandler _directionalInputEventHandler;
        private DelayedEventHandler _chargeBeginDelayedEventHandler;
		private DelayedEventHandler _chargeVFXDelayedEventHandler;

        public override void BeginState()
        {
			_playerController = GetNode<PlayerController>("/root/MainScene/PlayerController");

            _chargeBeginDelayedEventHandler = new DelayedEventHandler(
                _chargeVFXDelay,
                () =>
                {
                    _isBeforeCharging = false;
                    _isCharging = true;
                },
                false
            );

			_chargeVFXDelayedEventHandler = new DelayedEventHandler(
				_chargeVFXDelay,
				() => {
					_chargeVFXController = VFXGeneratorHelper.GenerateVFX(VFXGeneratorHelper.BusterChargeID, new Vector2(0, -20));
					_playerEntity.AddChild(_chargeVFXController);
                    Game.Instance.BattleSession.SubscribeVFXController(_chargeVFXController);

					_chargeVFXSpawned = true;
				},
				false
			);

            // set the proper animation
            _playerAnimationController.PlayAnimation(AnimationID.IDLE_ANIMATION_ID);

            _playerController.OnBButtonPressed += OnBButtonPressed;
			_playerController.OnBButton += OnBButton;
			_playerController.OnBButtonReleased += OnBButtonReleased;
			_playerController.OnAButtonPressed += OnAButtonPressed;

			_directionalInputEventHandler.Reset();

			_chargeTime = 0;
        }

        public override void PauseState()
        {
            _isPaused = true;
        }

        public override void ContinueState()
        {
            _isPaused = false;
        }

        public override void EndState()
        {
            if (_isCharging)
            {
                _chargeVFXController?.Free();
            }

            Game.Instance.BattleSession.UnsubscribeVFXController(_chargeVFXController);

            _playerController.OnBButtonPressed -= OnBButtonPressed;
			_playerController.OnBButton -= OnBButton;
			_playerController.OnBButtonReleased -= OnBButtonReleased;
			_playerController.OnAButtonPressed -= OnAButtonPressed;
        }

        public override string GetStateID()
        {
            return EntityState.PLAYER_MOVEMENT_STATE_ID;
        }

        public override void UpdateState(float deltaTime)
        {
			if (_isPaused)
			{
				return;
			}

            if (_isBeforeCharging)
            {
                _chargeBeginDelayedEventHandler.Update(deltaTime);
            }

			if (_isCharging)
			{
                if (!_chargingSFXPlayed)
                {
                    _chargingSFXPlayed = true;
                    _chargingAudioStreamPlayer.Play();
                }

				_chargeVFXDelayedEventHandler.Update(deltaTime);

				if (Game.Instance.GlobalVariables.GetFloat(GlobalVariableIDs.BUSTER_CHARGETIME_ID, out float chargeTime))
				{
					if (_chargeTime > chargeTime && _chargeVFXSpawned)
					{
						_chargeVFXController.PlayAnimation("BusterChargePurple");
                        
                        if (!_chargedSFXPlayed)
                        {
                            _chargedSFXPlayed = true;
                            _chargedAudioStreamPlayer.Play();
                        }
					}
				}

				_chargeTime += deltaTime;
			}

			_directionalInputEventHandler.Update((float)deltaTime);

			// sanitize the direction
			_sanitizedDirectionalInput = _playerController.DirectionalInput;
			if (Mathf.Abs(_sanitizedDirectionalInput.X) == 1 && 
				Mathf.Abs(_sanitizedDirectionalInput.Y) == 1)
			{
				_sanitizedDirectionalInput.Y = 0;
			}

			// negate the Y because of sprite z
			_sanitizedDirectionalInput.Y *= -1;

			if (_sanitizedDirectionalInput != Vector2.Zero && !_isMoving)
			{
				_directionalInputEventHandler.Input(1);
				_lastSanitizedDirectionalInput = _sanitizedDirectionalInput;
			}

			_movementController.UpdateMovement(deltaTime);
        }

        public override void _Ready()
        {
            _directionalInputEventHandler = new ThresholdedDelayedEventHandler(
			0, 
			_movementSpeed, 
			() => DelayedMovementUpdate()
			);
        }

		public void OnBButtonPressed()
		{
            _isBeforeCharging = true;
		}

		public void OnBButton()
		{
			_isBeforeCharging = true;
		}

		public void OnBButtonReleased()
		{
            if (_isPaused)
                return;

			if (_chargeVFXSpawned)
			{
				_chargeVFXController.Free();
			}

			_chargeVFXSpawned = false;
			_chargeVFXDelayedEventHandler.Reset();

            _chargingSFXPlayed = false;
            _chargedSFXPlayed = false;

			_isCharging = false;
            _isBeforeCharging = false;
            Game.Instance.GlobalVariables.SubmitFloat(GlobalVariableIDs.BUSTER_CURRENTCHARGETIME_ID, _chargeTime);
			_entityStateController.SetState(_busterShotState);
		}

		public void OnAButtonPressed()
		{
			if (Game.Instance.BattleSession.ChipsController.PeekChip() != null)
			{
                if (_isPaused)
                    return;

				if (_chargeVFXSpawned)
				{
					_chargeVFXController?.Free();
				}
				_chargeVFXSpawned = false;
				_chargeVFXDelayedEventHandler.Reset();
				_isCharging = false;

				_entityStateController.SetState(_chipState);
			}
		}

		public void DelayedMovementUpdate()
		{
			if (_movementController.CanMoveInDirection(_sanitizedDirectionalInput))
			{
				_playerAnimationController.PlayAnimation(AnimationID.MOVE_ANIMATION_ID);
				_playerAnimationController.OnAnimationEnded += OnInitialMovementAnimationEnded;
				_isMoving = true;
			}
		}

		private void OnInitialMovementAnimationEnded()
		{
			_movementController.TryMoveInDirection(_lastSanitizedDirectionalInput);
			_playerAnimationController.PlayAnimation(AnimationID.MOVE_ANIMATION_ID, true);
			_playerAnimationController.OnAnimationEnded -= OnInitialMovementAnimationEnded;
			_playerAnimationController.OnAnimationEnded += OnReverseMovementAnimationEnded;
		}

		private void OnReverseMovementAnimationEnded()
		{
			_playerAnimationController.OnAnimationEnded -= OnReverseMovementAnimationEnded;
			_playerAnimationController.PlayAnimation(AnimationID.IDLE_ANIMATION_ID, true);
			_isMoving = false;
		}
    }
}
