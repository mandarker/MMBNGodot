using Godot;
using System;

namespace MMBN.Gameplay.Entities
{
	public static class BattleEntityGeneratorHelper
	{
		public static readonly string EntityPath = "res://_scenePrefabs/";

		public static readonly string PlayerEntityID = "PlayerEntity";
		public static readonly string MettaurEntityID = "MettaurEntity";

		
		public static readonly string MettaurWaveEntityID = "MettaurWaveEntity";

		public static BattleEntity GenerateEntity(string entityID)
		{
			PackedScene entityPackedScene = GD.Load<PackedScene>(EntityPath + entityID + ".tscn");
			BattleEntity battleEntity = entityPackedScene.Instantiate<BattleEntity>();
			return battleEntity;
		}	
	}
}
