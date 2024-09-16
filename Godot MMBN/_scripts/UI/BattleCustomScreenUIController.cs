using Godot;
using MMBN.Gameplay.Battle;
using MMBN.UI.Displayers;
using MMBN.UI.GenericUI;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

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
        private GenericUIElement _okUIElement;

        [Export]
        private Sprite2D _chipDescSprite;
        [Export]
        private Sprite2D _chipCodeSprite;
        [Export]
        private Sprite2D _chipDamageTypeSprite;
        [Export]
        private NumberDisplayer _chipAttackDisplayer;
        [Export]
        private TextDisplayer _chipNameDisplayer;

        [Export]
        private AnimationPlayer _customAnimationPlayer;
        [Export]
        private AnimationPlayer _customSpinnerAnimationPlayer;

        [Export]
        private AudioStream _customOpenAudioStream;
        [Export]
        private AudioStream _customOKAudioStream;
        [Export]
        private AudioStream _customSelectAudioStream;
        [Export]
        private AudioStream _customCancelAudioStream;
        [Export]
        private AudioStream _customMovedAudioStream;

        private int _customBattleChipCount;

        public void ResetUI(List<BattleChipsManager.BattleChipStruct> customBattleChips)
        {
            // turn off all sprites in the list of loaded sprites
            foreach (Sprite2D loadedChipSprite in _loadedChipSprites)
            {
                loadedChipSprite.Visible = false;
            }

            _customBattleChipCount = customBattleChips.Count;

            for (int i = 0; i < _customChipSprites.Length; ++i)
            {
                if (i < customBattleChips.Count)
                {
                    _customChipSprites[i].Visible = true;
                    _customChipCodeSprites[i].Visible = true;
                    _customChipSprites[i].Texture = customBattleChips[i].ChipBase.ChipDataResource.ChipBattleTexture;
                    SetGrey(i, false);
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
                    _customChipCodeSprites[i].Visible = false;
                }
            }

            InitializeUI();
        }

        public void SetupUIEvents(Action<int> customAddFunction, Action<int> customShowFunction, Action customOKFunction)
        {
            for (int i = 0; i < _customUIElements.Length; ++i)
            {
                int index = i;
                if (i < _customBattleChipCount)
                {
                    _customUIElements[i].OnSelect = null;
                    _customUIElements[i].OnSelect += () => customAddFunction(index);

                    _customUIElements[i].OnHover = null;
                    _customUIElements[i].OnHover += () => customShowFunction(index);
                }
            }

            OnCursorMoved = null;
            OnCursorMoved += () => Game.Instance.SFXManager.PlaySFX(_customMovedAudioStream);

            _okUIElement.OnSelect = () => {
                customOKFunction?.Invoke();
                Game.Instance.SFXManager.PlaySFX(_customOKAudioStream);
                };

            _currentUIElement.OnHover?.Invoke();
        }

        public void ShowUI()
        {
            _customAnimationPlayer.Play("OpenBattleCustomScreen");

            Game.Instance.SFXManager.PlaySFX(_customOpenAudioStream);
        }

        public void HideUI()
        {
            _customAnimationPlayer.PlayBackwards("OpenBattleCustomScreen");

            CleanUpUI();
        }

        public void ShowChipDesc(BattleChipsManager.BattleChipStruct battleChip)
        {
            _chipDescSprite.Texture = battleChip.ChipBase.ChipDataResource.ChipDescriptionTexture;

            _chipDamageTypeSprite.Frame = (int)battleChip.ChipBase.ChipDataResource.Type;
            
            if (battleChip.Code == '*')
            {
                _chipCodeSprite.Frame = 26;
            }
            else
            {
                _chipCodeSprite.Frame = battleChip.Code - 'A';
            }

            _chipAttackDisplayer.SetNumber((int)battleChip.ChipBase.ChipDataResource.Attack);

            _chipNameDisplayer.SetText(battleChip.ChipBase.ChipDataResource.DisplayName);
        }

        public void AddLoadedChipSprite(int index, Texture2D chipSprite)
        {
            _loadedChipSprites[index].Visible = true;
            _loadedChipSprites[index].Texture = chipSprite;

            Game.Instance.SFXManager.PlaySFX(_customSelectAudioStream);
        }

        public void RemoveLoadedChipSprite(int index)
        {
            _loadedChipSprites[index].Visible = false;

            Game.Instance.SFXManager.PlaySFX(_customCancelAudioStream);
        }

        public void SetGrey(int index, bool grey)
        {
            ((ShaderMaterial)_customChipSprites[index].Material).SetShaderParameter("_greyscale", grey);
        }

        public void SetVisible(int index, bool visible)
        {
            _customChipSprites[index].Visible = visible;
        }

        public void RunSpinAnimation()
        {
            _customSpinnerAnimationPlayer.Stop();
            _customSpinnerAnimationPlayer.Play("Spin");
        }
    }
}
