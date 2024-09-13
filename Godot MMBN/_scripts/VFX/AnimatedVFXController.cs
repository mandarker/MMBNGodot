using Godot;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace MMBN.VFX
{
	public partial class AnimatedVFXController : Node2D
	{
		[Export]
		private string[] _animationIDs;

		[Export]
		private AnimationPlayer _animationPlayer;

		private bool _vfxFinishedTriggered;
		public Action OnVFXFinished;

		private bool _isPaused;

		public void Init(Vector2 position)
		{
			GlobalPosition = position;

			int randomIndex = GD.RandRange(0, _animationIDs.Length - 1);

			_animationPlayer.Play(_animationIDs[randomIndex]);
			_vfxFinishedTriggered = false;
			_isPaused = false;
		}

		public void PlayAnimation(string animationID)
		{
			_animationPlayer.Play(animationID);
		}

        public override void _Process(double delta)
        {
			if (_isPaused)
			{
				return;
			}

            if (!_animationPlayer.IsPlaying() && !_vfxFinishedTriggered)
			{
				OnVFXFinished?.Invoke();
				_vfxFinishedTriggered = true;
			}
        }

		public void Pause()
		{
			_isPaused = true;
			_animationPlayer?.Pause();
		}

		public void Unpause()
		{
			_isPaused = false;
			_animationPlayer?.Play();
		}
    }
}
