using System;
using System.Drawing;
using Gtk;
using RoboWarX.Arena;

// Widget for displaying robot status and controls

namespace RoboWarX.GTK
{
	[System.ComponentModel.ToolboxItem(true)]
    public partial class RobotWidget : Gtk.Bin
    {
        private Robot robot_;

        public RobotWidget()
        {
            this.Build();
            robot_ = null;
            update_info();
        }

        public Robot robot
        {
            get { return robot_; }
            set { robot_ = value; update_info(); }
        }

        public void update_info()
        {
            if (robot_ == null)
            {
                namelabel.Markup = "<big><b>---</b></big>";
                statuslabel1.Visible = false;
                statuslabel2.Visible = false;
            }
            else
            {
                namelabel.Markup = "<big><b>" + GLib.Markup.EscapeText(robot_.name) + "</b></big>";
                statuslabel2.Visible = true;
                if (robot.alive)
                {
                    statuslabel1.Visible = true;
                    statuslabel1.Text = "Energy:\nDamage:";
                    statuslabel2.Text = robot.energy.ToString() + "\n" + robot_.damage.ToString();
                }
                else
                {
                    statuslabel1.Visible = false;
                    String killmsg = "";
                    switch (robot.deathReason)
                    {
                    case DeathReason.Buggy: killmsg = "•• Buggy ••"; break;
                    case DeathReason.Killed: killmsg = robot.killer.name; break;
                    case DeathReason.Overloaded: killmsg = "•• Overloaded ••"; break;
                    case DeathReason.Suicided: killmsg = "•• Suicide ••"; break;
                    }
                    statuslabel2.Text = killmsg + "\nTime:  " + robot.deathTime.ToString();
                }
            }
            iconbox.QueueDraw();
        }
        
        public void AddChild(Gtk.Widget child)
        {
            hbox2.PackEnd(child, false, true, 0);
        }

        // Draw the robot's default icon
        protected void OnIconboxExposeEvent (object o, Gtk.ExposeEventArgs args)
        {
            args.Event.Window.Clear();
            if (robot_ != null)
            {
                int w, h;
                args.Event.Window.GetSize(out w, out h);
                Graphics g = Gtk.DotNet.Graphics.FromDrawable(args.Event.Window);
                robot_.file.draw(g, w / 2, h / 2, robot_.number, 0, null);
            }
            args.RetVal = true;
        }
    }
}