namespace RoboWarX.WinForms
{
    partial class RobotControl
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.iconbox = new System.Windows.Forms.PictureBox();
            this.namelabel = new System.Windows.Forms.Label();
            this.energytitle = new System.Windows.Forms.Label();
            this.damagetitle = new System.Windows.Forms.Label();
            this.energylabel = new System.Windows.Forms.Label();
            this.damagelabel = new System.Windows.Forms.Label();
            this.killerlabel = new System.Windows.Forms.Label();
            this.todlabel = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.iconbox)).BeginInit();
            this.SuspendLayout();
            // 
            // iconbox
            // 
            this.iconbox.Location = new System.Drawing.Point(4, 4);
            this.iconbox.Name = "iconbox";
            this.iconbox.Size = new System.Drawing.Size(32, 32);
            this.iconbox.TabIndex = 0;
            this.iconbox.TabStop = false;
            this.iconbox.Paint += new System.Windows.Forms.PaintEventHandler(this.iconbox_Paint);
            // 
            // namelabel
            // 
            this.namelabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 16.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.namelabel.Location = new System.Drawing.Point(42, 4);
            this.namelabel.Name = "namelabel";
            this.namelabel.Size = new System.Drawing.Size(169, 32);
            this.namelabel.TabIndex = 1;
            this.namelabel.Text = "Name";
            this.namelabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // energytitle
            // 
            this.energytitle.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.energytitle.Location = new System.Drawing.Point(204, 4);
            this.energytitle.Name = "energytitle";
            this.energytitle.Size = new System.Drawing.Size(55, 16);
            this.energytitle.TabIndex = 2;
            this.energytitle.Text = "Energy:";
            this.energytitle.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // damagetitle
            // 
            this.damagetitle.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.damagetitle.Location = new System.Drawing.Point(204, 20);
            this.damagetitle.Name = "damagetitle";
            this.damagetitle.Size = new System.Drawing.Size(55, 16);
            this.damagetitle.TabIndex = 3;
            this.damagetitle.Text = "Damage:";
            this.damagetitle.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // energylabel
            // 
            this.energylabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.energylabel.Location = new System.Drawing.Point(265, 4);
            this.energylabel.Name = "energylabel";
            this.energylabel.Size = new System.Drawing.Size(32, 16);
            this.energylabel.TabIndex = 4;
            this.energylabel.Text = "____";
            this.energylabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // damagelabel
            // 
            this.damagelabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.damagelabel.Location = new System.Drawing.Point(265, 20);
            this.damagelabel.Name = "damagelabel";
            this.damagelabel.Size = new System.Drawing.Size(32, 16);
            this.damagelabel.TabIndex = 5;
            this.damagelabel.Text = "____";
            this.damagelabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // killerlabel
            // 
            this.killerlabel.Location = new System.Drawing.Point(207, 6);
            this.killerlabel.Name = "killerlabel";
            this.killerlabel.Size = new System.Drawing.Size(90, 16);
            this.killerlabel.TabIndex = 6;
            this.killerlabel.Text = "Killer";
            this.killerlabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.killerlabel.Visible = false;
            // 
            // todlabel
            // 
            this.todlabel.Location = new System.Drawing.Point(207, 20);
            this.todlabel.Name = "todlabel";
            this.todlabel.Size = new System.Drawing.Size(90, 16);
            this.todlabel.TabIndex = 7;
            this.todlabel.Text = "Time of death";
            this.todlabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.todlabel.Visible = false;
            // 
            // RobotControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.todlabel);
            this.Controls.Add(this.killerlabel);
            this.Controls.Add(this.damagelabel);
            this.Controls.Add(this.energylabel);
            this.Controls.Add(this.damagetitle);
            this.Controls.Add(this.energytitle);
            this.Controls.Add(this.namelabel);
            this.Controls.Add(this.iconbox);
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "RobotControl";
            this.Size = new System.Drawing.Size(300, 40);
            ((System.ComponentModel.ISupportInitialize)(this.iconbox)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox iconbox;
        private System.Windows.Forms.Label namelabel;
        private System.Windows.Forms.Label energytitle;
        private System.Windows.Forms.Label damagetitle;
        private System.Windows.Forms.Label energylabel;
        private System.Windows.Forms.Label damagelabel;
        private System.Windows.Forms.Label killerlabel;
        private System.Windows.Forms.Label todlabel;
    }
}
