using System;
using Gtk;
using RoboWarX;
using RoboWarX.Arena;
using RoboWarX.FileFormats;

namespace RoboWarX.GTK
{
    public partial class MainWindow: Gtk.Window
    {
        private Arena.Arena arena;
        private uint gametimer; // Glib.Source handle
        
        private Gtk.UIManager uimanager;
        
        private BasicActions basicactions;
        private Gtk.Action openaction;
        private Gtk.Action playaction;
        private Gtk.Action pauseaction;
        private Gtk.Action newaction;
        private Gtk.Action quitaction;

        // Each robot loaded always has one of these
        private RobotFile[] files;
        private RobotWidget[] robotlist;
        private RobotActions[] robotactions;
        
        
        
        public MainWindow(string[] files) : base(Gtk.WindowType.Toplevel)
        {
            this.Build();
            
            this.files = new RobotFile[Constants.MAX_ROBOTS];
            robotlist = new RobotWidget[Constants.MAX_ROBOTS];
            robotactions = new RobotActions[Constants.MAX_ROBOTS];
            
            // FIXME: Have to wait until MonoDevelop 2.0 for UIManager integration, it seems.
            uimanager = new Gtk.UIManager();
            
            basicactions = new BasicActions();
            uimanager.InsertActionGroup(basicactions, -1);
            for (int i = 1; i <= Constants.MAX_ROBOTS; i++)
            {
                robotactions[i-1] = new RobotActions(i);
                uimanager.InsertActionGroup(robotactions[i-1], -1);
            }
            connectActions();
            
            uimanager.AddUiFromString(basicactions.buildMenuDescription());
            mainvbox.PackStart(uimanager.GetWidget("/Menubar"));
            
            gametimer = 0;
            reset_game();
            open_robots(files);
        }
        
        
        
        private void stop_game()
        {
            pauseaction.Visible = false;
            playaction.Visible = true;
            
            if (gametimer == 0) return;
            GLib.Source.Remove(gametimer);
            gametimer = 0;
        }
        
        public void start_game()
        {
            pauseaction.Visible = true;
            playaction.Visible = false;
            openaction.Sensitive = false;
        
            // This is where we fight! This is where they die!
            if (gametimer > 0) return;
            gametimer = GLib.Timeout.Add(50, frame);
        }
        
        private bool game_running
        {
            get { return gametimer > 0; }
        }

        private void reset_game()
        {
            stop_game();
            
            arena = new Arena.Arena();
            arena.loadDefaults();
            
            arenaview.arena = arena;
            arenaview.QueueDraw();
            
            chrononlabel.Text = "Chronon 0 (20 c/s)";
            seedlabel.Markup = "<small>Match seed: " + arena.seed + "</small>";
            
            for (int i = 0; i < 6; i++)
            {
                if (files[i] == null) continue;
                
                Robot robot = arena.loadRobot(files[i]);
                robotlist[robot.number].robot = robot;
                robotlist[robot.number].update_info();
            }
            
            openaction.Sensitive = true;
        }
        
        

        // Timer callback
        private bool frame()
        {
            int result = (int)Gtk.ResponseType.Close;
            
            try
            {
                try
                {
                    arena.stepChronon();
                }
                finally
                {
                    arenaview.QueueDraw();
                    foreach (RobotWidget w in robotlist)
                        if (w != null)
                            w.update_info();
                    chrononlabel.Text = "Chronon " + arena.chronon.ToString() + " (20 c/s)";
                }
            }
            catch (RobotException e)
            {
                result = new ErrorDialog(e).Run();
            }
            catch (MultipleErrorsException m)
            {
                foreach (Exception e in m.InnerExceptions)
                    if (e.GetType() == typeof(RobotException))
                    {
                        result = new ErrorDialog(e as RobotException).Run();
                        if (result == (int)Gtk.ResponseType.Close)
                            break;
                    }
                    else
                        throw e;
            }
                
            if (arena.finished)
            {
                stop_game();
                playbutton.Sensitive = false;
                return false;
            }
            else if (result == (int)Gtk.ResponseType.Cancel)
            {
                stop_game();
                return false;
            }
            return true;
        }

        
        
        private void open_robots(String[] filenames)
        {
            foreach (String filename in filenames)
            {
                RobotFile result = null;
                switch (System.IO.Path.GetExtension(filename).ToLower())
                {
                    case ".bin": result = ClassicMBinRobot.read(filename); break;
                    case ".rwr": result = WinRoboWar5.read(filename); break;
                    case ".rtxt": result = SourceTestLoader.read(filename); break;
                }

                if (result != null)
                {
                    // Assign a number first, before adding to the lists
                    Robot robot = arena.loadRobot(result);
                    files[robot.number] = result;
                    
                    RobotWidget widget = new RobotWidget();
                    robotlist[robot.number] = widget;
                    robotvbox.PackStart(widget, false, true, 0);
                    widget.robot = robot;
                    widget.update_info();
                    
                    /* FIXME: implement closing
                    Gtk.VBox rvbox = new Gtk.VBox();
                    widget.AddChild(rvbox);
                    rvbox.Homogeneous = true;
                    
                    Gtk.Button closebutton = new Gtk.Button(Stock.Close);
                    Gtk.Action closeaction = robotactions[robot.number].getRobotAction("CloseAction");
                    closeaction.ConnectProxy(closebutton);
                    rvbox.PackStart(closebutton);
                    */
                    
                    widget.ShowAll();
                    
                    Gtk.HSeparator rsep = new Gtk.HSeparator();
                    robotvbox.PackStart(rsep, false, true, 0);
                    rsep.ShowAll();
                    
                    arenaview.QueueDraw();
                }
            }
        }
        
        

        internal void OnPlayAction(object sender, System.EventArgs e)
        {
            start_game();
        }
        
        internal void OnPauseAction(object sender, System.EventArgs e)
        {
            stop_game();
        }

        internal void OnQuitAction(object sender, System.EventArgs e)
        {
            Application.Quit();
        }

        internal void OnNewAction(object sender, System.EventArgs e)
        {
            if (game_running) stop_game();
            playbutton.Sensitive = true;
            reset_game();
        }

        internal void OnOpenAction(object sender, System.EventArgs e)
        {
            FileChooserDialog chooser = new FileChooserDialog("Select a robot file", this, FileChooserAction.Open);  
            chooser.AddButton(Gtk.Stock.Cancel, Gtk.ResponseType.Cancel);
            chooser.AddButton(Gtk.Stock.Ok, Gtk.ResponseType.Ok);
            chooser.SelectMultiple = true;
            if (chooser.Run() == (int)Gtk.ResponseType.Ok)
                open_robots(chooser.Filenames);
            chooser.Destroy();
        }
        
        private void connectActions()
        {
            openaction = basicactions.GetAction("OpenAction");
            openaction.Activated += OnOpenAction;
            quitaction = basicactions.GetAction("QuitAction");
            quitaction.Activated += OnQuitAction;
            newaction = basicactions.GetAction("NewAction");
            newaction.Activated += OnNewAction;
            playaction = basicactions.GetAction("PlayAction");
            playaction.Activated += OnPlayAction;
            pauseaction = basicactions.GetAction("PauseAction");
            pauseaction.Activated += OnPauseAction;
            
            openaction.ConnectProxy(openbutton);
            newaction.ConnectProxy(newbutton);
            playaction.ConnectProxy(playbutton);
            pauseaction.ConnectProxy(pausebutton);
        }
        
        
        
        protected void OnDeleteEvent(object sender, DeleteEventArgs a)
        {
            OnQuitAction(sender, a);
            a.RetVal = true;
        }
    }
}