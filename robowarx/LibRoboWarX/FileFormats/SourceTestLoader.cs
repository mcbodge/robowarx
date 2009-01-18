using System;
using System.IO;

namespace RoboWarX.FileFormats
{
    public static class SourceTestLoader
    {
        public static RobotFile read(String filename)
        {
            RobotFile result = new RobotFile();
            StreamReader f = new StreamReader(filename);
            
            // Induce the robot name from the filename
            result.name = Path.GetFileNameWithoutExtension(filename);
            // Read the source
            result.code = f.ReadToEnd();
            // Immediately compile for convenience
            result.compile();
            
            // Default hardware is rather minimal, see LibRoboWarX/Arena/Robot.cs
            // Perhaps set some useful values here for testing
            result.hardware.hasMissiles = true;
            
            return result;
        }
    }
}
