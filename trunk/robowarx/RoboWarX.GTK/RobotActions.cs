using System;

namespace RoboWarX.GTK
{
    public partial class RobotActions : Gtk.ActionGroup
    {
        private Gtk.Action PropertiesAction;
        private Gtk.Action SaveAction;
        private Gtk.Action SaveAsAction;
        private Gtk.Action CloseAction;
        
        private String prefix;
        
        public RobotActions(int id) : base("RobotActions")
        {
            prefix = String.Format("Robot{0}", id);
            
            PropertiesAction = new Gtk.Action(prefix + "Action",
                                              String.Format("Robot _{0}...", id),
                                              null, "gtk-properties");
            PropertiesAction.ShortLabel = "_Properties...";
            Add(this.PropertiesAction, null);
            
            PropertiesAction = new Gtk.Action(prefix + "PropertiesAction", "_Properties...", null, "gtk-properties");
            PropertiesAction.ShortLabel = "_Properties...";
            Add(this.PropertiesAction, null);
            
            SaveAction = new Gtk.Action(prefix + "SaveAction", "_Save", null, "gtk-save");
            SaveAction.ShortLabel = "_Save";
            Add(this.SaveAction, null);
            
            SaveAsAction = new Gtk.Action(prefix + "SaveAsAction", "Save _As...", null, "gtk-save-as");
            SaveAsAction.ShortLabel = "Save _As...";
            Add(this.SaveAsAction, null);
            
            CloseAction = new Gtk.Action(prefix + "CloseAction", "_Close", null, "gtk-close");
            CloseAction.ShortLabel = "_Close";
            Add(this.CloseAction, null);
        }
        
        public String buildMenuDescription()
        {
            return String.Format(@"
                <ui>
                  <menubar name='Menubar'>
                    <menu name='FileMenu' action='FileMenuAction'>
                      <placeholder name='{0}'>
                        <menu name='{0}Menu' action='{0}Action'>
                          <menuitem name='{0}Properties' action='{0}PropertiesAction'/>
                          <separator/>
                          <menuitem name='{0}Save' action='{0}SaveAction'/>
                          <menuitem name='{0}SaveAs' action='{0}SaveAsAction'/>
                          <menuitem name='{0}Close' action='{0}CloseAction'/>
                        </menu>
                      </placeholder>
                    </menu>
                  </menubar>
                </ui>
            ", prefix);
        }
        
        public Gtk.Action getRobotAction(String name)
        {
            return this.GetAction(prefix + name);
        }
    }
}