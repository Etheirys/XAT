using System;

namespace XAT.Plugin.Utils;

public static class GameMath
{
    public static double DegreesToRadians(double degrees)
    {
        return degrees * (Math.PI / 180.0);
    }

    public static double RadiansToDegrees(double radians)
    {
        return radians * (180 / Math.PI);
    }
}
