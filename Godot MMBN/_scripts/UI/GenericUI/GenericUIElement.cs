using Godot;
using System;

namespace MMBN.UI.GenericUI
{
    public partial class GenericUIElement : Sprite2D
    {
        [Export]
        private GenericUIController _controller;

        [Export]
        private GenericUIElement _leftUIElement;
        [Export]
        private GenericUIElement _rightUIElement;
        [Export]
        private GenericUIElement _upUIElement;
        [Export]
        private GenericUIElement _downUIElement;

        public Action OnSelect;
        public Action OnHover;
        public Action OnExitHover;

        public virtual void Hover()
        {
            OnHover?.Invoke();
        }

	    public virtual void Select()
        {
            OnSelect?.Invoke();
        }

        public virtual void ExitHover()
        {
            OnExitHover?.Invoke();
        }

        public virtual void MoveLeft()
        {
            if (_leftUIElement != null)
            {
                _controller.HoverUIElement(_leftUIElement);
            }
        }

        public virtual void MoveRight()
        {
            if (_rightUIElement != null)
            {
                _controller.HoverUIElement(_rightUIElement);
            }
        }

        public virtual void MoveUp()
        {
            if (_upUIElement != null)
            {
                _controller.HoverUIElement(_upUIElement);
            }
        }

        public virtual void MoveDown()
        {
            if (_downUIElement != null)
            {
                _controller.HoverUIElement(_downUIElement);
            }
        }
    }
}
