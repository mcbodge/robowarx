namespace RoboWarX.WinForms
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.chrononlabel = new System.Windows.Forms.Label();
            this.playratelabel = new System.Windows.Forms.Label();
            this.playpausebutton = new System.Windows.Forms.Button();
            this.gametimer = new System.Windows.Forms.Timer(this.components);
            this.arenaview = new RoboWarX.WinForms.ArenaControl();
            this.robotControl1 = new RoboWarX.WinForms.RobotControl();
            this.robotControl2 = new RoboWarX.WinForms.RobotControl();
            this.robotControl3 = new RoboWarX.WinForms.RobotControl();
            this.robotControl4 = new RoboWarX.WinForms.RobotControl();
            this.robotControl5 = new RoboWarX.WinForms.RobotControl();
            this.robotControl6 = new RoboWarX.WinForms.RobotControl();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // chrononlabel
            // 
            this.chrononlabel.AutoSize = true;
            this.chrononlabel.Location = new System.Drawing.Point(411, 297);
            this.chrononlabel.Name = "chrononlabel";
            this.chrononlabel.Size = new System.Drawing.Size(62, 13);
            this.chrononlabel.TabIndex = 1;
            this.chrononlabel.Text = "Chronon __";
            // 
            // playratelabel
            // 
            this.playratelabel.AutoSize = true;
            this.playratelabel.Location = new System.Drawing.Point(411, 310);
            this.playratelabel.Name = "playratelabel";
            this.playratelabel.Size = new System.Drawing.Size(47, 13);
            this.playratelabel.TabIndex = 2;
            this.playratelabel.Text = "_.__ c/s";
            // 
            // playpausebutton
            // 
            this.playpausebutton.Location = new System.Drawing.Point(330, 297);
            this.playpausebutton.Name = "playpausebutton";
            this.playpausebutton.Size = new System.Drawing.Size(75, 26);
            this.playpausebutton.TabIndex = 3;
            this.playpausebutton.Text = "Play";
            this.playpausebutton.UseVisualStyleBackColor = true;
            this.playpausebutton.Click += new System.EventHandler(this.playpausebutton_Click);
            // 
            // gametimer
            // 
            this.gametimer.Tick += new System.EventHandler(this.frame);
            // 
            // arenaview
            // 
            this.arenaview.arena = null;
            this.arenaview.Location = new System.Drawing.Point(12, 36);
            this.arenaview.MaximumSize = new System.Drawing.Size(300, 300);
            this.arenaview.MinimumSize = new System.Drawing.Size(300, 300);
            this.arenaview.Name = "arenaview";
            this.arenaview.Size = new System.Drawing.Size(300, 300);
            this.arenaview.TabIndex = 5;
            this.arenaview.Text = "arenaControl1";
            // 
            // robotControl1
            // 
            this.robotControl1.Location = new System.Drawing.Point(330, 37);
            this.robotControl1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 0);
            this.robotControl1.Name = "robotControl1";
            this.robotControl1.robot = null;
            this.robotControl1.Size = new System.Drawing.Size(300, 40);
            this.robotControl1.TabIndex = 6;
            // 
            // robotControl2
            // 
            this.robotControl2.Location = new System.Drawing.Point(330, 77);
            this.robotControl2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.robotControl2.Name = "robotControl2";
            this.robotControl2.robot = null;
            this.robotControl2.Size = new System.Drawing.Size(300, 40);
            this.robotControl2.TabIndex = 7;
            // 
            // robotControl3
            // 
            this.robotControl3.Location = new System.Drawing.Point(330, 117);
            this.robotControl3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.robotControl3.Name = "robotControl3";
            this.robotControl3.robot = null;
            this.robotControl3.Size = new System.Drawing.Size(300, 40);
            this.robotControl3.TabIndex = 8;
            // 
            // robotControl4
            // 
            this.robotControl4.Location = new System.Drawing.Point(330, 157);
            this.robotControl4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.robotControl4.Name = "robotControl4";
            this.robotControl4.robot = null;
            this.robotControl4.Size = new System.Drawing.Size(300, 40);
            this.robotControl4.TabIndex = 9;
            // 
            // robotControl5
            // 
            this.robotControl5.Location = new System.Drawing.Point(330, 197);
            this.robotControl5.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.robotControl5.Name = "robotControl5";
            this.robotControl5.robot = null;
            this.robotControl5.Size = new System.Drawing.Size(300, 40);
            this.robotControl5.TabIndex = 10;
            // 
            // robotControl6
            // 
            this.robotControl6.Location = new System.Drawing.Point(330, 237);
            this.robotControl6.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.robotControl6.Name = "robotControl6";
            this.robotControl6.robot = null;
            this.robotControl6.Size = new System.Drawing.Size(300, 40);
            this.robotControl6.TabIndex = 11;
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(654, 24);
            this.menuStrip1.TabIndex = 12;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openToolStripMenuItem,
            this.toolStripSeparator1,
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(36, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // openToolStripMenuItem
            // 
            this.openToolStripMenuItem.Name = "openToolStripMenuItem";
            this.openToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.openToolStripMenuItem.Text = "Open...";
            this.openToolStripMenuItem.Click += new System.EventHandler(this.openToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(149, 6);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.exitToolStripMenuItem.Text = "Exit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(654, 346);
            this.Controls.Add(this.robotControl6);
            this.Controls.Add(this.robotControl5);
            this.Controls.Add(this.robotControl4);
            this.Controls.Add(this.robotControl3);
            this.Controls.Add(this.robotControl2);
            this.Controls.Add(this.robotControl1);
            this.Controls.Add(this.arenaview);
            this.Controls.Add(this.playpausebutton);
            this.Controls.Add(this.playratelabel);
            this.Controls.Add(this.chrononlabel);
            this.Controls.Add(this.menuStrip1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.MainMenuStrip = this.menuStrip1;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "MainForm";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.Text = "RoboWarX";
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label chrononlabel;
        private System.Windows.Forms.Label playratelabel;
        private System.Windows.Forms.Button playpausebutton;
        private System.Windows.Forms.Timer gametimer;
        private ArenaControl arenaview;
        private RobotControl robotControl1;
        private RobotControl robotControl2;
        private RobotControl robotControl3;
        private RobotControl robotControl4;
        private RobotControl robotControl5;
        private RobotControl robotControl6;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
    }
}

