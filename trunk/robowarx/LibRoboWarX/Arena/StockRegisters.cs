using System;
using RoboWarX;
using RoboWarX.Arena;

namespace RoboWarX.Arena.StockRegisters
{
    // User-defined variables. They may be used for any temporary storage that the robot needs.
    // They may be read or written.
    internal class CustomRegister : Register
    {
        public readonly String name;
        public readonly Int16 code;
        
        private Int16 value_;

        internal CustomRegister(char name)
        {
            this.name = name.ToString();
            this.code = (Int16) ((int)Bytecodes.REG_PRIV_MIN + (name - 'A'));
        }

        public override Int16 value
        {
            get { return value_; }
            set { value_ = value; }
        }

        public override Int16 param
        {
            set {}
        }

        public override Object Clone() { return new CustomRegister(name[0]); }
        public override bool checkInterrupt() { return false; }
        public override void updateInterruptState() {}
    }
    
    // Angle turret points. May be read or written. The angle is in degrees, oriented like a\
    // compass with 0 degrees pointing upward and 90 degrees pointing to the right. All bullets and
    // missiles are fired in the direction that the turret is pointing.
    internal class AimRegister : Register
    {
        public const String name = "AIM";
        public const Int16 code = (Int16)Bytecodes.REG_AIM;

        internal AimRegister() {}

        public override Int16 value
        {
            get { return (Int16) robot.aim; }
            set
            {
                robot.aim = (Int16)(value % 360);
                if (robot.aim < 0)
                    robot.aim += 360;
            }
        }

        public override Int16 param
        {
            set {}
        }

        public override Object Clone() { return new AimRegister(); }
        public override bool checkInterrupt() { return false; }
        public override void updateInterruptState() {}
    }
    
    // This register is used exclusively for interrupts. It has no effect if written and returns 0
    // if read.
    //
    // The bottom interrupt is triggered whenever a robot moves too close to the bottom wall.
    // SETPARAM determines the y coordinate at which the interrupt is triggered; by default, it is
    // 280.
    //
    // FIXME: interrupt support
    internal class BottomRegister : Register
    {
        public static readonly String[] names = new String[] { "BOT", "BOTTOM" };
        public const Int16 code = (Int16)Bytecodes.REG_BOTTOM;

        internal BottomRegister() {}

        public override Int16 value
        {
            get { return 0; }
            set {}
        }

        public override Int16 param
        {
            set {}
        }

        public override Object Clone() { return new BottomRegister(); }
        public override bool checkInterrupt() { return false; }
        public override void updateInterruptState() {}
    }
    
    // The robot’s broadcasting and receiving channel. May be read
    internal class ChannelRegister : Register
    {
        public const String name = "CHANNEL";
        public const Int16 code = (Int16)Bytecodes.REG_CHANNEL;

        internal ChannelRegister() {}

        public override Int16 value
        {
            get { return (Int16) robot.channel; }
            set
            {
                if (value < 1 || value > 10)
                    throw new HardwareException(this.robot, "Invalid channel (1-10).");
                robot.channel = value;
            }
        }

        public override Int16 param
        {
            set {}
        }

        public override Object Clone() { return new ChannelRegister(); }
        public override bool checkInterrupt() { return false; }
        public override void updateInterruptState() {}
    }
    
    // Returns the number of chronons elapsed in the current battle. CHRONON may only be read.
    //
    // This interrupt is triggered at the start of each chronon, starting at the chronon set by the
    // parameter. By default, the parameter is set to 0. This interrupt is useful for animated
    // icons or to change behavior after a specific amount of time.
    //
    // FIXME: interrupt support
    internal class ChrononRegister : Register
    {
        public const String name = "CHRONON";
        public const Int16 code = (Int16)Bytecodes.REG_CHRONON;

        internal ChrononRegister() {}

        public override Int16 value
        {
            get { return (Int16)robot.parent_.chronon_; }
            set {}
        }

        public override Int16 param
        {
            set {}
        }

        public override Object Clone() { return new ChrononRegister(); }
        public override bool checkInterrupt() { return false; }
        public override void updateInterruptState() {}
    }

