using Godot;
using System;
using System.ComponentModel;

namespace MMBN.UI
{
    public partial class BattleCustomBarUIController : Sprite2D
    {
        [Export]
        private Sprite2D _customBarFillWhiteSprite;

        [Export]
        private Sprite2D _customBarFullSprite;

        [Export]
        private Sprite2D _customLoRSprite;

        public void ResetUI()
        {
            ((ShaderMaterial)_customBarFillWhiteSprite.Material).SetShaderParameter("_fill", 0);

            _customBarFullSprite.Visible = false;
            _customLoRSprite.Visible = false;
        }

        public void SetVisible(bool visible)
        {
            this.Visible = visible;
        }

        public void SetBarFill(float fill)
        {
            ((ShaderMaterial)_customBarFillWhiteSprite.Material).SetShaderParameter("_fill", fill);

            if (fill >= 1)
            {
                _customBarFullSprite.Visible = true;
                _customLoRSprite.Visible = true;
            }
        }
    }
}
