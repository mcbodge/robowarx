using System;

namespace RoboWarX
{
    // Should be Int16, but that's not accepted apparently.
    // Short is the same.
    public enum Bytecodes : short
    {
        INVALID_CODE = -20000,

        // ==== Numbers ==== //
        NUM_MIN_CODE = -19999,
        NUM_MAX_CODE = 19999,

        // ==== Operators ==== //
        OP_MIN_CODE = 20000,

        OP_PLUS = OP_MIN_CODE,
        OP_MINUS,
        OP_TIMES,
        OP_DIVIDE,
        OP_GREATER,
        OP_LESS,
        OP_EQUAL,
        OP_NOTEQUAL,

        OP_STORE = 20100,
        OP_DROP,
        OP_SWAP,
        OP_ROLL,
        OP_JUMP,
        OP_CALL,
        OP_DUP,
        OP_IF,
        OP_IFE,
        OP_RECALL,
        OP_END,
        OP_NOP,
        OP_AND,
        OP_OR,
        OP_EOR,
        OP_MOD,
        OP_BEEP,
        OP_CHS,
        OP_NOT,
        OP_ARCTAN,
        OP_ABS,
        OP_SIN,
        OP_COS,
        OP_TAN,
        OP_SQRT,

        OP_ICON_MIN,
        OP_ICON_MAX = OP_ICON_MIN + 9,

        OP_PRINT,
        OP_SYNC,
        OP_VSTORE,
        OP_VRECALL,
        OP_DIST,
        OP_IFG,
        OP_IFEG,
        OP_DEBUG,

        OP_SND_MIN,
        OP_SND_MAX = OP_SND_MIN + 9,

        OP_INTON,
        OP_INTOFF,
        OP_RTI,
        OP_SETINT,
        OP_SETPARAM,
        OP_MRB,
        OP_DROPALL,
        OP_FLUSHINT,
        OP_MAX,
        OP_MIN,
        OP_ARCCOS,
        OP_ARCSIN,

        OP_MAX_CODE = OP_ARCSIN,
        
        // ==== Registers ==== //
        REG_MIN_CODE = 20300,

        // Note: check for X or Y before private registers!
        REG_PRIV_MIN = REG_MIN_CODE,
        REG_X = 20323,
        REG_Y = 20324,
        REG_PRIV_MAX = REG_PRIV_MIN + 25,

        REG_FIRE,
        REG_ENERGY,
        REG_SHIELD,
        REG_RANGE,
        REG_AIM,
        REG_SPEEDX,
        REG_SPEEDY,
        REG_DAMAGE,
        REG_RANDOM,
        REG_MISSILE,
        REG_NUKE,
        REG_COLLISION,
        REG_CHANNEL,
        REG_SIGNAL,
        REG_MOVEX,
        REG_MOVEY,
        REG_JOCE,
        REG_RADAR,
        REG_LOOK,
        REG_SCAN,
        REG_CHRONON,
        REG_HELLBORE,
        REG_DRONE,
        REG_MINE,
        REG_LASER,
        REG_SUSIE,
        REG_ROBOTS,
        REG_FRIEND,
        REG_BULLET,
        REG_DOPPLER,
        REG_STUNNER,
        REG_TOP,
        REG_BOTTOM,
        REG_LEFT,
        REG_RIGHT,
        REG_WALL,
        REG_TEAMMATES,
        REG_PROBE,
        REG_HISTORY,
        REG_ID,
        REG_KILLS,

        REG_STOCK_MAX_CODE = REG_KILLS,

        // ==== Custom registers ==== //
        REG_CUSTOM_MIN = 21000
    }
}