    // May only be read. If another robot has collided with the current robot, the COLLISION
    // variable returns 1; otherwise it returns 0. When a collision with another robot takes place,
    // both robots take one point of damage each chronon until they separate. They may either
    // separate by changing direction, or by blowing the rival to little pieces.
    //
    // Sets the collision interrupt, to occur whenever the collision register of a robot changes
    // from 0 to 1. SETPARAM has no effect on the collision interrupt.
    //
    // FIXME: interrupt support
    internal class CollisionRegister : Register
    {
        public const String name = "COLLISION";
        public const Int16 code = (Int16)Bytecodes.REG_COLLISION;

        internal CollisionRegister() {}

        public override Int16 value
        {
            get { return (Int16) (robot.collision ? 1 : 0); }
            set {}
        }

        public override Int16 param
        {
            set {}
        }

        public override Object Clone() { return new CollisionRegister(); }
        public override bool checkInterrupt() { return false; }
        public override void updateInterruptState() {}
    }
    
    // Robot’s current damage rating. May only be read. When the battle begins, the damage rating
    // starts at the maximum value set at the Hardware Store. Damage caused by bullets, missiles,
    // and TacNukes that is not absorbed by the robot’s shields is removed from the damage rating.
    // When it reaches 0, the robot is dead.
    //
    // The damage interrupt is triggered whenever a robot takes damage. SETPARAM sets the minimum
    // threshold required for the damage interrupt to occur; by default it is set to 150. This is
    // useful if a robot should only change its behavior when it is damaged beyond a certain point.
    //
    // FIXME: interrupt support
    internal class DamageRegister : Register
    {
        public const String name = "DAMAGE";
        public const Int16 code = (Int16)Bytecodes.REG_DAMAGE;

        internal DamageRegister() {}

        public override Int16 value
        {
            get { return (Int16)robot.damage_; }
            set {}
        }

        public override Int16 param
        {
            set {}
        }

        public override Object Clone() { return new DamageRegister(); }
        public override bool checkInterrupt() { return false; }
        public override void updateInterruptState() {}
    }
    
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
        public const String name = "DOPPLER";
        public const Int16 code = (Int16)Bytecodes.REG_DOPPLER;

        internal DopplerRegister() {}

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
                foreach (Robot other in robot.parent_.robots)
                {
                    if (other == null || !other.alive || other == robot)
                        continue;
                    
                    long a = (long)robot.x_;
                    long b = (long)robot.y_;
                    long c = (long)other.x_;
                    long d = (long)other.y_;
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
                    if (robot.team_ > 0 && robot.team_ == target.team_)
                        return 0;  // Don't shoot own team member
                    if (target.energy < 0 || target.stunned > 0 || target.collision || target.wall)
                        return 0;
                    
                    long a = (long)(robot.x_ - target.x_); // a = rx
                    long b = (long)(robot.y_ - target.y_); // b = ry
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

        public override Int16 param
        {
            set {}
        }

        public override Object Clone() { return new DopplerRegister(); }
        public override bool checkInterrupt() { return false; }
        public override void updateInterruptState() {}
    }
    
    // Robot’s current energy. May be read, but not written. ENERGY returns the amount of energy
    // the robot currently has. If not used for other purposes, energy is restored at 2 points per
    // chronon. However, if the energy ever drops below 0, the robot does not interpret any more
    // instructions or perform any more actions until the energy exceeds 0 again. When the battle
    // begins energy is set to the maximum energy value specified in the Hardware Store.
    internal class EnergyRegister : Register
    {
        public const String name = "ENERGY";
        public const Int16 code = (Int16)Bytecodes.REG_ENERGY;

        internal EnergyRegister() {}

        public override Int16 value
        {
            get { return (Int16)robot.energy_; }
            set {}
        }

        public override Int16 param
        {
            set {}
        }

        public override Object Clone() { return new EnergyRegister(); }
        public override bool checkInterrupt() { return false; }
        public override void updateInterruptState() {}
    }
    
    // Is the robot sensed in a collision on your team? FRIEND can only be read. If the collision
    // register holds a 1, the FRIEND register holds a 1 if the robot collided with is on the same
    // team. Otherwise, the FRIEND register holds a 0. This is useful to determine if a collision
    // has taken place with another robot on your same team.
    internal class FriendRegister : Register
    {
        public const String name = "FRIEND";
        public const Int16 code = (Int16)Bytecodes.REG_FRIEND;

        internal FriendRegister() {}

        public override Int16 value
        {
            get { return (Int16)(robot.friend ? 1 : 0); }
            set {}
        }

        public override Int16 param
        {
            set {}
        }

