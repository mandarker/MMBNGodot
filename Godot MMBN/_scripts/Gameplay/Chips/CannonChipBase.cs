using Godot;
using MMBN.Gameplay.Entities;
using MMBN.Gameplay.Entities.Animation;
using MMBN.Utility;
using MMBN.VFX;
using System;
using System.Collections.Generic;

namespace MMBN.Gameplay.Chips
{
    public partial class CannonChipBase : ChipBase
    {
        private const float _attackDelay = 0.33f;

        private DelayedEventHandler _cannonDelayedAttackEventHandler;

        private const int CANNON_CHIP_SFX_ID = 0;
        private const int CANNON_CHIP_HIT_SFX_ID = 1;

        private const int CANNON_VFX_OFFSET_X = 45;
        private const int CANNON_VFX_OFFSET_Y = -30;

        private AnimatedVFXController _cannonVFXController;

        public override void StartChip(EntityAnimationController playerAnimationController, BattleEntity playerEntity)
        {
            base.StartChip(playerAnimationController, playerEntity);

            _animationController.PlayAnimation(AnimationID.PLAYER_CANNON_SHOT_ANIMATION_ID);
            _animationController.OnAnimationEnded += OnAnimationEnded;

            _cannonDelayedAttackEventHandler = new DelayedEventHandler(
                _attackDelay,
                OnDelayedAttack,
                false
            );

            Vector2 spawnPosition = playerEntity.Position + new Vector2(CANNON_VFX_OFFSET_X, CANNON_VFX_OFFSET_Y);
            _cannonVFXController = VFXGeneratorHelper.GenerateBattleVFX(VFXGeneratorHelper.MEGAMAN_CANNONSHOT_ID, spawnPosition);

            _cannonVFXController.ZIndex = playerEntity.ZIndex + 1;
            _cannonVFXController.OnVFXFinished += _cannonVFXController.Free;
        }

        public override void EndChip()
        {
            if (_cannonVFXController != null
                && GodotObject.IsInstanceValid(_cannonVFXController)
                && !_cannonVFXController.IsQueuedForDeletion())
            {
                _cannonVFXController.Visible = false;
            }
        }

        public override void RunChip(float deltaTime)
        {
            _cannonDelayedAttackEventHandler.Update(deltaTime);
        }

        private void OnDelayedAttack()
        {
            // deal damage
			Vector2 pos = _playerEntity.MovementController.TilePosition;
			bool entityFound = false;

            AttackData cannonAttackData = new AttackData(_chipDataResource, _playerEntity.BattleEntityID);

            // play sound effect of the cannon shot
            Game.Instance.SFXManager.PlaySFX(_chipDataResource.AudioStreams[CANNON_CHIP_SFX_ID]);

			for (int i = (int)pos.X + 1; i < 6; ++i)
			{
				List<BattleEntity> entities = Game.Instance.BattleSession.GetEntitiesAtPosition(new Vector2(i, pos.Y));

				for (int j = 0; j < entities.Count; ++j)
				{
					if (!entityFound)
					{
						if (entities[j].Interactable)
						{
							entities[j].HealthController.DealDamage(cannonAttackData);

                            // play sound effect of the cannon hit
                            Game.Instance.SFXManager.PlaySFX(_chipDataResource.AudioStreams[CANNON_CHIP_HIT_SFX_ID]);

                            Vector2 spawnPosition = entities[j].Position + new Vector2(GD.RandRange(-10, 10), GD.RandRange(-20, -30));
                            AnimatedVFXController vfxController = VFXGeneratorHelper.GenerateBattleVFX(VFXGeneratorHelper.ExplosionID, spawnPosition);

                            vfxController.ZIndex = entities[j].ZIndex;
                            vfxController.OnVFXFinished += vfxController.Free;

                            entityFound = true;
						}
					}
				}
			}
        }

        private void OnAnimationEnded()
        {
            _animationController.OnAnimationEnded -= OnAnimationEnded;
            OnChipFinished?.Invoke();
        }
    }
}
