using Godot;
using MMBN.Utility;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.ConstrainedExecution;

namespace MMBN.Gameplay.Entities.Animation
{
	public partial class EntityAnimationController : Node
	{
		[Export]
		private string[] _animationID;
		[Export]
		private CanvasItem[] _animationSprites;
		[Export]
		private AnimationPlayer _animationPlayer;
		private Dictionary<string, CanvasItem> _idSpriteDictionary;
		[Export]
		private HealthController _healthController;

		[Export]
		private string _defaultAnimationID;
		private bool _animationEndedTriggered = false;
		private string _currentAnimationID;

		public Action OnAnimationEnded;

		private bool _isPaused;

		private DelayedEventHandler _spriteWhiteResetEventHandler;
		private DelayedEventHandler _spriteClearResetEventHandler;
        private DelayedEventHandler _spriteFlashResetEventHandler;

        public void Init()
        {
			_idSpriteDictionary = new Dictionary<string, CanvasItem>();
			for (int i = 0; i < _animationID.Length; ++i)
			{
				_idSpriteDictionary.Add(_animationID[i], _animationSprites[i]);
			}

			_isPaused = false;

			_currentAnimationID = _defaultAnimationID;
        }

        public void Update(float deltaTime)
        {
			if (_isPaused)
			{
				return;
			}

			_spriteClearResetEventHandler?.Update(deltaTime);
			_spriteWhiteResetEventHandler?.Update(deltaTime);
            _spriteFlashResetEventHandler?.Update(deltaTime);

            if (!_animationEndedTriggered && !_animationPlayer.IsPlaying())
			{
				_animationEndedTriggered = true;
				OnAnimationEnded?.Invoke();
			}
        }

        public void PlayAnimation(string animationID, bool reversed = false)
		{
			if (_idSpriteDictionary.ContainsKey(animationID))
			{
				if (_currentAnimationID != null)
				{
					_idSpriteDictionary[_currentAnimationID].Visible = false;
				}

				_currentAnimationID = animationID;
				_idSpriteDictionary[_currentAnimationID].Visible = true;
				
				if (!reversed)
				{
					_animationPlayer.Play(_currentAnimationID);
				}
				else
				{
					_animationPlayer.PlayBackwards(_currentAnimationID);
				}
				
				_animationEndedTriggered = false;
			}
		}

		public void SetAnimationPaused(bool paused)
		{
			if (paused)
			{
				_animationPlayer.Pause();
			}
			else
			{
				_animationPlayer.Play();
			}

			_isPaused = paused;
		}

		public void SetSpriteWhiteDuration(float duration)
		{
			SetSpriteWhite(true);

			_spriteWhiteResetEventHandler = new DelayedEventHandler(
				duration,
				() => SetSpriteWhite(false),
				false
			);
		}

		public void SetSpriteClearDuration(float duration)
		{
			SetSpriteTransparency(0);

			_spriteClearResetEventHandler = new DelayedEventHandler(
				duration,
				() => SetSpriteTransparency(1),
				false
			);
		}

        public void SetSpriteFlashDuration(float duration)
        {
            SetSpriteFlash(true);

            _spriteFlashResetEventHandler = new DelayedEventHandler(
                duration,
                () => SetSpriteFlash(false),
                false
            );
        }

        // there has to be a better way to do this, but brute force for now
        public void SetSpriteWhite(bool white)
		{
            foreach (CanvasItem sprite in _animationSprites)
            {
			    ((ShaderMaterial)sprite.Material).SetShaderParameter("_white", white);
            }
		}

		public void SetSpriteTransparency(float transparency)
		{
            foreach (CanvasItem sprite in _animationSprites)
            {
                ((ShaderMaterial)sprite.Material).SetShaderParameter("_transparency", transparency);
            }
		}

        public void SetSpriteFlash(bool flash)
        {
            foreach (CanvasItem sprite in _animationSprites)
            {
                ((ShaderMaterial)sprite.Material).SetShaderParameter("_flash", flash);
            }
        }

        public void SetSpritePixelation(float pixelation)
        {
            foreach (CanvasItem sprite in _animationSprites)
            {
                ((ShaderMaterial)sprite.Material).SetShaderParameter("_pixelationStrength", pixelation);
            }
        }
    }
}