        public override Object Clone() { return new FriendRegister(); }
        public override bool checkInterrupt() { return false; }
        public override void updateInterruptState() {}
    }
    
    // Maintains a history of results and observations between battles. Each robot has 50 history
    // registers. The history register to read or write is selected with the setparam command (e.g.
    // 2 HISTORY' SETPARAM). The history registers are:
    //    1) Number of battles fought
    //    2) Kills made in previous battle
    //    3) Kills made in all battles
    //    4) Survival points in previous battle
    //    5) Survival points from all battles
    //    6) 1 if last battle timed out
    //    7) Teammates alive at end of last battle (excluding self)
    //    8) Teammates alive at end of all battle
    //    9) Damage at end of last battle (0 if dead)
    //    10) Chronons elapsed at end of last battle
    //    11) Chronons elapsed in all previous battles
    //    12-30) Reserved for future RoboWar versions
    //    31-50) User-defined.
    // When no battles have been fought yet, numbers default to zero. User-defined history
    // registers may be read or written and are preserved between rounds so robots can learn from
    // the results of previous rounds. All other history registers may only be read. Suggestions
    // for new history registers are welcome. History is zeroed anytime robots are added or deleted
    // in the arena (or between rounds in tournaments). It may also be erased or viewed with the
    // history commands under the Arena menu.
    // Warning: in very long tournaments, it is conceivable that the history register could
    // overflow 32767, the maximum integer in RoboWar. This is especially true for register 11.
    // Hacker beware!
    internal class HistoryRegister : Register
    {
        public const String name = "HISTORY";
        public const Int16 code = (Int16)Bytecodes.REG_HISTORY;
        
        private int index;

        internal HistoryRegister()
        {
            index = 0;
        }

        public override Int16 value
        {
            get { return robot.history[index]; }
            set
            {
                if (index < 30)
                    throw new HardwareException(this.robot, "Store to reserved history register.");
                robot.history[index] = value;
            }
        }

        public override Int16 param
        {
            set
            {
                if (value <= 0 || value > robot.history.Length)
                    throw new HardwareException(this.robot, "Illegal history value.");
                index = value - 1;
            }
        }

        public override Object Clone() { return new HistoryRegister(); }
        public override bool checkInterrupt() { return false; }
        public override void updateInterruptState() {}
    }
    
    // Robot's unique ID number. Each robot in the Arena has an ID from 0-5; it can be used to tell
    // robots apart.
    internal class IDRegister : Register
    {
        public const String name = "ID";
        public const Int16 code = (Int16)Bytecodes.REG_ID;

        internal IDRegister() {}

        public override Int16 value
        {
            get { return (Int16)robot.number; }
            set {}
        }

        public override Int16 param
        {
            set {}
        }

        public override Object Clone() { return new IDRegister(); }
        public override bool checkInterrupt() { return false; }
        public override void updateInterruptState() {}
    }
    
    // The number of kills the robot has made in this battle. A robot gets no credit for killing
    // itself or for crushing other robots during a collision.
    internal class KillsRegister : Register
    {
        public const String name = "KILLS";
        public const Int16 code = (Int16)Bytecodes.REG_KILLS;

        internal KillsRegister() {}

        public override Int16 value
        {
            get { return (Int16)robot.kills; }
            set {}
        }

        public override Int16 param
        {
            set {}
        }

        public override Object Clone() { return new KillsRegister(); }
        public override bool checkInterrupt() { return false; }
        public override void updateInterruptState() {}
    }
    
    // This register is used exclusively for interrupts. It has no effect if written and returns 0
    // if read.
    //
    // The left interrupt is triggered whenever a robot moves too close to the left wall. SETPARAM
    // determines the x coordinate at which the interrupt is triggered; by default, it is 20.
    //
    // FIXME: interrupt support
    internal class LeftRegister : Register
    {
        public const String name = "LEFT";
        public const Int16 code = (Int16)Bytecodes.REG_LEFT;

        internal LeftRegister() {}

        public override Int16 value
        {
            get { return 0; }
            set {}
        }

        public override Int16 param
        {
            set {}
        }

        public override Object Clone() { return new LeftRegister(); }
        public override bool checkInterrupt() { return false; }
        public override void updateInterruptState() {}
    }

    // Targeting offset from AIM. The RANGE command returns a distance to the nearest robot in the
    // direction AIM+LOOK. This might be useful for some tracking algorithm. If not otherwise set,
    // LOOK defaults to 0. LOOK may be read or written.
    internal class LookRegister : Register
    {
        public const String name = "LOOK";
        public const Int16 code = (Int16)Bytecodes.REG_LOOK;

