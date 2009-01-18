using System;

/* FIXME: this should probably be moved into the MainWindow itself. */

namespace RoboWarX.GTK
{
    public partial class BasicActions : Gtk.ActionGroup
    {
        public BasicActions() : base("RoboWarX.GTK.BasicActions")
        {
            this.Build();
        }
        
        public String buildMenuDescription()
        {
            return @"
                <ui>
                  <menubar name='Menubar'>
                    <menu name='FileMenu' action='FileMenuAction'>
                      <menuitem name='Play' action='PlayAction'/>
                      <menuitem name='Pause' action='PauseAction'/>
                      <separator/>
                      <placeholder name='Robot1'/>
                      <placeholder name='Robot2'/>
                      <placeholder name='Robot3'/>
                      <placeholder name='Robot4'/>
                      <placeholder name='Robot5'/>
                      <placeholder name='Robot6'/>
                      <menuitem name='OpenRobot' action='OpenAction'/>
                      <separator/>
                      <menuitem name='Quit' action='QuitAction'/>
                    </menu>
                  </menubar>
                </ui>
            ";
        }
    }
}