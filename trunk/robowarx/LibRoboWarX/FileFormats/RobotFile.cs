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

        // Draw a specific state (color index, icon, aim, position) of this robot
        // to the Graphics object
        public void draw(Graphics gfx, int x, int y, int num, int icon, int aim)
        {
            Brush color;
            switch (num) {
                case 0:
                    color = Brushes.Red;
                    break;
                case 1:
                    color = Brushes.Cyan;
                    break;
                case 2:
                    color = Brushes.Green;
                    break;
                case 3:
                    color = Brushes.Blue;
                    break;
                case 4:
                    color = Brushes.Magenta;
                    break;
                case 5:
                    color = Brushes.Yellow;
                    break;
                default:
                    color = Brushes.Black;
                    break;
            }

            gfx.FillEllipse(color, new Rectangle(x - 12, y - 12, 24, 24));

            gfx.DrawLine(Pens.Black, x, y,
                x + (int)((Constants.ROBOT_RADIUS - 1) * Math.Sin(aim * Constants.DEG_TO_RAD)),
                y - (int)((Constants.ROBOT_RADIUS - 1) * Math.Cos(aim * Constants.DEG_TO_RAD)));
        }
        
        
        // Convenience function to construct a RobotFile from a file
        public static RobotFile OpenFile(String filename)
        {
            RobotFile f = new RobotFile();
            FileStream s = new FileStream(filename, FileMode.Open, FileAccess.Read);

            // Sane default in case the file does not contain the robot name
            f.name = Path.GetFileNameWithoutExtension(filename);

            // Determine which file format class to use based on extension
            Type format;
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