        internal LookRegister() {}

        public override Int16 value
        {
            get { return (Int16)robot.look; }
            set
            {
                robot.look = (Int16)(value % 360);
                if (robot.look < 0)
                    robot.look += 360;
            }
        }

        public override Int16 param
        {
            set {}
        }

        public override Object Clone() { return new LookRegister(); }
        public override bool checkInterrupt() { return false; }
        public override void updateInterruptState() {}
    }

    // Used to move the robot a given distance in the X direction without changing SPEEDX. Returns
    // 0 if read, moves the robot the specified distance if written. Movement costs two points of
    // energy per unit moved. Note that you can’t fire a weapon of any type and move in the same
    // chronon.
    internal class MoveXRegister : Register
    {
        public const String name = "MOVEX";
        public const Int16 code = (Int16)Bytecodes.REG_MOVEX;

        internal MoveXRegister() {}

        public override Int16 value
        {
            get { return 0; }
            set
            {
                int energy = robot.useEnergy(Math.Abs(value) * 2);
                double newx = robot.x_ + Math.Sign(value) * Math.Floor((float)energy / 2);
                newx = Math.Min(newx, Constants.ARENA_SIZE);
                newx = Math.Max(newx, 0);
                robot.x_ = newx;
            }
        }

        public override Int16 param
        {
            set {}
        }
                    
        public override Object Clone() { return new MoveXRegister(); }
        public override bool checkInterrupt() { return false; }
        public override void updateInterruptState() {}
    }

    // Used to move the robot a given distance in the Y direction without changing SPEEDY. MOVEY
    // has the same characteristics as MOVEX. Note that you can’t fire a weapon of any type and
    // move in the same chronon.
    internal class MoveYRegister : Register
    {
        public const String name = "MOVEY";
        public const Int16 code = (Int16)Bytecodes.REG_MOVEY;

        internal MoveYRegister() {}

        public override Int16 value
        {
            get { return 0; }
            set
            {
                int energy = robot.useEnergy(Math.Abs(value) * 2);
                double newy = robot.y_ + Math.Sign(value) * Math.Floor((float)energy / 2);
                newy = Math.Min(newy, Constants.ARENA_SIZE);
                newy = Math.Max(newy, 0);
                robot.y_ = newy;
            }
        }
        
        public override Int16 param
        {
            set {}
        }
        
        public override Object Clone() { return new MoveYRegister(); }
        public override bool checkInterrupt() { return false; }
        public override void updateInterruptState() {}
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
        
        public const String name = "PROBE";
        public const Int16 code = (Int16)Bytecodes.REG_PROBE;
        
        private ProbeTarget register;

        internal ProbeRegister()
        {
            register = ProbeTarget.Damage;
        }

