using Godot;
using MMBN.Utility;
using System;
using System.Diagnostics;
using System.Net.Http.Headers;

namespace MMBN.Gameplay.Entities
{
	public partial class EntityHealthDisplayer : Node2D
	{
		[Export]
		private HealthController _entityHealthController;

		[Export]
		private Sprite2D[] _digitSprites;

		[Export]
		private Color _damageColor;

		[Export]
		private float _digitSpread = 8;

		private DelayedEventHandler _colorResetEventHandler;
        private InterpolatedEventHandler<float> _healthChangeInterpolationEventHandler;

        private bool _isPlayerHealthDisplayer = false;
        private int _lastHealth = 0;

		public override void _Ready()
        {
            // assume it's the player's health if there's no attached EntityHealthController reference
            if (_entityHealthController == null)
            {
                _entityHealthController = Game.Instance.BattleSession.PlayerBattleEntity.HealthController;
                _isPlayerHealthDisplayer = true;
                _lastHealth = _entityHealthController.Health;
            }

            _lastHealth = _entityHealthController.Health;

            _colorResetEventHandler = new DelayedEventHandler(
				EntityGlobalConstants.ENTITY_HIT_FLASH_DURATION,
				() => SetColor(new Color(1, 1, 1, 1)),
				false
			);

            _healthChangeInterpolationEventHandler = new InterpolatedEventHandler<float>(
                _lastHealth, _lastHealth,
                InterpolationFunctions.FloatLinearFunction,
                EntityGlobalConstants.ENTITY_HIT_FLASH_DURATION
            );
            _healthChangeInterpolationEventHandler.OnInterpolationAction +=
                (health) => SetDigits((int)health);

            // when damage is being dealt
            _entityHealthController.OnDamageDealt += () => 
			{
				SetHealth(false);
				SetColor(_damageColor);

                // change the color for a tiny bit
				_colorResetEventHandler.Reset();

                // set what the last health was here
                _lastHealth = _entityHealthController.Health;
            };

			_entityHealthController.OnHealthReachedZero += () =>
			{
                // if the enemy dies
                if (!_isPlayerHealthDisplayer)
                {
                    // remove the visual
				    foreach (Sprite2D digitSprite in _digitSprites)
				    {
					    digitSprite.Visible = false;
				    }
                }
			};

			SetHealth(true);
		}

        public override void _Process(double delta)
        {
            _colorResetEventHandler.Update((float) delta);
            _healthChangeInterpolationEventHandler.Update((float) delta);
        }

        public void SetColor(Color color)
		{
			foreach (Sprite2D sprite in _digitSprites)
			{
				((ShaderMaterial)sprite.Material).SetShaderParameter("_col", color);
			}
		}

        public void SetHealth(bool immediate)
		{
			if (immediate)
			{
				SetDigits(_entityHealthController.Health);
			}
            else
            {
                // interpolate the health after resetting the values
                _healthChangeInterpolationEventHandler.SetValues(
                    _lastHealth,
                    _entityHealthController.Health,
                    GetInterpolationDuration(_lastHealth, _entityHealthController.Health)
                    );

                _healthChangeInterpolationEventHandler.Reset();
            }
		}

        private float GetInterpolationDuration(int lastHealth, int currentHealth)
        {
            return (float)Math.Log10(Math.Abs(lastHealth - currentHealth) + 10) * EntityGlobalConstants.ENTITY_HIT_FLASH_DURATION;
        }

		private void SetDigits(int value)
		{
			if (value <= 0)
			{
				value = 0;
			}

			int valueCopy = value;
			int lastDigitIndex = 0;

			for (int i = 0; i < _digitSprites.Length; ++i)
			{
				_digitSprites[i].Frame = valueCopy % 10;
				if (valueCopy % 10 != 0)
				{
					lastDigitIndex = i;
				}
				valueCopy = valueCopy / 10;
			}

            if (!_isPlayerHealthDisplayer)
            {
			    for (int i = 0; i < _digitSprites.Length; ++i)
			    {
				    if (i <= lastDigitIndex)
				    {
					    _digitSprites[i].Position = new Vector2((((float) lastDigitIndex / 2) - i) * _digitSpread, _digitSprites[i].Position.Y);
				    }
				    else
				    {
					    _digitSprites[i].Visible = false;
				    }
			    }
            }
            else
            {
                for (int i = 0; i < _digitSprites.Length; ++i)
                {
                    if (i > lastDigitIndex)
                    {
                        _digitSprites[i].Visible = false;
                    }
                }
            }
		}
	}
}
