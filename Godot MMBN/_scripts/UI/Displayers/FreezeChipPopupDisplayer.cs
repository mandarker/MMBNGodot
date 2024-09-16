using Godot;
using MMBN.Gameplay.Chips;
using System;

namespace MMBN.UI.Displayers
{
    public partial class FreezeChipPopupDisplayer : Node2D
    {
        [Export]
        private Sprite2D[] _chipNameChars;

        [Export]
        private Sprite2D[] _chipDmgChars;

        [Export]
        private int _pixelSeparation = 8;

	    public void SetText(FreezeChipBase freezeChipBase)
        {
            string displayName = freezeChipBase.ChipDataResource.DisplayName;

            for (int i = 0; i < _chipNameChars.Length; i++)
            {
                if (i < displayName.Length)
                {
                    _chipNameChars[i].Visible = true;
                    SetCharSprite(freezeChipBase.ChipDataResource.DisplayName[i], i);
                }
                else
                {
                    _chipNameChars[i].Visible = false;
                }
            }

            int chipDamage = (int)freezeChipBase.ChipDataResource.Attack;
            int digits = 0;

            for (int i = _chipDmgChars.Length - 1; i >= 0; i--)
            {
                if (!(chipDamage % 10 == 0 && chipDamage / 10 == 0))
                {
                    _chipDmgChars[i].Visible = true;
                    _chipDmgChars[i].Frame = chipDamage % 10;

                    chipDamage /= 10;
                    digits++;
                }
                else
                {
                    _chipDmgChars[i].Visible = false;
                }
            }

            int totalCharacters = digits + displayName.Length;

            for (int i = 0; i < _chipNameChars.Length; ++i)
            {
                _chipNameChars[i].Position = new Vector2((i - ((float)totalCharacters - 1) / 2) * _pixelSeparation, 0);
            }

            for (int i = _chipDmgChars.Length - digits; i < _chipDmgChars.Length; ++i)
            {
                _chipDmgChars[i].Position = new Vector2(((i - (_chipDmgChars.Length - digits) + displayName.Length) - ((float)totalCharacters - 1) / 2) * _pixelSeparation, 0);
            }
        }

        private void SetCharSprite(char c, int spriteIndex)
        {
            if (spriteIndex < 0 || spriteIndex >= _chipNameChars.Length) { return; }

            if (c >= 'a' && c <= 'z')
            {
                _chipNameChars[spriteIndex].FrameCoords = new Vector2I(c - 'a', 1);
            }
            else if (c >= 'A' && c <= 'Z')
            {
                _chipNameChars[spriteIndex].FrameCoords = new Vector2I(c - 'A', 2);
            }
            else if (c >= '0' && c <= '9')
            {
                _chipNameChars[spriteIndex].FrameCoords = new Vector2I(c - '0', 0);
            }
            else
            {
                switch (c)
                {
                    case '=':
                        _chipNameChars[spriteIndex].FrameCoords = new Vector2I(0, 3);
                        break;
                    case ':':
                        _chipNameChars[spriteIndex].FrameCoords = new Vector2I(1, 3);
                        break;
                    case '%':
                        _chipNameChars[spriteIndex].FrameCoords = new Vector2I(2, 3);
                        break;
                    case '?':
                        _chipNameChars[spriteIndex].FrameCoords = new Vector2I(3, 3);
                        break;
                    case '+':
                        _chipNameChars[spriteIndex].FrameCoords = new Vector2I(4, 3);
                        break;
                    case '-':
                        _chipNameChars[spriteIndex].FrameCoords = new Vector2I(5, 3);
                        break;
                    case '!':
                        _chipNameChars[spriteIndex].FrameCoords = new Vector2I(6, 3);
                        break;
                    case '*':
                        _chipNameChars[spriteIndex].FrameCoords = new Vector2I(7, 3);
                        break;
                    case ',':
                        _chipNameChars[spriteIndex].FrameCoords = new Vector2I(8, 3);
                        break;
                    case '.':
                        _chipNameChars[spriteIndex].FrameCoords = new Vector2I(9, 3);
                        break;
                    case '/':
                        _chipNameChars[spriteIndex].FrameCoords = new Vector2I(10, 3);
                        break;
                }
            }
        }
    }
}