        public override Int16 value
        {
            get
            {
                if (!robot.hardware_.probeFlag)
                    throw new HardwareException(this.robot, "Probes not enabled.");

                // Sin and cos of angle
                double m = Util.Sin(robot.aim + robot.look + 270);
                double n = Util.Cos(robot.aim + robot.look + 270);
                
                // Coordinates for this robot
                long a = (long)robot.x_;
                long b = (long)robot.y_;
                
                Robot target = null;
                int dist = int.MaxValue;
                foreach (Robot other in robot.parent_.robots)
                {
                    if (other == null || !other.alive || other == robot)
                        continue;
                    
                    long c = (long)other.x_;
                    long d = (long)other.y_;
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
                if (robot.team_ > 0 && robot.team_ == target.team_)
                    return 0; // Don't probe team member
                
                switch (register)
                {
                case ProbeTarget.Damage: return (Int16)target.damage;
                case ProbeTarget.Energy: return (Int16)target.energy;
                case ProbeTarget.Shield: return (Int16)target.shield;
                case ProbeTarget.ID: return (Int16)target.number;
                case ProbeTarget.TeamMates: 
                    Int16 retval = 0;
                    foreach (Robot other in robot.parent_.robots)
                        if (other != null && other != target && other.alive_ &&
                                other.team_ == target.team_)
                            retval++;
                    return retval;
                case ProbeTarget.Aim: return (Int16)target.aim;
                case ProbeTarget.Look: return (Int16)target.look;
                case ProbeTarget.Scan: return (Int16)target.scan;
                }
                
                // FIXME: "not all code paths return a value".. Huh?
                throw new Exception("RoboWarX internal error: should not reach.");
            }
            set {}
        }
        
        public override Int16 param
        {
            set
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
        
        public override Object Clone() { return new ProbeRegister(); }
        public override bool checkInterrupt() { return false; }
        public override void updateInterruptState() {}
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
    //
    // FIXME: interrupt support
    internal class RadarRegister : Register
    {
        public const String name = "RADAR";
        public const Int16 code = (Int16)Bytecodes.REG_RADAR;

        internal RadarRegister() {}

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
            set {}
        }

        public override Int16 param
        {
            set {}
        }

        public override Object Clone() { return new RadarRegister(); }
        public override bool checkInterrupt() { return false; }
        public override void updateInterruptState() {}
    }

    // A random number from 0 to 359. May only be read.
    internal class RandomRegister : Register
    {
        public const String name = "RANDOM";
        public const Int16 code = (Int16)Bytecodes.REG_RANDOM;

        internal RandomRegister() {}

        public override Int16 value
        {
            get { return (Int16)(robot.parent.prng.Next(0, 359)); }
            set {}
        }

        public override Int16 param
        {
            set {}
        }

        public override Object Clone() { return new RandomRegister(); }
        public override bool checkInterrupt() { return false; }
        public override void updateInterruptState() {}
    }
    
    // Range to nearest target in sights. May only be read. If there is a target in the direction
    // the robot’s AIM points, RANGE returns the distance. Actually checks in direction AIM+LOOK,
    // but look defaults to 0. Otherwise, it returns 0.
    //
    // The range interrupt is much like RADAR, but triggers at either the beginning of a chronon or
    // when the aim or look registers change and RANGE is nonzero. SETPARAM determines the maximum
    // range that will trigger an interrupt and is also 600 by default.
    // anything.
    //
    // FIXME: interrupt support
    internal class RangeRegister : Register
    {
        public const String name = "RANGE";
        public const Int16 code = (Int16)Bytecodes.REG_RANGE;

        internal RangeRegister() {}

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
                long a = (long)robot.x_;
                long b = (long)robot.y_;
                
                foreach (Robot other in robot.parent.robots)
                {
                    if (other == null || other == robot || !other.alive_)
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
                if (target != null && robot.team_ != 0 && robot.team_ == target.team_)
                    return 0;
                return (Int16)retval;
            }
            set {}
        }

        public override Int16 param
        {
            set {}
        }

        public override Object Clone() { return new RangeRegister(); }
        public override bool checkInterrupt() { return false; }
        public override void updateInterruptState() {}
    }
    
    // This register is used exclusively for interrupts. It has no effect if written and returns 0
    // if read.
    //
    // The right interrupt is triggered whenever a robot moves too close to the right wall.
    // SETPARAM determines the x coordinate at which theinterrupt is triggered; by default, it is
    // 280.
    //
    // FIXME: interrupt support
    internal class RightRegister : Register
    {
        public const String name = "RIGHT";
        public const Int16 code = (Int16)Bytecodes.REG_RIGHT;

        internal RightRegister() {}

        public override Int16 value
        {
            get { return 0; }
            set {}
        }

        public override Int16 param
        {
            set {}
        }

        public override Object Clone() { return new RightRegister(); }
        public override bool checkInterrupt() { return false; }
        public override void updateInterruptState() {}
    }
    
    // Number of robots alive. Returns the number of robots alive in the arena, including the robot
    // itself.
    //
    // The ROBOTS interrupt is triggered whenever a robot is killed. The robot can specify for the
    // interrupt to only occur when the number (including yourself) is below a particular value by
    // using SETPARAM. For instance, to only interrupt when all other robots are dead, set the
    // parameter to 2. By default, the parameter is set to 6, causing an interrupt when any robot
    // dies.
    //
    // FIXME: interrupt support
    internal class RobotsRegister : Register
    {
        public const String name = "ROBOTS";
        public const Int16 code = (Int16)Bytecodes.REG_ROBOTS;

        internal RobotsRegister() {}

        public override Int16 value
        {
            get
            {
                Int16 retval = 0;
                foreach (Robot r in robot.parent_.robots)
                    if (r != null && r.alive_)
                        retval++;
                return retval;
            }
            set {}
        }

        public override Int16 param
        {
            set {}
        }

