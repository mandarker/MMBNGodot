using Godot;
using MMBN.Gameplay.Chips;
using MMBN.Gameplay.Entities.Animation;
using MMBN.Utility;
using System;
using System.Collections.Generic;

namespace MMBN.Gameplay.Entities.AttackBehaviours
{
    public partial class SpawnMettaurWaveAttackBehaviour : BaseAttackBehaviour
    {
		[Export]
		private bool _removeEntity = false;

		[Export]
		private float _attackSpawnDelay = 0.15f;

        [Export]
        private int _attackDamage = 10;
		
		private DelayedEventHandler _spawnAttackEventHandler;

        private HashSet<BattleEntity> _damagedEntities;

        public override void BeginAttack()
        {
            _animatorController.PlayAnimation(AnimationID.ATTACK_ANIMATION_ID);
            _animatorController.OnAnimationEnded += OnAnimationEnded;

			_spawnAttackEventHandler = new DelayedEventHandler(
				_attackSpawnDelay,
				OnSpawnAttack,
				false
				);

            _damagedEntities = new HashSet<BattleEntity>();
        }

        public override void EndAttack()
        {

        }


        public override void DoAttack(float deltaTime)
        {
			base.DoAttack(deltaTime);

			_spawnAttackEventHandler.Update(deltaTime);

            List<BattleEntity> entities = Game.Instance.BattleSession.GetEntitiesAtPosition(_currentEntity.MovementController.TilePosition);

            foreach (BattleEntity entity in entities)
            {
                if (entity != _currentEntity && entity.EntityType != BattleEntity.BattleEntityType.ENEMY && !_damagedEntities.Contains(entity))
                {
                    AttackData mettaurWaveAttackData = new AttackData((uint)_attackDamage, _currentEntity.BattleEntityID, AttackData.DamageType.NEUTRAL);

                    entity.HealthController.DealDamage(mettaurWaveAttackData);

                    _damagedEntities.Add(entity);
                }
            }
        }

        public void OnAnimationEnded()
        {
            _animatorController.OnAnimationEnded -= OnAnimationEnded;
			OnAttackFinished?.Invoke();
			
			if (_removeEntity)
			{
				Game.Instance.BattleSession.UnhighlightGridTile(_currentEntity.MovementController.TilePosition);
				Game.Instance.BattleSession.RemoveEntity(_currentEntity);
				_currentEntity.Free();
			}
        }

		private void OnSpawnAttack()
		{
			if (_currentEntity.MovementController.TilePosition.X != 0)
			{
				Vector2 spawnPosition = _currentEntity.MovementController.TilePosition + new Vector2(-1, 0);

				BattleEntity waveEntity = BattleEntityGeneratorHelper.GenerateEntity(BattleEntityGeneratorHelper.MettaurWaveEntityID);
				Game.Instance.BattleSession.SpawnEntity(waveEntity, spawnPosition);
				Game.Instance.BattleSession.HighlightGridTile(spawnPosition);
			}
		}
    }
}
