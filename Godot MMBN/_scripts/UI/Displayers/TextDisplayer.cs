using Godot;
using System;

namespace MMBN.UI.Displayers
{
    public partial class TextDisplayer : Node
    {
        [Export]
        private Sprite2D[] _charSprites;

        [Export]
        private bool _justifyLeft;

        public void SetText(string text)
        {
            if (_justifyLeft)
            {
                for (int i = 0; i < _charSprites.Length; i++)
                {
                    if (i < text.Length)
                    {
                        _charSprites[i].Visible = true;
                        SetCharSprite(text[i], i);
                    }
                    else
                    {
                        _charSprites[i].Visible = false;
                    }
                }
            }
            else
            {
                for (int i = _charSprites.Length - 1; i >= 0; i--)
                {
                    if (_charSprites.Length - 1 - i < text.Length)
                    {
                        _charSprites[i].Visible = true;
                        SetCharSprite(text[_charSprites.Length - 1 - i], i);
                    }
                    else
                    {
                        _charSprites[i].Visible = false;
                    }
                }
            }
        }

        private void SetCharSprite(char c, int spriteIndex)
        {
            if (spriteIndex < 0 || spriteIndex >= _charSprites.Length) { return; }

            if (c >= 'a' && c <= 'z')
            {
                _charSprites[spriteIndex].FrameCoords = new Vector2I(c - 'a', 1);
            }
            else if (c >= 'A' && c <= 'Z')
            {
                _charSprites[spriteIndex].FrameCoords = new Vector2I(c - 'A', 2);
            }
            else if (c >= '0' && c <= '9')
            {
                _charSprites[spriteIndex].FrameCoords = new Vector2I(c - '0', 0);
            }
            else
            {
                switch (c)
                {
                    case '=':
                        _charSprites[spriteIndex].FrameCoords = new Vector2I(0, 3);
                        break;
                    case ':':
                        _charSprites[spriteIndex].FrameCoords = new Vector2I(1, 3);
                        break;
                    case '%':
                        _charSprites[spriteIndex].FrameCoords = new Vector2I(2, 3);
                        break;
                    case '?':
                        _charSprites[spriteIndex].FrameCoords = new Vector2I(3, 3);
                        break;
                    case '+':
                        _charSprites[spriteIndex].FrameCoords = new Vector2I(4, 3);
                        break;
                    case '-':
                        _charSprites[spriteIndex].FrameCoords = new Vector2I(5, 3);
                        break;
                    case '!':
                        _charSprites[spriteIndex].FrameCoords = new Vector2I(6, 3);
                        break;
                    case '*':
                        _charSprites[spriteIndex].FrameCoords = new Vector2I(7, 3);
                        break;
                    case ',':
                        _charSprites[spriteIndex].FrameCoords = new Vector2I(8, 3);
                        break;
                    case '.':
                        _charSprites[spriteIndex].FrameCoords = new Vector2I(9, 3);
                        break;
                    case '/':
                        _charSprites[spriteIndex].FrameCoords = new Vector2I(10, 3);
                        break;
                }
            }
        }
    }
}
