using Godot;
using MMBN.Gameplay.Entities.Animation;
using MMBN.Gameplay.Entities.AttackBehaviours;
using System;

namespace MMBN.Gameplay.Entities.EntityStateMachine
{
    public partial class AttackState : EntityState
    {

        [Export]
        private BaseAttackBehaviour[] _attackBehaviours;

		[Export]
		private EntityState _nextState;

        private int _attackBehaviourIndex = 0;

        public override void BeginState()
        {
            _attackBehaviourIndex = (int)GD.Randi() % _attackBehaviours.Length;

            _attackBehaviours[_attackBehaviourIndex].OnAttackFinished += OnAttackFinished;
            _attackBehaviours[_attackBehaviourIndex].BeginAttack();
        }

        public override void EndState()
        {
            _attackBehaviours[_attackBehaviourIndex].EndAttack();
        }

        public override string GetStateID()
        {
            return EntityState.ATTACK_STATE_ID;
        }

        public override void PauseState()
        {
            _attackBehaviours[_attackBehaviourIndex].PauseAttack();
        }

        public override void ContinueState()
        {
            _attackBehaviours[_attackBehaviourIndex].UnpauseAttack();
        }

        public override void UpdateState(float deltaTime)
        {
            _attackBehaviours[_attackBehaviourIndex].DoAttack(deltaTime);
        }

        private void OnAttackFinished()
        {
            _attackBehaviours[_attackBehaviourIndex].OnAttackFinished -= OnAttackFinished;
            _entityStateController.SetState(_nextState);
            _entityStateController.OnAttackStateFinished?.Invoke();
        }
    }
}
