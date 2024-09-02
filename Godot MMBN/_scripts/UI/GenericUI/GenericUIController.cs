using Godot;
using MMBN.Utility;
using System;

namespace MMBN.UI.GenericUI
{
    public partial class GenericUIController : Node2D
    {
        [Export]
        private GenericUIElement _startingUIElement;

        [Export]
        protected Sprite2D _cursor;

        [Export]
        private float _uiScrollSpeed;

        protected GenericUIElement _currentUIElement;

        private ThresholdedDelayedEventHandler _uiMovementDelayedEventHandler;

        private Vector2 _sanitizedDirectionalInput;

        private bool _isUIOpen;

        public Action OnCursorMoved;

        public void InitializeUI()
        {
            _currentUIElement = _startingUIElement;
            _cursor.GlobalPosition = _startingUIElement.GlobalPosition;

            PlayerController playerController = Game.Instance.PlayerController;
            playerController.ClearInputs();
            playerController.OnAButtonPressed += SelectCurrentUIElement;

            _uiMovementDelayedEventHandler = new ThresholdedDelayedEventHandler(
                0, 
                _uiScrollSpeed, 
                () => DelayedMovementUpdate()
                );

            _isUIOpen = true;
        }

        public void CleanUpUI()
        {
            _isUIOpen = false;
            Game.Instance.PlayerController.ClearInputs();
        }

        private void DelayedMovementUpdate()
        {
            if (_sanitizedDirectionalInput == Vector2.Zero)
            {
                return;
            }

            if (_sanitizedDirectionalInput.X < 0)
            {
                MoveUILeft();
            }
            else if (_sanitizedDirectionalInput.X > 0)
            {
                MoveUIRight();
            }
            else if (_sanitizedDirectionalInput.Y < 0)
            {
                MoveUIUp();
            }
            else if (_sanitizedDirectionalInput.Y > 0)
            {
                MoveUIDown();
            }
        }

        public override void _Process(double delta)
        {
            if (_isUIOpen)
            {
                _uiMovementDelayedEventHandler.Update((float)delta);

                // sanitize the direction
                _sanitizedDirectionalInput = Game.Instance.PlayerController.DirectionalInput;
                if (Mathf.Abs(_sanitizedDirectionalInput.X) == 1 &&
                    Mathf.Abs(_sanitizedDirectionalInput.Y) == 1)
                {
                    _sanitizedDirectionalInput.Y = 0;
                }

                // negate the Y because of sprite z
                _sanitizedDirectionalInput.Y *= -1;

                if (_sanitizedDirectionalInput != Vector2.Zero)
                {
                    _uiMovementDelayedEventHandler.Input(1);
                }
            }
        }

        public void HoverUIElement(GenericUIElement genericUIElement)
        {
            _currentUIElement.ExitHover();

            _currentUIElement = genericUIElement;
            _cursor.GlobalPosition = _currentUIElement.GlobalPosition;

            _currentUIElement.Hover();
        }

        public void SelectCurrentUIElement()
        {
            _currentUIElement.Select();
        }

        public void MoveUILeft()
        {
            if (_currentUIElement.MoveLeft())
            {
                OnCursorMoved?.Invoke();
            }
        }

        public void MoveUIRight()
        {
            if (_currentUIElement.MoveRight())
            {
                OnCursorMoved?.Invoke();
            }
        }

        public void MoveUIDown()
        {
            if (_currentUIElement.MoveDown())
            {
                OnCursorMoved?.Invoke();
            }
        }

        public void MoveUIUp()
        {
            if (_currentUIElement.MoveUp())
            {
                OnCursorMoved?.Invoke();
            }
        }
    }
}
