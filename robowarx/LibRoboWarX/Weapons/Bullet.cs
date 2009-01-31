using System;
using System.Drawing;
using RoboWarX;
using RoboWarX.Arena;

namespace RoboWarX.Weapons.Bullet
{
    // Bullet projectile class
    internal class BulletObject : Projectile
    {
        private int energy;
        private BulletType type;

        public const bool offset = true;
        public const bool collideProjectiles = false;

        public BulletObject(Arena.Arena P, double X, double Y) : base(P, X, Y) { }

        public void onShoot(int energy_, BulletType type_)
        {
            energy = energy_;
            type = type_;
            // Because a bullet updates twice, effective speed is 12
            speedx = anglex * 6.0;
            speedy = angley * 6.0;
        }

        public override void update()
        {
            // Bullets update twice in a chronon
            base.update();
            // FIXME: do we need to check destruction here?
            base.update();
        }

        public override bool onHit(ArenaObject other)
        {
            if (other != null)
            {
                if (!(other is Robot))
                    return false;

                if (type == BulletType.Explosive)
                    owner.parent.spawn(typeof(BigExplosion), x, y, owner, energy);
                else
                {
                    int damage = energy;
                    if (type == BulletType.Rubber)
                        damage /= 2;

                    owner.parent.spawn(typeof(Explosion), x, y, owner, other, damage);
                }
            }
            destroy();

            return true;
        }

        public override void draw(System.Drawing.Graphics gfx)
        {
            // FIXME: Cache? I believe the Resources class does this already.
            gfx.DrawImage(Resources.Projectile.Bullet,
                          new Point((int)x - 2, (int)y - 2));
        }
    }

    // Fires a normal bullet if either normal or explosive bullets are enabled. This is primarily
    // useful so that robots equipped with explosive bullets can fire at short range without
    // engulfing themselves in the explosion. Returns 0 if read.
    internal class BulletRegister : Register
    {
        
        internal BulletRegister() {
            name = "BULLET";
            code = (Int16)Bytecodes.REG_BULLET;
        }

        public override Int16 value
        {
            get
            {
                return 0;
            }

            set
            {
                if (robot.hardware.gunType == BulletType.None ||
                        robot.hardware.gunType == BulletType.Rubber)
                    throw new HardwareException(this.robot, "Normal bullets not available.");
                int power = robot.useEnergy(value);
                if (power > 0)
                    robot.shoot(typeof(BulletObject), power, BulletType.Normal);
            }
        }

        public override Int16 param
        {
            set
            {
            }
        }

        public override Object Clone()
        {
            return new BulletRegister();
        }

        public override bool checkInterrupt()
        {
            return false;
        }

        public override void updateInterruptState()
        {
        }
    }

    // Used to shoot bullets. Returns 0 if read, shoots bullet with energy investment equal to
    // amount written. This energy investment is removed from the robot’s energy supply. It may
    // exceed the robot’s current energy value (placing the robot at negative energy and
    // immobilizing it), but may not exceed the robots energy maximum. Depending on the settings
    // from the Hardware Store, bullets may be normal, rubber, or explosive. Explosive bullets
    // explode like TacNukes in a 36 pixel radius when they hit their target. Whey they detonate
    // (3 chronons after impact) they do damage of 2*energy investment to all robots in the blast
    // radius. This is a larger, faster explosion than in versions of RoboWar before 3.0. Normal
    // bullets do damage equal to the energy investment when they hit their targets. Rubber bullets
    // only do half damage if they hit. Bullets move across the screen at a speed of 12 pixels per
    // chronon, heading in the direction that the robot’s turret pointed when the shot was fired.
    internal class FireRegister : Register
    {
        internal FireRegister()
        {
            name = "FIRE";
            code = (Int16)Bytecodes.REG_FIRE;
        }

        public override Int16 value
        {
            get
            {
                return 0;
            }

            set
            {
                if (robot.hardware.gunType == BulletType.None)
                    throw new HardwareException(this.robot, "Gun not enabled.");
                int power = robot.useEnergy(value);
                if (power > 0)
                    robot.shoot(typeof(BulletObject), power, robot.hardware.gunType);
            }
        }

        public override Int16 param
        {
            set
            {
            }
        }

        public override Object Clone()
        {
            return new FireRegister();
        }

        public override bool checkInterrupt()
        {
            return false;
        }

        public override void updateInterruptState()
        {
        }
    }

    // Bullet plugin entry
    public class BulletEntry : IPluginEntry
    {
        public ITemplateRegister[] getPrototypes()
        {
            BulletRegister bullet = new BulletRegister();
            FireRegister fire = new FireRegister();
            return new ITemplateRegister[] { bullet, fire };
        }
    }
}
