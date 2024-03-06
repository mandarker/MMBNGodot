using Godot;
using MMBN.Gameplay.Entities;
using System;

namespace MMBN.Gameplay.Battle
{
    public sealed class EnemiesDeadBattleWinCondition : BattleWinCondition
    {
		private int _entityCount;
		private int _currentEntitiesDead;

		public EnemiesDeadBattleWinCondition(BattleEntity[] entities)
		{
			_entityCount = entities.Length;
			_currentEntitiesDead = 0;
			
			foreach (BattleEntity entity in entities)
			{
				entity.HealthController.OnHealthReachedZero += () => OnHealthReachedZero();
			}
		}

		private void OnHealthReachedZero()
		{
			++_currentEntitiesDead;

			CheckWinConditionMet();
		}

		protected override void CheckWinConditionMet()
		{
			if (_currentEntitiesDead >= _entityCount)
			{
				OnWinConditionMet?.Invoke();
			}
		}
    }
}
