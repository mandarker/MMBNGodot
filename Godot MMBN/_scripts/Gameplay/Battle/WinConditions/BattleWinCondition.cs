using Godot;
using System;

namespace MMBN.Gameplay.Battle
{
	public abstract class BattleWinCondition
	{
		public Action OnWinConditionMet;

		protected abstract void CheckWinConditionMet();
	}
}
