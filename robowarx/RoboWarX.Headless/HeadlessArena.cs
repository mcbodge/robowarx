using System;
using System.Collections.Generic;
using System.Text;
using RoboWarX.FileFormats;
using RoboWarX.Arena;
using System.Threading;
using System.IO;
using System.Linq;

namespace RoboWarX.Headless
{
    /// <summary>
    /// A little console display wrapper
    /// </summary>
    public class HeadlessArena
    {
        public const string OUTPUT_STRING = "{0},{1},{2},{3},{4},{5},{6},{7},{8},{9}";
        private const int DEFAULT_CHRONON_LIMIT = 0;
        private const int DEFAULT_CHRONONS_PER_UPDATE = 20;
        private const string EOF_STR = "\x04";
        private const string EOM_STR = "\x04";
        private const string QUIT_STR = "\x03";
        Arena.Arena arena;
        List<RobotWrapper> robots = new List<RobotWrapper>();

        private int chrononLimit;
        /// <summary>
        /// Indicates the maximum chronon for the match
        /// </summary>
        public int ChrononLimit
        {
            get { return chrononLimit; }
            set
            {
                chrononLimit = value;
                if (arena != null)
                    arena.chrononLimit = value;
            }
        }
        /// <summary>
        /// Indicates how fast the simulation should run
        /// </summary>
        public int ChrononsPerUpdate { get; set; }

        /// <summary>
        /// Indicates that this should be run in interactive mode. That means that
        /// robots should be taken from stdin.
        /// </summary>
        public bool InteractiveMode { get; set; }

        /// <summary>
        /// Adds a robot to the arena.
        /// </summary>
        /// <param name="file"></param>
        public void AddRobot(RobotFile file)
        {
            robots.Add(new RobotWrapper(arena.loadRobot(file)));
        }

        public HeadlessArena()
        {
            ChrononLimit = DEFAULT_CHRONON_LIMIT;
            ChrononsPerUpdate = DEFAULT_CHRONONS_PER_UPDATE;
            new_game();
        }

        private void new_game()
        {
            arena = new Arena.Arena();
            arena.chrononLimit = ChrononLimit;
            robots.Clear();
        }

        /// <summary>
        /// Runs the game according to the parameters
        /// </summary>
        public void Run() {
            if (InteractiveMode)
            {
                bool continueRunning = true;
                while (continueRunning)
                {
                    // read robots until we see EOM
                    string readLine;
                    while ((readLine = Console.ReadLine()) != EOM_STR)
                    {
                        // quit when told
                        if (readLine == QUIT_STR)
                            return;
                        // read off a robot, first line should be the name
                        try
                        {
                            ReadRobot(readLine);
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(String.Format("Error adding robot: {0}", e.Message));
                        }
                    }
                    runGame();
                    new_game();

                }
            }
            else
                runGame();
        }

        /// <summary>
        /// Reads a robot from stdin
        /// </summary>
        /// <returns>False if the quit character was seen, true otherwise.</returns>
        private bool ReadRobot(string name)
        {
            StringBuilder roboFile = new StringBuilder();
            string readLine;
            while ((readLine = Console.ReadLine()) != EOF_STR)
            {
                if (readLine == QUIT_STR)
                    return false;
                roboFile.AppendLine(readLine);
            }
            MemoryStream ms = new MemoryStream(ASCIIEncoding.ASCII.GetBytes(roboFile.ToString()));
            
            RobotFile result = new RobotFile();
            result.name = name;
            SourceTestLoader.read(result, ms);
            AddRobot(result);

            return true;
        }

        /// <summary>
        /// Runs the currently loaded arena
        /// </summary>
        private void runGame()
        {
            if (robots.Count == 0)
            {
                Console.WriteLine("No Robots! Nothing to run");
            }
            else
            {
                while (!arena.finished)
                {
                    try
                    {
                        arena.stepChronon();

                        if (ChrononsPerUpdate > 0 && arena.chronon % ChrononsPerUpdate == 0)
                            DisplayState();
                    }
                    catch (Exception e)
                    {
                        if (!HandleError(e))
                            break;
                    }
                }
                DisplayState();
        Console.WriteLine("".PadLeft(40, '#'));
            }
        }

        /// <summary>
        /// Handles an error, current implementation simply displays the error message.
        /// </summary>
        /// <param name="e">The exception to handle</param>
        /// <returns>True if processing should continue, false otherwise</returns>
        private bool HandleError(Exception e)
        {
            Console.WriteLine(String.Format("Encountered an Error: {0}", e.Message));
            return true;
        }

        /// <summary>
        /// Displays the current state of the game to the user
        /// </summary>
        private void DisplayState()
        {
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine(String.Format("Chronon: {0}", arena.chronon.ToString()));
            // Console.WriteLine(String.Format("RunTime: {0}ms", DateTime.Now.Subtract(gameStart).TotalMilliseconds));
            Console.WriteLine(String.Format(OUTPUT_STRING, "Name", "Number", "Energy", 
                "Damage", "X", "Y", "team","Alive", "Killer", "DeathReason"));
            Console.WriteLine("".PadLeft(40, '-'));
            foreach(RobotWrapper rw in robots) 
                Console.WriteLine(rw.ToString());
        }

    }
}
