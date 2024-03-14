using Godot;
using System;

namespace MMBN.VFX
{
	public static class VFXGeneratorHelper
	{
		public static readonly string VFXPath = "res://_scenePrefabs/Effects/";

		public static readonly string ExplosionID = "Explosion";
		public static readonly string BusterHitID = "BusterHit";
		public static readonly string BusterChargeID = "BusterCharge";

        public static readonly string MEGAMAN_CANNONSHOT_ID = "CannonShot";
        public static readonly string MEGAMAN_SWORDSLASH_ID = "SwordSlash";

		public static AnimatedVFXController GenerateVFX(string vfxID, Vector2 position)
		{
			PackedScene entityPackedScene = GD.Load<PackedScene>(VFXPath + vfxID + ".tscn");
			AnimatedVFXController vfxController = entityPackedScene.Instantiate<AnimatedVFXController>();
			vfxController.Init(position);
			return vfxController;	
		}

        public static AnimatedVFXController GenerateBattleVFX(string vfxID, Vector2 position)
        {
            AnimatedVFXController vfxController = GenerateVFX(vfxID, position);

            Game.Instance.AddChild(vfxController);
            Game.Instance.BattleSession.SubscribeVFXController(vfxController);

            return vfxController;
        }
	}
}
