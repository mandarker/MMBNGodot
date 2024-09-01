using Godot;
using MMBN.Gameplay.Battle;
using MMBN.UI.GenericUI;
using System;
using System.Collections.Generic;

namespace MMBN.UI
{
    public partial class BattleCustomScreenUIController : GenericUIController
    {

        [Export]
        private Sprite2D[] _customChipSprites;

        [Export]
        private Sprite2D[] _customChipCodeSprites;

        [Export]
        private Sprite2D[] _loadedChipSprites;

        [Export]
        private GenericUIElement[] _customUIElements;

        [Export]
        private AnimationPlayer _customAnimationPlayer;

        public void ResetUI(List<BattleChipsManager.BattleChipStruct> customBattleChips)
        {
            // turn off all sprites in the list of loaded sprites
            foreach (Sprite2D loadedChipSprite in _loadedChipSprites)
            {
                loadedChipSprite.Visible = false;
            }

            for (int i = 0; i < _customChipSprites.Length; ++i)
            {
                if (i < customBattleChips.Count)
                {
                    _customChipSprites[i].Visible = true;
                    _customChipCodeSprites[i].Visible = true;
                    _customChipSprites[i].Texture = customBattleChips[i].ChipBase.ChipDataResource.ChipBattleTexture;
                    if (customBattleChips[i].Code == '*')
                    {
                        // the very last frame
                        _customChipCodeSprites[i].Frame = 26;
                    }
                    else
                    {
                        _customChipCodeSprites[i].Frame = customBattleChips[i].Code - 'A';
                    }
                }
                else
                {
                    _customChipSprites[i].Visible = false;
                }
            }
        }

        public void ShowUI()
        {
            _customAnimationPlayer.Play("OpenBattleCustomScreen");
        }

        public void AddLoadedChipSprite(int index, Texture2D chipSprite)
        {
            _loadedChipSprites[index].Visible = true;
            _loadedChipSprites[index].Texture = chipSprite;
        }

        public void RemoveLoadedChipSprite(int index)
        {
            _loadedChipSprites[index].Visible = false;
        }

        public void SetGrey(int index, bool grey)
        {
            ((ShaderMaterial)(_customChipSprites[index].Material)).Set("_greyscale", grey);
        }
    }
}
