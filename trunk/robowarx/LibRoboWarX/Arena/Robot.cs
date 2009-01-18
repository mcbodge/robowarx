using System;
using System.IO;
using System.Drawing;
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
        public int energyMax;            // Maximum amount of energy
        public int damageMax;            // Maximum amount of damage
        public int shieldMax;            // Maximum shield level for normal discharge
        public int processorSpeed;       // Instructions per chronon

        public BulletType gunType;
        public bool hasMissiles;           // 1 = has missiles
        public bool hasTacNukes;           // 1 = has tacNukes
        public bool hasLasers;             // 1 = lasers
        public bool hasHellbores;          // 1 = hellbore
        public bool hasDrones;             // 1 = drone
        public bool hasMines;              // 1 = mine
        public bool hasStunners;           // 1 = stunner

        public bool noNegEnergy;           // 1 = No Negative energy

        public bool probeFlag;             // 1 = probes
        public bool deathIconFlag;         // 1 = use icon for death
        public bool collisionIconFlag;     // 1 = use icon for collision
        public bool shieldHitIconFlag;     // 1 = use icon for shield hit
        public bool hitIconFlag;           // 1 = use icon for hit
        public bool shieldOnIconFlag;      // 1 = use icon for shieldon

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
        internal RobotFile file_;
        
        internal int number_;
        internal int team_;

        internal bool alive_;
        internal DeathReason deathReason_;
        internal int deathTime_;
        internal Robot killer_;
        internal RobotException bug_;

        internal int icon;

        internal Interpreter interp;
        internal bool interrupts;

        internal HardwareInfo hardware_;

        internal int energy_;
        internal int damage_;
        internal int shield;
        internal int aim;
        internal int look;
        internal int scan;

        internal bool collision;
        internal bool wall;
        internal bool friend;
        internal int stunned;
        internal int hit;

        internal int kills;
        internal int survival;
        internal int[] killTime;
        
        internal Int16[] history;
        
        internal int channel;
        internal Int16[] signals;

        // Some defaults that will be overridden in OnSpawn
        public Robot(Arena P, double X, double Y) : base(P, X, Y)
        {
            number_ = -1;
            
            team_ = 0;
            alive_ = true;
            deathReason_ = DeathReason.Suicided;
            icon = 0;
            interrupts = false;
            
            hardware_ = new HardwareInfo();
            energy_ = hardware_.energyMax;
            damage_ = hardware_.damageMax;
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
            deathTime_ = -1;
            killer_ = null;
            
            // FIXME: actually implement the history
            history = new Int16[50];
            
            // FIXME: actually implement signals
            signals = new Int16[10];
        }

        public void onSpawn(int number_, RobotFile file_)
        {
            this.number_ = number_;
            this.file_ = file_;
            
            interp = new Interpreter(new MemoryStream(file_.program));

            hardware_ = file_.hardware;
            energy_ = hardware_.energyMax;
            damage_ = hardware_.damageMax;
        }

        public int number
        {
            get { return number_; }
        }
        
        public RobotFile file
        {
            get { return file_; }
        }

        public String name
        {
            get { return file_.name; }
        }

        public int team
        {
            get { return team_; }
        }

        public bool alive
        {
            get { return alive_; }
        }

        public DeathReason deathReason
        {
            get { return deathReason_; }
        }

        public HardwareInfo hardware
        {
            get { return hardware_; }
        }

        public int energy
        {
            get { return energy_; }
        }

        public int damage
        {
            get { return damage_; }
        }

        public int deathTime
        {
            get { return deathTime_; }
        }

        public Robot killer
        {
            get { return killer_; }
        }

        // We got hit by someone
        public bool doShotDamage(int amount, Robot from)
        {
            // Attribute kills to proper owner
            int oldDamage = damage_;
            doDamage(amount);
            if (damage_ <= 0 && oldDamage > 0 && this != from
                && (from.energy_ > -200) && !(team_ != 0 && (team_ == from.team_)))
            {
                deathReason_ = DeathReason.Killed;
                from.kills++;
                from.killTime[number_] = parent_.chronon;
                killer_ = from;
            }
            return damage_ < oldDamage; // return true if robot took damage
        }

        // Take damage
        public void doDamage(int amount)
        {
            if (shield == 0)
                damage_ -= amount;
            else if (shield < amount)
            {
                damage_ -= amount - shield;
                shield = 0;
            }
            else
                shield -= amount;
        }

        // He's dead, Jim.
        internal void explode()
        {
            shield = 0;
            alive_ = false;
            deathTime_ = parent.chronon;

            // Hack for the exploding animation
            aim = 1;
            speedx = x_;
            speedy = y_;

            // Move out of the way
            x_ = 5000;
            y_ = 5000;
        }

        // Instantiate a projectile, taking into account the robots aim.
        public ArenaObject shoot(Type objtype, params object[] parameters)
        {
            FieldInfo finfo = objtype.GetField("offset");
            if (finfo == null)
                throw new ArenaObjectExtensionException(
                    "Object requires an offset field to be shot");
            bool offset = (bool)finfo.GetValue(null);

            ArenaObject retval;
            if (offset)
            {
                double anglex = Util.Sin(aim + 270);
                double angley = Util.Cos(aim + 270);
                retval = parent_.spawn(objtype,
                    x_ + anglex * (Constants.ROBOT_RADIUS + 1),
                    y_ + angley * (Constants.ROBOT_RADIUS + 1),
                    this, anglex, angley);
            }
            else
                retval = parent_.spawn(objtype, x_, y_, this, 0, 0);

            if (parameters.Length > 0)
            {
                List<Type> types = new List<Type>(Math.Max(3, parameters.Length));
                foreach (object parameter in parameters)
                    types.Add(parameter.GetType());
                MethodInfo minfo = objtype.GetMethod("onShoot", types.ToArray());
                if (minfo == null)
                    throw new ArenaObjectExtensionException(
                        "Cannot find a suitable onShoot method for object.");

                minfo.Invoke(retval, parameters);
            }

            return retval;
        }

        public int useEnergy(int amount)
        {
            amount = Math.Min(amount, hardware_.energyMax);
            if (hardware_.noNegEnergy)
                amount = Math.Min(amount, energy_);
            if (amount > 600 || amount < 0)
                throw new HardwareException(this, "Illegal energy usage.");
            energy_ -= amount;
            return amount;
        }

        // First kind of update, the 'environmental update'
        public override void update()
        {
            if (alive_ && stunned == 0)
            {
                if (energy_ < hardware_.energyMax)
                    energy_ = Math.Min(energy_ + 2, hardware_.energyMax);
                else if (energy_ > hardware_.energyMax)
                    throw new HardwareException(this, "Maximum energy exceeded.");

                if (shield != 0)
                {
                    if (shield > hardware_.shieldMax)
                        shield -= 2;
                    else if (shield > 0 && (parent.chronon % 2) > 0)
                        shield--;
                    if (shield < 0)
                        shield = 0;
                }
                if (energy_ > 0)
                    base.update(); // Move
                if (energy_ < -200)
                {
                    damage_ = -10;
                    deathReason_ = DeathReason.Overloaded;
                }
            }
        }

        // Second kind of update, where the program is executed
        internal void executeChronon()
        {
            if (alive_)
            {
                interp.processInterrupts();

                if (stunned > 0)
                    stunned--;
                else
                {
                    if (energy_ > 0)
                    {
                        int cycleNum = hardware_.processorSpeed;
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

                    if (energy_ < -200)
                    {
                        damage_ = -10;
                        deathReason_ = DeathReason.Overloaded;
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

        public override void draw(Graphics gfx)
        {
            Brush color;
            switch (number_) {
                case 0:
                    color = Brushes.Red;
                    break;
                case 1:
                    color = Brushes.Cyan;
                    break;
                case 2:
                    color = Brushes.Green;
                    break;
                case 3:
                    color = Brushes.Blue;
                    break;
                case 4:
                    color = Brushes.Magenta;
                    break;
                case 5:
                    color = Brushes.Yellow;
                    break;
                default:
                    color = Brushes.Black;
                    break;
            }

            if (alive_)
            {
                gfx.FillEllipse(color, new Rectangle((int)x - 12, (int)y - 12, 24, 24));

                gfx.DrawLine(Pens.Black, (int)x, (int)y,
                    (int)x + (int)((Constants.ROBOT_RADIUS - 1) * Math.Sin(aim * Constants.DEG_TO_RAD)),
                    (int)y - (int)((Constants.ROBOT_RADIUS - 1) * Math.Cos(aim * Constants.DEG_TO_RAD)));
            }
            else if (aim > 0)
            {
                Rectangle r = new Rectangle((int)speedx - 12, (int)speedy - 12, 24, 24);
                r.Offset(2 * aim, aim);
                gfx.FillPie(color, r, 0, 45);
                r.Offset(-aim, aim);
                gfx.FillPie(color, r, 45, 45);
                r.Offset(-2 * aim, 0);
                gfx.FillPie(color, r, 90, 45);
                r.Offset(-aim, -aim);
                gfx.FillPie(color, r, 135, 45);
                r.Offset(0, -2 * aim);
                gfx.FillPie(color, r, 180, 45);
                r.Offset(aim, -aim);
                gfx.FillPie(color, r, 225, 45);
                r.Offset(2 * aim, 0);
                gfx.FillPie(color, r, 270, 45);
                r.Offset(aim, aim);
                gfx.FillPie(color, r, 315, 45);
            }
        }
    }
}
