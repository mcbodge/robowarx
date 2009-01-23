using System;
using System.Drawing;
using System.Reflection;
using RoboWarX;

namespace RoboWarX.Arena
{
    // Base class of all projectiles
    public abstract class Projectile : ArenaObject
    {
        // Cannot be readonly, because it is initialized after construction
        public Robot owner { get; private set; }

        protected double anglex;
        protected double angley;

        // Implementors should additionally define these.
        // public const bool offset;
        // public const bool collideProjectiles;

        protected Projectile(Arena P, double X, double Y) : base(P, X, Y) { }

        public void onSpawn(Robot owner__, double anglex_, double angley_)
        {
            owner = owner__;
            anglex = anglex_;
            angley = angley_;
        }

        public override void update()
        {
            base.update();
            checkHitTarget();
        }
       
        internal void checkHitTarget ()
        {
            // Check if it went off the arena
            if (x < 0 || x > Constants.ARENA_SIZE ||
                y < 0 || y > Constants.ARENA_SIZE)
            {
                if (onHit(null))
                    return;
            }

            // Check for robots
            foreach (Robot target in parent.robots)
            {
                if (target == null || !target.alive)
                    continue;

                // Quick check of position
                if (Math.Abs((int)x - (int)target.x) >= Constants.ROBOT_RADIUS ||
                    Math.Abs((int)y - (int)target.y) >= Constants.ROBOT_RADIUS)
                    continue;

                // Use full floating point accuracy on final check
                double dxf = x - target.x;
                double dyf = y - target.y;
                if (dxf * dxf + dyf * dyf >= Constants.ROBOT_RADIUS * Constants.ROBOT_RADIUS)
                    continue;

                if (onHit(target))
                    return;
            }

            // Check for other projectiles
            // FIXME: Cache?
            FieldInfo finfo = this.GetType().GetField("collideProjectiles");
            if (finfo == null)
                throw new ArenaObjectExtensionException(
                    "Projectile requires a collideProjectiles field.");
            bool collideProjectiles = (bool)finfo.GetValue(this);

            if (collideProjectiles)
            {
                foreach (ArenaObject target in parent.objects)
                {
                    if (target == this)
                        continue;

                    if (Math.Abs((int)x - (int)target.x) >= 5 ||
                        Math.Abs((int)y - (int)target.y) >= 5)
                        continue;

                    if (onHit(target))
                        return;
                }
            }
        }

        // Other is null if it went off the arena
        public abstract bool onHit(ArenaObject other);
    }
}
