using System;
using RoboWarX;
using RoboWarX.Arena;

namespace RoboWarX.Arena.Registers
{
    // Robot’s current damage rating. May only be read. When the battle begins, the damage rating
    // starts at the maximum value set at the Hardware Store. Damage caused by bullets, missiles,
    // and TacNukes that is not absorbed by the robot’s shields is removed from the damage rating.
    // When it reaches 0, the robot is dead.
    //
    // The damage interrupt is triggered whenever a robot takes damage. SETPARAM sets the minimum
    // threshold required for the damage interrupt to occur; by default it is set to 150. This is
    // useful if a robot should only change its behavior when it is damaged beyond a certain point.
    internal class DamageRegister : InterruptRegister
    {
        internal DamageRegister() {
            name = "DAMAGE";
            code = (Int16)Bytecodes.REG_DAMAGE;
            param = 150;
        }

        public override Int16 value
        {
            get { return (Int16)robot.damage; }
            internal set {}
        }
        
        public override int order { get { return 400; } }
        
        // Interrupt triggers when robot.damage on each impact when the resulting damage
        // is below the threshold set in the param.
        private int oldDamage;
        public override bool checkInterrupt() {
            bool retval = false;
            if (robot.damage != oldDamage && robot.damage < param)
                retval = true;

            oldDamage = robot.damage;
            
            return retval;
        }
    }
    
    // Robot’s current energy. May be read, but not written. ENERGY returns the amount of energy
    // the robot currently has. If not used for other purposes, energy is restored at 2 points per
    // chronon. However, if the energy ever drops below 0, the robot does not interpret any more
    // instructions or perform any more actions until the energy exceeds 0 again. When the battle
    // begins energy is set to the maximum energy value specified in the Hardware Store.
    internal class EnergyRegister : Register
    {
        internal EnergyRegister() {
            name = "ENERGY";
            code = (Int16)Bytecodes.REG_ENERGY;
        }

        public override Int16 value
        {
            get { return (Int16)robot.energy; }
            internal set {}
        }
    }
    
    // Robot's unique ID number. Each robot in the Arena has an ID from 0-5; it can be used to tell
    // robots apart.
    internal class IDRegister : Register
    {
        internal IDRegister() {
            name = "ID";
            code = (Int16)Bytecodes.REG_ID;
        }

        public override Int16 value
        {
            get { return (Int16)robot.number; }
            internal set {}
        }
    }

    // Robot’s current shield level. May be read or written. If read, it returns the current level
    // of the shield, or 0 if no shields are up. If written, is sets the shield level to the value
    // written. If the current level is less than the level written, a point of energy is used for
    // each point added to the shields. If not enough energy is available to set the shields, the
    // shields are only strengthened as far as remaining energy permits. If the current level is
    // greater than the level written, a point of energy is regained for each point of difference,
    // although energy cannot exceed the maximum energy value set in the Hardware Store. Shields
    // can absorb damage from bullets, missiles, or TacNukes that otherwise would have been
    // deducted from a robot’s damage score. Each point of damage that is done deducts one point
    // from the shield level, until no power is left in the shields. The remaining damage is then
    // done to a robot’s damage score. Even if shields are not hit, they decrease by one half point
    // each chronon from natural energy decay. Note that this replaces the old drain of one point
    // per chronon in previous versions of RoboWar. Shields may be charged above the maximum shield
    // value set in the Hardware Store (although they may never exceed 150), but if they are above
    // maximum, they decrease by two points instead of one half per chronon. Shields are set to 0
    // when the battle begins.
    //
    // The shield interrupt is triggered whenever a robot's shield transitions from above to below
    // a predetermined threshold. SETPARAM sets this threshold; by default it is 25.
    internal class ShieldRegister : InterruptRegister
    {
        internal ShieldRegister()
        {
            name = "SHIELD";
            code = (Int16)Bytecodes.REG_SHIELD;
            param = 25;
        }

        public override Int16 value
        {
            get { return (Int16)robot.shield; }
            internal set
            {
                if (value < 0)
                    throw new HardwareException(this.robot, "Cannot set shield below zero.");
                int v = Math.Min((int)value, 150);

                int old = robot.shield;
                if (robot.hardware.noNegEnergy && v > robot.energy + old)
                    v = robot.energy + old;
                robot.shield = v;
                robot.energy += Math.Min(old - v, robot.hardware.energyMax);
            }
        }
        
        public override int order { get { return 450; } }
        
        // Interrupt triggers when robot shield level drops below the threshold param.
        private int oldShield;
        public override bool checkInterrupt() {
            bool retval = false;
            if (oldShield >= param && robot.shield < param)
                retval = true;

            oldShield = robot.shield;
            
            return retval;
        }
    }
    
    // X position of robot. May range from 0 to 300 (the boundaries of the board). 0 is the left
    // side; 300 is the right. X may be read but may not be written (no unrestricted teleporting!).
    internal class XRegister : Register
    {
        internal XRegister() {
            name = "X";
            code = (Int16)Bytecodes.REG_X;
        }

        public override Int16 value
        {
            get { return (Int16)robot.x; }
            internal set {}
        }
    }
    
    // Y position of robot. May range from 0 to 300. 0 is the top; 300 is the bottom. Y may be read
    // but not written.
    internal class YRegister : Register
    {
        internal YRegister() {
            name = "Y";
            code = (Int16)Bytecodes.REG_Y;
        }

        public override Int16 value
        {
            get { return (Int16)robot.y; }
            internal set {}
        }
    }
}