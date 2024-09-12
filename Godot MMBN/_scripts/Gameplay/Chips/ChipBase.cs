using Godot;
using MMBN.Gameplay.Entities;
using MMBN.Gameplay.Entities.Animation;
using System;
using System.Reflection.Emit;

namespace MMBN.Gameplay.Chips
{
	public abstract partial class ChipBase
	{
		protected ChipDataResource _chipDataResource;
		public ChipDataResource ChipDataResource { get { return _chipDataResource;}}

		protected EntityAnimationController _animationController;
		protected BattleEntity _playerEntity;

		public Action OnChipFinished;

		public virtual void Init(ChipDataResource chipDataResource)
		{
			_chipDataResource = chipDataResource;
		}

		public virtual void StartChip(EntityAnimationController playerAnimationController, BattleEntity playerEntity)
		{
			_animationController = playerAnimationController;
			_playerEntity = playerEntity;
		}

        public abstract void EndChip();

		public abstract void RunChip(float deltaTime);
	}
}
