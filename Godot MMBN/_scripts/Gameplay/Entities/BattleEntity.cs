using Godot;
using MMBN.Gameplay.Battle;
using MMBN.Gameplay.Battle.BattleStateMachine;
using MMBN.Gameplay.Entities.Animation;
using MMBN.Gameplay.Entities.EntityStateMachine;
using System;

namespace MMBN.Gameplay.Entities
{
	public partial class BattleEntity : Node2D
	{
		public enum BattleEntityType {PLAYER, NEUTRAL, ENEMY}

		[Export]
		private string _battleEntityID;
		public string BattleEntityID { get { return _battleEntityID; }}

		[Export]
		private HealthController _healthController;
		[Export]
		private MovementControllerBase _movementController;
		[Export]
		private EntityStateController _stateMachine;
		[Export]
		private EntityAnimationController _animationController;
		[Export]
		private bool _interactable;
		public bool Interactable { get { return _interactable; } }

		[Export]
		protected BattleEntityType _entityType = BattleEntityType.NEUTRAL;
		public BattleEntityType EntityType { get { return _entityType; } }

		public HealthController HealthController { get { return _healthController; }}
		public MovementControllerBase MovementController { get { return _movementController; }}
		public EntityStateController StateMachine { get { return _stateMachine; }}
		public EntityAnimationController AnimationController { get { return _animationController; }}

		public void Init(BattleGrid grid, Vector2 initPosition)
		{
			_movementController?.Init(grid, initPosition, this);
			_animationController?.Init();
			_stateMachine?.Init();
            _healthController?.Init(this);

            _healthController.OnHealthReachedZero += () => _interactable = false;
            _healthController.OnDamageDealt += OnDamageDealt;
		}

        private void OnDamageDealt()
        {
            if (!Game.Instance.BattleSession.GetCurrentStateID().Equals(BattleFreezeChipState.STATE_ID))
            {
                _animationController.SetSpriteWhiteDuration(EntityGlobalConstants.ENTITY_HIT_FLASH_DURATION);
            }
        }

        public void SetDead()
        {
            _interactable = false;
            _animationController.SetSpriteWhite(true);
        }
	}
}
