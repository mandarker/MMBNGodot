using Godot;
using System;

namespace MMBN.Utility
{
    public abstract partial class GeneralState : Node
    {
        [Export]
        protected GeneralStateMachine _parentStateMachine;

        public abstract void StartState();
        public abstract void UpdateState(float deltaTime);
        public abstract void EndState();
    }
}
