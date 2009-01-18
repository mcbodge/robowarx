using System;

namespace RoboWarX
{
    public static class Constants
    {
        // These could potentially be variable! Wouldn't that be rad?
        public const int ARENA_SIZE = 300;
        public const int MAX_ROBOTS = 6;
        
        public const int ROBOT_RADIUS = 10;
        public const int PROJECTILE_RADIUS = 5;
        
        // Stick as close to RoboWar as we can
        public const double DEG_TO_RAD = System.Math.PI / 180;
        public const double RAD_TO_DEG = 180 / System.Math.PI;
    }
}
