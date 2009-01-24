using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using RoboWarX.Arena;

namespace RoboWarX.FileFormats
{
    public class MacBinaryException : Exception
    {
        public MacBinaryException(String msg) : base(msg) { }
    }

    public class MacResourceForkException : Exception
    {
        public MacResourceForkException(String msg) : base(msg) { }
    }

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

    // Represents a single resource
    public class MacResource
    {
        public readonly String type;
        public readonly String name;
        public readonly int id;
        public readonly byte[] data;

        internal MacResource(Stream input, String type, String name, int id)
        {
            this.type = type;
            this.name = name;
            this.id = id;
            
            byte[] buf = new byte[4];
            if (input.Read(buf, 0, 4) != 4)
                throw new MacResourceForkException("Could not read resource length.");
            int datalen = MacUtil.read32(buf, 0);

            data = new byte[datalen];
            if (input.Read(data, 0, datalen) != datalen)
                throw new MacResourceForkException("Could not read resource data.");
        }
    }

    // Contains all resources in a resource fork of a specific type
    public class MacResourceType
    {
        private Dictionary<int, MacResource> resources;

        public readonly String type;

        internal MacResourceType(String type, int capacity)
        {
            this.type = type;
            this.resources = new Dictionary<int,MacResource>(capacity);
        }

        internal void Add(MacResource res)
        {
            resources.Add(res.id, res);
        }

        // Handy indexing is handy
        public MacResource this[int id]
        {
            get
            {
                return resources[id];
            }
        }

        public bool Contains(int id)
        {
            return resources.ContainsKey(id);
        }
    }

    // Represents a macintosh resource fork
    public class MacResourceFork
    {
        // The contents
        private Dictionary<String, MacResourceType> types;

        public MacResourceFork(Stream input)
        {
            long start = input.Position;
            byte[] buf;

            // Read the resource fork header
            buf = new byte[16];
            if (input.Read(buf, 0, 16) != 16)
                throw new MacResourceForkException("Could not read header.");
            long dataoffset = start + MacUtil.read32(buf, 0);
            long mapoffset = start + MacUtil.read32(buf, 4);

            long typelistoffset, namelistoffset;
            byte[] typelist;
            {
                // Seek to the resource map
                input.Seek(mapoffset, SeekOrigin.Begin);

                // Read the resource map header
                buf = new byte[30];
                if (input.Read(buf, 0, 30) != 30)
                    throw new MacResourceForkException("Could not read resource map header.");
                typelistoffset = mapoffset + MacUtil.read16(buf, 24);
                namelistoffset = mapoffset + MacUtil.read16(buf, 26);
                int numtypes = MacUtil.read16(buf, 28) + 1;

                // Create the types dictionary
                types = new Dictionary<string,MacResourceType>(numtypes);

                // Read the resource type list
                int listlen = numtypes * 8;
                typelist = new byte[listlen];
                if (input.Read(typelist, 0, listlen) != listlen)
                    throw new MacResourceForkException("Could not read resource type list header.");
            }

            // Walk the resource type list
            for (int i = 0; i < typelist.Length; i += 8)
            {
                MacResourceType type;
                long reflistoffset;
                byte[] reflist;
                {
                    // Read the resource type list entry
                    String typename = MacUtil.readString(typelist, i, 4);
                    int numresources = MacUtil.read16(typelist, i + 4);
                    reflistoffset = typelistoffset + MacUtil.read16(typelist, i + 6);

                    // Create the resource type
                    type = new MacResourceType(typename, numresources);
                    types.Add(typename, type);

                    // Seek to the reference list for this type
                    input.Seek(reflistoffset, SeekOrigin.Begin);

                    // Read the reference list
                    int listlen = numresources * 12;
                    reflist = new byte[listlen];
                    if (input.Read(reflist, 0, listlen) != listlen)
                        throw new MacResourceForkException("Could not read the reference list.");
                }

                for (int j = 0; j < reflist.Length; j += 12)
                {
                    // Read the reference list entry
                    int id = MacUtil.read16(reflist, j);
                    long resnameoffset = MacUtil.read16(reflist, j + 2);
                    long resdataoffset = dataoffset + MacUtil.read24(reflist, j + 5);

                    String name = null;
                    if (resnameoffset != -1)
                    {
                        // Seek to the name list entry
                        resnameoffset += namelistoffset;
                        input.Seek(resnameoffset, SeekOrigin.Begin);

                        // Read the resource name
                        int resnamelen = input.ReadByte();
                        buf = new byte[resnamelen];
                        if (input.Read(buf, 0, resnamelen) != resnamelen)
                            throw new MacResourceForkException("Could not read resource name.");
                        name = MacUtil.readString(buf, 0, resnamelen);
                    }

                    // Seek to the resource data
                    input.Seek(resdataoffset, SeekOrigin.Begin);

                    // Create the resource
                    MacResource res = new MacResource(input, type.type, name, id);
                    type.Add(res);
                }
            }
        }

