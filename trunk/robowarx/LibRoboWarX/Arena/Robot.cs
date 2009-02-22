using System;
using System.IO;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Reflection;
using System.Collections.Generic;
using RoboWarX;
using RoboWarX.VM;
using RoboWarX.FileFormats;

namespace RoboWarX.Arena
{
    public enum BulletType
    {
        None,
        Rubber,
        Normal,
        Explosive
    }

    public enum TurretType
    {
        None,
        Line,
        Dot
    }
    
    public sealed class HardwareInfo
    {
        public int energyMax { get; set; }            // Maximum amount of energy
        public int damageMax { get; set; }            // Maximum amount of damage
        public int shieldMax { get; set; }            // Maximum shield level for normal discharge
        public int processorSpeed { get; set; }       // Instructions per chronon

        public BulletType gunType { get; set; }
        public bool hasMissiles { get; set; }           // 1 = has missiles
        public bool hasTacNukes { get; set; }           // 1 = has tacNukes
        public bool hasLasers { get; set; }             // 1 = lasers
        public bool hasHellbores { get; set; }          // 1 = hellbore
        public bool hasDrones { get; set; }             // 1 = drone
        public bool hasMines { get; set; }              // 1 = mine
        public bool hasStunners { get; set; }           // 1 = stunner

        public bool noNegEnergy { get; set; }           // 1 = No Negative energy

        public bool probeFlag { get; set; }             // 1 = probes
        public bool deathIconFlag { get; set; }         // 1 = use icon for death
        public bool collisionIconFlag { get; set; }     // 1 = use icon for collision
        public bool shieldHitIconFlag { get; set; }     // 1 = use icon for shield hit
        public bool hitIconFlag { get; set; }           // 1 = use icon for hit
        public bool shieldOnIconFlag { get; set; }      // 1 = use icon for shieldon

        public HardwareInfo()
        {
            energyMax = 100;
            damageMax = 100;
            shieldMax = 50;
            processorSpeed = 10;
            gunType = BulletType.Normal;
            hasMissiles = false;
            hasTacNukes = false;
            hasHellbores = false;
            hasDrones = false;
            hasLasers = false;
            hasMines = false;
            hasStunners = false;
            noNegEnergy = false;
            probeFlag = false;
            deathIconFlag = false;
            collisionIconFlag = false;
            shieldHitIconFlag = false;
            hitIconFlag = false;
            shieldOnIconFlag = true;
        }

        // Calculate number of points
        public int advantages
        {
            get
            {
                int retval = 0;
                switch (energyMax)
                {
                    case 150: retval += 3; break;
                    case 100: retval += 2; break;
                    case 60: retval += 1; break;
                    case 40: break;
                    default: retval += 100; break;
                }
                switch (damageMax)
                {
                    case 150: retval += 3; break;
                    case 100: retval += 2; break;
                    case 60: retval += 1; break;
                    case 30: break;
                    default: retval += 100; break;
                }
                switch (shieldMax)
                {
                    case 100: retval += 3; break;
                    case 50: retval += 2; break;
                    case 25: retval += 1; break;
                    case 0: break;
                    default: retval += 100; break;
                }
                switch (processorSpeed)
                {
                    case 50: retval += 4; break;
                    case 30: retval += 3; break;
                    case 15: retval += 2; break;
                    case 10: retval += 1; break;
                    case 5: break;
                    default: retval += 100; break;
                }
                switch (gunType)
                {
                    case BulletType.Explosive: retval += 2; break;
                    case BulletType.Normal: retval += 1; break;
                    case BulletType.Rubber: break;
                    default: retval += 100; break;
                }
                if (hasMissiles) retval++;
                if (hasTacNukes) retval++;
                if (hasDrones) retval++;
                if (hasHellbores) retval++;
                if (hasMines) retval++;
                if (hasLasers) retval++;
                if (hasStunners) retval++;
                if (probeFlag) retval++;
                return retval;
            }
        }
    }
    
    public enum DeathReason
    {
        Killed,
        Suicided,
        Overloaded,
        Buggy
    }

    public sealed class Robot : ArenaObject
    {
        public RobotFile file { get; private set; }
        
        public int number { get; internal set; }
        public int team { get; internal set; }

        public bool alive { get; private set; }
        public DeathReason deathReason { get; internal set; }
        public int deathTime { get; private set; }
        public Robot killer { get; private set; }
        public RobotException bug { get; private set; }

