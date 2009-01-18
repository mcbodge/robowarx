using System;

namespace RoboWarX
{
    // Interface implemented by a plugin
    public interface IPluginEntry
    {
        // Return a list of register prototypes
        ITemplateRegister[] getPrototypes();
    }
}
