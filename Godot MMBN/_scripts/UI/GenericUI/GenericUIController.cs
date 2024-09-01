using Godot;
using System;

namespace MMBN.UI.GenericUI
{
    public partial class GenericUIController : Node2D
    {
        [Export]
        private GenericUIElement _startingUIElement;

        [Export]
        protected Sprite2D _cursor;

        private GenericUIElement _currentUIElement;

        public void InitializeUI()
        {
            _currentUIElement = _startingUIElement;
            _cursor.Position = _currentUIElement.Position;
        }

	    public void HoverUIElement(GenericUIElement genericUIElement)
        {
            _currentUIElement.ExitHover();

            _currentUIElement = genericUIElement;
            _cursor.Position = _currentUIElement.Position;

            _currentUIElement.Hover();
        }

        public void SelectCurrentUIElement()
        {
            _currentUIElement.Select();
        }

        public void MoveUILeft()
        {
            _currentUIElement.MoveLeft();
        }

        public void MoveUIRight()
        {
            _currentUIElement.MoveRight();
        }

        public void MoveDown()
        {
            _currentUIElement.MoveDown();
        }

        public void MoveUp()
        {
            _currentUIElement.MoveUp();
        }
    }
}
