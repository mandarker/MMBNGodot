using Godot;
using System;

namespace MMBN.Gameplay.Entities.EntityStateMachine
{
	public abstract partial class EntityState : Node
	{
		protected EntityStateController _entityStateController;

		public void Init(EntityStateController stateController)
		{
			_entityStateController = stateController;
		}
		
		public static string PLAYER_MOVEMENT_STATE_ID = "PLAYER_MOVEMENT_STATE_ID";
		public static string TRACK_PLAYER_STATE_ID = "TRACK_PLAYER_STATE_ID";
		public static string IDLE_STATE_ID = "IDLE_STATE_ID";
		public static string ATTACK_STATE_ID = "ATTACK_STATE_ID";
		public static string DEATH_STATE_ID = "DEATH_STATE_ID";
        public static string DAMAGED_STATE_ID = "DAMAGED_STATE_ID";

		public abstract string GetStateID();

		public abstract void BeginState();

		public abstract void PauseState();

		public abstract void ContinueState();

		public abstract void UpdateState(float deltaTime);
		
		public abstract void EndState();
	}
}
