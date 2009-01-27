using System;
using System.IO;
using System.Text;
using System.Linq;
using System.Reflection;
using RoboWarX.Compiler;

namespace RoboWarX.FileFormats
{
    public static class SourceTestLoader
    {
        private const int BUFFER_SIZE = 8096;
        private const string HARDWARE_STRING = "#!!hardware";

        public static void read(RobotFile f, Stream s)
        {
            // a buffer for the stream contents
            StringBuilder sb = new StringBuilder();
            StringBuilder hardwareSpecifiers = new StringBuilder();

            // read the stream
            byte[] buffer = new byte[BUFFER_SIZE];
            int len;
            while ((len = s.Read(buffer, 0, BUFFER_SIZE)) > 0)
            {
                string line = ASCIIEncoding.ASCII.GetString(buffer, 0, len); 
                sb.Append(line);

                // lets check to see if this is a hardware specifier
                setHardware(f, line);
            }
            f.code = sb.ToString();

            // Immediately compile for convenience
            f.compile();
            

        }

        /// <summary>
        /// Checks if the specified line is a hardware specifier. If so, it reflects the specifier onto the 
        /// bot.
        /// </summary>
        /// <param name="f"></param>
        /// <param name="line"></param>
        private static void setHardware(RobotFile f, string line)
        {
            if (line.Trim().ToLower().StartsWith(HARDWARE_STRING))
            {
                string hardwareSpecifier = line.Trim().Substring(HARDWARE_STRING.Length);
                // split off the values and look for
                string[] values = hardwareSpecifier.Split(':').Select(str => str.Trim()).ToArray();
                if (values.Length == 2)
                {
                    PropertyInfo property = f.hardware.GetType().GetProperty(values[0]);
                    // basic checks and errors
                    if (property != null)
                    {
                        if (property.CanWrite)
                        {
                            object convertedValue;
                            try
                            {
                                // parse if it's an enum, use built-in converters otherwise.
                                if (property.PropertyType.IsEnum)
                                    convertedValue = Enum.Parse(property.PropertyType, values[1], true);
                                else
                                    convertedValue = Convert.ChangeType(values[1], property.PropertyType);
                                property.SetValue(f.hardware, convertedValue, null);
                            }
                            catch
                            {
                                throw new CompilerException(String.Format(
                                    "Unable to set hardware specifier {0}, was expecting a {1} received {2}",
                                    values[0], property.PropertyType.Name, values[1]));
                            }
                        }
                        else
                            throw new CompilerException(String.Format(
                                "Unable to set hardware specifier {0} it is read-only", values[0]));
                    }
                    else
                        throw new CompilerException(
                            String.Format("Unable to find hardware specifier named: {0}", values[0]));
                }
                else
                    throw new CompilerException(String.Format("MalFormed hardware specifier: {0}", line));
            
            }
        }
        
        public static void write(RobotFile f, Stream s)
        {
            // FIXME
        }
    }
}