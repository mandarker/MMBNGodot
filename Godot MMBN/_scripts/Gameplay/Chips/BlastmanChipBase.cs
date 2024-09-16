using Godot;
using MMBN.Gameplay.Entities;
using MMBN.Utility;
using MMBN.VFX;
using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;

namespace MMBN.Gameplay.Chips
{
    public partial class BlastmanChipBase : FreezeChipBase
    {
        AnimatedVFXController _blastmanVFXController;
        AnimatedVFXController _blastmanShotVFXController1;
        AnimatedVFXController _blastmanShotVFXController2;
        AnimatedVFXController _blastmanShotVFXController3;

        private DelayedEventHandler _playerReappearDelayedEventHandler;

        private DelayedEventHandler _blastmanShotDelayedEventHandler;
        private DelayedEventHandler _blastmanAppearSFXDelayedEventHandler;

        private float _waitBeforeDimming = 0.5f;

        private bool _isChipStillRunning;
        private bool _isShooting;

        private float _blastmanVFXOffset_X = 0f;
        private float _blastmanVFXOffset_Y = -10f;

        private const int BLASTMAN_SHOT_SFX_ID = 0;
        private const int BLASTMAN_HIT_SFX_ID = 1;
        private const int BLASTMAN_APPEAR_SFX_ID = 2;

        private const float BLASTMAN_SHOT_SPEED = 240f;
        private float _shotXPosition;

        public override void BeforeRunChip()
        {
            _playerEntity.AnimationController.PlayAnimation("Move");
            _playerEntity.AnimationController.OnAnimationEnded += OnPlayerDisappearAnimationEnded;

            _playerEntity.AnimationController.SetAnimationPaused(false);

            _playerReappearDelayedEventHandler = new DelayedEventHandler(
                _waitBeforeDimming,
                () => OnChipFinished?.Invoke(),
                false
                );

            _blastmanAppearSFXDelayedEventHandler = new DelayedEventHandler(
                _chipDataResource.AudioOffsets[BLASTMAN_APPEAR_SFX_ID],
                () => Game.Instance.SFXManager.PlaySFX(_chipDataResource.AudioStreams[BLASTMAN_APPEAR_SFX_ID]),
                false
                );

            _blastmanShotDelayedEventHandler = new DelayedEventHandler(
                _chipDataResource.AudioOffsets[BLASTMAN_SHOT_SFX_ID],
                () => {
                    Game.Instance.SFXManager.PlaySFX(_chipDataResource.AudioStreams[BLASTMAN_SHOT_SFX_ID]);
                    _isShooting = true;
                    },
                false
                );

            _blastmanShotVFXController1 = VFXGeneratorHelper.GenerateBattleVFX(
                VFXGeneratorHelper.NAVI_BLASTMANPROJECTILE_ID,
                Game.Instance.BattleSession.BattleGrid.TilePositionToWorldPosition(new Vector2(0, _playerEntity.MovementController.TilePosition.Y - 1)) - new Vector2(64, 0)
                );
            if (_playerEntity.MovementController.TilePosition.Y == 0)
            {
                _blastmanShotVFXController1.Visible = false;
            }
            _blastmanShotVFXController2 = VFXGeneratorHelper.GenerateBattleVFX(
                VFXGeneratorHelper.NAVI_BLASTMANPROJECTILE_ID,
                Game.Instance.BattleSession.BattleGrid.TilePositionToWorldPosition(new Vector2(0, _playerEntity.MovementController.TilePosition.Y)) - new Vector2(64, 0)
                );
            _blastmanShotVFXController3 = VFXGeneratorHelper.GenerateBattleVFX(
                VFXGeneratorHelper.NAVI_BLASTMANPROJECTILE_ID,
                Game.Instance.BattleSession.BattleGrid.TilePositionToWorldPosition(new Vector2(0, _playerEntity.MovementController.TilePosition.Y + 1)) - new Vector2(64, 0)
                );
            if (_playerEntity.MovementController.TilePosition.Y == 2)
            {
                _blastmanShotVFXController3.Visible = false;
            }

            _shotXPosition = -0.5f;

            _isChipStillRunning = true;
            _isShooting = false;
        }

