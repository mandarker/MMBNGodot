using Godot;
using MMBN.Gameplay.Battle;
using MMBN.Gameplay.Chips;
using MMBN.Gameplay.Entities.Animation;
using MMBN.Utility;
using MMBN.VFX;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace MMBN.Gameplay.Entities.AttackBehaviours
{
    public partial class BusterAttackBehaviour : BaseAttackBehaviour
    {
		[Export]
		private float _attackSpawnDelay = 0.15f;

		[Export]
		private int _busterAttackPower = 1;
		
		[Export]
		private int _busterAttackChargeMultiplier = 10;

        [Export]
        private AudioStreamPlayer _busterAttackAudioStreamPlayer;

        [Export]
        private AudioStreamPlayer _busterHitAudioStreamPlayer;
		
		private DelayedEventHandler _delayedShotEventHandler;

        public override void BeginAttack()
        {
            _animatorController.PlayAnimation(AnimationID.PLAYER_BUSTER_SHOT_ANIMATION_ID);
            _animatorController.OnAnimationEnded += OnAnimationEnded;

			_delayedShotEventHandler = new DelayedEventHandler(
				_attackSpawnDelay,
				OnDelayedAttack,
				false
				);
        }

        public override void EndAttack()
        {

        }

        public override void DoAttack(float deltaTime)
        {
			base.DoAttack(deltaTime);

			_delayedShotEventHandler.Update(deltaTime);
        }

        public void OnAnimationEnded()
        {
            _animatorController.OnAnimationEnded -= OnAnimationEnded;
			OnAttackFinished?.Invoke();
        }

		private void OnDelayedAttack()
		{
			// deal damage
			Vector2 pos = _currentEntity.MovementController.TilePosition;
			bool entityFound = false;

			int calculatedDamage = _busterAttackPower;

			if (Game.Instance.GlobalVariables.GetFloat(GlobalVariableIDs.BUSTER_CURRENTCHARGETIME_ID, out float value))
			{
				if (Game.Instance.GlobalVariables.GetFloat(GlobalVariableIDs.BUSTER_CHARGETIME_ID, out float chargeTime))
				{
					if (value > chargeTime)
					{
						calculatedDamage *= _busterAttackChargeMultiplier;
					}
				}
			}

            // play the sound
            _busterAttackAudioStreamPlayer.Seek(0);
            _busterAttackAudioStreamPlayer.Play();

            AttackData attackData = new AttackData((uint)calculatedDamage, _currentEntity.BattleEntityID, AttackData.DamageType.NEUTRAL);

			for (int i = (int)pos.X + 1; i < 6; ++i)
			{
				List<BattleEntity> entities = Game.Instance.BattleSession.GetEntitiesAtPosition(new Vector2(i, pos.Y));

				for (int j = 0; j < entities.Count; ++j)
				{
					if (!entityFound)
					{
						if (entities[j].Interactable)
						{
							entities[j].HealthController.DealDamage(attackData);
							entityFound = true;

							Vector2 spawnPosition = entities[j].Position + new Vector2(GD.RandRange(-10, 10), GD.RandRange(-20, -30));
							AnimatedVFXController vfxController = VFXGeneratorHelper.GenerateBattleVFX(VFXGeneratorHelper.BusterHitID, spawnPosition);
							
                            vfxController.ZIndex = entities[j].ZIndex;
							vfxController.OnVFXFinished += vfxController.Free;

                            // play the sound
                            _busterHitAudioStreamPlayer.Seek(0);
                            _busterHitAudioStreamPlayer.Play();
						}
					}
				}
			}
		}
    }
}
