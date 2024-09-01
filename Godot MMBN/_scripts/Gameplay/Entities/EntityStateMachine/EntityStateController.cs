using Godot;
using MMBN.Utility;
using System;

namespace MMBN.Gameplay.Entities.EntityStateMachine
{
	public partial class EntityStateController : Node
	{
		[Export]
		private EntityState[] _states;

		[Export]
		private EntityState _deathState;

		[Export]
		private BattleEntity _currentEntity;

		private EntityState _currentState = null;

		public string CurrentStateID => _currentState.GetStateID();

		private LockedBoolean _isStateMachinePaused;

		public Action OnAttackFinished;
		public Action OnAttackStateFinished;

        public void Init()
        {
			_isStateMachinePaused = new LockedBoolean();

			_currentEntity.HealthController.OnHealthReachedZero += () => SetState(_deathState);

            foreach (EntityState state in _states)
			{
				state.Init(this);
			}

			if (_states.Length > 0)
			{
				SetState(_states[0]);
			}
        }

		public void SetState(EntityState state)
		{
			if (state == null)
			{
				return;
			}

			if (_currentState != null)
			{
				_currentState.EndState();
			}

			_currentState = state;

			state.BeginState();
		}

		public void UpdateStateMachine(float deltaTime)
		{
			_currentState.UpdateState(deltaTime);
		}

		public void PauseStateMachine(Object obj)
		{
			_isStateMachinePaused.SubmitLocker(obj);
			_currentState.PauseState();
		}

		public void UnpauseStateMachine(Object obj)
		{
			_isStateMachinePaused.RemoveLocker(obj);

            if (!_isStateMachinePaused.IsLocked)
			    _currentState.ContinueState();
		}
    }
}