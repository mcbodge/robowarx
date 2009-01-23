using System;
using RoboWarX;
using RoboWarX.VM;

namespace RoboWarX.Arena
{
    // Convenience base class for registers.
    public abstract class Register : ITemplateRegister
    {
        protected Robot robot { get; private set; }
        public Int16 interrupt { private get; set; }

        // const String name;
        // const Int16 code;

        public virtual String[] names
        {
            get
            {
                return new string[] { name };
            }
        }

        public virtual Int16 code { get; protected set; }



        protected virtual string name { get; set; }

        public abstract Int16 value
        {
            get;
            set;
        }

        public abstract Int16 param
        {
            set;
        }

        protected Interpreter interp
        {
            get
            {
                return robot.interp;
            }
        }


        internal void assign(Robot robot__)
        {
            robot = robot__;
        }

        internal void fireInterrupt()
        {
        }

        public abstract Object Clone();
        public abstract bool checkInterrupt();
        public abstract void updateInterruptState();
    }
}
