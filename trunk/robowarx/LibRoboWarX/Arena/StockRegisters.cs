using System;
using RoboWarX;
using RoboWarX.Arena.Registers;
using RoboWarX.Arena.Weapons;

namespace RoboWarX.Arena
{
    public static class StockRegisters
    {
        public static Register[] instantiate()
        {
            Register[] retval = new Register[61];
			
            for (char c = 'A'; c <= 'Z'; c++)
            {
                if (c == 'X' || c == 'Y')
                    continue;
                retval[c - 'A'] = new CustomRegister(c);
            }
            retval[23] = new XRegister();
            retval[24] = new YRegister();
			
            retval[26] = new AimRegister();
            retval[27] = new BottomRegister();
            retval[28] = new ChannelRegister();
            retval[29] = new ChrononRegister();
            retval[30] = new CollisionRegister();
            retval[31] = new DamageRegister();
            retval[32] = new DopplerRegister();
            retval[33] = new EnergyRegister();
            retval[34] = new FriendRegister();
            retval[35] = new HistoryRegister();
            retval[36] = new IDRegister();
            retval[37] = new KillsRegister();
            retval[38] = new LeftRegister();
            retval[39] = new LookRegister();
            retval[40] = new MoveXRegister();
            retval[41] = new MoveYRegister();
            retval[42] = new ProbeRegister();
            retval[43] = new RadarRegister();
            retval[44] = new RandomRegister();
            retval[45] = new RangeRegister();
            retval[46] = new RightRegister();
            retval[47] = new RobotsRegister();
            retval[48] = new ScanRegister();
            retval[49] = new ShieldRegister();
            retval[50] = new SignalRegister();
            retval[51] = new SpeedXRegister();
            retval[52] = new SpeedYRegister();
            retval[53] = new TeamMatesRegister();
            retval[54] = new TopRegister();
            retval[55] = new WallRegister();
			
			retval[56] = new BulletRegister();
			retval[57] = new FireRegister();
			retval[58] = new MissileRegister();
			retval[59] = new HellboreRegister();
			retval[60] = new StunnerRegister();
			
            return retval;
        }
		
		public static void inject(IRegisterBin bin)
		{
			Register[] defaultRegisters = instantiate();
			foreach (Register register in defaultRegisters)
				bin.addRegister(register);
		}
    }
}
