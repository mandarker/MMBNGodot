using Godot;
using System;

namespace MMBN.Utility
{
    public abstract partial class GeneralState : Node
    {
        [Export]
        private GeneralStateMachine _parentStateMachine;

        public abstract void StartState();
        public abstract void UpdateState(double deltaTime);
        public abstract void EndState();
    }
}
