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
    public partial class SpreaderChipBase : ChipBase
    {
        private const float _attackDelay = 0.05f;

        private DelayedEventHandler _spreaderDelayedAttackEventHandler;

        private const int SPREADER_CHIP_SFX_ID = 0;
        private const int SPREADER_CHIP_HIT_SFX_ID = 1;

        private const int SPREADER_VFX_OFFSET_X = 19;
        private const int SPREADER_VFX_OFFSET_Y = -27;

        private AnimatedVFXController _spreaderVFXController;

        public override void StartChip(EntityAnimationController playerAnimationController, BattleEntity playerEntity)
        {
            base.StartChip(playerAnimationController, playerEntity);

            _animationController.PlayAnimation(AnimationID.PLAYER_SPREADER_SHOT_ANIMATION_ID);
            _animationController.OnAnimationEnded += OnAnimationEnded;

            _spreaderDelayedAttackEventHandler = new DelayedEventHandler(
                _attackDelay,
                OnDelayedAttack,
                false
            );

            Vector2 spawnPosition = playerEntity.Position + new Vector2(SPREADER_VFX_OFFSET_X, SPREADER_VFX_OFFSET_Y);
            _spreaderVFXController = VFXGeneratorHelper.GenerateBattleVFX(VFXGeneratorHelper.MEGAMAN_SPREADERSHOT_ID, spawnPosition);

            _spreaderVFXController.ZIndex = playerEntity.ZIndex + 1;
            _spreaderVFXController.OnVFXFinished += _spreaderVFXController.Free;
        }

        public override void EndChip()
        {
            if (_spreaderVFXController != null
                && GodotObject.IsInstanceValid(_spreaderVFXController)
                && !_spreaderVFXController.IsQueuedForDeletion())
            {
                _spreaderVFXController.Visible = false;
            }
        }

        public override void RunChip(float deltaTime)
        {
            _spreaderDelayedAttackEventHandler.Update(deltaTime);
        }

        private void OnDelayedAttack()
        {
            // deal damage
            Vector2 pos = _playerEntity.MovementController.TilePosition;
            bool entityFound = false;

            AttackData spreaderAttackData = new AttackData(_chipDataResource, _playerEntity.BattleEntityID);

            // play sound effect of the cannon shot
            Game.Instance.SFXManager.PlaySFX(_chipDataResource.AudioStreams[SPREADER_CHIP_SFX_ID]);

            Vector2 centerPosition = Vector2.Zero;

            for (int i = (int)pos.X + 1; i < 6; ++i)
            {
                List<BattleEntity> entities = Game.Instance.BattleSession.GetEntitiesAtPosition(new Vector2(i, pos.Y));

                for (int j = 0; j < entities.Count; ++j)
                {
                    if (!entityFound)
                    {
                        if (entities[j].Interactable)
                        {
                            centerPosition = new Vector2(i, pos.Y);

                            Game.Instance.SFXManager.PlaySFX(_chipDataResource.AudioStreams[SPREADER_CHIP_HIT_SFX_ID]);

                            entityFound = true;
                        }
                    }
                }
            }

            if (entityFound)
            {
                for (int x = -1; x <=1; ++x)
                {
                    for (int y = -1; y <=1; ++y)
                    {
                        Vector2 offsetPosition = centerPosition + new Vector2(x, y);

                        if (Game.Instance.BattleSession.BattleGrid.IsTilePositionInBounds(offsetPosition))
                        {
                            List<BattleEntity> entities = Game.Instance.BattleSession.GetEntitiesAtPosition(offsetPosition);

                            foreach (BattleEntity entity in entities)
                            {
                                entity.HealthController.DealDamage(spreaderAttackData);
                            }
                        }

                        AnimatedVFXController vfxController = VFXGeneratorHelper.GenerateBattleVFX(
                            VFXGeneratorHelper.MEGAMAN_SPREADERHIT_ID,
                            Game.Instance.BattleSession.BattleGrid.TilePositionToWorldPosition(offsetPosition)
                            );
                        vfxController.OnVFXFinished += vfxController.Free;
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