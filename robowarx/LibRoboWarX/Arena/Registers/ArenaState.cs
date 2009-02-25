using System;
using RoboWarX;
using RoboWarX.Arena;

namespace RoboWarX.Arena.Registers
{
    // Returns the number of chronons elapsed in the current battle. CHRONON may only be read.
    //
    // This interrupt is triggered at the start of each chronon, starting at the chronon set by the
    // parameter. By default, the parameter is set to 0. This interrupt is useful for animated
    // icons or to change behavior after a specific amount of time.
    internal class ChrononRegister : InterruptRegister
    {
        internal ChrononRegister()
        {
            name = "CHRONON";
            code = (Int16)Bytecodes.REG_CHRONON;
            param = 0;
        }

        public override Int16 value
        {
            get { return (Int16)robot.parent.chronon; }
            set {}
        }
        
        public override int order { get { return 1400; } }
        
        public override bool checkInterrupt() {
            return robot.parent.chronon >= param;
        }

        public override Object Clone() { return new ChrononRegister(); }
    }
    
    // Number of robots alive. Returns the number of robots alive in the arena, including the robot
    // itself.
    //
    // The ROBOTS interrupt is triggered whenever a robot is killed. The robot can specify for the
    // interrupt to only occur when the number (including yourself) is below a particular value by
    // using SETPARAM. For instance, to only interrupt when all other robots are dead, set the
    // parameter to 2. By default, the parameter is set to 6, causing an interrupt when any robot
    // dies.
    internal class RobotsRegister : InterruptRegister
    {
        internal RobotsRegister()
        {
            name = "ROBOTS";
            code = (Int16)Bytecodes.REG_ROBOTS;
            param = 6;
        }

        public override Int16 value
        {
            get
            {
                Int16 retval = 0;
                foreach (Robot r in robot.parent.robots)
                    if (r != null && r.alive)
                        retval++;
                return retval;
            }
            set {}
        }
        
        public override int order { get { return 750; } }
        
        // Interrupt triggers every time a robot is killed while below the threshold param.
        private int oldRobots;
        public override bool checkInterrupt() {
            Int16 temp = value;
            
            bool retval = false;
            if (oldRobots <= param && temp < oldRobots)
                retval = true;

            oldRobots = temp;
            
            return retval;
        }

        public override Object Clone() { return new RobotsRegister(); }
    }
    
    // Number of living teammates, not including self. May only be read.
    //
    // The TEAMMATES interrupt is triggered whenever the robot's teammate is killed. The robot can
    // specify for the interrupt to only occur when the number (excluding yourself) is below a
    // particular value by using SETPARAM. For instance, to only interrupt when all of your
    // teammates are dead, set the parameter to 1. By default, the parameter is set to 5, causing
    // an interrupt when any teammate dies.
    internal class TeamMatesRegister : InterruptRegister
    {
        internal TeamMatesRegister()
        {
            name = "TEAMMATES";
            code = (Int16)Bytecodes.REG_TEAMMATES;
            param = 5;
        }

        public override Int16 value
        {
            get
            {
                if (robot.team == 0)
                    return 0;
                Int16 retval = 0;
                foreach (Robot other in robot.parent.robots)
                    if (other != null && other != robot && other.alive &&
                            other.team == robot.team)
                        retval++;
                return retval;
            }
            set {}
        }
        
        public override int order { get { return 700; } }
        
        // Interrupt triggers every time a teammate is killed while below the threshold.
        private int oldTeamMates;
        public override bool checkInterrupt() {
            Int16 temp = value;
            
            bool retval = false;
            if (oldTeamMates <= param && temp < oldTeamMates)
                retval = true;

            oldTeamMates = temp;
            
            return retval;
        }
        
        public override Object Clone() { return new TeamMatesRegister(); }
    }
}