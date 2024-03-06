using Godot;
using System;

using MMBN.Utility;
using System.Diagnostics;

public partial class PlayerController : Node
{
	private Vector2 _directionalInput;
	public Vector2 DirectionalInput { get{ return _directionalInput; } }

	public Action OnAButtonPressed;
	public Action OnAButtonReleased;

	public Action OnBButtonPressed;
	public Action OnBButton;
	public Action OnBButtonReleased;

    public Action OnStartButtonPressed;

	private bool _prevBButtonState = false;
	private bool _prevAButtonState = false;
    private bool _prevStartButtonState = false;

    public override void _Input(InputEvent @event)
    {
		if (@event.IsActionPressed("movement_left"))
		{
			_directionalInput.X += -1;
		}
		if (@event.IsActionReleased("movement_left"))
		{
			_directionalInput.X += 1;
		}

		if (@event.IsActionPressed("movement_right"))
		{
			_directionalInput.X += 1;
		}
		if (@event.IsActionReleased("movement_right"))
		{
			_directionalInput.X += -1;
		}

		if (@event.IsActionPressed("movement_up"))
		{
			_directionalInput.Y += 1;
		}
		if (@event.IsActionReleased("movement_up"))
		{
			_directionalInput.Y += -1;
		}

		if (@event.IsActionPressed("movement_down"))
		{
			_directionalInput.Y += -1;
		}
		if (@event.IsActionReleased("movement_down"))
		{
			_directionalInput.Y += 1;
		}
    }

    public override void _Process(double delta)
    {
        if (Input.IsKeyPressed(Key.Down))
		{
			if (_prevBButtonState == false)
			{
				OnBButtonPressed?.Invoke();
				_prevBButtonState = true;
			}	
			else
			{
				OnBButton?.Invoke();
			}
		}
		else
		{
			if (_prevBButtonState == true)
			{
				OnBButtonReleased?.Invoke();
				_prevBButtonState = false;
			}
		}

		if (Input.IsKeyPressed(Key.Right))
		{
			if (_prevAButtonState == false)
			{
				OnAButtonPressed?.Invoke();
				_prevAButtonState = true;
			}	
			else
			{
				
			}
		}
		else
		{
			if (_prevAButtonState == true)
			{
				OnAButtonReleased?.Invoke();
				_prevAButtonState = false;
			}
		}

        if (Input.IsKeyPressed(Key.F))
        {
            if (_prevStartButtonState == false)
            {
                OnStartButtonPressed?.Invoke();
                _prevStartButtonState = true;
            }
        }
        else
        {
            if (_prevStartButtonState == true)
            {
                _prevStartButtonState = false;
            }
        }
    }
}
