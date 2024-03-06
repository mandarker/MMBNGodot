using Godot;
using MMBN.Gameplay.Entities.Animation;
using MMBN.Gameplay.Entities.StatusEffects;
using MMBN.Utility;
using System;

namespace MMBN.Gameplay.Entities.EntityStateMachine
{
    public partial class DamagedState : EntityState
    {
        [Export]
        private HealthController _healthController;
        [Export]
        private EntityAnimationController _entityAnimationController;
        [Export]
        private EntityState _movementState;
        [Export]
        private AudioStreamPlayer _hurtAudioStreamPlayer;

        [Export]
        private float _invincibilityDuration;

        public override void BeginState()
        {
            // apply invincibility
            InvincibleStatusEffect invincibleStatus = new InvincibleStatusEffect(_invincibilityDuration, GetStateID(),  _healthController.StatusManager);
            invincibleStatus.OnStatusEnded += () => _healthController.StatusManager.RemoveStatus(invincibleStatus);
            _healthController.StatusManager.ApplyStatus(invincibleStatus);

            // change the animation turn into the damaged animation
            _entityAnimationController.PlayAnimation(AnimationID.DAMAGED_ANIMATION_ID);
            _entityAnimationController.OnAnimationEnded += SetNextState;

            // make player sprite flash
            _entityAnimationController.SetSpriteFlashDuration(_invincibilityDuration);

            _hurtAudioStreamPlayer.Seek(0);
            _hurtAudioStreamPlayer.Play();
        }

        private void SetNextState()
        {
            _entityStateController.SetState(_movementState);
        }

        public override void ContinueState()
        {

        }

        public override void EndState()
        {
            _entityAnimationController.OnAnimationEnded -= SetNextState;
        }

        public override string GetStateID()
        {
            return EntityState.DAMAGED_STATE_ID;
        }

        public override void PauseState()
        {

        }

        public override void UpdateState(float deltaTime)
        {

        }
    }
}