        public int icon { get; internal set; }

        public Interpreter interp { get; private set; }

        public HardwareInfo hardware { get; private set; }

        public int energy { get; internal set; }
        public int damage { get; internal set; }
        public int shield { get; internal set; }
        public int aim { get; internal set; }
        public int look { get; internal set; }
        public int scan { get; internal set; }

        public bool collision { get; internal set; }
        public bool wall { get; internal set; }
        public bool friend { get; internal set; }
        public int stunned { get; internal set; }
        public int hit { get; internal set; }

        public int kills { get; internal set; }
        public int survival { get; internal set; }
        internal int[] killTime { get; set; }

        internal Int16[] history { get; set; }

        public int channel { get; internal set; }
        internal Int16[] signals { get; set; }

        // Robot is the only ArenaObject with no OnSpawn.
        internal Robot(Arena P, double X, double Y, int number, RobotFile file) : base(P, X, Y)
        {
            this.number = number;
			this.file = file;
			
            interp = new Interpreter(new MemoryStream(file.program));
            
            team = 0;
            alive = true;
            deathReason = DeathReason.Suicided;
            icon = 0;
            
            hardware = file.hardware;
            energy = hardware.energyMax;
            damage = hardware.damageMax;
            shield = 0;
            aim = 90;
            look = 0;
            scan = 0;

            collision = false;
            wall = false;
            friend = false;
            stunned = 0;
            hit = 0;

            kills = 0;
            survival = 0;
            killTime = new int[6]{ -1, -1, -1, -1, -1, -1 };
            deathTime = -1;
            killer = null;
            
            // FIXME: actually implement the history
            history = new Int16[50];
            
            // FIXME: actually implement signals
            signals = new Int16[10];
        }

        public String name
        {
            get { return file.name; }
        }

        // We got hit by someone
        public bool doShotDamage(int amount, Robot from)
        {
            // Attribute kills to proper owner
            int oldDamage = damage;
            doDamage(amount);
            if (damage <= 0 && oldDamage > 0 && this != from
                && (from.energy > -200) && !(team != 0 && (team == from.team)))
            {
                deathReason = DeathReason.Killed;
                from.kills++;
                from.killTime[number] = parent.chronon;
                killer = from;
            }
            return damage < oldDamage; // return true if robot took damage
        }

        // Take damage
        public void doDamage(int amount)
        {
            if (shield == 0)
                damage -= amount;
            else if (shield < amount)
            {
                damage -= amount - shield;
                shield = 0;
            }
            else
                shield -= amount;
        }

        // He's dead, Jim.
        internal void explode()
        {
            shield = 0;
            alive = false;
            deathTime = parent.chronon;

            // Hack for the exploding animation
            aim = 1;
            speedx = x;
            speedy = y;

            // Move out of the way
            x = 5000;
            y = 5000;
        }

        // Instantiate a projectile, taking into account the robots aim.
        public ArenaObject shoot(Projectile projectile, int energy, params object[] parameters)
        {
            projectile.parent = parent;
            if (projectile.offset)
            {
                double anglex = Util.Sin(aim + 270);
                double angley = Util.Cos(aim + 270);
                projectile.x = x + anglex * (Constants.ROBOT_RADIUS + 1);
                projectile.y = y + angley * (Constants.ROBOT_RADIUS + 1);
                projectile.onSpawn(this, anglex, angley);
            }
            else
            {
                projectile.x = x;
                projectile.y = y;
                projectile.onSpawn(this, 0, 0);
            }

            projectile.onShoot(energy, parameters);
            parent.RegisterObject(projectile);

            return projectile;
        }

        public int useEnergy(int amount)
        {
            amount = Math.Min(amount, hardware.energyMax);
            if (hardware.noNegEnergy)
                amount = Math.Min(amount, energy);
            if (amount > 600 || amount < 0)
                throw new HardwareException(this, "Illegal energy usage.");
            energy -= amount;
            return amount;
        }

        // First kind of update, the 'environmental update'
        public override void update()
        {
            if (alive && stunned == 0)
            {
                if (energy < hardware.energyMax)
                    energy = Math.Min(energy + 2, hardware.energyMax);
                else if (energy > hardware.energyMax)
                    throw new HardwareException(this, "Maximum energy exceeded.");

                if (shield != 0)
                {
                    if (shield > hardware.shieldMax)
                        shield -= 2;
                    else if (shield > 0 && (parent.chronon % 2) > 0)
                        shield--;
                    if (shield < 0)
                        shield = 0;
                }
                if (energy > 0)
                    base.update(); // Move
                if (energy < -200)
                {
                    damage = -10;
                    deathReason = DeathReason.Overloaded;
                }
            }
        }

