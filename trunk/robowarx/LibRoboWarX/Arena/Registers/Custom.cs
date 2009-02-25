using System;
using RoboWarX;
using RoboWarX.Arena;

namespace RoboWarX.Arena.Registers
{
    // User-defined variables. They may be used for any temporary storage that the robot needs.
    // They may be read or written.
    internal class CustomRegister : Register
    {
        internal CustomRegister(char name)
        {
            this.name = name.ToString();
            this.code = (Int16) ((int)Bytecodes.REG_PRIV_MIN + (name - 'A'));
        }
        
        // FIXME: mono doesn't like autoimpl property overrides
        private Int16 value_;
        public override Int16 value { get { return value_; } set { value_ = value; } }
    }
}