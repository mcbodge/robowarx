using System;
using System.Collections.Generic;
using System.Drawing;
using RoboWarX;
using RoboWarX.Arena;

namespace RoboWarX.Arena.Weapons
{
    // Our small explosion type
    internal class HellboreExplosion : BaseExplosion
    {
        public HellboreExplosion(Arena P, double X, double Y) : base(P, X, Y) { }
        
        public void onSpawn(Robot owner_, Robot target_)
        {
            base.baseOnSpawn(owner_, target_);
        }
        
        protected override void impact()
        {
            target.shield = 0;
        }
    }
    
    // Hellbore projectile class
    internal class HellboreObject : Projectile
    {
        public override bool offset { get { return true; } }
        public override bool collideProjectiles { get { return false; } }

        public HellboreObject(Arena P, double X, double Y) : base(P, X, Y) { }
        public HellboreObject() { }
        public override void onShoot(int energy_, params object[] args )
        {
            speedx = anglex * energy_ / 2.0;
            speedy = angley * energy_ / 2.0;
        }

        public override IEnumerable<SimulationEvent> update()
        {
            // Hellbores update twice in a chronon
            foreach (SimulationEvent e in base.update())
                yield return e;
            // FIXME: do we need to check destruction here?
            foreach (SimulationEvent e in base.update())
                yield return e;
        }

        public override bool onHit(ArenaObject other)
        {
            if (other != null)
            {
                if (!(other is Robot))
                    return false;

                owner.parent.spawn(typeof(HellboreExplosion), x, y, owner, other);
            }
            destroy();

            return true;
        }

        public override void draw(System.Drawing.Graphics gfx)
        {
            gfx.DrawImage(Resources.Projectile.Hellbore, new Point((int)x - 4, (int)y - 4));
        }
    }
    
    // Used to launch hellbores. Returns 0 if read, shoots hellbore with speed equal to amount
    // written (this is different than hellbores in RoboWar 2.1.2). This amount is removed from the
    // robot's energy supply. Hellbores reduce the shield of any robot they hit to zero but do no
    // other damage. They must move at a speed from 4 to 20 in the direction that the robot's
    // turret pointed when the shot was fired. Hellbores cannot be used unless they were first
    // enabled at the hardware store.
    internal class HellboreRegister : Register
    {
        public override string[] names
        {
            get
            {
                return new String[] { "HELL", "HELLBORE" };
            }
        }
        internal HellboreRegister() {
            code = (Int16)Bytecodes.REG_HELLBORE;
        }

        public override Int16 value
        {
            get
            {
                return 0;
            }

            internal set
            {
                if (value < 4 || value > 20)
                    return;
                if (!robot.hardware.hasHellbores)
                    throw new HardwareException(this.robot, "Hellbores not enabled.");
                int power = robot.useEnergy(value);
                if (power > 0)
                    robot.shoot(new HellboreObject(), power);
            }
        }
    }
}
