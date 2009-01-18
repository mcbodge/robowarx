using System;
using System.Collections.Generic;
using GLib;
using RoboWarX;

// Friendly robot exception display

namespace RoboWarX.GTK
{
    public partial class ErrorDialog : Gtk.Dialog
    {
        public ErrorDialog(RobotException e)
        {
            this.Build();
            
            titlelabel.Markup = "<big><b>Error in robot '" +
                Markup.EscapeText(e.robot.name) + "'</b></big>";
            
            errorlabel.Markup = "The robot encountered a problem during execution, " +
                "and will now terminate.\n\n<b>Details:</b>\nAt instruction " +
                    Markup.EscapeText(e.ProgramLocation) + ":\n" +
                    Markup.EscapeText(e.Message);
            
        }

        protected virtual void OnButtonCloseClicked (object sender, System.EventArgs e)
        {
            Respond(Gtk.ResponseType.Close);
            Destroy();
        }

        protected virtual void OnButtonStopClicked (object sender, System.EventArgs e)
        {
            Respond(Gtk.ResponseType.Cancel);
            Destroy();
        }
    }
}