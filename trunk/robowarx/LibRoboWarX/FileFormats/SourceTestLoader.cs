using System;
using System.IO;
using System.Text;

namespace RoboWarX.FileFormats
{
    public static class SourceTestLoader
    {
        private const int BUFFER_SIZE = 8096;

        public static void read(RobotFile f, Stream s)
        {
            // a buffer for the stream contents
            StringBuilder sb = new StringBuilder();

            // read the stream
            byte[] buffer = new byte[BUFFER_SIZE];
            int len;
            while ((len = s.Read(buffer, 0, BUFFER_SIZE)) > 0)
                sb.Append(ASCIIEncoding.ASCII.GetString(buffer,0,len));
            f.code = sb.ToString();

            // Immediately compile for convenience
            f.compile();
            
            // FIXME: read hardware from a comment in the source file
        }
        
        public static void write(RobotFile f, Stream s)
        {
            // FIXME
        }
    }
}
