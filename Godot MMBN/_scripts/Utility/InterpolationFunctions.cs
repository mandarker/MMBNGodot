using Godot;
using System;

namespace MMBN.Utility
{
    public static class InterpolationFunctions
    {
	    public static float FloatLinearFunction(float a, float b, float interpolant)
        {
            return a + (b - a) * interpolant;
        }
    }
}