        // Handy indexing is handy
        public MacResourceType this[String type]
        {
            get
            {
                return types[type];
            }
        }

        public bool Contains(String type)
        {
            return types.ContainsKey(type);
        }
    }

    // Represents the MacBinary file in-memory
    public class MacBinary
    {
        // Supports all versions! Whoop whoop!
        public enum Version
        {
            MacBinaryI,
            MacBinaryII,
            MacBinaryIII
        }

        public readonly Version version;

        // These map to the header fields
        public readonly String filename;
        public readonly String filetype;
        public readonly String filecreator;
        public readonly int finderflags;
        public readonly int finderflags2;
        public readonly int windowposy;
        public readonly int windowposx;
        public readonly int windowid;
        public readonly bool protectedflag;
        public readonly int ctime;
        public readonly int mtime;
        public readonly int fxScript;
        public readonly int fdXFlags;
        public readonly String comment;

        // The contents
        public readonly MacResourceFork res;
        public readonly byte[] dat;

        public MacBinary(Stream input)
        {
            byte[] buf = new byte[128];
            if (input.Read(buf, 0, 128) != 128)
                throw new MacBinaryException("Could not read header.");

            version = Version.MacBinaryIII;
            // Version
            if (buf[0] != 0)
                throw new MacBinaryException("Incompatible version.");
            // Signature 'mBIN'
            if (buf[102] != 'm' || buf[103] != 'B' || buf[104] != 'I' || buf[105] != 'N')
                version = Version.MacBinaryII;
            // Zero padding
            if (buf[74] != 0 || buf[82] != 0 || buf[126] != 0 || buf[127] != 0)
                throw new MacBinaryException("Zero padding is not zero.");
            // MacBinary version of this file.
            if (version == Version.MacBinaryII && buf[122] != 129)
                throw new MacBinaryException("Version field corrupt for a MacBinary II file.");
            if (version == Version.MacBinaryIII && buf[122] != 130)
            {
                if (buf[122] == 129)
                    version = Version.MacBinaryII;
                else
                    throw new MacBinaryException("Version field corrupt for a MacBinary III file.");
            }
            // CRC at the end of the header
            int crc = MacUtil.read16(buf, 124);
            // FIXME: check CRC
            if (crc == 0)
                version = Version.MacBinaryI;

            // File name
            int namelen = buf[1];
            if (namelen < 1 || namelen > 63)
                throw new MacBinaryException("Invalid file length.");
            filename = MacUtil.readString(buf, 2, namelen);
            // File type
            filetype = MacUtil.readString(buf, 65, 4);
            // File creator
            filecreator = MacUtil.readString(buf, 69, 4);
            // Finder flags
            finderflags = buf[73];
            // Window position
            windowposy = MacUtil.read16(buf, 75);
            windowposx = MacUtil.read16(buf, 77);
            // Window or folder ID
            windowid = MacUtil.read16(buf, 79);
            // Protected flag
            protectedflag = (buf[81] & 1) == 1;
            // Data en resource fork lengths
            int datlen = MacUtil.read32(buf, 83);
            int reslen = MacUtil.read32(buf, 87);
            // Creation and modification dates
            ctime = MacUtil.read32(buf, 91);
            mtime = MacUtil.read32(buf, 95);

            int commentlen = 0;
            int secheaderlen = 0;
            if (version != Version.MacBinaryI)
            {
                // Get Info comment length
                commentlen = MacUtil.read16(buf, 99);
                // Finder flags
                finderflags2 = buf[101];
                if (version != Version.MacBinaryII)
                {
                    // Finder fxInfo fields
                    fxScript = buf[106];
                    fdXFlags = buf[107];
                }
                // Length of total files when packed files are unpacked. (never really used)
                //int unpackedlen = read32(buf, 117);
                // Length of secondary header.
                secheaderlen = MacUtil.read16(buf, 120);
                // Minimum version to support in order to read this file (ignored)
                //int minversion = buf[123];
            }

            // Skip the secondary header
            if (secheaderlen > 0)
            {
                // Round up to 128 bytes
                secheaderlen = (int)Math.Ceiling((double)secheaderlen / 128) * 128;
                input.Seek(secheaderlen, SeekOrigin.Current);
            }

            // Read data fork
            dat = new byte[datlen];
            if (input.Read(dat, 0, datlen) != datlen)
                throw new MacBinaryException("Could not read data fork.");
            // Round up to 128 bytes and skip padding
            long temp = (int)Math.Ceiling((double)datlen / 128) * 128 - datlen;
            input.Seek(temp, SeekOrigin.Current);

            // Read the resource fork
            temp = input.Position + (int)Math.Ceiling((double)reslen / 128) * 128;
            res = new MacResourceFork(input);
            input.Seek(temp, SeekOrigin.Begin);

            if (commentlen > 0)
            {
                buf = new byte[commentlen];
                if (input.Read(buf, 0, commentlen) != commentlen)
                    throw new MacBinaryException("Could not read Get Info comment.");
                comment = MacUtil.readString(buf, 0, commentlen);
            }
        }
    }

