using System;
using System.IO;
using System.Reflection;
using System.Collections.Generic;

namespace RoboWarX
{
    internal static class Util
    {
        internal delegate void loadRegisterDelegate(ITemplateRegister register);

        internal static List<ITemplateRegister> defaultsCache = new List<ITemplateRegister>();
        // Function for scanning the current directory for DLLs that contain plugins.
        // Looks for classes implementing IPluginEntry.
        // Used by both the compiler and virtual machine.
        // FIXME: this should probably be restricted.
        internal static void loadDefaults(loadRegisterDelegate f)
        {
            if (defaultsCache.Count == 0)
            {
                lock (defaultsCache)
                {
                    if (defaultsCache.Count == 0)
                    {

                        String[] assemblies = Directory.GetFiles(
                            Path.GetDirectoryName(Environment.GetCommandLineArgs()[0]),
                            "*.dll", SearchOption.TopDirectoryOnly);
                        foreach (String assembly in assemblies)
                        {
                            Assembly plugin;
                            try
                            {
                                plugin = Assembly.LoadFrom(assembly);
                            }
                            catch
                            {
                                continue;
                            }
                            Type[] exports = plugin.GetExportedTypes();
                            foreach (Type export in exports)
                            {
                                Type[] interfaces = export.GetInterfaces();
                                foreach (Type i in interfaces)
                                    if (i == typeof(IPluginEntry))
                                    {
                                        IPluginEntry entry = Activator.CreateInstance(export) as IPluginEntry;
                                        ITemplateRegister[] registers = entry.getPrototypes();
                                        foreach (ITemplateRegister register in registers)
                                            defaultsCache.Add(register);
                                    }
                            }
                        }
                    }
                }
            }
            foreach (ITemplateRegister itr in defaultsCache)
                f(itr);
        }

        // Version of Sin that tries to mimic RoboWar behavior
        internal static double Sin(double a)
        {
            return Math.Sin((90 - a) * Constants.DEG_TO_RAD);
        }

        // Version of Cos that tries to mimic RoboWar behavior
        internal static double Cos(double a)
        {
            return Math.Cos((90 - a) * Constants.DEG_TO_RAD);
        }

        // Version of Tan that tries to mimic RoboWar behavior
        internal static double Tan(double a)
        {
            return Math.Tan((90 - a) * Constants.DEG_TO_RAD);
        }
    }
}
