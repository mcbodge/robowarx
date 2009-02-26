using System;
using RoboWarX;
using RoboWarX.Arena;

namespace RoboWarX.Arena.Registers
{
    // May only be read. If another robot has collided with the current robot, the COLLISION
    // variable returns 1; otherwise it returns 0. When a collision with another robot takes place,
    // both robots take one point of damage each chronon until they separate. They may either
    // separate by changing direction, or by blowing the rival to little pieces.
    //
    // Sets the collision interrupt, to occur whenever the collision register of a robot changes
    // from 0 to 1. SETPARAM has no effect on the collision interrupt.
    internal class CollisionRegister : InterruptRegister
    {
        internal CollisionRegister() {
            name = "COLLISION";
            code = (Int16)Bytecodes.REG_COLLISION;
            oldCollision = false;
        }

        public override Int16 value
        {
            get { return (Int16) (robot.collision ? 1 : 0); }
            internal set {}
        }
        
        public override int order { get { return 300; } }
        
        // Interrupt triggers on the rising edge of robot.collision.
        private bool oldCollision;
        public override bool checkInterrupt() {
            bool retval = false;
            if (robot.collision && !oldCollision)
                retval = true;

            oldCollision = robot.collision;
            
            return retval;
        }
    }
    
    // Is the robot sensed in a collision on your team? FRIEND can only be read. If the collision
    // register holds a 1, the FRIEND register holds a 1 if the robot collided with is on the same
    // team. Otherwise, the FRIEND register holds a 0. This is useful to determine if a collision
    // has taken place with another robot on your same team.
    internal class FriendRegister : Register
    {
        internal FriendRegister()
        {
            name = "FRIEND";
            code = (Int16)Bytecodes.REG_FRIEND;
        }

        public override Int16 value
        {
            get { return (Int16)(robot.friend ? 1 : 0); }
            internal set {}
        }
    }
    
    // Long range probe of opponent's systems. Returns information about the target in the
    // direction of the AIM register when read; no effect if written. The register to probe is
    // chosen with the SETPARAM command (e.g. SHIELD' PROBE' SETPARAM to select the SHIELD register
    // for probing); it may be one of DAMAGE, ENERGY, SHIELD, ID, TEAMMATES, AIM, LOOK, SCAN.
    // Probes must be enabled in the hardware store at the cost of one hardware point. PROBE
    // defaults to probing the DAMAGE register if no other parameter has been set.
    internal class ProbeRegister : Register
    {
        enum ProbeTarget { Damage, Energy, Shield, ID, TeamMates, Aim, Look, Scan }
        private ProbeTarget register;

        internal ProbeRegister()
        {
            register = ProbeTarget.Damage;
            name = "PROBE";
            code = (Int16)Bytecodes.REG_PROBE;
        }

        public override Int16 value
        {
            get
            {
                if (!robot.hardware.probeFlag)
                    throw new HardwareException(this.robot, "Probes not enabled.");

                // Sin and cos of angle
                double m = Util.Sin(robot.aim + robot.look + 270);
                double n = Util.Cos(robot.aim + robot.look + 270);
                
                // Coordinates for this robot
                long a = (long)robot.x;
                long b = (long)robot.y;
                
                Robot target = null;
                int dist = int.MaxValue;
                foreach (Robot other in robot.parent.robots)
                {
                    if (other == null || !other.alive || other == robot)
                        continue;
                    
                    long c = (long)other.x;
                    long d = (long)other.y;
                    double t = (m * c + n * d - m * a -n * b) / (m * m + n * n);
                    // in sights
                    if (t > 0 && Math.Pow(m * t + a - c, 2) + Math.Pow(n * t + b - d, 2) < 
                            Math.Pow(Constants.ROBOT_RADIUS, 2) - 9 && t < dist)
                    {
                        dist = (int)t;
                        target = other;
                    }
                }
                if (target == null)
                    return 0;
                if (robot.team > 0 && robot.team == target.team)
                    return 0; // Don't probe team member
                
                switch (register)
                {
                case ProbeTarget.Damage: return (Int16)target.damage;
                case ProbeTarget.Energy: return (Int16)target.energy;
                case ProbeTarget.Shield: return (Int16)target.shield;
                case ProbeTarget.ID: return (Int16)target.number;
                case ProbeTarget.TeamMates: 
                    Int16 retval = 0;
                    foreach (Robot other in robot.parent.robots)
                        if (other != null && other != target && other.alive &&
                                other.team == target.team)
                            retval++;
                    return retval;
                case ProbeTarget.Aim: return (Int16)target.aim;
                case ProbeTarget.Look: return (Int16)target.look;
                case ProbeTarget.Scan: return (Int16)target.scan;
                }
                
                // FIXME: "not all code paths return a value".. Huh?
                throw new Exception("RoboWarX internal error: should not reach.");
            }
            internal set {}
        }
        
