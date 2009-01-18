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
        private Robot owner_;
        public Robot owner
        {
            get
            {
                return owner_;
            }
        }

        protected double anglex;
        protected double angley;

        // Implementors should additionally define these.
        // public const bool offset;
        // public const bool collideProjectiles;

        protected Projectile(Arena P, double X, double Y) : base(P, X, Y) { }

        public void onSpawn(Robot owner__, double anglex_, double angley_)
        {
            owner_ = owner__;
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
            if (x_ < 0 || x_ > Constants.ARENA_SIZE ||
                y_ < 0 || y_ > Constants.ARENA_SIZE)
            {
                if (onHit(null))
                    return;
            }

            // Check for robots
            foreach (Robot target in parent_.robots)
            {
                if (target == null || !target.alive_)
                    continue;

                // Quick check of position
                if (Math.Abs((int)x_ - (int)target.x_) >= Constants.ROBOT_RADIUS ||
                    Math.Abs((int)y_ - (int)target.y_) >= Constants.ROBOT_RADIUS)
                    continue;

                // Use full floating point accuracy on final check
                double dxf = x_ - target.x_;
                double dyf = y_ - target.y_;
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
                foreach (ArenaObject target in parent_.objects)
                {
                    if (target == this)
                        continue;

                    if (Math.Abs((int)x_ - (int)target.x_) >= 5 ||
                        Math.Abs((int)y_ - (int)target.y_) >= 5)
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