        public override Object Clone() { return new RobotsRegister(); }
        public override bool checkInterrupt() { return false; }
        public override void updateInterruptState() {}
    }

    // Similar to LOOK, the radar offset from the AIM. The RADAR command searches for projectiles
    // in the directon AIM+SCAN. This might beuseful if a robot wants to look for targets in one
    // direction, but check for danger in another direction. If not otherwise set, SCAN defaults
    // to 0. SCAN may be read or written.
    internal class ScanRegister : Register
    {
        public const String name = "SCAN";
        public const Int16 code = (Int16)Bytecodes.REG_SCAN;

        internal ScanRegister() {}

        public override Int16 value
        {
            get { return (Int16)robot.scan; }
            set
            {
                robot.scan = (Int16)(value % 360);
                if (robot.scan < 0)
                    robot.scan += 360;
            }
        }

        public override Int16 param
        {
            set {}
        }

        public override Object Clone() { return new ScanRegister(); }
        public override bool checkInterrupt() { return false; }
        public override void updateInterruptState() {}
    }

    // Robot’s current shield level. May be read or written. If read, it returns the current level
    // of the shield, or 0 if no shields are up. If written, is sets the shield level to the value
    // written. If the current level is less than the level written, a point of energy is used for
    // each point added to the shields. If not enough energy is available to set the shields, the
    // shields are only strengthened as far as remaining energy permits. If the current level is
    // greater than the level written, a point of energy is regained for each point of difference,
    // although energy cannot exceed the maximum energy value set in the Hardware Store. Shields
    // can absorb damage from bullets, missiles, or TacNukes that otherwise would have been
    // deducted from a robot’s damage score. Each point of damage that is done deducts one point
    // from the shield level, until no power is left in the shields. The remaining damage is then
    // done to a robot’s damage score. Even if shields are not hit, they decrease by one half point
    // each chronon from natural energy decay. Note that this replaces the old drain of one point
    // per chronon in previous versions of RoboWar. Shields may be charged above the maximum shield
    // value set in the Hardware Store (although they may never exceed 150), but if they are above
    // maximum, they decrease by two points instead of one half per chronon. Shields are set to 0
    // when the battle begins.
    //
    // The shield interrupt is triggered whenever a robot's shield transitions from above to below
    // a predetermined threshold. SETPARAM sets this threshold; by default it is 25.
    //
    // FIXME: interrupt support
    internal class ShieldRegister : Register
    {
        public const String name = "SHIELD";
        public const Int16 code = (Int16)Bytecodes.REG_SHIELD;

        internal ShieldRegister() {}

        public override Int16 value
        {
            get { return (Int16)robot.shield; }
            set
            {
                if (value < 0)
                    throw new HardwareException(this.robot, "Cannot set shield below zero.");
                int v = Math.Min((int)value, 150);

                int old = robot.shield;
                if (robot.hardware_.noNegEnergy && v > robot.energy_ + old)
                    v = robot.energy_ + old;
                robot.shield = v;
                robot.energy_ += Math.Min(old - v, robot.hardware_.energyMax);
            }
        }

        public override Int16 param
        {
            set {}
        }

        public override Object Clone() { return new ShieldRegister(); }
        public override bool checkInterrupt() { return false; }
        public override void updateInterruptState() {}
    }
    
    // The signal value on the robot’s current channel. May be read or written. If it is read, it
    // returns the last value broadcast over the channel by any robot on the same team. If it is
    // written, the value written is broadcast over the channel and may be read any time in the
    // future by any other robot on the same team. Typically signals and channels are used by two
    // or more robots to coordinate movement or team up against another set of robots.
    //
    // The signal interrupt is triggered when data is broadcast over the communication channels by
    // a robot's teammate. SETPARAM determines which channel number is being checked; by default it
    // is channel 0. It is generally a good idea for different robots to transmit on different
    // channels to prevent one robot from overwriting the data sent by the other. Also, since
    // multiple messages that are rapidly sent might be lost, it is generally wise for the RoboTalk
    // hacker to devise some protocol for teammates to acknowledge each other's transmissions.
    //
    // FIXME: interrupt support
    internal class SignalRegister : Register
    {
        public const String name = "SIGNAL";
        public const Int16 code = (Int16)Bytecodes.REG_SIGNAL;

        internal SignalRegister() {}

