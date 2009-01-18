using System;
using System.Drawing;

namespace RoboWarX.Arena
{
    // Base class of all objects on the arena
    public abstract class ArenaObject
    {
        internal double x_;
        internal double y_;
        internal Arena parent_;

        public double speedx;
        public double speedy;

        protected ArenaObject(Arena parent__, double x__, double y__)
        {
            x_ = x__;
            y_ = y__;
            speedx = 0;
            speedy = 0;
            parent_ = parent__;
        }

        public double x
        {
            get
            {
                return x_;
            }
        }

        public double y
        {
            get
            {
                return y_;
            }
        }

        public Arena parent
        {
            get
            {
                return parent_;
            }
        }

        public virtual void update()
        {
            x_ += speedx;
            y_ += speedy;
        }

        public void destroy()
        {
            parent_.delObjects.AddLast(this);
        }

        // FIXME: Perhaps this should be moved outside of the library?
        // That way, there's not System.Drawing dependency.
        public abstract void draw(Graphics gfx);
    }
}
