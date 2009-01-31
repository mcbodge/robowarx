using System;

namespace RoboWarX.Arena
{
    // Small explosion (used by non-explosive bullets)
    public class Explosion : BaseExplosion
    {
        private int damage;
        
        public Explosion(Arena P, double X, double Y) : base(P, X, Y) { }
        
        public void onSpawn(Robot owner_, Robot target_, int damage_)
        {
            base.baseOnSpawn(owner_, target_);
            damage = damage_;
        }
        
        protected override void impact()
        {
            bool damaged = target.doShotDamage(damage, owner);
            if (damaged)
                target.hit = 2;
            else if (target.hit == 0)
                target.hit = 1;
        }
    }
}
