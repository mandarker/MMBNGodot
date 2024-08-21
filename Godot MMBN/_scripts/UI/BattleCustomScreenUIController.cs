using Godot;
using System;

namespace MMBN.UI
{
    public partial class BattleCustomScreenUIController : Node
    {
        [Export]
        private Sprite2D[] chipSprites;
        [Export]
        private Sprite2D[] loadedChipSprites;

	    public void Init()
        {
            // turn off all sprites in the list of loaded sprites
            foreach (Sprite2D loadedChipSprite in loadedChipSprites) 
            {
                loadedChipSprite.Visible = false;
            }
        }

        public void ShowUI()
        {
            Game.Instance.BattleSession.SetPaused(true);


        }
    }
}
