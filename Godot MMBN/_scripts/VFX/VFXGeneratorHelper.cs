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

		public static AnimatedVFXController GenerateVFX(string vfxID, Vector2 position)
		{
			PackedScene entityPackedScene = GD.Load<PackedScene>(VFXPath + vfxID + ".tscn");
			AnimatedVFXController vfxController = entityPackedScene.Instantiate<AnimatedVFXController>();
			vfxController.Init(position);
			return vfxController;	
		}
	}
}
