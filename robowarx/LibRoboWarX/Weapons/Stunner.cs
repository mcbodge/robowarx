using System;
using System.Drawing;
using RoboWarX;
using RoboWarX.Arena;

namespace RoboWarX.Weapons.Stunner
{
    // Our small explosion type
    internal class StunnerExplosion : BaseExplosion
    {
        private int energy;
        
        public StunnerExplosion(Arena.Arena P, double X, double Y) : base(P, X, Y) { }
        
        public void onSpawn(Robot owner_, Robot target_, int energy_)
        {
            base.baseOnSpawn(owner_, target_);
            energy = energy_;
        }
        
        protected override void impact()
        {
            target.stunned += energy / 4;
        }
    }
    
    // Stunner projectile class
    internal class StunnerObject : Projectile
    {
        private int energy;

        public override bool offset { get { return true; } }
        public override bool collideProjectiles { get { return false; } }

        public StunnerObject(Arena.Arena P, double X, double Y) : base(P, X, Y) { }
        public StunnerObject() { }

        public override void onShoot(int energy_, params object[] args)
        {
            energy = energy_;
            speedx = anglex * 7;
            speedy = angley * 7;
        }

        public override void update()
        {
            // Stunners update twice in a chronon
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

                owner.parent.spawn(typeof(StunnerExplosion), x, y, owner, other, energy);
            }
            destroy();

            return true;
        }

        public override void draw(System.Drawing.Graphics gfx)
        {
            gfx.DrawLine(Pens.Black, (float)x - 2, (float)y - 2, (float)x + 2, (float)y + 2);
            gfx.DrawLine(Pens.Black, (float)x + 2, (float)y - 2, (float)x - 2, (float)y + 2);
        }
    }
    
    // Used to fire a stasis capsule. Returns 0 if read, shoots stasis capsule with speed 14 in the
    // directon that the robot's turret points. The amound written is removed from the robot's
    // energy supply; if a stasis capsule hits a robot, the robot is placed in stasis for one
    // chronon for every four points of energy invested in the capsule. While in stasis, a robot
    // does not move, interpret instructions, or regain energy; however, the robot's shields do not
    // decay. Stunners cannot be used unless they are first enabled in the Hardware Store.
    internal class StunnerRegister : Register
    {
        internal StunnerRegister() {
            name = "STUNNER";
            code = (Int16)Bytecodes.REG_STUNNER;
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
                if (!robot.hardware.hasStunners)
                    throw new HardwareException(this.robot, "Stunners not enabled.");
                int power = robot.useEnergy(value);
                if (power > 0)
                    robot.shoot(new StunnerObject(), power);
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
            return new StunnerRegister();
        }

        public override bool checkInterrupt()
        {
            return false;
        }

        public override void updateInterruptState()
        {
        }
    }

    // Stunner plugin entry
    public class StunnerEntry : IPluginEntry
    {
        public ITemplateRegister[] getPrototypes()
        {
            StunnerRegister stunner = new StunnerRegister();
            return new ITemplateRegister[] { stunner };
        }
    }
}
