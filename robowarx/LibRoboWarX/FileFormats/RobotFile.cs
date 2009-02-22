using System;
using System.IO;
using System.Drawing;
using RoboWarX.Compiler;
using RoboWarX.Arena;

namespace RoboWarX.FileFormats
{
    // A robot's data in-memory. Tries to be storage agnostic.
    public class RobotFile
    {
        public String name;

        public String password;
        public bool locked;

        public byte[] program;
        public HardwareInfo hardware;
        public TurretType turretType;
        public Image[] icons;
        public int[] sounds;

        public String code;

        // Constructor adheres to RoboWar defaults
        public RobotFile()
        {
            name = "New Robot";

            password = null;
            locked = false;

            byte[] temp = BitConverter.GetBytes((Int16)Bytecodes.OP_END);
            if (BitConverter.IsLittleEndian)
                program = new byte[] { temp[1], temp[0] };
            else
                program = temp;
            hardware = new HardwareInfo();
            turretType = TurretType.Line;
            icons = new Image[10];
            sounds = new int[10];

            code = "";
        }

        // Convenience function that takes the code field and compiles to the program field
        public void compile()
        {
            MemoryStream output = new MemoryStream();
            Compiler.Compiler cc = new Compiler.Compiler(
                new StringReader(code), output);
            cc.loadDefaults();
            cc.compile();
            program = output.ToArray();
        }

        // Draw the robot's default icon.
        public void draw(Graphics gfx, int x, int y, int num, int icon, int? aim)
        {
            Image basicimage;
            switch (num) {
            default: basicimage = Resources.Robot.Basic1; break;
            case 1:  basicimage = Resources.Robot.Basic2; break;
            case 2:  basicimage = Resources.Robot.Basic3; break;
            case 3:  basicimage = Resources.Robot.Basic4; break;
            case 4:  basicimage = Resources.Robot.Basic5; break;
            case 5:  basicimage = Resources.Robot.Basic6; break;
            }
            gfx.DrawImage(basicimage, x - 16, y - 16);
            
            if (aim.HasValue)
            {
                double aim_ = (double)aim; // leave me alone, compiler!
                // FIXME: different turret types
                // FIXME: possibly invert color like the original?
                gfx.DrawLine(Pens.Black, x, y,
                    x + (int)((Constants.ROBOT_RADIUS - 1) * Math.Sin(aim_ * Constants.DEG_TO_RAD)),
                    y - (int)((Constants.ROBOT_RADIUS - 1) * Math.Cos(aim_ * Constants.DEG_TO_RAD)));
            }
        }
        
        
        // Convenience function to construct a RobotFile from a file
        public static RobotFile OpenFile(String filename)
        {
            RobotFile f = new RobotFile();
            FileStream s = new FileStream(filename, FileMode.Open, FileAccess.Read);

            // Sane default in case the file does not contain the robot name
            f.name = Path.GetFileNameWithoutExtension(filename);

            // Determine which file format class to use based on extension
            switch (System.IO.Path.GetExtension(filename).ToLower())
            {
            case ".bin":    ClassicMBinRobot.read(f, s);    break;
            case ".rwr":    WinRoboWar5.read(f, s);         break;
            case ".rtxt":   SourceTestLoader.read(f, s);    break;
            case ".rbin":   BinaryTestLoader.read(f, s);    break;
            default: throw new ArgumentException("Not a robot file.");    
            }
            
            return f;
        }
    }
}
