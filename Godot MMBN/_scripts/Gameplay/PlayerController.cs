using Godot;
using System;

using System.Linq;
using MMBN.Utility;
using System.Diagnostics;
using Godot.Collections;
using System.Collections.Generic;

public partial class PlayerController : Node
{
	private Vector2 _directionalInput;
	public Vector2 DirectionalInput { get{ return _directionalInput; } }

    public enum ButtonDictionaryEnum
    {
        A_BUTTON_PRESSED,
        A_BUTTON_RELEASED,

        B_BUTTON_PRESSED,
        B_BUTTON,
        B_BUTTON_RELEASED,

        START_BUTTON_PRESSED,
        
        L_BUTTON_PRESSED,

        R_BUTTON_PRESSED
    }

    private List<(object, Action)> OnAButtonPressedDict = new List<(object, Action)>();
    private List<(object, Action)> OnAButtonReleasedDict = new List<(object, Action)>();

    private List<(object, Action)> OnBButtonPressedDict = new List<(object, Action)>();
    private List<(object, Action)> OnBButtonDict = new List<(object, Action)>();
    private List<(object, Action)> OnBButtonReleasedDict = new List<(object, Action)>();

    private List<(object, Action)> OnStartButtonPressedDict = new List<(object, Action)>();

    private List<(object, Action)> OnLButtonPressedDict = new List<(object, Action)>();

    private List<(object, Action)> OnRButtonPressedDict = new List<(object, Action)>();

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

    public void ClearInputs(object obj)
    {
        OnAButtonPressedDict.RemoveAll(pair => pair.Item1 == obj);
        OnAButtonReleasedDict.RemoveAll(pair => pair.Item1 == obj);

        OnBButtonPressedDict.RemoveAll(pair => pair.Item1 == obj);
        OnBButtonDict.RemoveAll(pair => pair.Item1 == obj);
        OnBButtonReleasedDict.RemoveAll(pair => pair.Item1 == obj);

        OnStartButtonPressedDict.RemoveAll(pair => pair.Item1 == obj);

        OnLButtonPressedDict.RemoveAll(pair => pair.Item1 == obj);

        OnRButtonPressedDict.RemoveAll(pair => pair.Item1 == obj);
    }

    public void SubscribeInput(object subscriber, ButtonDictionaryEnum buttonEnum, Action action)
    {
        switch (buttonEnum)
        {
            default:
            case ButtonDictionaryEnum.A_BUTTON_PRESSED:
                OnAButtonPressedDict.Add((subscriber, action));
                break;
            case ButtonDictionaryEnum.A_BUTTON_RELEASED:
                OnAButtonReleasedDict.Add((subscriber, action));
                break;

            case ButtonDictionaryEnum.B_BUTTON_PRESSED:
                OnBButtonPressedDict.Add((subscriber, action));
                break;
            case ButtonDictionaryEnum.B_BUTTON:
                OnBButtonDict.Add((subscriber, action));
                break;
            case ButtonDictionaryEnum.B_BUTTON_RELEASED:
                OnBButtonReleasedDict.Add((subscriber, action));
                break;

            case ButtonDictionaryEnum.START_BUTTON_PRESSED:
                OnStartButtonPressedDict.Add((subscriber, action));
                break;

            case ButtonDictionaryEnum.L_BUTTON_PRESSED:
                OnLButtonPressedDict.Add((subscriber, action));
                break;

            case ButtonDictionaryEnum.R_BUTTON_PRESSED:
                OnRButtonPressedDict.Add((subscriber, action));
                break;
        }
        
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
                    foreach ((object, Action) pair in OnBButtonPressedDict)
                    {
                        pair.Item2?.Invoke();
                    }
				    _prevBButtonState = true;
			    }	
			    else
			    {
                    foreach ((object, Action) pair in OnBButtonDict)
                    {
                        pair.Item2?.Invoke();
                    }
                }
		    }
		    else
		    {
                if (_prevBButtonState == true)
			    {
                    foreach ((object, Action) pair in OnBButtonReleasedDict)
                    {
                        pair.Item2?.Invoke();
                    }
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
                    foreach ((object, Action) pair in OnAButtonPressedDict)
                    {
                        pair.Item2?.Invoke();
                    }
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
                    foreach ((object, Action) pair in OnAButtonReleasedDict)
                    {
                        pair.Item2?.Invoke();
                    }
                    _prevAButtonState = false;
			    }
		    }
        }

        if (Input.IsKeyPressed(START_KEY))
        {
            if (_prevStartButtonState == false)
            {
                foreach ((object, Action) pair in OnStartButtonPressedDict)
                {
                    pair.Item2?.Invoke();
                }
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
                foreach ((object, Action) pair in OnLButtonPressedDict)
                {
                    pair.Item2?.Invoke();
                }
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
                foreach ((object, Action) pair in OnRButtonPressedDict)
                {
                    pair.Item2?.Invoke();
                }
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
