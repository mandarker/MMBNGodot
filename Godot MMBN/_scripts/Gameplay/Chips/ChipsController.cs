using Godot;
using MMBN.Gameplay.Entities;
using System;
using System.Collections.Generic;

namespace MMBN.Gameplay.Chips
{
	public partial class ChipsController : Node2D
	{
		private List<ChipBase> _chipQueue;
		[Export]
		private Sprite2D[] _chipNodes;
        [Export]
        private Sprite2D[] _chipTextureNodes;

		public void Init(BattleEntity playerEntity)
		{
			_chipQueue = new List<ChipBase>();
			playerEntity.AddChild(this);
		}

		public void EnqueueChip(ChipBase chip)
		{
			_chipQueue.Add(chip);
		}

		public ChipBase DequeueChip()
		{
			if (_chipQueue.Count == 0)
			{
				return null;
			}

			ChipBase frontChip = _chipQueue[0];
			_chipQueue.RemoveAt(0);

			DisplayChipsOnPlayer();

			return frontChip;
		}

		public ChipBase PeekChip()
		{
			if (_chipQueue.Count == 0)
			{
				return null;
			}

			return _chipQueue[0];
		}

		public void DisplayChipsOnPlayer()
		{
			for (int i = 0; i < _chipNodes.Length; ++i)
			{
				if (i < _chipQueue.Count)
				{
					_chipNodes[i].Visible = true;
					_chipTextureNodes[i].Texture = _chipQueue[i].ChipDataResource.ChipBattleTexture;
				}
				else
				{
					_chipNodes[i].Visible = false;
				}
			}
		}

		public void HideChipsOnPlayer()
		{
			for (int i = 0; i < _chipNodes.Length; ++i)
			{
				_chipNodes[i].Visible = false;
			}
		}
	}
}
