using System;
using System.Drawing;

namespace RoboWarX.Arena
{
    // Small explosion base class
    // This intermediate step in the class hierarchy is used because hellbores and
    // stunners use small explosions too, though don't do damage.
    public abstract class BaseExplosion : ArenaObject
    {
        protected Robot owner;
        protected Robot target;

        public BaseExplosion(Arena P, double X, double Y) : base(P, X, Y) { }

        public void baseOnSpawn(Robot owner_, Robot target_)
        {
            owner = owner_;
            target = target_;
        }

        public override void update()
        {
            base.update();

            if (target != null)
                impact();

            destroy();
        }
        
        // Damage the target
        protected abstract void impact();

        public override void draw(Graphics gfx)
        {
            gfx.FillEllipse(Brushes.Black, new Rectangle((int)x - 5, (int)y - 5, 10, 10));
        }
    }
}
