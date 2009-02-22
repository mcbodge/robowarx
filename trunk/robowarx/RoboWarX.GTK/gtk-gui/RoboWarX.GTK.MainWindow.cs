// ------------------------------------------------------------------------------
//  <autogenerated>
//      This code was generated by a tool.
//      
// 
//      Changes to this file may cause incorrect behavior and will be lost if 
//      the code is regenerated.
//  </autogenerated>
// ------------------------------------------------------------------------------

namespace RoboWarX.GTK {
    
    
    public partial class MainWindow {
        
        private Gtk.VBox mainvbox;
        
        private Gtk.HBox hbox1;
        
        private RoboWarX.GTK.ArenaWidget arenaview;
        
        private Gtk.VBox vbox1;
        
        private Gtk.VBox robotvbox;
        
        private Gtk.Label spacer1;
        
        private Gtk.HBox hbox15;
        
        private Gtk.Label spacer2;
        
        private Gtk.Button openbutton;
        
        private Gtk.HBox hbox8;
        
        private Gtk.HButtonBox hbuttonbox1;
        
        private Gtk.Button newbutton;
        
        private Gtk.Button playbutton;
        
        private Gtk.Button pausebutton;
        
        private Gtk.VBox vbox4;
        
        private Gtk.Label chrononlabel;
        
        private Gtk.Label seedlabel;
        
