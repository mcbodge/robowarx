using System;
using System.IO;
using System.Drawing;
using System.Windows.Forms;
using RoboWarX.Arena;
using RoboWarX.FileFormats;

namespace RoboWarX.WinForms
{
    public partial class MainForm : Form
    {
        private Arena.Arena arena;
        private RobotControl[] robotlist;

        public MainForm(string[] files)
        {
            InitializeComponent();

            playratelabel.Text = "20 c/s";
            robotlist = new RobotControl[6] {
                robotControl1, robotControl2, robotControl3,
                robotControl4, robotControl5, robotControl6
            };
            openFileDialog1.Multiselect = true;
            openFileDialog1.Filter = "Original format in MacBinary (.BIN)|*.bin|Windows format (.RWR)|*.rwr";
            openFileDialog1.Title = "Select a robot file...";
            openFileDialog1.FileName = "";
            gametimer.Interval = 50;

            new_game();
            open_robots(files);
        }

        private void new_game()
        {
            arena = new Arena.Arena();
            arena.loadDefaults();
            arenaview.arena = arena;
            chrononlabel.Text = "Chronon 0";
            foreach (RobotControl c in robotlist)
            {
                c.robot = null;
                c.update_info();
            }
        }

        // Batch open a bunch of robots
        private void open_robots(String[] filenames)
        {
            foreach (String filename in filenames)
            {
                RobotFile result = null;
                switch (Path.GetExtension(filename).ToLower())
                {
                    case ".bin": result = ClassicMBinRobot.read(filename); break;
                    case ".rwr": result = WinRoboWar5.read(filename); break;
                }

                if (result != null)
                {
                    Robot robot = arena.loadRobot(result);
                    robotlist[robot.number].robot = robot;
                    robotlist[robot.number].update_info();
                    arenaview.Invalidate();
                }
            }
        }

        private void playpausebutton_Click(object sender, EventArgs e)
        {
            if (gametimer.Enabled)
            {
                gametimer.Stop();
                playpausebutton.Text = "Play";
            }
            else
            {
                gametimer.Start();
                playpausebutton.Text = "Pause";
                openToolStripMenuItem.Enabled = false;
            }
        }

        // Handle for timer ticks, step a chronon
        private void frame(object sender, EventArgs e)
        {
            arena.stepChronon();
            arenaview.Invalidate();
            foreach (RobotControl c in robotlist)
                c.update_info();
            chrononlabel.Text = "Chronon " + arena.chronon.ToString();

            if (arena.finished)
            {
                gametimer.Stop();
                playpausebutton.Text = "Finished";
                playpausebutton.Enabled = false;
            }
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
                open_robots(openFileDialog1.FileNames);
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}