    // Class for disecting a RoboWar file stored in MacBinary
    public static class ClassicMBinRobot
    {
        private const String resPasswordType = "!@#$";
        private const int resPasswordID = 1000;

        private const String resCodeLengthType = "RLEN";
        private const int resCodeLengthID = 1000;

        private const String resRobotCodeType = "RCOD";
        private const int resRobotCodeID = 1000;

        private const String resIconType = "ICON";
        private const String resColorIconType = "CICN";
        private const int resIconMinID = 1000;

        private const String resSoundType = "snd ";
        private const int resSoundMinID = 2000;

        private const String resHardwareType = "HARD";
        private const int resHardwareID = 1000;

        private const String resTurretType = "TURT";
        private const int resTurretID = 1000;

        public static void read(RobotFile f, Stream s)
        {
            MacBinary mbin = new MacBinary(s);

            f.name = mbin.filename;

            if (mbin.res.Contains(resPasswordType) && mbin.res[resPasswordType].Contains(resPasswordID))
            {
                byte[] passbytes = mbin.res[resPasswordType][resPasswordID].data;
                f.password = MacUtil.readString(passbytes, 0, passbytes.Length);
                f.locked = true;
            }
            else
                f.locked = false;

            if (mbin.res.Contains(resCodeLengthType) && mbin.res[resCodeLengthType].Contains(resCodeLengthID) &&
                mbin.res.Contains(resRobotCodeType) && mbin.res[resRobotCodeType].Contains(resRobotCodeID))
            {
                //int codelen = Util.read16(mbin.res[resCodeLengthType][resCodeLengthID].data, 0);
                f.program = mbin.res[resRobotCodeType][resRobotCodeID].data;
            }
            
            if (mbin.res.Contains(resColorIconType))
            {
                MacResourceType t = mbin.res[resColorIconType];
                for (int i = 0; i < 10; i++)
                {
                    if (!t.Contains(resIconMinID + i))
                        continue;

                    byte[] iconbytes = t[resIconMinID + i].data;
                    // FIXME: set up icon
                }
            }
            if (mbin.res.Contains(resIconType))
            {
                MacResourceType t = mbin.res[resIconType];
                for (int i = 0; i < 10; i++)
                {
                    if (f.icons[i] != null || !t.Contains(resIconMinID + i))
                        continue;
                    
                    byte[] iconbytes = t[resIconMinID + i].data;
                    // FIXME: set up icon
                }
            }

            if (mbin.res.Contains(resSoundType))
            {
                MacResourceType t = mbin.res[resSoundType];
                for (int i = 0; i < 10; i++)
                {
                    if (!t.Contains(resSoundMinID + i))
                        continue;

                    byte[] soundbytes = t[resSoundMinID + i].data;
                    // FIXME: set up sound
                }
            }
            
            if (mbin.res.Contains(resHardwareType) && mbin.res[resHardwareType].Contains(resHardwareID))
            {
                byte[] hardbytes = mbin.res[resHardwareType][resHardwareID].data;
                if (hardbytes.Length >= 40)
                {
                    f.hardware.energyMax = MacUtil.read16(hardbytes, 0);
                    f.hardware.damageMax = MacUtil.read16(hardbytes, 2);
                    f.hardware.shieldMax = MacUtil.read16(hardbytes, 4);
                    f.hardware.processorSpeed = MacUtil.read16(hardbytes, 6);
                    int gun = MacUtil.read16(hardbytes, 8);
                    switch (gun)
                    {
                    case 1: f.hardware.gunType = BulletType.Rubber; break;
                    case 3: f.hardware.gunType = BulletType.Explosive; break;
                    case 2:
                    default: f.hardware.gunType = BulletType.Normal; break;
                    }
                    f.hardware.hasMissiles = MacUtil.read16(hardbytes, 10) > 0;
                    f.hardware.hasTacNukes = MacUtil.read16(hardbytes, 12) > 0;
                    // ignore advantages
                    f.hardware.hasLasers = MacUtil.read16(hardbytes, 16) > 0;
                    f.hardware.hasHellbores = MacUtil.read16(hardbytes, 18) > 0;
                    f.hardware.hasDrones = MacUtil.read16(hardbytes, 20) > 0;
                    f.hardware.hasMines = MacUtil.read16(hardbytes, 22) > 0;
                    f.hardware.hasStunners = MacUtil.read16(hardbytes, 24) > 0;
                    f.hardware.noNegEnergy = MacUtil.read16(hardbytes, 26) > 0;
                    f.hardware.probeFlag = MacUtil.read16(hardbytes, 28) > 0;
                    f.hardware.deathIconFlag = MacUtil.read16(hardbytes, 30) > 0;
                    f.hardware.collisionIconFlag = MacUtil.read16(hardbytes, 32) > 0;
                    f.hardware.shieldHitIconFlag = MacUtil.read16(hardbytes, 34) > 0;
                    f.hardware.hitIconFlag = MacUtil.read16(hardbytes, 36) > 0;
                    f.hardware.shieldOnIconFlag = MacUtil.read16(hardbytes, 38) > 0;
                }
            }

            if (mbin.res.Contains(resTurretType) && mbin.res[resTurretType].Contains(resTurretID))
            {
                byte[] turtbytes = mbin.res[resTurretType][resTurretID].data;
                if (turtbytes.Length >= 2)
                {
                    int turt = MacUtil.read16(turtbytes, 0);
                    switch (turt)
                    {
                        case 2: f.turretType = TurretType.Dot; break;
                        case 3: f.turretType = TurretType.None; break;
                        case 1:
                        default: f.turretType = TurretType.Line; break;
                    }
                }
            }
        }
        
        public static void write(RobotFile f, Stream s)
        {
            // FIXME
        }
    }
}
