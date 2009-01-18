using System;
using System.Drawing;
using System.Windows.Forms;
using RoboWarX.Arena;

namespace RoboWarX.WinForms
{
    // Control for drawing the Robowar arena
    public partial class ArenaControl : Control
    {
        private Arena.Arena arena_;

        public ArenaControl()
        {
            InitializeComponent();
            DoubleBuffered = true;
        }

        public Arena.Arena arena
        {
            get { return arena_; }
            set { arena_ = value; }
        }

        protected override void OnPaint(PaintEventArgs pe)
        {
            pe.Graphics.FillRectangle(Brushes.White, new Rectangle(0, 0, 300, 300));

            if (arena_ != null)
                arena_.draw(pe.Graphics);

            base.OnPaint(pe);
        }
    }
}
