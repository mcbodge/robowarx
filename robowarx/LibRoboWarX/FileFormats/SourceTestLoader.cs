using System;
using System.IO;
using System.Text;

namespace RoboWarX.FileFormats
{
    public static class SourceTestLoader
    {
        private const int BUFFER_SIZE = 2048;
        public static RobotFile read(String filename)
        {
            RobotFile result = new RobotFile();
            using (FileStream f = new FileStream(filename, FileMode.Open, FileAccess.Read))
            {
                return read(Path.GetFileNameWithoutExtension(filename), f);
            }
        }

        public static RobotFile read(string name, Stream s)
        {
            RobotFile result = new RobotFile();
            result.name = name;

            // a buffer for the stream contents
            StringBuilder sb = new StringBuilder();

            // read the stream
            byte[] buffer = new byte[BUFFER_SIZE];
            int len;
            while ((len = s.Read(buffer, 0, BUFFER_SIZE)) > 0)
                sb.Append(ASCIIEncoding.ASCII.GetString(buffer,0,len));
            result.code = sb.ToString();

            // Immediately compile for convenience
            result.compile();

            // Default hardware is rather minimal, see LibRoboWarX/Arena/Robot.cs
            // Perhaps set some useful values here for testing
            result.hardware.hasMissiles = true;

            return result;
        }
    }
}
