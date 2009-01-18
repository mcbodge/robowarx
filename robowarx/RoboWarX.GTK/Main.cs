using System;
using Gtk;

namespace RoboWarX.GTK
{
    class MainClass
    {
        // Entry point
        public static void Main (string[] args)
        {
            Application.Init ();
            MainWindow win = new MainWindow (args);
            win.Show ();
            Application.Run ();
        }
    }
}