        protected virtual void Build() {
            Stetic.Gui.Initialize(this);
            // Widget RoboWarX.GTK.MainWindow
            this.Name = "RoboWarX.GTK.MainWindow";
            this.Title = Mono.Unix.Catalog.GetString("RoboWarX");
            this.WindowPosition = ((Gtk.WindowPosition)(4));
            this.DefaultWidth = 600;
            this.DefaultHeight = 320;
            // Container child RoboWarX.GTK.MainWindow.Gtk.Container+ContainerChild
            this.mainvbox = new Gtk.VBox();
            this.mainvbox.Name = "mainvbox";
            // Container child mainvbox.Gtk.Box+BoxChild
            this.hbox1 = new Gtk.HBox();
            this.hbox1.Name = "hbox1";
            this.hbox1.Spacing = 7;
            this.hbox1.BorderWidth = ((uint)(12));
            // Container child hbox1.Gtk.Box+BoxChild
            this.arenaview = new RoboWarX.GTK.ArenaWidget();
            this.arenaview.Name = "arenaview";
            this.hbox1.Add(this.arenaview);
            Gtk.Box.BoxChild w1 = ((Gtk.Box.BoxChild)(this.hbox1[this.arenaview]));
            w1.Position = 0;
            w1.Expand = false;
            w1.Fill = false;
            // Container child hbox1.Gtk.Box+BoxChild
            this.vbox1 = new Gtk.VBox();
            this.vbox1.Name = "vbox1";
            // Container child vbox1.Gtk.Box+BoxChild
            this.robotvbox = new Gtk.VBox();
            this.robotvbox.Name = "robotvbox";
            this.robotvbox.Spacing = 2;
            // Container child robotvbox.Gtk.Box+BoxChild
            this.spacer1 = new Gtk.Label();
            this.spacer1.Name = "spacer1";
            this.robotvbox.Add(this.spacer1);
            Gtk.Box.BoxChild w2 = ((Gtk.Box.BoxChild)(this.robotvbox[this.spacer1]));
            w2.PackType = ((Gtk.PackType)(1));
            w2.Position = 0;
            // Container child robotvbox.Gtk.Box+BoxChild
            this.hbox15 = new Gtk.HBox();
            this.hbox15.Name = "hbox15";
            this.hbox15.Spacing = 6;
            // Container child hbox15.Gtk.Box+BoxChild
            this.spacer2 = new Gtk.Label();
            this.spacer2.Name = "spacer2";
            this.hbox15.Add(this.spacer2);
            Gtk.Box.BoxChild w3 = ((Gtk.Box.BoxChild)(this.hbox15[this.spacer2]));
            w3.Position = 0;
            // Container child hbox15.Gtk.Box+BoxChild
            this.openbutton = new Gtk.Button();
            this.openbutton.CanFocus = true;
            this.openbutton.Name = "openbutton";
            this.openbutton.UseStock = true;
            this.openbutton.UseUnderline = true;
            this.openbutton.Label = "gtk-open";
            this.hbox15.Add(this.openbutton);
            Gtk.Box.BoxChild w4 = ((Gtk.Box.BoxChild)(this.hbox15[this.openbutton]));
            w4.Position = 1;
            w4.Expand = false;
            w4.Fill = false;
            this.robotvbox.Add(this.hbox15);
            Gtk.Box.BoxChild w5 = ((Gtk.Box.BoxChild)(this.robotvbox[this.hbox15]));
            w5.PackType = ((Gtk.PackType)(1));
            w5.Position = 1;
            w5.Expand = false;
            w5.Fill = false;
            this.vbox1.Add(this.robotvbox);
            Gtk.Box.BoxChild w6 = ((Gtk.Box.BoxChild)(this.vbox1[this.robotvbox]));
            w6.Position = 0;
            // Container child vbox1.Gtk.Box+BoxChild
            this.hbox8 = new Gtk.HBox();
            this.hbox8.Name = "hbox8";
            this.hbox8.Spacing = 12;
            // Container child hbox8.Gtk.Box+BoxChild
            this.hbuttonbox1 = new Gtk.HButtonBox();
            this.hbuttonbox1.Name = "hbuttonbox1";
            this.hbuttonbox1.Spacing = 6;
            // Container child hbuttonbox1.Gtk.ButtonBox+ButtonBoxChild
            this.newbutton = new Gtk.Button();
            this.newbutton.CanFocus = true;
            this.newbutton.Name = "newbutton";
            this.newbutton.UseStock = true;
            this.newbutton.UseUnderline = true;
            this.newbutton.Label = "gtk-new";
            this.hbuttonbox1.Add(this.newbutton);
            Gtk.ButtonBox.ButtonBoxChild w7 = ((Gtk.ButtonBox.ButtonBoxChild)(this.hbuttonbox1[this.newbutton]));
            w7.Expand = false;
            w7.Fill = false;
            // Container child hbuttonbox1.Gtk.ButtonBox+ButtonBoxChild
            this.playbutton = new Gtk.Button();
            this.playbutton.CanFocus = true;
            this.playbutton.Name = "playbutton";
            this.playbutton.UseStock = true;
            this.playbutton.UseUnderline = true;
            this.playbutton.Label = "gtk-media-play";
            this.hbuttonbox1.Add(this.playbutton);
            Gtk.ButtonBox.ButtonBoxChild w8 = ((Gtk.ButtonBox.ButtonBoxChild)(this.hbuttonbox1[this.playbutton]));
            w8.Position = 1;
            w8.Expand = false;
            w8.Fill = false;
            // Container child hbuttonbox1.Gtk.ButtonBox+ButtonBoxChild
            this.pausebutton = new Gtk.Button();
            this.pausebutton.CanFocus = true;
            this.pausebutton.Name = "pausebutton";
            this.pausebutton.UseStock = true;
            this.pausebutton.UseUnderline = true;
            this.pausebutton.Label = "gtk-media-pause";
            this.hbuttonbox1.Add(this.pausebutton);
            Gtk.ButtonBox.ButtonBoxChild w9 = ((Gtk.ButtonBox.ButtonBoxChild)(this.hbuttonbox1[this.pausebutton]));
            w9.Position = 2;
            w9.Expand = false;
            w9.Fill = false;
            this.hbox8.Add(this.hbuttonbox1);
            Gtk.Box.BoxChild w10 = ((Gtk.Box.BoxChild)(this.hbox8[this.hbuttonbox1]));
            w10.Position = 0;
            w10.Expand = false;
            // Container child hbox8.Gtk.Box+BoxChild
            this.vbox4 = new Gtk.VBox();
            this.vbox4.Name = "vbox4";
            this.vbox4.Homogeneous = true;
            this.vbox4.Spacing = 2;
            // Container child vbox4.Gtk.Box+BoxChild
            this.chrononlabel = new Gtk.Label();
            this.chrononlabel.Name = "chrononlabel";
            this.chrononlabel.Xalign = 0F;
            this.chrononlabel.LabelProp = Mono.Unix.Catalog.GetString("Chronon __ (~_.__ c/s)");
            this.vbox4.Add(this.chrononlabel);
            Gtk.Box.BoxChild w11 = ((Gtk.Box.BoxChild)(this.vbox4[this.chrononlabel]));
            w11.Position = 0;
            // Container child vbox4.Gtk.Box+BoxChild
            this.seedlabel = new Gtk.Label();
            this.seedlabel.Name = "seedlabel";
            this.seedlabel.Xalign = 0F;
            this.seedlabel.LabelProp = Mono.Unix.Catalog.GetString("<small>Match seed: _________________</small>");
            this.seedlabel.UseMarkup = true;
            this.vbox4.Add(this.seedlabel);
            Gtk.Box.BoxChild w12 = ((Gtk.Box.BoxChild)(this.vbox4[this.seedlabel]));
            w12.Position = 1;
            w12.Expand = false;
            w12.Fill = false;
            this.hbox8.Add(this.vbox4);
            Gtk.Box.BoxChild w13 = ((Gtk.Box.BoxChild)(this.hbox8[this.vbox4]));
            w13.Position = 1;
            this.vbox1.Add(this.hbox8);
            Gtk.Box.BoxChild w14 = ((Gtk.Box.BoxChild)(this.vbox1[this.hbox8]));
            w14.Position = 1;
            w14.Expand = false;
            w14.Fill = false;
            this.hbox1.Add(this.vbox1);
            Gtk.Box.BoxChild w15 = ((Gtk.Box.BoxChild)(this.hbox1[this.vbox1]));
            w15.Position = 1;
            this.mainvbox.Add(this.hbox1);
            Gtk.Box.BoxChild w16 = ((Gtk.Box.BoxChild)(this.mainvbox[this.hbox1]));
            w16.PackType = ((Gtk.PackType)(1));
            w16.Position = 0;
            this.Add(this.mainvbox);
            if ((this.Child != null)) {
                this.Child.ShowAll();
            }
            this.Show();
            this.DeleteEvent += new Gtk.DeleteEventHandler(this.OnDeleteEvent);
        }
    }
}
