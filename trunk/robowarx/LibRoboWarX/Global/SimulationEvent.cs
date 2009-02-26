using System;

namespace RoboWarX
{
    public abstract class SimulationEvent {}
    
    public abstract class RobotEvent : SimulationEvent
    {
        public Arena.Robot robot { get; private set; }
        
        protected RobotEvent(Arena.Robot robot)
        {
            this.robot = robot;
        }
    }
    
    public enum SimulationEventType : int
    {
        ChrononEndEvent = 1,
        RobotFaultEvent = 2
    }
    
    
    public class ChrononEndEvent : SimulationEvent {}
    
    public class RobotFaultEvent : RobotEvent
    {
        public Exception exception { get; private set; }
        
        internal RobotFaultEvent(Arena.Robot robot, Exception exception) : base (robot)
        {
            this.exception = exception;
        }
    }
    
    public class InterruptEvent : RobotEvent
    {
        public Int16 code { get; private set; }
        
        internal InterruptEvent(Arena.Robot robot, Int16 code) : base (robot)
        {
            this.code = code;
        }
    }
    
    public class StepEvent : RobotEvent
    {
        internal StepEvent(Arena.Robot robot) : base (robot) {}
    }
}