        public override void EndChip()
        {
            if (_blastmanShotVFXController1 != null
                && GodotObject.IsInstanceValid(_blastmanShotVFXController1)
                && !_blastmanShotVFXController1.IsQueuedForDeletion())
            {
                VFXGeneratorHelper.FreeBattleVFX(_blastmanShotVFXController1);
                VFXGeneratorHelper.FreeBattleVFX(_blastmanShotVFXController2);
                VFXGeneratorHelper.FreeBattleVFX(_blastmanShotVFXController3);
            }
        }

        public override void RunChip(float deltaTime)
        {
            if (_isChipStillRunning)
            {
                _playerEntity.AnimationController.Update(deltaTime);

                _blastmanShotDelayedEventHandler.Update(deltaTime);
                _blastmanAppearSFXDelayedEventHandler.Update(deltaTime);

                if (_isShooting)
                {
                    _blastmanShotVFXController1.Position += new Vector2(BLASTMAN_SHOT_SPEED * deltaTime, 0);
                    _blastmanShotVFXController2.Position += new Vector2(BLASTMAN_SHOT_SPEED * deltaTime, 0);
                    _blastmanShotVFXController3.Position += new Vector2(BLASTMAN_SHOT_SPEED * deltaTime, 0);

                    // calculate based on tile width
                    _shotXPosition += (BLASTMAN_SHOT_SPEED * deltaTime) / 40;

                    bool entityHit = false;

                    if (_blastmanShotVFXController1.Visible)
                    {
                        Vector2 tilePosition = new Vector2((int)_shotXPosition, _playerEntity.MovementController.TilePosition.Y - 1);
                        Game.Instance.BattleSession.UnhighlightGridTile(tilePosition - new Vector2(1, 0), this);
                        Game.Instance.BattleSession.HighlightGridTile(tilePosition, this);
                        List<BattleEntity> entities = Game.Instance.BattleSession.GetEntitiesAtPosition(tilePosition);
                        entities.RemoveAll(entity => entity.EntityType == BattleEntity.BattleEntityType.PLAYER || !entity.Interactable);

                        if (entities.Count != 0)
                        {
                            _blastmanShotVFXController1.Visible = false;
                            AnimatedVFXController blastmanHitVFXController = VFXGeneratorHelper.GenerateBattleVFX(
                                VFXGeneratorHelper.NAVI_BLASTMANHIT_ID,
                                _blastmanShotVFXController1.Position + new Vector2(GD.RandRange(10, 20), GD.RandRange(-10, 10))
                                );
                            blastmanHitVFXController.OnVFXFinished += blastmanHitVFXController.Free;

                            entityHit = true;

                            AttackData blastmanAttackData = new AttackData(_chipDataResource, _playerEntity.BattleEntityID);

                            foreach (var entity in entities)
                            {
                                entity.HealthController.DealDamage(blastmanAttackData);
                            }

                            Game.Instance.BattleSession.UnhighlightGridTile(tilePosition, this);
                        }
                    }
                    if (_blastmanShotVFXController2.Visible)
                    {
                        Vector2 tilePosition = new Vector2((int)_shotXPosition, _playerEntity.MovementController.TilePosition.Y);
                        Game.Instance.BattleSession.UnhighlightGridTile(tilePosition - new Vector2(1, 0), this);
                        Game.Instance.BattleSession.HighlightGridTile(tilePosition, this);
                        List<BattleEntity> entities = Game.Instance.BattleSession.GetEntitiesAtPosition(tilePosition);
                        entities.RemoveAll(entity => entity.EntityType == BattleEntity.BattleEntityType.PLAYER || !entity.Interactable);

                        if (entities.Count != 0)
                        {
                            _blastmanShotVFXController2.Visible = false;
                            AnimatedVFXController blastmanHitVFXController = VFXGeneratorHelper.GenerateBattleVFX(
                                VFXGeneratorHelper.NAVI_BLASTMANHIT_ID,
                                _blastmanShotVFXController2.Position + new Vector2(GD.RandRange(10, 20), GD.RandRange(-10, 10))
                                );
                            blastmanHitVFXController.OnVFXFinished += blastmanHitVFXController.Free;

                            entityHit = true;

                            AttackData blastmanAttackData = new AttackData(_chipDataResource, _playerEntity.BattleEntityID);

                            foreach (var entity in entities)
                            {
                                entity.HealthController.DealDamage(blastmanAttackData);
                            }

                            Game.Instance.BattleSession.UnhighlightGridTile(tilePosition, this);
                        }
                    }
                    if (_blastmanShotVFXController3.Visible)
                    {
                        Vector2 tilePosition = new Vector2((int)_shotXPosition, _playerEntity.MovementController.TilePosition.Y + 1);
                        Game.Instance.BattleSession.UnhighlightGridTile(tilePosition - new Vector2(1, 0), this);
                        Game.Instance.BattleSession.HighlightGridTile(tilePosition, this);
                        List<BattleEntity> entities = Game.Instance.BattleSession.GetEntitiesAtPosition(tilePosition);
                        entities.RemoveAll(entity => entity.EntityType == BattleEntity.BattleEntityType.PLAYER || !entity.Interactable);

                        if (entities.Count != 0)
                        {
                            _blastmanShotVFXController3.Visible = false;
                            AnimatedVFXController blastmanHitVFXController = VFXGeneratorHelper.GenerateBattleVFX(
                                VFXGeneratorHelper.NAVI_BLASTMANHIT_ID,
                                _blastmanShotVFXController3.Position + new Vector2(GD.RandRange(10, 20), GD.RandRange(-10, 10))
                                );
                            blastmanHitVFXController.OnVFXFinished += blastmanHitVFXController.Free;

                            entityHit = true;

                            AttackData blastmanAttackData = new AttackData(_chipDataResource, _playerEntity.BattleEntityID);

                            foreach (var entity in entities)
                            {
                                entity.HealthController.DealDamage(blastmanAttackData);
                            }

                            Game.Instance.BattleSession.UnhighlightGridTile(tilePosition, this);
                        }
                    }

                    if (entityHit)
                    {
                        Game.Instance.SFXManager.PlaySFX(_chipDataResource.AudioStreams[BLASTMAN_HIT_SFX_ID]);
                    }
                }
            }
            else
            {
                _playerReappearDelayedEventHandler.Update(deltaTime);
            }
        }

        private void OnPlayerDisappearAnimationEnded()
        {
            _playerEntity.AnimationController.OnAnimationEnded -= OnPlayerDisappearAnimationEnded;
            _playerEntity.Visible = false;

            _blastmanVFXController = VFXGeneratorHelper.GenerateBattleVFX(
                VFXGeneratorHelper.NAVI_BLASTMANSPAWN_ID,
                _playerEntity.Position + new Vector2(_blastmanVFXOffset_X, _blastmanVFXOffset_Y)
                );
            _blastmanVFXController.OnVFXFinished += OnBlastmanVFXFinished;
        }

        private void OnBlastmanVFXFinished()
        {
            _blastmanVFXController.Free();

            _playerEntity.Visible = true;
            _playerEntity.AnimationController.PlayAnimation("Move", true);
            _playerEntity.AnimationController.OnAnimationEnded += OnPlayerReappearAnimationEnded;
        }

        private void OnPlayerReappearAnimationEnded()
        {
            _playerEntity.AnimationController.OnAnimationEnded -= OnPlayerReappearAnimationEnded;
            _isChipStillRunning = false;
        }
    }
}
