using System;
using System.Collections.Generic;
using RoboWarX.Arena;
using RoboWarX.VM;

namespace RoboWarX
{
    /// Exception class for collecting multiple exceptions that happen during a single chronon.
    public class MultipleErrorsException : Exception
    {
        public readonly RobotException[] InnerExceptions;
        
        internal MultipleErrorsException(RobotException[] inner) :
            base("Multiple exceptions caught.")
        {
            InnerExceptions = inner;
        }
    }
    
    /// Exception class for illegal operations in the specific object state.
    public class StateException : Exception
    {
        internal StateException(String msg) : base(msg) {}
    }
    
    /// Exceptions class for unhandled errors during execution of extension objects.
    public class ArenaObjectExtensionException : Exception
    {
        internal ArenaObjectExtensionException(String msg) : base(msg) {}
    }
    
    /// Exceptions class for unhandled errors during execution of extension registers.
    public class RegisterExtensionException : Exception
    {
        internal RegisterExtensionException(String msg) : base(msg) {}
    }

    /// Exception class for all errors occurring in the Virtual Machine.
    public class VMachineException : Exception
    {
        private readonly Interpreter interp;


        internal VMachineException(Interpreter interp, String msg, Exception innerException)
            : base(msg, innerException)
        {
            this.interp = interp;
        }

        internal VMachineException(Interpreter interp, String msg) : base(msg)
        {
            this.interp = interp;
        }
        
        public String ProgramLocation
        {
            get
            {
                int pc = -1;
                if (interp != null)
                    pc = interp.pc - 1;
                
                if (pc < 0)
                    return "0 (program start)";
                if (pc >= interp.program.Count)
                    pc = interp.program.Count - 1;
                return pc.ToString() + " (" + ((Bytecodes)interp.program[pc]).ToString() + ")";
            }
        }
    }
    
    /// Exceptions for input and output values that are out of range.
    internal class OutOfBoundsException : VMachineException
    {
        public OutOfBoundsException(Interpreter interp) : base(interp, "Number out of bounds.") {}
    }
    
    /// Exception class for all exceptions that terminate a robot's execution.
    public class RobotException : VMachineException
    {
        public readonly Robot robot = null;

        internal RobotException(Robot robot, String reason, Exception innerException)
            : base(robot.interp, reason, innerException)
        {
        }

        internal RobotException(Robot robot, String reason) : base(robot.interp, reason)
        {
            this.robot = robot;
        }
        
        internal RobotException(Robot robot, VMachineException e) : this(robot, e.Message) {}
    }
    
    /// Exceptions class for hardware errors and illegal operations.
    public class HardwareException : RobotException
    {
        internal HardwareException(Robot robot, String msg) : base(robot, msg) {}
    }
}
