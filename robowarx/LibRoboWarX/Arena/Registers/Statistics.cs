using System;
using RoboWarX;
using RoboWarX.Arena;

namespace RoboWarX.Arena.Registers
{
    // Maintains a history of results and observations between battles. Each robot has 50 history
    // registers. The history register to read or write is selected with the setparam command (e.g.
    // 2 HISTORY' SETPARAM). The history registers are:
    //    1) Number of battles fought
    //    2) Kills made in previous battle
    //    3) Kills made in all battles
    //    4) Survival points in previous battle
    //    5) Survival points from all battles
    //    6) 1 if last battle timed out
    //    7) Teammates alive at end of last battle (excluding self)
    //    8) Teammates alive at end of all battle
    //    9) Damage at end of last battle (0 if dead)
    //    10) Chronons elapsed at end of last battle
    //    11) Chronons elapsed in all previous battles
    //    12-30) Reserved for future RoboWar versions
    //    31-50) User-defined.
    // When no battles have been fought yet, numbers default to zero. User-defined history
    // registers may be read or written and are preserved between rounds so robots can learn from
    // the results of previous rounds. All other history registers may only be read. Suggestions
    // for new history registers are welcome. History is zeroed anytime robots are added or deleted
    // in the arena (or between rounds in tournaments). It may also be erased or viewed with the
    // history commands under the Arena menu.
    // Warning: in very long tournaments, it is conceivable that the history register could
    // overflow 32767, the maximum integer in RoboWar. This is especially true for register 11.
    // Hacker beware!
    internal class HistoryRegister : Register
    {
        private int index;

        internal HistoryRegister()
        {
            index = 0;
            name = "HISTORY";
            code = (Int16)Bytecodes.REG_HISTORY;
        }

        public override Int16 value
        {
            get { return robot.history[index]; }
            internal set
            {
                if (index < 30)
                    throw new HardwareException(this.robot, "Store to reserved history register.");
                robot.history[index] = value;
            }
        }

        public override Int16 param
        {
            internal set
            {
                if (value <= 0 || value > robot.history.Length)
                    throw new HardwareException(this.robot, "Illegal history value.");
                index = value - 1;
            }
        }
    }
    
    // The number of kills the robot has made in this battle. A robot gets no credit for killing
    // itself or for crushing other robots during a collision.
    internal class KillsRegister : Register
    {
        internal KillsRegister() {
            name = "KILLS";
            code = (Int16)Bytecodes.REG_KILLS;
        }

        public override Int16 value
        {
            get { return (Int16)robot.kills; }
            internal set {}
        }
    }
}