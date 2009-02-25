using System;
using System.Drawing;
using RoboWarX;
using RoboWarX.Arena;

namespace RoboWarX.Arena.Weapons
{
    // Missile projectile class
    internal class MissileObject : Projectile
    {
        private int energy;

        public override bool offset { get { return true; } }
        public override bool collideProjectiles  { get { return false; } }

        public MissileObject(Arena P, double X, double Y) : base(P, X, Y) { }
        public MissileObject() { }
        public override void onShoot(int energy_, params object[] args)
        {
            energy = energy_;
            speedx = anglex * 5.0;
            speedy = angley * 5.0;
        }

        public override bool onHit(ArenaObject other)
        {
            if (other != null)
            {
                if (!(other is Robot))
                    return false;

                owner.parent.spawn(typeof(Explosion), x, y, owner, other, energy * 2);
            }
            destroy();

            return true;
        }

        public override void draw(System.Drawing.Graphics gfx)
        {
            gfx.DrawLine(Pens.Black, (float)x, (float)y,
                (float)(x + speedx), (float)(y + speedy));
        }
    }

    // Used to shoot missiles. Returns 0 if read, shoots missile with energy investment equal to
    // amount written. This energy investment is removed from the robot’s energy supply. It may not
    // exceed the robot's energy max; if it does, only EnergyMax energy is used (this is changed;
    // prior to RoboWar 4.1.2, a maximum of 50 points could be used). Missiles do 2*energy
    // investment in damage if they hit their targets. Missiles move across the screen at a speed
    // of 5 pixels per chronon, heading in the direction that the robot’s turret pointed when the
    // shot was fired. Missiles cannot be used unless they were first enabled at the hardware store.
    internal class MissileRegister : Register
    {
        internal MissileRegister() {
            name = "MISSILE";
            code = (Int16)Bytecodes.REG_MISSILE;
        }

        public override Int16 value
        {
            get
            {
                return 0;
            }

            set
            {
                if (value <= 0)
                    return;
                if (!robot.hardware.hasMissiles)
                    throw new HardwareException(this.robot, "Missiles not enabled.");
                int power = robot.useEnergy(value);
                if (power > 0)
                    robot.shoot(new MissileObject(), power);
            }
        }
    }
}
