using System;
using RoboWarX;
using RoboWarX.Arena;

namespace RoboWarX.Arena.Registers
{
    // This register is used exclusively for interrupts. It has no effect if written and returns 0
    // if read.
    //
    // The bottom interrupt is triggered whenever a robot moves too close to the bottom wall.
    // SETPARAM determines the y coordinate at which the interrupt is triggered; by default, it is
    // 280.
    internal class BottomRegister : InterruptRegister
    {
        public override string[] names { get { return new String[] { "BOT", "BOTTOM" }; } }
        internal BottomRegister() {
            code = (Int16)Bytecodes.REG_BOTTOM;
            param = 280;
        }

        public override Int16 value
        {
            get { return 0; }
            internal set {}
        }
        
        public override int order { get { return 550; } }
        
        // Interrupt triggers when the robot crosses the threshold distance from the bottom wall.
        private int oldY;
        public override bool checkInterrupt() {
            bool retval = false;
            if (robot.y >= param && oldY < param)
                retval = true;

            oldY = (int)robot.y;
            
            return retval;
        }
    }
    
    // This register is used exclusively for interrupts. It has no effect if written and returns 0
    // if read.
    //
    // The left interrupt is triggered whenever a robot moves too close to the left wall. SETPARAM
    // determines the x coordinate at which the interrupt is triggered; by default, it is 20.
    internal class LeftRegister : InterruptRegister
    {
        internal LeftRegister()
        {
            name = "LEFT";
            code = (Int16)Bytecodes.REG_LEFT;
            param = 20;
        }

        public override Int16 value
        {
            get { return 0; }
            internal set {}
        }
        
        public override int order { get { return 600; } }
        
        // Interrupt triggers when the robot crosses the threshold distance from the left wall.
        private int oldX;
        public override bool checkInterrupt() {
            bool retval = false;
            if (robot.x <= param && oldX > param)
                retval = true;

            oldX = (int)robot.x;
            
            return retval;
        }
    }
    
    // This register is used exclusively for interrupts. It has no effect if written and returns 0
    // if read.
    //
    // The right interrupt is triggered whenever a robot moves too close to the right wall.
    // SETPARAM determines the x coordinate at which theinterrupt is triggered; by default, it is
    // 280.
    internal class RightRegister : InterruptRegister
    {
        internal RightRegister()
        {
            name = "RIGHT";
            code = (Int16)Bytecodes.REG_RIGHT;
            param = 280;
        }

        public override Int16 value
        {
            get { return 0; }
            internal set {}
        }
        
        public override int order { get { return 650; } }
        
        // Interrupt triggers when the robot crosses the threshold distance from the right wall.
        private int oldX;
        public override bool checkInterrupt() {
            bool retval = false;
            if (robot.x >= param && oldX < param)
                retval = true;

            oldX = (int)robot.x;
            
            return retval;
        }
    }
    
    // This register is used exclusively for interrupts. It has no effect if written and returns 0
    // if read.
    //
    // The top interrupt is triggered whenever a robot moves too close to the top wall. SETPARAM
    // determines the y coordinate at which the interrupt is triggered; by default, it is 20.
    // Note that the directional interrupts are only triggered once when the robot crosses the
    // threshold.
    internal class TopRegister : InterruptRegister
    {
        internal TopRegister() {
            name = "TOP";
            code = (Int16)Bytecodes.REG_TOP;
            param = 20;
        }

        public override Int16 value
        {
            get { return 0; }
            internal set {}
        }
        
        public override int order { get { return 500; } }
        
        // Interrupt triggers when the robot crosses the threshold distance from the bottom wall.
        private int oldY;
        public override bool checkInterrupt() {
            bool retval = false;
            if (robot.y <= param && oldY > param)
                retval = true;

            oldY = (int)robot.y;
            
            return retval;
        }
    }

    // Is the robot touching the electrified walls? Returns 1 when read if the robot is touching
    // the wall, or 0 otherwise. No effect if written.
    //
    // Much like collision, but occurs when a robot runs into a wall. SETPARAM also has no effect.
    internal class WallRegister : InterruptRegister
    {
        internal WallRegister()
        {
            name = "WALL";
            code = (Int16)Bytecodes.REG_WALL;
        }

        public override Int16 value
        {
            get { return (Int16) (robot.wall ? 1 : 0); }
            internal set {}
        }
        
        public override int order { get { return 350; } }
        
        // Interrupt triggers when the robot crosses the threshold distance from the bottom wall.
        private bool oldWall;
        public override bool checkInterrupt() {
            bool newWall =
                robot.y < Constants.ROBOT_RADIUS ||
                robot.y > Constants.ARENA_SIZE - Constants.ROBOT_RADIUS ||
                robot.x < Constants.ROBOT_RADIUS ||
                robot.x > Constants.ARENA_SIZE - Constants.ROBOT_RADIUS;
            
            bool retval = false;
            if (!oldWall && newWall)
                retval = true;

            oldWall = newWall;
            
            return retval;
        }
    }
}