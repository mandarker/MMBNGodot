using Godot;
using System;

namespace MMBN.UI
{
    public partial class PauseUIController : Node2D
    {
        public override void _Ready()
        {
            Game.Instance.BattleSession.OnPaused += SetPauseScreen;
        }

        private void SetPauseScreen(bool paused)
        {
            Visible = paused;
        }
    }
}
