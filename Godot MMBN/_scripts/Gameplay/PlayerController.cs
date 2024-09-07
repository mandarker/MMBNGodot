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

    public Action OnLButtonPressed;

    public Action OnRButtonPressed;

	private bool _prevBButtonState = false;
	private bool _prevAButtonState = false;
    private bool _prevStartButtonState = false;
    private bool _prevLButtonState = false;
    private bool _prevRButtonState = false;

    private bool _aButtonDisabled = false;
    private bool _bButtonDisabled = false;

    private const Key B_KEY = Key.Down;
    private const Key A_KEY = Key.Right;
    private const Key START_KEY = Key.F;
    private const Key L_Key = Key.Q;
    private const Key R_Key = Key.E;

    public void SetAButtonDisabled(bool disabled)
    {
        _aButtonDisabled = disabled;
    }

    public void SetBButtonDisabled(bool disabled)
    {
        _bButtonDisabled = disabled;
    }

    public void ClearInputs()
    {
        OnAButtonPressed = null;
        OnAButtonReleased = null;

        OnBButtonPressed = null;
        OnBButton = null;
        OnBButtonReleased = null;

        OnStartButtonPressed = null;

        OnLButtonPressed = null;
        OnRButtonPressed = null;
    }

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
        if (!_bButtonDisabled)
        {
            if (Input.IsKeyPressed(B_KEY))
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
        }

        if (!_aButtonDisabled)
        {
		    if (Input.IsKeyPressed(A_KEY))
		    {
                if (_aButtonDisabled)
                    return;

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
                if (_aButtonDisabled)
                    return;
            
                if (_prevAButtonState == true)
			    {
				    OnAButtonReleased?.Invoke();
				    _prevAButtonState = false;
			    }
		    }
        }

        if (Input.IsKeyPressed(START_KEY))
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

        if (Input.IsKeyPressed(L_Key))
        {
            if (_prevLButtonState == false)
            {
                OnLButtonPressed?.Invoke();
                _prevLButtonState = true;
            }
        }
        else
        {
            if (_prevLButtonState == true)
            {
                _prevLButtonState = false;
            }
        }

        if (Input.IsKeyPressed(R_Key))
        {
            if (_prevRButtonState == false)
            {
                OnRButtonPressed?.Invoke();
                _prevRButtonState = true;
            }
        }
        else
        {
            if (_prevRButtonState == true)
            {
                _prevRButtonState = false;
            }
        }
    }
}
