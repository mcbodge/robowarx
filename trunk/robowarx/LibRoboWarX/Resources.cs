using System;
using System.Reflection;
using System.Resources;
using System.Drawing;

namespace RoboWarX
{
    public class Resources
    {
        public static Image GetImage(string name)
        {
            Assembly a = Assembly.GetExecutingAssembly();
            return Image.FromStream(a.GetManifestResourceStream(name));
        }
    }
}
