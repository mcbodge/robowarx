using System;
using System.Drawing;
using System.Collections.Generic;
using System.Text;

namespace RoboWarX.Arena
{
    // Big explosion. (used by explosive bullets)
    public class BigExplosion : ArenaObject
    {
        private Robot owner;
        private int energy;
        private byte frame;

        public BigExplosion(Arena P, double X, double Y) : base(P, X, Y) { }

        public void onSpawn(Robot owner_, int energy_)
        {
            owner = owner_;
            energy = energy_;
            frame = 0;
        }

        public override IEnumerable<SimulationEvent> update()
        {
            foreach (SimulationEvent e in base.update())
                yield return e;

            if (frame < 3)
                frame++;
            else
            {
                long z = 36 + Constants.ROBOT_RADIUS;
                z = z * z;
                
                foreach (Robot robot in parent.robots)
                {
                    if (robot == null || !robot.alive)
                        break;
                    long x_ = (long)robot.x - (long)x;
                    long y_ = (long)robot.y - (long)y;
                    if (x_ * x_ + y_ * y_ < z)
                        robot.doShotDamage(energy * 2, owner);
                }
                      
                /*  
                foreach (ArenaObject o in parent_.objects)
                {
                    if (cur->type == drone)
                    {
                        long x_ = cur->xPosInt-what->xPosInt;
                        long y_ = cur->yPosInt-what->yPosInt;
                        if (x_ * x_ + y_ * y_ < z)
                        {
                            cur->type = explode;
                            cur->xAngle = -1;
                        }
                    }
                }
                */
                destroy();
            }
            yield break;
        }

        public override void draw(Graphics gfx)
        {
            int rad = frame * 12;
            gfx.FillEllipse(new SolidBrush(Color.FromArgb(128, 0, 0, 0)),
                                new Rectangle((int)x - rad, (int)y - rad, 2 * rad, 2 * rad));
        }
    }
}