        public override Int16 param
        {
            internal set
            {
                switch (value)
                {
                case (Int16)Bytecodes.REG_DAMAGE: register = ProbeTarget.Damage; break;
                case (Int16)Bytecodes.REG_ENERGY: register = ProbeTarget.Energy; break;
                case (Int16)Bytecodes.REG_SHIELD: register = ProbeTarget.Shield; break;
                case (Int16)Bytecodes.REG_ID: register = ProbeTarget.ID; break;
                case (Int16)Bytecodes.REG_TEAMMATES: register = ProbeTarget.TeamMates; break;
                case (Int16)Bytecodes.REG_AIM: register = ProbeTarget.Aim; break;
                case (Int16)Bytecodes.REG_LOOK: register = ProbeTarget.Look; break;
                case (Int16)Bytecodes.REG_SCAN: register = ProbeTarget.Scan; break;
                default: throw new HardwareException(this.robot, "Illegal probe register.");
                }
            }
        }
    }
    
    // Range to nearest bullet, missile, mine, stunner or TacNuke in the path of AIM. May only be
    // read. RADAR checks a path 40 degrees wide centered on the AIM (actually AIM+SCAN, but SCAN
    // defaults to 0). It returns the distance to the nearest bullet, missile, or TacNuke in this
    // path. If there are none, it returns 0. Note that the weapon detected might be moving
    // perpendicular to the aim, not toward the robot.
    //
    // The radar interrupt is triggered at the beginning of a chronon or when the aim or scan
    // registers change and the RADAR register is nonzero. SETPARAM determines the maximum distance
    // at which a projectile will set off the interrupt; by default it is 600 to trigger on
    // anything.
    internal class RadarRegister : InterruptRegister
    {
        internal RadarRegister() {
            name = "RADAR";
            code = (Int16)Bytecodes.REG_RADAR;
            param = 600;
        }

        public override Int16 value
        {
            get
            {
                int scan = (robot.aim + robot.scan) % 360;
                int close = int.MaxValue;
                foreach (ArenaObject o in robot.parent.objects)
                {
                    //if (cur->type == laser) continue;
                    int theta = (int)(450 - Math.Atan2(robot.y - (int)o.y, (int)o.x - robot.x) *
                    Constants.RAD_TO_DEG) % 360;
                    if (Math.Abs(theta - scan) < 20 || Math.Abs(theta - scan) > 340)
                    {
                        int range = (int)(Math.Pow(robot.y - (int)o.y, 2) +
                                Math.Pow(robot.x - (int)o.x, 2));
                        if (range < close)
                            close = range;
                    }
                }
                if (close == int.MaxValue)
                    return 0;
                return (Int16)Math.Sqrt(close);
            }
            internal set {}
        }
        
        public override int order { get { return 1200; } }
        
        public override bool checkInterrupt() {
            Int16 dist = value;
            return dist > 0 && dist <= param;
        }
    }
    
    // Range to nearest target in sights. May only be read. If there is a target in the direction
    // the robot’s AIM points, RANGE returns the distance. Actually checks in direction AIM+LOOK,
    // but look defaults to 0. Otherwise, it returns 0.
    //
    // The range interrupt is much like RADAR, but triggers at either the beginning of a chronon or
    // when the aim or look registers change and RANGE is nonzero. SETPARAM determines the maximum
    // range that will trigger an interrupt and is also 600 by default.
    // anything.
    internal class RangeRegister : InterruptRegister
    {
        internal RangeRegister()
        {
            name = "RANGE";
            code = (Int16)Bytecodes.REG_RANGE;
            param = 600;
        }

        public override Int16 value
        {
            get
            {
                int retval = 0;
                Robot target = null;

                // Sin and cos of angle
                double m = Util.Sin(robot.aim + robot.look + 270);
                double n = Util.Cos(robot.aim + robot.look + 270);
                
                // Coordinates for this robot
                long a = (long)robot.x;
                long b = (long)robot.y;
                
                foreach (Robot other in robot.parent.robots)
                {
                    if (other == null || other == robot || !other.alive)
                        continue;
                    
                    // Coordinates for other robot
                    long c = (long)other.x;
                    long d = (long)other.y;
                    // Distance along line of sight
                    double t = m * (c - a) + n * (d - b);
                    // First a crude Manhatten Metric check
                    if (t > 0 && Math.Abs((m + n) * t + a + b - c - d) < 20 &&
                            // in sights
                            Math.Pow(m * t + a - c, 2) + Math.Pow(n * t + b - d, 2) <
                            (Math.Pow(Constants.ROBOT_RADIUS, 2) - 9) &&
                            // closer
                            (retval == 0 || t < retval))
                        {
                            retval = (int)t;
                            target = other;
                        }
                }
                
                // Don't shoot own team member
                if (target != null && robot.team != 0 && robot.team == target.team)
                    return 0;
                return (Int16)retval;
            }
            internal set {}
        }
        
        public override int order { get { return 1300; } }
        
        public override bool checkInterrupt() {
            Int16 dist = this.value;
            if (dist > 0 && dist < this.param)
                return true;
            else
                return false;
        }
    }
}