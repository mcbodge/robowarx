using System;
using System.Drawing;
using System.Windows.Forms;
using RoboWarX.Arena;

namespace RoboWarX.WinForms
{
    // Control for displaying the robot status area and controls
    public partial class RobotControl : UserControl
    {
        private Robot robot_;

        public RobotControl()
        {
            InitializeComponent();
            robot_ = null;
            update_info();
        }

        public Robot robot
        {
            get { return robot_; }
            set { robot_ = value; update_info(); }
        }

        // Called after every chronon, pretty much
        public void update_info()
        {
            if (robot_ == null)
            {
                iconbox.Image = null;
                namelabel.Text = "---";
                energytitle.Visible = false;
                energylabel.Visible = false;
                damagetitle.Visible = false;
                damagelabel.Visible = false;
                killerlabel.Visible = false;
                todlabel.Visible = false;
            }
            else
            {
                iconbox.Image = null;
                namelabel.Text = robot_.name;
                if (robot.alive)
                {
                    energytitle.Visible = true;
                    energylabel.Visible = true;
                    energylabel.Text = robot.energy.ToString();
                    damagetitle.Visible = true;
                    damagelabel.Visible = true;
                    damagelabel.Text = robot_.damage.ToString();
                    killerlabel.Visible = false;
                    todlabel.Visible = false;
                }
                else
                {
                    energytitle.Visible = false;
                    energylabel.Visible = false;
                    damagetitle.Visible = false;
                    damagelabel.Visible = false;
                    killerlabel.Visible = true;
                    if (robot.killer != null)
                        killerlabel.Text = robot.killer.name;
                    else
                        killerlabel.Text = "** Suicide **";
                    todlabel.Visible = true;
                    todlabel.Text = "Time: " + robot.deathTime.ToString(); ;
                }
            }
            iconbox.Invalidate();
        }

        // Draw the robot's default icon in the iconbox
        private void iconbox_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.Clear(iconbox.BackColor);
            if (robot_ != null)
                robot_.file.draw(g, 16, 16, robot_.number, 0, 90);
        }
    }
}