        public override Int16 value
        {
            get { return robot.signals[robot.channel]; }
            set
            {
                if (robot.team_ == 0)
                    robot.signals[robot.channel] = value;
                else
                {
                    foreach (Robot other in robot.parent_.robots)
                        if (other != null && other.alive && robot.team_ == other.team_)
                            other.signals[robot.channel] = value;
                }
            }
        }

        public override Int16 param
        {
            set {}
        }

        public override Object Clone() { return new SignalRegister(); }
        public override bool checkInterrupt() { return false; }
        public override void updateInterruptState() {}
    }
    
    // Speed of robot in left-right direction. May be read or written. Positive speeds move right,
    // while negative speeds move to the left of the screen. If SPEEDX is read, it returns the
    // current velocity; if it is written, it sets the velocity. Speeds must be in the range of -20
    // to 20. Each point of change in speed costs 2 points of energy; thus going from 10 to -2
    // costs 24 energy.
    internal class SpeedXRegister : Register
    {
        public const String name = "SPEEDX";
        public const Int16 code = (Int16)Bytecodes.REG_SPEEDX;

        internal SpeedXRegister() {}

        public override Int16 value
        {
            get { return (Int16) robot.speedx; }
            set
            {
                int old = value;
                if (value > 20) value = 20;
                else if (value < -20) value = -20;
                int cost = Math.Abs(value - old) << 1;
                if (robot.hardware_.noNegEnergy && cost > robot.energy_) {
                    int delta = robot.energy_ / 2;
                    if (value < old) robot.speedx -= delta;
                    else robot.speedx += delta;
                    robot.energy_ = 0;
                }
                else
                {
                    robot.speedx = value;
                    robot.useEnergy(cost);
                }
            }
        }

        public override Int16 param
        {
            set {}
        }

        public override Object Clone() { return new SpeedXRegister(); }
        public override bool checkInterrupt() { return false; }
        public override void updateInterruptState() {}
    }
    
    // Speed of robot in up-down direction. May be read or written. Positive values move down,
    // while negative values move up. SPEEDY has the same limits and characteristics as SPEEDX.
    internal class SpeedYRegister : Register
    {
        public const String name = "SPEEDY";
        public const Int16 code = (Int16)Bytecodes.REG_SPEEDY;

        internal SpeedYRegister() {}

        public override Int16 value
        {
            get { return (Int16) robot.speedy; }
            set
            {
                int old = value;
                if (value > 20) value = 20;
                else if (value < -20) value = -20;
                int cost = Math.Abs(value - old) << 1;
                if (robot.hardware_.noNegEnergy && cost > robot.energy_) {
                    int delta = robot.energy_ / 2;
                    if (value < old) robot.speedy -= delta;
                    else robot.speedy += delta;
                    robot.energy_ = 0;
                }
                else
                {
                    robot.speedy = value;
                    robot.useEnergy(cost);
                }
            }
        }

        public override Int16 param
        {
            set {}
        }

        public override Object Clone() { return new SpeedYRegister(); }
        public override bool checkInterrupt() { return false; }
        public override void updateInterruptState() {}
    }
    
    // Number of living teammates, not including self. May only be read.
    //
    // The TEAMMATES interrupt is triggered whenever the robot's teammate is killed. The robot can
    // specify for the interrupt to only occur when the number (excluding yourself) is below a
    // particular value by using SETPARAM. For instance, to only interrupt when all of your
    // teammates are dead, set the parameter to 1. By default, the parameter is set to 5, causing
    // an interrupt when any teammate dies.
    //
    // FIXME: interrupt support
    internal class TeamMatesRegister : Register
    {
        public const String name = "TEAMMATES";
        public const Int16 code = (Int16)Bytecodes.REG_TEAMMATES;

        internal TeamMatesRegister() {}

        public override Int16 value
        {
            get
            {
                if (robot.team_ == 0)
                    return 0;
                Int16 retval = 0;
                foreach (Robot other in robot.parent_.robots)
                    if (other != null && other != robot && other.alive_ &&
                            other.team_ == robot.team_)
                        retval++;
                return retval;
            }
            set {}
        }

        public override Int16 param
        {
            set {}
        }

        public override Object Clone() { return new TeamMatesRegister(); }
        public override bool checkInterrupt() { return false; }
        public override void updateInterruptState() {}
    }
    
