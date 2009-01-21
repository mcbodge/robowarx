using System;
using System.Collections.Generic;
using System.Text;
using RoboWarX.FileFormats;
using RoboWarX.Arena;
using System.Threading;

namespace RoboWarX.Headless
{
    /// <summary>
    /// A little console display wrapper
    /// </summary>
    public class HeadlessArena
    {
        public const string OUTPUT_STRING = "{0},{1},{2},{3},{4},{5},{6}";
        Arena.Arena arena;
        List<RobotWrapper> robots = new List<RobotWrapper>();

        private int _chrononLimit = 0;
        public int ChrononLimit
        {
            get { return _chrononLimit; }
            set { _chrononLimit = value; }
        }

        private int _chrononsPerUpdate = 20;
        /// <summary>
        /// Indicates how fast the simulation should run
        /// </summary>
        public int ChrononsPerUpdate
        {
            get { return _chrononsPerUpdate; }
            set { _chrononsPerUpdate = value; }
        }

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
            new_game();
        }

        private void new_game()
        {
            arena = new Arena.Arena();
            arena.loadDefaults();
        }

        /// <summary>
        /// Runs the game according to the parameters
        /// </summary>
        public void Run() {
            if(robots.Count == 0 ) {
                Console.WriteLine("No Robots! Nothing to run");
            }
            else {
                gameStart = DateTime.Now;
                DisplayState();
                while(!arena.finished) {
                    try
                    {
                        arena.stepChronon();

                        if (_chrononsPerUpdate > 0 && arena.chronon % _chrononsPerUpdate == 0)
                            DisplayState();
                    }
                    catch (Exception e)
                    {
                        if (!HandleError(e))
                            break;
                    }
                }
                DisplayState();
            }
        }

        DateTime gameStart;

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
                "Damage", "X", "Y", "team"));
            Console.WriteLine("".PadLeft(40, '-'));
            foreach(RobotWrapper rw in robots) 
                Console.WriteLine(rw.ToString());
        }

    }
}
