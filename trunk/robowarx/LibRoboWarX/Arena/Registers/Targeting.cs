using System;
using RoboWarX;
using RoboWarX.Arena;

namespace RoboWarX.Arena.Registers
{
    // Angle turret points. May be read or written. The angle is in degrees, oriented like a\
    // compass with 0 degrees pointing upward and 90 degrees pointing to the right. All bullets and
    // missiles are fired in the direction that the turret is pointing.
    internal class AimRegister : Register
    {
        internal AimRegister() {
            name = "AIM";
            code = (Int16)Bytecodes.REG_AIM;
        }

        public override Int16 value
        {
            get { return (Int16) robot.aim; }
            internal set
            {
                robot.aim = (Int16)(value % 360);
                if (robot.aim < 0)
                    robot.aim += 360;
            }
        }
    }

    // Targeting offset from AIM. The RANGE command returns a distance to the nearest robot in the
    // direction AIM+LOOK. This might be useful for some tracking algorithm. If not otherwise set,
    // LOOK defaults to 0. LOOK may be read or written.
    internal class LookRegister : Register
    {
        internal LookRegister()
        {
            name = "LOOK";
            code = (Int16)Bytecodes.REG_LOOK;
        }

        public override Int16 value
        {
            get { return (Int16)robot.look; }
            internal set
            {
                robot.look = (Int16)(value % 360);
                if (robot.look < 0)
                    robot.look += 360;
            }
        }
    }

    // Similar to LOOK, the radar offset from the AIM. The RADAR command searches for projectiles
    // in the directon AIM+SCAN. This might beuseful if a robot wants to look for targets in one
    // direction, but check for danger in another direction. If not otherwise set, SCAN defaults
    // to 0. SCAN may be read or written.
    internal class ScanRegister : Register
    {
        internal ScanRegister() {
            name = "SCAN";
            code = (Int16)Bytecodes.REG_SCAN;
        }

        public override Int16 value
        {
            get { return (Int16)robot.scan; }
            internal set
            {
                robot.scan = (Int16)(value % 360);
                if (robot.scan < 0)
                    robot.scan += 360;
            }
        }
    }
}