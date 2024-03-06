using Godot;
using System;

namespace MMBN.Utility
{
	public partial class ThresholdedDelayedEventHandler
	{
		private float _threshold;
		private float _delay;
		private Action _actionEvent;

		private float _currentTime;

		public ThresholdedDelayedEventHandler(float threshold, float delay, Action actionEvent, bool startImmediate = true)
		{
			_threshold = threshold;
			_delay = delay;
			_actionEvent = actionEvent;

			_currentTime = startImmediate ? _delay : 0;
		}	

		public void Input(float input)
		{
			if (input < _threshold)
			{
				return;
			}

			if (_currentTime >= _delay)
			{
				if (input >= _threshold)
				{
					_currentTime = 0;
					_actionEvent?.Invoke();
				}
			}
		}

		public void Update(float deltaTime)
		{
			_currentTime += deltaTime;

			if (_currentTime > _delay)
			{
				_currentTime = _delay;
			}
		}

		public void Reset()
		{
			_currentTime = _delay;
		}
	}
}
