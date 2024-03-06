using Godot;
using System;
using System.Collections.Generic;

namespace MMBN.Gameplay.Entities.EntityGroupedBehaviours
{
	public sealed class MettaurGroupedBehaviour
	{
		private List<BattleEntity> _entities;

		private int _currentIndex;

		public MettaurGroupedBehaviour()
		{
			_entities = new List<BattleEntity>();
			_currentIndex = 0;
		}

		public void SubmitMettaur(BattleEntity mettaurEntity)
		{
			if (_entities.Contains(mettaurEntity))
			{
				return;
			}

			if (!mettaurEntity.BattleEntityID.Equals("_mettaur"))
			{
				return;
			}

			mettaurEntity.HealthController.OnHealthReachedZero += () => OnMettaurDied(mettaurEntity);
			mettaurEntity.StateMachine.OnAttackStateFinished += OnMettaurAttacked;

			if (_entities.Count > 0)
			{
				mettaurEntity.StateMachine.PauseStateMachine(this);
			}
			
			_entities.Add(mettaurEntity);
		}

		private void OnMettaurDied(BattleEntity mettaurEntity)
		{
			if (_entities.Count == 0)
			{
				return;
			}

			int removedIndex = _entities.FindIndex(entity => entity.Name == mettaurEntity.Name);
			_entities.Remove(mettaurEntity);

			if (removedIndex == _currentIndex)
			{
				if (_entities.Count > 0)
				{
					if (_currentIndex >= _entities.Count)
					{
						_currentIndex = 0;
					}

					_entities[_currentIndex].StateMachine.UnpauseStateMachine(this);
				}
			}
			else if (removedIndex < _currentIndex)
			{
				--_currentIndex;
			}
		}

		private void OnMettaurAttacked()
		{
			_entities[_currentIndex].StateMachine.PauseStateMachine(this);

			_currentIndex = (_currentIndex + 1) % _entities.Count;

			_entities[_currentIndex].StateMachine.UnpauseStateMachine(this);
		}
	}
}
