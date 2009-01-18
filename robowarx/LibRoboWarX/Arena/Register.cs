using System;
using RoboWarX;
using RoboWarX.VM;

namespace RoboWarX.Arena
{
    // Convenience base class for registers.
    public abstract class Register : ITemplateRegister
    {
        private Robot robot_;
        private Int16 interrupt_;

        // const String name;
        // const Int16 code;

        public abstract Int16 value
        {
            get;
            set;
        }

        public abstract Int16 param
        {
            set;
        }

        public Int16 interrupt
        {
            set
            {
                interrupt_ = value;
            }
        }

        protected Robot robot
        {
            get
            {
                return robot_;
            }
        }

        protected Interpreter interp
        {
            get
            {
                return robot_.interp;
            }
        }


        internal void assign(Robot robot__)
        {
            robot_ = robot__;
        }

        internal void fireInterrupt()
        {
        }

        public abstract Object Clone();
        public abstract bool checkInterrupt();
        public abstract void updateInterruptState();
    }
}
