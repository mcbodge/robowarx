using System;
using System.IO;
using RoboWarX.Arena;

namespace RoboWarX.FileFormats
{
    internal static class WinUtil
    {
        // Read a 16-bit integer, and byteswap if necessary
        internal static int read16(byte[] input, int offset)
        {
            if (BitConverter.IsLittleEndian)
                return BitConverter.ToUInt16(input, offset);
            else
                return BitConverter.ToUInt16(new byte[] { input[offset + 1], input[offset] }, 0);
        }

        // Read a 32-bit integer, and byteswap if necessary
        internal static int read32(byte[] input, int offset)
        {
            if (BitConverter.IsLittleEndian)
                return BitConverter.ToInt32(input, offset);
            else
                return BitConverter.ToInt32(new byte[] { input[offset + 3], input[offset + 2], input[offset + 1], input[offset] }, 0);
        }
    }

    // Class for disecting a RoboWar5 file
    public static class WinRoboWar5
    {
        public static void read(RobotFile f, Stream s)
        {
            byte[] buf = new byte[141];
            if (s.Read(buf, 0, 141) != 141)
                throw new IOException("Could not read header.");

            f.hardware.energyMax = WinUtil.read16(buf, 0);
            f.hardware.damageMax = WinUtil.read16(buf, 2);
            f.hardware.shieldMax = WinUtil.read16(buf, 4);
            f.hardware.processorSpeed = buf[6];
            f.hardware.noNegEnergy = buf[7] > 0;
            switch (buf[8])
            {
                case 1: f.turretType = TurretType.Line; break;
                case 2: f.turretType = TurretType.Dot; break;
                case 3:
                default: f.turretType = TurretType.None; break;
            }
            // FIXME: do I have to support case 20?
            switch (buf[9])
            {
                case 0: f.hardware.gunType = BulletType.Rubber; break;
                case 2: f.hardware.gunType = BulletType.Explosive; break;
                case 1:
                default: f.hardware.gunType = BulletType.Normal; break;
            }
            f.hardware.hasMissiles = buf[10] > 0;
            f.hardware.hasTacNukes = buf[11] > 0;
            f.hardware.hasHellbores = buf[12] > 0;
            f.hardware.hasMines = buf[13] > 0;
            f.hardware.hasStunners = buf[14] > 0;
            f.hardware.hasDrones = buf[15] > 0;
            f.hardware.hasLasers = buf[16] > 0;
            f.hardware.probeFlag = buf[17] > 0;
            // 4 reserved bytes
            // FIXME: shieldicon (1 byte)
            // FIXME: deathicon (1 byte)
            // FIXME: hiticon (1 byte)
            // FIXME: blockicon (1 byte)
            // FIXME: collisionicon (1 byte)
            // FIXME: cursor position (4 bytes)
            // FIXME: 10 x 4 bytes, "Sound # Start"
            // FIXME: 10 x 4 bytes, "Icon # Start"
            int programstart = WinUtil.read32(buf, 111) - 1;
            int codestart = WinUtil.read32(buf, 115) - 1;
            // FIXME: 10 x 1 bytes, "Sound # Exists"
            // FIXME: 10 x 1 bytes, "Icon # Exists"
            f.locked = buf[139] > 0;
            // FIXME: language (1 byte)

            // Read the byte code
            int programlen = codestart - programstart;
            s.Seek(programstart, SeekOrigin.Begin);
            byte[] temp = new byte[programlen];
            if (s.Read(temp, 0, programlen) != programlen)
                throw new IOException("Could not read program.");
            // Swap byte order
            f.program = new byte[programlen];
            for (int i = 0; i < programlen; i += 2)
            {
                f.program[i] = temp[i + 1];
                f.program[i + 1] = temp[i];
            }

            // FIXME: read source code
        }
        
        public static void write(RobotFile f, Stream s)
        {
            // FIXME
        }
    }
}
