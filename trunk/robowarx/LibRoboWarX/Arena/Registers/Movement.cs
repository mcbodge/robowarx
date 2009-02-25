using System;
using RoboWarX;
using RoboWarX.Arena;

namespace RoboWarX.Arena.Registers
{
    // Used to move the robot a given distance in the X direction without changing SPEEDX. Returns
    // 0 if read, moves the robot the specified distance if written. Movement costs two points of
    // energy per unit moved. Note that you can’t fire a weapon of any type and move in the same
    // chronon.
    internal class MoveXRegister : Register
    {
        internal MoveXRegister()
        {
            name = "MOVEX";
            code = (Int16)Bytecodes.REG_MOVEX;
        }

        public override Int16 value
        {
            get { return 0; }
            set
            {
                int energy = robot.useEnergy(Math.Abs(value) * 2);
                double newx = robot.x + Math.Sign(value) * Math.Floor((float)energy / 2);
                newx = Math.Min(newx, Constants.ARENA_SIZE);
                newx = Math.Max(newx, 0);
                robot.x = newx;
            }
        }
    }

    // Used to move the robot a given distance in the Y direction without changing SPEEDY. MOVEY
    // has the same characteristics as MOVEX. Note that you can’t fire a weapon of any type and
    // move in the same chronon.
    internal class MoveYRegister : Register
    {
        internal MoveYRegister()
        {
            name = "MOVEY";
            code = (Int16)Bytecodes.REG_MOVEY;
        }

        public override Int16 value
        {
            get { return 0; }
            set
            {
                int energy = robot.useEnergy(Math.Abs(value) * 2);
                double newy = robot.y + Math.Sign(value) * Math.Floor((float)energy / 2);
                newy = Math.Min(newy, Constants.ARENA_SIZE);
                newy = Math.Max(newy, 0);
                robot.y = newy;
            }
        }
    }
    
    // Speed of robot in left-right direction. May be read or written. Positive speeds move right,
    // while negative speeds move to the left of the screen. If SPEEDX is read, it returns the
    // current velocity; if it is written, it sets the velocity. Speeds must be in the range of -20
    // to 20. Each point of change in speed costs 2 points of energy; thus going from 10 to -2
    // costs 24 energy.
    internal class SpeedXRegister : Register
    {
        internal SpeedXRegister()
        {
            name = "SPEEDX";
            code = (Int16)Bytecodes.REG_SPEEDX;
        }

        public override Int16 value
        {
            get { return (Int16) robot.speedx; }
            set
            {
                int old = value;
                if (value > 20) value = 20;
                else if (value < -20) value = -20;
                int cost = Math.Abs(value - old) << 1;
                if (robot.hardware.noNegEnergy && cost > robot.energy) {
                    int delta = robot.energy / 2;
                    if (value < old) robot.speedx -= delta;
                    else robot.speedx += delta;
                    robot.energy = 0;
                }
                else
                {
                    robot.speedx = value;
                    robot.useEnergy(cost);
                }
            }
        }
    }
    
    // Speed of robot in up-down direction. May be read or written. Positive values move down,
    // while negative values move up. SPEEDY has the same limits and characteristics as SPEEDX.
    internal class SpeedYRegister : Register
    {
        internal SpeedYRegister()
        {
            name = "SPEEDY";
            code = (Int16)Bytecodes.REG_SPEEDY;
        }

        public override Int16 value
        {
            get { return (Int16) robot.speedy; }
            set
            {
                int old = value;
                if (value > 20) value = 20;
                else if (value < -20) value = -20;
                int cost = Math.Abs(value - old) << 1;
                if (robot.hardware.noNegEnergy && cost > robot.energy) {
                    int delta = robot.energy / 2;
                    if (value < old) robot.speedy -= delta;
                    else robot.speedy += delta;
                    robot.energy = 0;
                }
                else
                {
                    robot.speedy = value;
                    robot.useEnergy(cost);
                }
            }
        }
    }
}