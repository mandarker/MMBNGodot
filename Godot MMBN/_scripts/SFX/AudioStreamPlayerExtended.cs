using Godot;
using System;

namespace MMBN.SFX
{
    public partial class AudioStreamPlayerExtended : AudioStreamPlayer
    {
        [Export]
        private float _introEnd;
        [Export]
        private float _loopEnd;

	    public override void _Ready()
	    {
            base._Ready();
	    }

	    public override void _Process(double delta)
	    {
            base._Process(delta);

            float currentTime = GetPlaybackPosition();

            if (currentTime > _loopEnd)
            {
                currentTime = currentTime - _loopEnd + _introEnd;
                
                Play(currentTime);
            }
	    }
    }
}
