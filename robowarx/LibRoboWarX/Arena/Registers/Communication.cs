using System;
using RoboWarX;
using RoboWarX.Arena;

namespace RoboWarX.Arena.Registers
{
    // The robot’s broadcasting and receiving channel. May be read
    internal class ChannelRegister : Register
    {
        internal ChannelRegister() {
            name = "CHANNEL";
            code = (Int16)Bytecodes.REG_CHANNEL;
        }

        public override Int16 value
        {
            get { return (Int16) robot.channel; }
            set
            {
                if (value < 1 || value > 10)
                    throw new HardwareException(this.robot, "Invalid channel (1-10).");
                robot.channel = value;
            }
        }

        public override Object Clone() { return new ChannelRegister(); }
    }
    
    // The signal value on the robot’s current channel. May be read or written. If it is read, it
    // returns the last value broadcast over the channel by any robot on the same team. If it is
    // written, the value written is broadcast over the channel and may be read any time in the
    // future by any other robot on the same team. Typically signals and channels are used by two
    // or more robots to coordinate movement or team up against another set of robots.
    //
    // The signal interrupt is triggered when data is broadcast over the communication channels by
    // a robot's teammate. SETPARAM determines which channel number is being checked; by default it
    // is channel 0. It is generally a good idea for different robots to transmit on different
    // channels to prevent one robot from overwriting the data sent by the other. Also, since
    // multiple messages that are rapidly sent might be lost, it is generally wise for the RoboTalk
    // hacker to devise some protocol for teammates to acknowledge each other's transmissions.
    internal class SignalRegister : InterruptRegister
    {
        internal SignalRegister()
        {
            name = "SIGNAL";
            code = (Int16)Bytecodes.REG_SIGNAL;
            param = 0;
        }

        public override Int16 value
        {
            get { return robot.signals[robot.channel]; }
            set
            {
                if (robot.team == 0)
                    robot.signals[robot.channel] = value;
                else
                {
                    foreach (Robot other in robot.parent.robots)
                        if (other != null && other.alive && robot.team == other.team)
                            other.signals[robot.channel] = value;
                }
            }
        }
        
        public override int order { get { return 800; } }
        
        public override bool checkInterrupt() {
            // FIXME
            return false;
        }

        public override Object Clone() { return new SignalRegister(); }
    }
}