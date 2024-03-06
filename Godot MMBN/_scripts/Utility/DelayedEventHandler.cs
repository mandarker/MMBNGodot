using Godot;
using System;

namespace MMBN.Utility
{
	public partial class DelayedEventHandler : Node
	{
		private float _delay;
		private Action _actionEvent;


		private bool _startImmediate;
		private float _currentTime;
		private bool _actionTriggered;

		public DelayedEventHandler(float delay, Action actionEvent, bool startImmediate = true)
		{
			_delay = delay;
			_actionEvent = actionEvent;
			
			_startImmediate = startImmediate;
			_currentTime = startImmediate ? _delay : 0;
			_actionTriggered = false;
		}	

		public void Update(float deltaTime)
		{
			_currentTime += deltaTime;

			if (_currentTime >= _delay)
			{
				_currentTime = _delay;

				if (!_actionTriggered)
				{
					_actionTriggered = true;
					_actionEvent?.Invoke();
				}
			}
		}

		public void Reset()
		{
			_currentTime = _startImmediate ? _delay : 0;
			_actionTriggered = false;
		}
	}
}