    // This register is used exclusively for interrupts. It has no effect if written and returns 0
    // if read.
    //
    // The top interrupt is triggered whenever a robot moves too close to the top wall. SETPARAM
    // determines the y coordinate at which the interrupt is triggered; by default, it is 20.
    // Note that the directional interrupts are only triggered once when the robot crosses the
    // threshold.
    //
    // FIXME: interrupt support
    internal class TopRegister : Register
    {
        public const String name = "TOP";
        public const Int16 code = (Int16)Bytecodes.REG_TOP;

        internal TopRegister() {}

        public override Int16 value
        {
            get { return 0; }
            set {}
        }

        public override Int16 param
        {
            set {}
        }

        public override Object Clone() { return new TopRegister(); }
        public override bool checkInterrupt() { return false; }
        public override void updateInterruptState() {}
    }

    // Is the robot touching the electrified walls? Returns 1 when read if the robot is touching
    // the wall, or 0 otherwise. No effect if written.
    //
    // Much like collision, but occurs when a robot runs into a wall. SETPARAM also has no effect.
    //
    // FIXME: interrupt support
    internal class WallRegister : Register
    {
        public const String name = "WALL";
        public const Int16 code = (Int16)Bytecodes.REG_WALL;

        internal WallRegister() {}

        public override Int16 value
        {
            get { return (Int16) (robot.wall ? 1 : 0); }
            set {}
        }

        public override Int16 param
        {
            set {}
        }

        public override Object Clone() { return new WallRegister(); }
        public override bool checkInterrupt() { return false; }
        public override void updateInterruptState() {}
    }
    
    // X position of robot. May range from 0 to 300 (the boundaries of the board). 0 is the left
    // side; 300 is the right. X may be read but may not be written (no unrestricted teleporting!).
    internal class XRegister : Register
    {
        public const String name = "X";
        public const Int16 code = (Int16)Bytecodes.REG_X;

        internal XRegister() {}

        public override Int16 value
        {
            get { return (Int16)robot.x; }
            set {}
        }

        public override Int16 param
        {
            set {}
        }

        public override Object Clone() { return new XRegister(); }
        public override bool checkInterrupt() { return false; }
        public override void updateInterruptState() {}
    }
    
    // Y position of robot. May range from 0 to 300. 0 is the top; 300 is the bottom. Y may be read
    // but not written.
    internal class YRegister : Register
    {
        public const String name = "Y";
        public const Int16 code = (Int16)Bytecodes.REG_Y;

        internal YRegister() {}

        public override Int16 value
        {
            get { return (Int16)robot.y; }
            set {}
        }

        public override Int16 param
        {
            set {}
        }

        public override Object Clone() { return new YRegister(); }
        public override bool checkInterrupt() { return false; }
        public override void updateInterruptState() {}
    }

    // Stock plugin entry
    public class StockPluginEntry : IPluginEntry
    {
        public ITemplateRegister[] getPrototypes()
        {
            ITemplateRegister[] retval = new ITemplateRegister[56];
            for (char c = 'A'; c <= 'Z'; c++)
            {
                if (c == 'X' || c == 'Y')
                    continue;
                retval[c - 'A'] = new CustomRegister(c);
            }
            retval[26] = new AimRegister();
            retval[27] = new BottomRegister();
            retval[28] = new ChannelRegister();
            retval[29] = new ChrononRegister();
            retval[30] = new CollisionRegister();
            retval[31] = new DamageRegister();
            retval[32] = new DopplerRegister();
            retval[33] = new EnergyRegister();
            retval[34] = new FriendRegister();
            retval[35] = new HistoryRegister();
            retval[36] = new IDRegister();
            retval[37] = new KillsRegister();
            retval[38] = new LeftRegister();
            retval[39] = new LookRegister();
            retval[40] = new MoveXRegister();
            retval[41] = new MoveYRegister();
            retval[42] = new ProbeRegister();
            retval[43] = new RadarRegister();
            retval[44] = new RandomRegister();
            retval[45] = new RangeRegister();
            retval[46] = new RightRegister();
            retval[47] = new RobotsRegister();
            retval[48] = new ScanRegister();
            retval[49] = new ShieldRegister();
            retval[50] = new SignalRegister();
            retval[51] = new SpeedXRegister();
            retval[52] = new SpeedYRegister();
            retval[53] = new TeamMatesRegister();
            retval[54] = new TopRegister();
            retval[55] = new WallRegister();
            retval[23] = new XRegister();
            retval[24] = new YRegister();
            return retval;
        }
    }
}
