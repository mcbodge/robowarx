using System;
using RoboWarX;
using RoboWarX.VM;

namespace RoboWarX.Arena
{
    // Convenience base class for registers.
    public abstract class Register : ITemplateRegister
    {
        // For IClonable
        public abstract Object Clone();

        // Naming
        public virtual Int16 code { get; protected set; }
        protected virtual string name { get; set; }
        public virtual String[] names { get { return new string[] { name }; } }
        
        // Upward accessors
        public Robot robot { get; internal set; }
        public Interpreter interp { get { return robot.interp; } }

        // Core functionality
        public abstract Int16 value { get; set; }
        public virtual Int16 param {
            get { return 0; }
            set
            {
                throw RobotException(robot, "Illegal interrupt name");
            }
        }
        public virtual Int16 interrupt
        {
            get { return -1; }
            set
            {
                throw RobotException(robot, "Illegal interrupt name");
            }
        }

        // Interrupt processing
        internal void fireInterrupt() {}
        public virtual bool checkInterrupt() { return false; }
        public virtual void updateInterruptState() {}
    }
    
    public abstract class InterruptRegister : Register
    {
        public override Int16 param { get; set; }
        public override Int16 interrupt { get; set; }
        internal void fireInterrupt() {}
        public virtual bool checkInterrupt() {}
        public virtual void updateInterruptState() {}
    }
}
