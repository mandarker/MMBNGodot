using Godot;
using System;

namespace MMBN.Utility
{
    public partial class GeneralStateMachine : Node
    {
        private bool _isPaused;
        public bool IsPaused {  get { return _isPaused; } }

        [Export]
        private GeneralState _currentState;

        public override void _Ready()
        {
            _currentState.StartState();
        }

        public void Update(float delta)
        {
            if (_isPaused)
                return;

            _currentState.UpdateState(delta);
        }

        public void PauseStateMachine()
        {
            _isPaused = true;
        }

        public void UnpauseStateMachine()
        {
            _isPaused = false;
        }

        public void SetState(GeneralState state)
        {
            _currentState.EndState();
            _currentState = state;
            _currentState.StartState();
        }
    }
}