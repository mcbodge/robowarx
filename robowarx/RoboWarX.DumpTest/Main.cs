using System;
using System.IO;
using System.Text;
using System.Diagnostics;
using RoboWarX;
using RoboWarX.Arena;
using RoboWarX.FileFormats;

namespace RoboWarX.DumpTest
{
    // FIXME: share this code somewhere with MacBinary
    internal static class MacUtil
    {
        // A decoder instance for the macintosh encoding
        internal static Decoder macdecoder;
        
        static MacUtil()
        {
            Encoding macencoding;
            try
            {
                macencoding = Encoding.GetEncoding("macintosh");
            }
            catch (ArgumentException)
            {
                macencoding = Encoding.ASCII;
            }
            macdecoder = macencoding.GetDecoder();
        }

        // Read a 16-bit integer, and byteswap if necessary
        internal static int read16(byte[] input, int offset)
        {
            if (BitConverter.IsLittleEndian)
                return BitConverter.ToUInt16(new byte[] { input[offset + 1], input[offset] }, 0);
            else
                return BitConverter.ToUInt16(input, offset);
        }

        // Read a 24-bit integer, and byteswap if necessary
        internal static int read24(byte[] input, int offset)
        {
            if (BitConverter.IsLittleEndian)
                return BitConverter.ToInt32(new byte[] { input[offset + 2], input[offset + 1], input[offset], 0 }, 0);
            else
                return BitConverter.ToInt32(new byte[] { 0, input[offset], input[offset + 1], input[offset + 2] }, offset);
        }

        // Read a 32-bit integer, and byteswap if necessary
        internal static int read32(byte[] input, int offset)
        {
            if (BitConverter.IsLittleEndian)
                return BitConverter.ToInt32(new byte[] { input[offset + 3], input[offset + 2], input[offset + 1], input[offset] }, 0);
            else
                return BitConverter.ToInt32(input, offset);
        }

        // Read a string of the specified size
        internal static String readString(byte[] input, int offset, int len)
        {
            int bytesUsed, charsUsed;
            bool completed;
            char[] outbuf = new char[len];
            macdecoder.Convert(input, offset, len, outbuf, 0, len, true,
                out bytesUsed, out charsUsed, out completed);
            return new String(outbuf, 0, charsUsed);
        }
    }

    
    
    class MainClass
    {
        private static FileStream f;
        private static RobotFile[] robotFiles;
        
        public static void Main(string[] args)
        {
            if (args.Length != 1)
            {
                Console.WriteLine("Usage: RoboWarX.DumpTest.exe <dumpfile>");
                return;
            }
            
            f = File.Open(args[0], FileMode.Open, FileAccess.Read);
            
            LoadStartHeader();
            
            Console.WriteLine("No failures.");
        }
        
        public static void LoadStartHeader()
        {
            // Load the header and robots from the file
            
            byte[] buffer = new byte[258];
            f.Read(buffer, 0, 8);
            Debug.Assert(MacUtil.readString(buffer, 0, 4) == "RDMP");
            int seed = MacUtil.read16(buffer, 4);
            int numBots = MacUtil.read16(buffer, 6);
            Debug.Assert(numBots < 7);
            
            robotFiles = new RobotFile[numBots];
            for (int i = 0; i < numBots; i++)
            {
                RobotFile nf = new RobotFile();
                robotFiles[i] = nf;
                
                f.Read(buffer, 0, 258);
                Debug.Assert(buffer[0] > 0);
                nf.name = MacUtil.readString(buffer, 1, buffer[0]);
                int progLen = MacUtil.read16(buffer, 256);
                Debug.Assert(progLen <= (int)Bytecodes.NUM_MAX_CODE);
                
                nf.program = new byte[progLen * 2];
                f.Read(nf.program, 0, progLen * 2);
                
                f.Read(buffer, 0, 40);
                
                nf.hardware.energyMax = MacUtil.read16(buffer, 0);
                nf.hardware.damageMax = MacUtil.read16(buffer, 2);
                nf.hardware.shieldMax = MacUtil.read16(buffer, 4);
                nf.hardware.processorSpeed = MacUtil.read16(buffer, 6);
                int gun = MacUtil.read16(buffer, 8);
                switch (gun)
                {
                case 1: nf.hardware.gunType = BulletType.Rubber; break;
                case 3: nf.hardware.gunType = BulletType.Explosive; break;
                case 2:
                default: nf.hardware.gunType = BulletType.Normal; break;
                }
                nf.hardware.hasMissiles = MacUtil.read16(buffer, 10) > 0;
                nf.hardware.hasTacNukes = MacUtil.read16(buffer, 12) > 0;
                nf.hardware.hasLasers = MacUtil.read16(buffer, 16) > 0;
                nf.hardware.hasHellbores = MacUtil.read16(buffer, 18) > 0;
                nf.hardware.hasDrones = MacUtil.read16(buffer, 20) > 0;
                nf.hardware.hasMines = MacUtil.read16(buffer, 22) > 0;
                nf.hardware.hasStunners = MacUtil.read16(buffer, 24) > 0;
                nf.hardware.noNegEnergy = MacUtil.read16(buffer, 26) > 0;
                nf.hardware.probeFlag = MacUtil.read16(buffer, 28) > 0;
                nf.hardware.deathIconFlag = MacUtil.read16(buffer, 30) > 0;
                nf.hardware.collisionIconFlag = MacUtil.read16(buffer, 32) > 0;
                nf.hardware.shieldHitIconFlag = MacUtil.read16(buffer, 34) > 0;
                nf.hardware.hitIconFlag = MacUtil.read16(buffer, 36) > 0;
                nf.hardware.shieldOnIconFlag = MacUtil.read16(buffer, 38) > 0;
                
                int advantages = MacUtil.read16(buffer, 14);
                Debug.Assert(advantages == nf.hardware.advantages);
            }
            
            // Print info header
            Console.WriteLine("Replaying match with mac seed {0} and {1} robot(s):",
                              seed, numBots);
            string[] names = new string[numBots];
            for (int i = 0; i < numBots; i++)
                names[i] = robotFiles[i].name;
            Console.WriteLine(String.Join(", ", names));
        }
    }
}