        // Second kind of update, where the program is executed
        internal void executeChronon()
        {
            if (alive)
            {
                interp.processInterrupts();

                if (stunned > 0)
                    stunned--;
                else
                {
                    if (energy > 0)
                    {
                        int cycleNum = hardware.processorSpeed;
                        while (cycleNum > 0)
                        {
                            try
                            {
                                cycleNum -= interp.step();
                            }
                            catch (RobotException)
                            {
                                throw;
                            }
                            catch (VMachineException e)
                            {
                                throw new RobotException(this, e);
                            }
                        }
                    }

                    if (energy < -200)
                    {
                        damage = -10;
                        deathReason = DeathReason.Overloaded;
                    }
                }
            }
            else if (aim > 0)
            {
                aim++;
                if (aim > 15)
                    aim = 0; // Done exploding
            }
        }

        private delegate void DrawOffsetHelper(int ox, int oy);
        public override void draw(Graphics gfx)
        {
            // Determine the basic image to use
            Image basicimage;
            if (shield > 0)
            {
                switch (number) {
                default: basicimage = Resources.Robot.Basic1Shield; break;
                case 1:  basicimage = Resources.Robot.Basic2Shield; break;
                case 2:  basicimage = Resources.Robot.Basic3Shield; break;
                case 3:  basicimage = Resources.Robot.Basic4Shield; break;
                case 4:  basicimage = Resources.Robot.Basic5Shield; break;
                case 5:  basicimage = Resources.Robot.Basic6Shield; break;
                }
            }
            else
            {
                switch (number) {
                default: basicimage = Resources.Robot.Basic1; break;
                case 1:  basicimage = Resources.Robot.Basic2; break;
                case 2:  basicimage = Resources.Robot.Basic3; break;
                case 3:  basicimage = Resources.Robot.Basic4; break;
                case 4:  basicimage = Resources.Robot.Basic5; break;
                case 5:  basicimage = Resources.Robot.Basic6; break;
                }
            }
            
            if (alive)
            {
                // FIXME: we should just call RobotFile.draw here.
                gfx.DrawImage(basicimage, (int)x - 16, (int)y - 16);
                
                // FIXME: different turret types
                // FIXME: possibly invert color like the original?
                gfx.DrawLine(Pens.Black, (int)x, (int)y,
                    (int)(x + ((Constants.ROBOT_RADIUS - 1) * Math.Sin(aim * Constants.DEG_TO_RAD))),
                    (int)(y - ((Constants.ROBOT_RADIUS - 1) * Math.Cos(aim * Constants.DEG_TO_RAD))));
            }
            
            else if (aim > 0)
            {
                // Create a brush to fill pie pieces with. Don't do wrapping / tiling.
                TextureBrush brush = new TextureBrush(basicimage, WrapMode.Clamp);
            
                // The texture starts at (0,0). Here, we initialy position it correctly.
                brush.TranslateTransform((int)speedx - 16, (int)speedy - 16);
            
                // Our pie bounds. This is needs to fit a circle that fully encompasses the icon.
                Rectangle r = new Rectangle((int)speedx - 23, (int)speedy - 23, 46, 46);
            
                // We need to offset both the texture and the rectangle each time. :(  
                // Utility function, ahoy!
                DrawOffsetHelper offset = delegate(int ox, int oy) {
                    r.Offset(ox, oy);
                    brush.TranslateTransform(ox, oy);
                };
            
                offset(2 * aim, aim);
                gfx.FillPie(brush, r, 0, 45);
                offset(-aim, aim);
                gfx.FillPie(brush, r, 45, 45);
                offset(-2 * aim, 0);
                gfx.FillPie(brush, r, 90, 45);
                offset(-aim, -aim);
                gfx.FillPie(brush, r, 135, 45);
                offset(0, -2 * aim);
                gfx.FillPie(brush, r, 180, 45);
                offset(aim, -aim);
                gfx.FillPie(brush, r, 225, 45);
                offset(2 * aim, 0);
                gfx.FillPie(brush, r, 270, 45);
                offset(aim, aim);
                gfx.FillPie(brush, r, 315, 45);
            }
        }
    }
}
