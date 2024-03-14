using Godot;
using MMBN.Gameplay.Chips;
using MMBN.Gameplay.Entities.Animation;
using MMBN.Gameplay.Entities;
using MMBN.Utility;
using MMBN.VFX;
using MMBN;
using System;
using System.Collections.Generic;

namespace MMBN.Gameplay.Chips
{
    public partial class SwordChipBase : ChipBase
    {
        private const float _attackDelay = 0.15f;

        private DelayedEventHandler _swordDelayedAttackEventHandler;

        private const int SWORD_CHIP_SFX_ID = 0;
        private const int SWORD_CHIP_HIT_SFX_ID = 1;

        private const int SWORDSLASH_VFX_OFFSET_X = 45;
        private const int SWORDSLASH_VFX_OFFSET_Y = -30;

        public override void StartChip(EntityAnimationController playerAnimationController, BattleEntity playerEntity)
        {
            base.StartChip(playerAnimationController, playerEntity);

            _animationController.PlayAnimation(AnimationID.PLAYER_SWORD_SLASH_ANIMATION_ID);
            _animationController.OnAnimationEnded += OnAnimationEnded;

            _swordDelayedAttackEventHandler = new DelayedEventHandler(
                _attackDelay,
                OnDelayedAttack,
                false
            );
        }

        public override void EndChip()
        {

        }

        public override void RunChip(float deltaTime)
        {
            _swordDelayedAttackEventHandler.Update(deltaTime);
        }

        private void OnDelayedAttack()
        {
            // deal damage
            Vector2 pos = _playerEntity.MovementController.TilePosition;

            AttackData swordAttackData = new AttackData(_chipDataResource, _playerEntity.BattleEntityID);

            List<BattleEntity> entities = Game.Instance.BattleSession.GetEntitiesAtPosition(new Vector2(pos.X + 1, pos.Y));

            Game.Instance.SFXManager.PlaySFX(_chipDataResource.AudioStreams[SWORD_CHIP_SFX_ID]);

            Vector2 spawnPosition = _playerEntity.Position + new Vector2(SWORDSLASH_VFX_OFFSET_X, SWORDSLASH_VFX_OFFSET_Y);
            AnimatedVFXController _swordSlashVFXController = VFXGeneratorHelper.GenerateBattleVFX(VFXGeneratorHelper.MEGAMAN_SWORDSLASH_ID, spawnPosition);

            _swordSlashVFXController.ZIndex = _playerEntity.ZIndex;
            _swordSlashVFXController.OnVFXFinished += _swordSlashVFXController.Free;

            for (int j = 0; j < entities.Count; ++j)
            {
                if (entities[j].Interactable)
                {
                    Game.Instance.SFXManager.PlaySFX(_chipDataResource.AudioStreams[SWORD_CHIP_HIT_SFX_ID]);
                    entities[j].HealthController.DealDamage(swordAttackData);
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
