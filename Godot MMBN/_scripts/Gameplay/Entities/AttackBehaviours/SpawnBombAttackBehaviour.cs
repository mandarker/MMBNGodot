using Godot;
using MMBN.Gameplay.Chips;
using MMBN.Gameplay.Entities.Animation;
using MMBN.Gameplay.Entities.EntityStateMachine;
using MMBN.Utility;
using System;
using System.Collections.Generic;

namespace MMBN.Gameplay.Entities.AttackBehaviours
{
    public partial class SpawnBombAttackBehaviour : BaseAttackBehaviour
    {
        [Export]
        private float _attackDelay;

        [Export]
        private float _intangibleDelay;

        [Export]
        private float _nonintangibleDelay;

        [Export]
        private float _moveDelay;

        [Export]
        private EntityState _idleState;

        private DelayedEventHandler _spawnBombEventHandler;
        private DelayedEventHandler _intangibleEventHandler;
        private DelayedEventHandler _nonIntangibleEventHandler;
        private DelayedEventHandler _moveDelayedEventHandler;

        private BattleEntity _bombEntity;

        private readonly string BOMBOUT_ANIMATION_ID = "BombOut";

        public override void BeginAttack()
        {
            _animatorController.PlayAnimation(AnimationID.ATTACK_ANIMATION_ID);
            _animatorController.OnAnimationEnded += OnAnimationEnded;

            _spawnBombEventHandler = new DelayedEventHandler(
                _attackDelay,
                OnDelayedAttack,
                false
                );

            _intangibleEventHandler = new DelayedEventHandler(
                _intangibleDelay,
                OnIntangible,
                false
                );

            _nonIntangibleEventHandler = new DelayedEventHandler(
                _nonintangibleDelay,
                OnNonIntangible,
                false
                );

            _moveDelayedEventHandler = new DelayedEventHandler(
                _moveDelay,
                OnDelayedMove,
                false
                );
        }

        public override void DoAttack(float deltaTime)
        {
            base.DoAttack(deltaTime);

            _spawnBombEventHandler.Update(deltaTime);
            _intangibleEventHandler.Update(deltaTime);
            _nonIntangibleEventHandler.Update(deltaTime);
            _moveDelayedEventHandler.Update(deltaTime);
        }

        private void OnAnimationEnded()
        {
            _animatorController.PlayAnimation(BOMBOUT_ANIMATION_ID);
        }

        private void OnDelayedAttack()
        {
            Vector2 spawnPosition = _currentEntity.MovementController.TilePosition + new Vector2(-1, 0);

            AttackData bombSpawnAttackData = new AttackData(10, _currentEntity.BattleEntityID, AttackData.DamageType.NONE);
            List<BattleEntity> entities = Game.Instance.BattleSession.GetEntitiesAtPosition(spawnPosition);
            if (entities.Count > 0)
            {
                foreach (var entity in entities)
                {
                    if (entity.EntityType != BattleEntity.BattleEntityType.ENEMY)
                    {
                        entity.HealthController.DealDamage(bombSpawnAttackData);

                        entity.StateMachine.SetState(_idleState);
                    }
                }
            }
            else
            {
                _bombEntity = BattleEntityGeneratorHelper.GenerateEntity(BattleEntityGeneratorHelper.HandyBombEntityID);
                Game.Instance.BattleSession.SpawnEntity(_bombEntity, spawnPosition);

                _bombEntity.OnEntityDeath += OnEntityDeath;
                _bombEntity.AnimationController.OnAnimationEnded += OnBombExplosion;
            }
        }

        private void OnIntangible()
        {
            if (_bombEntity != null)
            {
                _currentEntity.Interactable = false;
            }
        }

        private void OnNonIntangible()
        {
            if (_bombEntity != null)
            {
                _currentEntity.Interactable = true;
            }
        }

        private void OnDelayedMove()
        {
            List<Vector2> potentialMovementTiles = new List<Vector2>();
            
            for (int i = 5; i >= 1; --i)
            {
                for (int j = 0; j < 3; ++j)
                {
                    Vector2 tilePosition = new Vector2(i, j);
                    if (Game.Instance.BattleSession.BattleGrid.TryGetTile(tilePosition, out BattleGridTile movementTile))
                    {
                        if (Game.Instance.BattleSession.BattleGrid.TryGetTile(tilePosition + new Vector2(-1, 0), out BattleGridTile bombTile))
                        {
                            if (!movementTile.IsPlayerTile && bombTile.IsPlayerTile && movementTile.GetEntities().Count == 0)
                            {
                                potentialMovementTiles.Add(tilePosition);
                            }
                        }
                    }
                }
            }

            int rand = GD.RandRange(0, potentialMovementTiles.Count - 1);

            _currentEntity.MovementController.TryMovetoPosition(potentialMovementTiles[rand]);
        }

        private void OnBombExplosion()
        {
            AttackData explosionAttackData = new AttackData(50, _currentEntity.BattleEntityID, AttackData.DamageType.NONE);

            for (int i = 0; i < 6; ++i)
            {
                for (int j = 0; j < 3; ++j)
                {
                    Vector2 tilePosition = new Vector2(i, j);
                    Game.Instance.BattleSession.BattleGrid.TryGetTile(tilePosition, out BattleGridTile tile);

                    if (tile.IsPlayerTile)
                    {
                        List<BattleEntity> battleEntities = Game.Instance.BattleSession.GetEntitiesAtPosition(tilePosition);

                        foreach (var entity in battleEntities)
                        {
                            if (entity.EntityType == BattleEntity.BattleEntityType.PLAYER)
                            {
                                entity.HealthController.DealDamage(explosionAttackData);
                            }
                        }
                    }
                }
            }

            _animatorController.OnAnimationEnded = null;
            _currentEntity.StateMachine.SetState(_idleState);

            Game.Instance.BattleSession.RemoveEntity(_bombEntity);
            _bombEntity.Free();
        }

        private void OnEntityDeath()
        {
            _animatorController.OnAnimationEnded = null;
            _currentEntity.StateMachine.SetState(_idleState);
        }

        public override void EndAttack()
        {

        }
    }
}
