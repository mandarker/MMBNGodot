using Godot;
using MMBN.Gameplay.Battle;
using System;
using System.Collections.Generic;

namespace MMBN.UI
{
    public partial class BattleCustomScreenUIController : Node2D
    {

        [Export]
        private Sprite2D[] customChipSprites;

        [Export]
        private Sprite2D[] customChipCodeSprites;

        [Export]
        private Sprite2D[] loadedChipSprites;

        private List<BattleChipsManager.BattleChipStruct> _customChips;
        private List<BattleChipsManager.BattleChipStruct> _loadedChips;

        private const int MAXIMUM_CUSTOM_CHIPS = 5;

        public void Initialize()
        {
            _customChips = new List<BattleChipsManager.BattleChipStruct>();
            _loadedChips = new List<BattleChipsManager.BattleChipStruct>();
        }

        public void ResetUI()
        {
            // turn off all sprites in the list of loaded sprites
            foreach (Sprite2D loadedChipSprite in loadedChipSprites)
            {
                loadedChipSprite.Visible = false;
            }

            if (_customChips.Count < MAXIMUM_CUSTOM_CHIPS)
            {
                List<BattleChipsManager.BattleChipStruct> newBattleChips = Game.Instance.BattleChipsManager.GetRandomAvailableBattleChips(MAXIMUM_CUSTOM_CHIPS - _customChips.Count);
                _customChips.AddRange(newBattleChips);

                // update selectable chips here
                for (int i = 0; i < customChipSprites.Length; ++i)
                {
                    if (i < MAXIMUM_CUSTOM_CHIPS)
                    {
                        customChipSprites[i].Visible = true;
                        customChipSprites[i].Texture = _customChips[i].ChipBase.ChipDataResource.ChipBattleTexture;
                        if (_customChips[i].Code == '*')
                        {
                            // the very last frame
                            customChipCodeSprites[i].Frame = 26;
                        }
                        else
                        {
                            customChipCodeSprites[i].Frame = _customChips[i].Code - 'A';
                        }
                    }
                    else
                    {
                        customChipSprites[i].Visible = false;
                    }
                }
            }
        }

        public void ShowUI()
        {
            Game.Instance.BattleSession.SetPaused(true);

            this.Visible = true;
        }
    }
}
