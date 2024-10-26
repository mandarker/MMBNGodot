using Godot;
using System;

namespace MMBN.Gameplay.Entities
{
	public static class BattleEntityGeneratorHelper
	{
		public static readonly string EntityPath = "res://_scenePrefabs/Entities/";

		public static readonly string PlayerEntityID = "PlayerEntity";
		
        public static readonly string MettaurEntityID = "Enemies/MettaurEntity";
        public static readonly string HandyEntityID = "Enemies/HandyEntity";

        public static readonly string HandyBombEntityID = "Attacks/HandyBombEntity";
        public static readonly string MettaurWaveEntityID = "Attacks/MettaurWaveEntity";

		public static BattleEntity GenerateEntity(string entityID)
		{
			PackedScene entityPackedScene = GD.Load<PackedScene>(EntityPath + entityID + ".tscn");
			BattleEntity battleEntity = entityPackedScene.Instantiate<BattleEntity>();
			return battleEntity;
		}	
	}
}
