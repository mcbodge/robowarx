using System;
using System.Drawing;
using System.Collections.Generic;
using System.Text;

namespace RoboWarX.Arena
{
    // Small explosion (used by non-explosive bullets)
    public class Explosion : ArenaObject
    {
        private Robot owner;
        private Robot target;
        private int damage;

        public Explosion(Arena P, double X, double Y) : base(P, X, Y) { }

        public void onSpawn(Robot owner_, Robot target_, int damage_)
        {
            owner = owner_;
            target = target_;
            damage = damage_;
        }

        public override void update()
        {
            base.update();

            if (target != null)
            {
                bool damaged = target.doShotDamage(damage, owner);

                if (damaged)
                    target.hit = 2;
                else if (target.hit == 0)
                        target.hit = 1;
            }

            destroy();
        }

        public override void draw(Graphics gfx)
        {
            gfx.FillEllipse(Brushes.Black, new Rectangle((int)x - 5, (int)y - 5, 10, 10));
        }
    }
}
