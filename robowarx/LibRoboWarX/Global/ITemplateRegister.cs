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
        string[] names
        {
            get;
        }

        Int16 code
        {
            get;
        }

        // Accessor used when the register is assigned or dereferenced
        Int16 value
        {
            get;
            set;
        }

        // Accessor for register parameter assignment
        Int16 param
        {
            get;
            set;
        }

        // Accessor used to assign the interrupt routine to the register
        Int16 interrupt
        {
            get;
            set;
        }
        
        // The index used for ordering interrupts. Typically in the range 0 to 1000, where lower
        // numbers have higher priority.
        // Interrupts ordered 1000 or higher are special, in that they never fire or queue
        // themselves as long as other interrupts are pending. (This is used for RADAR, RANGE and
        // CHRONON.)
        // A value of -1 is used for non-interrupt controllers.
        int order
        {
            get;
        }
        
        // Perform the interrupt check
        bool checkInterrupt();
    }
}
