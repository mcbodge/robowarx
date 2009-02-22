using System;
using System.Drawing;

namespace RoboWarX.Arena
{
    // Base class of all objects on the arena
    public abstract class ArenaObject
    {
        public double x { get; internal set; }
        public double y { get; internal set; }
        public Arena parent { get; internal set; }

        public double speedx { get; set; }
        public double speedy { get; set; }

        protected ArenaObject(Arena parent, double x, double y)
        {
            this.x = x;
            this.y = y;
            speedx = 0;
            speedy = 0;
            this.parent = parent;
        }

        protected ArenaObject() { }

        public virtual void update()
        {
            x += speedx;
            y += speedy;
        }

        public void destroy()
        {
            parent.delObjects.Add(this);
        }

        // FIXME: Perhaps this should be moved outside of the library?
        // That way, there's not System.Drawing dependency.
        public abstract void draw(Graphics gfx);
    }
}
