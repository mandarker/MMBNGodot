using Godot;
using System;

namespace MMBN.UI
{
    public partial class PauseUIController : Node2D
    {
        public void SetPauseScreen(bool paused)
        {
            Visible = paused;
        }
    }
}
