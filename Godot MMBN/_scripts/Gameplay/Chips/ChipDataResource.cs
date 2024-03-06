using Godot;
using System;

namespace MMBN.Gameplay.Chips
{
	public partial class ChipDataResource : Resource
	{
		[Export]
		private uint _attack;

		[Export]
		private Texture2D _chipBattleTexture;

        [Export]
        private AttackData.DamageType _damageType;

        [Export]
        private AudioStream[] _audioStreams;

        [Export]
        private float[] _audioOffsets;

		public uint Attack { get { return _attack; }}
		public Texture2D ChipBattleTexture { get { return _chipBattleTexture;}}
        public AttackData.DamageType Type { get { return _damageType; }}
        public AudioStream[] AudioStreams { get { return _audioStreams; } }
        public float[] AudioOffsets { get { return _audioOffsets; } }
	}
}
