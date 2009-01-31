using System;
using System.Reflection;
using System.Resources;
using System.Drawing;

namespace RoboWarX
{
    public static class Resources
    {
        private static void LoadImages(Type container)
        {
            Assembly a = Assembly.GetExecutingAssembly();
            foreach (FieldInfo f in container.GetFields())
            {
                if (f.FieldType != typeof(Image)) continue;
                f.SetValue(null, Image.FromStream(a.GetManifestResourceStream(
                        String.Format("{0}.{1}", container.Name, f.Name)))); 
            }
        }
        
        public static class Projectile
        {
            public static Image ArmedMine;
            public static Image Bullet;
            public static Image DroneDown;
            public static Image DroneDownLeft;
            public static Image DroneDownRight;
            public static Image DroneLeft;
            public static Image DroneRight;
            public static Image DroneUp;
            public static Image DroneUpLeft;
            public static Image DroneUpRight;
            public static Image Hellbore;
            public static Image NewMine;
            
            static Projectile()
            {
                LoadImages(typeof(Projectile));
            }
        }
        
        public static class Robot
        {
            public static Image Basic1;
            public static Image Basic1Shield;
            public static Image Basic2;
            public static Image Basic2Shield;
            public static Image Basic3;
            public static Image Basic3Shield;
            public static Image Basic4;
            public static Image Basic4Shield;
            public static Image Basic5;
            public static Image Basic5Shield;
            public static Image Basic6;
            public static Image Basic6Shield;
            
            static Robot()
            {
                LoadImages(typeof(Robot));
            }
        }
    }
}
