using Godot;
using System;
using System.Collections.Generic;

namespace MMBN.UI.Displayers
{
    public partial class NumberDisplayer : Node
    {
        [Export]
        private Sprite2D[] _digitSprites;

        [Export]
        private bool _displayLeft;

        [Export]
        private bool _showRemainingZeroes;

        public void SetNumber(int number)
        {
            for (int i = 0; i < _digitSprites.Length; i++)
            {
                _digitSprites[i].Visible = true;
            }

            if (_displayLeft)
            {
                Stack<int> digitStack = new Stack<int>();
                int digitCount = 0;

                while (!(number / 10 == 0 && number % 10 == 0))
                {
                    digitStack.Push(number % 10);
                    number = number / 10;
                    digitCount++;
                }

                for (int i = 0; i < _digitSprites.Length; ++i)
                {
                    if (i < digitCount)
                    {
                        _digitSprites[i].Frame = digitStack.Pop();
                    }
                    else
                    {
                        if (_showRemainingZeroes)
                        {
                            _digitSprites[i].Frame = 0;
                        }
                        else
                        {
                            _digitSprites[i].Visible = false;
                        }
                    }
                }
            }
            else
            {
                for (int i = _digitSprites.Length - 1; i >= 0; --i)
                {
                    if (!(number / 10 == 0 && number % 10 == 0))
                    {
                        _digitSprites[i].Frame = number % 10;
                        number = number / 10;
                    }
                    else
                    {
                        if (_showRemainingZeroes)
                        {
                            _digitSprites[i].Frame = 0;
                        }
                        else
                        {
                            _digitSprites[i].Visible = false;
                        }
                    }
                }
            }
        }
    }
}
