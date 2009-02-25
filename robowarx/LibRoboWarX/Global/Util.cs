using System;
using System.IO;
using System.Reflection;
using System.Collections.Generic;

namespace RoboWarX
{
    internal static class Util
    {
        // Version of Sin that tries to mimic RoboWar behavior
        internal static double Sin(double a)
        {
            return Math.Sin((90 - a) * Constants.DEG_TO_RAD);
        }

        // Version of Cos that tries to mimic RoboWar behavior
        internal static double Cos(double a)
        {
            return Math.Cos((90 - a) * Constants.DEG_TO_RAD);
        }

        // Version of Tan that tries to mimic RoboWar behavior
        internal static double Tan(double a)
        {
            return Math.Tan((90 - a) * Constants.DEG_TO_RAD);
        }
    }
}
