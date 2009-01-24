using System;
using System.IO;
using System.Collections.Generic;

namespace RoboWarX.FileFormats
{
    public static class BinaryTestLoader
    {
        private const int BUFFER_SIZE = 8096;

        public static void read(RobotFile f, Stream s)
        {
            // Buffer into this memory stream
            System.IO.MemoryStream ms = new MemoryStream(BUFFER_SIZE);
            
            // read the stream
            byte[] buffer = new byte[BUFFER_SIZE];
            int len;
            while ((len = s.Read(buffer, 0, BUFFER_SIZE)) > 0)
                ms.Write(buffer, 0, len);
            f.program = ms.GetBuffer();
            
            // FIXME: where would one store hardware here?
        }
        
        public static void write(RobotFile f, Stream s)
        {
            // FIXME
        }
    }
}
