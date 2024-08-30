using Godot;
using System;

namespace MMBN.Utility
{
    public sealed class InterpolatedEventHandler<T>
    {
        private float _currentTime;
        private float _totalDuration;

        private T _startValue, _endValue;
        private Func<T, T, float, T> _interpolationFunction;

        public Action<T> OnInterpolationAction;
        public Action OnInterpolationEndedAction;

        private bool _endedInvoked;

	    public InterpolatedEventHandler(T startValue, T endValue, Func<T, T, float, T> interpolationFunction, float duration)
        {
            _currentTime = 0;
            _totalDuration = duration;

            _startValue = startValue;
            _endValue = endValue;
            _interpolationFunction = interpolationFunction;

            _endedInvoked = false;
        }

        public void Update(float deltaTime)
        {
            _currentTime += deltaTime;

            if (_currentTime > _totalDuration)
            {
                _currentTime = _totalDuration;
            }

            if (!_endedInvoked)
            {
                OnInterpolationAction?.Invoke(_interpolationFunction(_startValue, _endValue, _currentTime / _totalDuration));
            }

            if (_currentTime == _totalDuration)
            {
                if (!_endedInvoked)
                {
                    _endedInvoked = true;
                    OnInterpolationEndedAction?.Invoke();
                }
            }
        }

        public void SetValues(T startValue, T endValue, float duration)
        {
            _startValue = startValue;
            _endValue = endValue;
            _totalDuration = duration;
        }

        public void Reset()
        {
            _currentTime = 0;
            _endedInvoked = false;
        }
    }
}
