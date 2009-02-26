using System;
using System.Collections.Generic;
using GLib;
using RoboWarX;

// Friendly robot exception display

namespace RoboWarX.GTK
{
    public partial class ErrorDialog : Gtk.Dialog
    {
        public ErrorDialog(RobotFaultEvent e)
        {
            this.Build();
            
            titlelabel.Markup = "<big><b>Error in robot '" +
                Markup.EscapeText(e.robot.name) + "'</b></big>";
            
            String message = e.exception.Message;
            if (e.exception.GetType() == typeof(RobotException))
                message = "(At instruction " + (e.exception as RobotException).ProgramLocation + ")\n" + message;
            
            errorlabel.Markup = "The robot encountered a problem during execution, " +
                "and will now terminate.\n\n<b>Details:</b>\n" + message;
            
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