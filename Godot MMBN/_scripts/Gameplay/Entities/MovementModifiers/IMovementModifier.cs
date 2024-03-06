using Godot;
using System;

namespace MMBN.Gameplay.Entities
{
	public abstract partial class MovementModifier : Node
	{
		public static readonly int MOVEMENT_MODIFIER_PRIORITY_1 = 1;
		public static readonly int MOVEMENT_MODIFIER_PRIORITY_2 = 2;
		public static readonly int MOVEMENT_MODIFIER_PRIORITY_3 = 3;

		public abstract string GetModifierID();
		public abstract bool ApplyModifier(Vector2 movementVector, out Vector2 outputMovement);
		public abstract int GetPriority();

		
	}
}
