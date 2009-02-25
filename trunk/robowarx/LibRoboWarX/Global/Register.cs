using System;
using RoboWarX;

namespace RoboWarX
{
    // Convenience base class for registers.
    public abstract class Register
    {
        // Naming
        public virtual Int16 code { get; protected set; }
        protected virtual string name { get; set; }
        public virtual String[] names { get { return new string[] { name }; } }
        
        // Upward accessor
        public Arena.Robot robot { get; internal set; }

        // Core functionality
        public abstract Int16 value { get; set; }
        
        // Interrupt processing
        public virtual Int16 param {
            get { return 0; }
            set
            {
                throw new RobotException(robot, "Illegal interrupt name");
            }
        }
        public virtual Int16 interrupt
        {
            get { return -1; }
            set
            {
                throw new RobotException(robot, "Illegal interrupt name");
            }
        }
        public virtual int order { get { return -1; } }
        public virtual bool checkInterrupt() { return false; }
    }
    
    public abstract class InterruptRegister : Register
    {
        // FIXME: mono doesn't like autoimpl property overrides
        private Int16 param_;
        public override Int16 param { get { return param_; } set { param_ = value;} }
        
        // FIXME: ditto
        private Int16 interrupt_;
        public override Int16 interrupt { get { return interrupt_; } set { interrupt_ = value;} }
    }
}
