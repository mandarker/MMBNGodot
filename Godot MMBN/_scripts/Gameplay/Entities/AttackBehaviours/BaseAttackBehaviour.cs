using Godot;
using System;
using MMBN.Gameplay.Entities.Animation;

namespace MMBN.Gameplay.Entities.AttackBehaviours
{
	public abstract partial class BaseAttackBehaviour : Node
	{
		[Export]
		protected EntityAnimationController _animatorController;

		[Export]
		protected BattleEntity _currentEntity;

		public Action OnAttackFinished;
		protected bool _isPaused;

		public abstract void BeginAttack();
        public abstract void EndAttack();
		public virtual void DoAttack(float deltaTime)
		{
			if (_isPaused)
			{
				return;
			}
		}
		public void PauseAttack()
		{
			_isPaused = true;
            _animatorController.SetAnimationPaused(true);
		}
		public void UnpauseAttack()
		{
			_isPaused = false;
            _animatorController.SetAnimationPaused(false);
		}
	}
}
