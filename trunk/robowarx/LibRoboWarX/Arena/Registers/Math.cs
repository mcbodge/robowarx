using System;
using RoboWarX;
using RoboWarX.Arena;

namespace RoboWarX.Arena.Registers
{
    // This register is used to help tracking routines. It is used in conjunction with the RANGE
    // command. If no target is in the robot's sights, (i.e. the RANGE register is 0), DOPPLER
    // returns 0. Otherwise, it returns the speed of the target in a direction perpendicular to the
    // aim. For instance, if the target is moving directly toward or away from the robot, DOPPLER
    // will return 0. If the robot's sights are pointing directly up and the target is moving from
    // left to right at speed 8, the DOPPLER register will contain 8. DOPPLER has no effect if
    // written. (Note: the DOPPLER command existed in some earlier versions of RoboWar, but was
    // somewhat buggy and had not been documented.)
    internal class DopplerRegister : Register
    {
        internal DopplerRegister() {
            name = "DOPPLER";
            code = (Int16)Bytecodes.REG_DOPPLER;
        }

        public override Int16 value
        {
            get
            {
                // Doppler computes the velocity of the robot detected by range perpendicular
                // to the aim vector.  It neglects any movement of the tracking robot.
                //
                // Doppler first finds the nearest robot in sight, if there is one.  Then it
                // uses the following formula to compute perpendicular velocity:
                // (v = velocity vector of target, r = vector from tracker to target)
                //               ------------------
                //              /          (r•v)^2
                //  dop = ± \  /  |v|^2  - -------
                //           \/             |r|^2
                // 
                // where the sign is positive if r x v ≥ 0 (or maybe the other way round).

                // Sin and cos of angle
                double m = Util.Sin(robot.aim + robot.look + 270);
                double n = Util.Cos(robot.aim + robot.look + 270);

                int dist = int.MaxValue;
                Robot target = null;
                foreach (Robot other in robot.parent.robots)
                {
                    if (other == null || !other.alive || other == robot)
                        continue;
                    
                    long a = (long)robot.x;
                    long b = (long)robot.y;
                    long c = (long)other.x;
                    long d = (long)other.y;
                    // /(m * m + n * n) deleted because it seems to equal 1
                    double t = (m * c + n * d - m * a - n * b);
                    // in sights
                    if (t > 0 && Math.Pow(m * t + a - c, 2) + Math.Pow(n * t + b - d, 2) < 
                        (Math.Pow(Constants.ROBOT_RADIUS, 2) - 9) &&
                        // closer
                        t < dist)
                    {
                        dist = (int)t;
                        target = other;
                    }
                }
                
                if (target != null)
                {
                    if (robot.team > 0 && robot.team == target.team)
                        return 0;  // Don't shoot own team member
                    if (target.energy < 0 || target.stunned > 0 || target.collision || target.wall)
                        return 0;
                    
                    long a = (long)(robot.x - target.x); // a = rx
                    long b = (long)(robot.y - target.y); // b = ry
                    long c = (long)(a * target.speedx + b * target.speedy); // c = r•v
                    double t = Math.Pow(target.speedx, 2) + Math.Pow(target.speedy, 2) -
                         (double)(c * c) / (double)(a * a + b * b);
                    double doppler = Math.Sqrt(t);
                    if (doppler - (long)doppler > 0.5)
                         doppler += 1.0;
                    return (Int16)(a * target.speedy - b * target.speedx > 0 ? -doppler : doppler);
                }
                return 0;
            }
            set {}
        }
    }
	
    // A random number from 0 to 359. May only be read.
    internal class RandomRegister : Register
    {
        internal RandomRegister() {
            name = "RANDOM";
            code = (Int16)Bytecodes.REG_RANDOM;
        }

        public override Int16 value
        {
            get { return (Int16)(robot.parent.prng.Next(0, 359)); }
            set {}
        }
    }
}