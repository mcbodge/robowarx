using System;
using System.Collections.Generic;
using System.Text;
using RoboWarX.FileFormats;
using RoboWarX.Arena;
using System.IO;

namespace RoboWarX.Headless
{
    class Program
    {
        static void Main(string[] args)
        {
            HeadlessArena ha = new HeadlessArena();
            if (!parseArgs(args, ha))
                return;
            ha.Run();

        }

        /// <summary>
        /// Parses out the commandline arguments into the arena
        /// </summary>
        /// <param name="args">The list of commandline arguments</param>
        /// <param name="ha">The instance of the headless arena</param>
        /// <returns>true if the parsing was successful, false otherwise</returns>
        static bool parseArgs(string[] args, HeadlessArena ha)
        {

            if (args.Length == 0)
            {
                showHelp();
                return false;
            }

            for (int i = 0; i < args.Length; i++)
            {
                // parse out chronon's per second
                if (args[i] == "-cpu")
                {
                    int? cs = loadNextInt("-cpu",args, i);
                    if (cs.HasValue)
                    {
                        ha.ChrononsPerUpdate = cs.Value;
                        i++;
                    }
                    else
                        return false;
                }
                // parse out updates per second
                else if (args[i] == "-cl")
                {
                    int? us = loadNextInt("-cl", args, i);
                    if (us.HasValue)
                    {
                        ha.ChrononLimit = us.Value;
                        i++;
                    }
                    else
                        return false;
                }
                // check for interactive mode
                else if (args[i] == "-i")
                {
                    ha.InteractiveMode = true;
                }
                // show help when requested
                else if (args[i].ToLower() == "-h")
                {
                    showHelp();
                    return false;
                }
                // otherwise, assume it's a filename and try to load it
                else
                {
                    try
                    {
                        ha.AddRobot(RobotFile.OpenFile(args[i]));
                    }
                    catch (Exception e)
                    {
                        showError(String.Format("Unable to load robot file: {0}\n {1}",
                                                args[i], e.Message));
                    }
                }

            }
            return true;

        }
        /// <summary>
        /// Loads a successive integer from the parameter string, shows appropriate errors.
        /// </summary>
        /// <param name="paramName">The name of the parameter this integer is associated with</param>
        /// <param name="args">The list of commandline arguments</param>
        /// <param name="index">The current index</param>
        /// <returns>The integer if it can be found and parsed, null otherwise</returns>
        static int? loadNextInt(string paramName, string[] args, int index)
        {
            if (args.Length < index + 2)
            {
                showError(String.Format("{0} must be followed by a value", paramName));
                return null;
            }
            int val;
            if (Int32.TryParse(args[index + 1], out val))
                return val;
            else
            {
                showError(String.Format("Argument to {0} must be an integer", paramName));
                return null;
            }

        }

        static void showError(string errorString)
        {
            Console.WriteLine(errorString);
            showHelp();
        }


        static void showHelp()
        {
            Console.WriteLine("Usage: RoboWarCL.exe [-cpu <chronons per update>]\n\t[-cl <chrononLimit>] <filename1> <filename2> ...");
            Console.WriteLine("Defaults:\n\t20 chronon's per update, no chronon-limit ");
        }
    }
}
