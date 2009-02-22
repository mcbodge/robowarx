using System;
using System.Drawing;
using Gtk;
using RoboWarX;
using RoboWarX.Arena;

namespace RoboWarX.GTK
{
    public enum ScalingMode
    {
        None,
        Nearest2x,
        Nearest3x,
        Bilinear2x,
        Bilinear3x,
        // FIXME:  The following are not implemented, but would be neat
        TwoxSaI,
        hq2x,
        hq3x
    }

    // Widget for displaying the arena
    [System.ComponentModel.ToolboxItem(true)]
    public partial class ArenaWidget : Gtk.Bin
    {
        private Arena.Arena arena_ = null;
        private ScalingMode scaling_ = ScalingMode.Nearest2x;
        private int dimensions;
        
        public ArenaWidget()
        {
            this.Build();
            view.DoubleBuffered = true;
            update_size();
        }

        public Arena.Arena arena
        {
            get { return arena_; }
            set { arena_ = value; }
        }
        
        public ScalingMode scaling
        {
            get { return scaling_; }
            set
            {
                scaling_ = value;
                update_size();
            }
        }
        
        private void update_size()
        {
            dimensions = Constants.ARENA_SIZE;
            switch (scaling_)
            {
            case ScalingMode.Nearest2x:
            case ScalingMode.Bilinear2x:
            case ScalingMode.TwoxSaI:
            case ScalingMode.hq2x:
                dimensions *= 2;
                break;
            case ScalingMode.Nearest3x:
            case ScalingMode.Bilinear3x:
            case ScalingMode.hq3x:
                dimensions *= 3;
                break;
            }
            view.SetSizeRequest(dimensions + 2, dimensions + 2);
            view.QueueDraw();
        }

        protected virtual void OnViewExposeEvent (object o, Gtk.ExposeEventArgs args)
        {
            Gdk.Window win = args.Event.Window;
            
            Gdk.GC whitegc = new Gdk.GC(win);
            whitegc.Foreground = view.Style.White;
            
            Gdk.GC darkgc = new Gdk.GC(win);
            darkgc.Foreground = view.Style.Dark(StateType.Normal);
            
            win.DrawRectangle(darkgc, false, 0, 0, dimensions + 1, dimensions + 1);
            win.DrawRectangle(whitegc, true, 1, 1, dimensions, dimensions);
            
            if (arena != null)
            {
                const int d = Constants.ARENA_SIZE;
                
                Gdk.Pixmap pm = new Gdk.Pixmap(win, d, d);
                pm.DrawRectangle(whitegc, true, 0, 0, d, d);
                arena.draw(Gtk.DotNet.Graphics.FromDrawable(pm));
                
                if (scaling_ == ScalingMode.None)
                    win.DrawDrawable(whitegc, pm, 0, 0, 1, 1, d, d);
                else
                {            
                    Gdk.Pixbuf pb = Gdk.Pixbuf.FromDrawable(pm, win.Screen.DefaultColormap,
                        0, 0, 0, 0, d, d);
                    
                    Gdk.Pixbuf spb = null;
                    switch (scaling_)
                    {
                    case ScalingMode.Nearest2x:
                    case ScalingMode.Nearest3x:
                        spb = pb.ScaleSimple(dimensions, dimensions, Gdk.InterpType.Nearest);
                        break;
                    case ScalingMode.Bilinear2x:
                    case ScalingMode.Bilinear3x:
                        spb = pb.ScaleSimple(dimensions, dimensions, Gdk.InterpType.Bilinear);
                        break;
                    }
                    
                    win.DrawPixbuf(whitegc, spb, 0, 0, 1, 1, dimensions, dimensions,
                                   Gdk.RgbDither.None, 0, 0);
                }
            }
            
            args.RetVal = true;
        }
    }
}