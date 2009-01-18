using System;

namespace RoboWarX
{
    // The interface implemented by all registers used in RoboWar.
    // These follow the prototype pattern.
    public interface ITemplateRegister : ICloneable
    {
        // Implementors should additionally define these.
        // const String name;
        // const Int16 code;

        // Accessor used when the register is assigned or dereferenced
        Int16 value
        {
            get;
            set;
        }

        // Accessor for register parameter assignment
        Int16 param
        {
            set;
        }

        // Accessor used to assign the interrupt routine to the register
        Int16 interrupt
        {
            set;
        }
    }
}
