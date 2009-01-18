using System;
using System.IO;
using System.Reflection;
using System.Collections.Generic;
using RoboWarX;

namespace RoboWarX.VM
{
    public partial class Interpreter
    {
        internal Stack<Int16> stack;
        private Dictionary<Int16, ITemplateRegister> registerMap;
        private List<ITemplateRegister> registerList;

        internal List<Int16> program;
        internal Int16 pc; // program counter
        
        private Int16[] vector; // vector storage in robowar

        public Interpreter()
        {
            stack = new Stack<Int16>(100);
            registerMap = new Dictionary<Int16, ITemplateRegister>(67);
            registerList = new List<ITemplateRegister>(67);
            program = new List<Int16>(512);
            pc = 0;
            vector = new Int16[101];
        }

        public Interpreter(Stream source)
            : this()
        {
            loadProgram(source);
        }

        // Load the program from the stream, byte swapping along the way
        public void loadProgram(Stream source)
        {
            if (program.Count > 0)
                throw new ArgumentException("Program already loaded.");

            Int16 op = (Int16)Bytecodes.INVALID_CODE;
            while (op != (Int16)Bytecodes.OP_END)
            {
                byte[] bytes = new byte[2];
                if (source.Read(bytes, 0, 2) != 2)
                    throw new ArgumentException("No END operator found.");
                
                if (BitConverter.IsLittleEndian)
                    op = BitConverter.ToInt16(new byte[] {bytes[1], bytes[0]}, 0);
                else
                    op = BitConverter.ToInt16(bytes, 0);
                
                program.Add(op);
                if (program.Count > (int)Bytecodes.NUM_MAX_CODE)
                    throw new OverflowException("Program exceeds maximum size.");
            }
        }

        // Add the provided register to the registerMap attribute
        public void loadRegister(ITemplateRegister register)
        {
            FieldInfo finfo = register.GetType().GetField("code");
            if (finfo == null)
                throw new RegisterExtensionException("Register requires a code field.");
            Int16 code = (Int16)finfo.GetValue(register);

            if (registerMap.ContainsKey(code))
                throw new ArgumentException("Register already loaded.");

            registerMap.Add(code, register);
            registerList.Add(register);
        }

        // Load registers implemented by DLLs in the current directory
        public void loadDefaults()
        {
            Util.loadDefaults(loadRegister);
        }

        public void processInterrupts()
        {
            // FIXME
        }
        
        // Execute a single operation, and return it's cost
        private int opcode(Int16 code)
        {
            if ((code >= (int)Bytecodes.NUM_MIN_CODE && code <= (int)Bytecodes.NUM_MAX_CODE) ||
                code >= (int)Bytecodes.REG_MIN_CODE)
            {
                stack.Push(code);
                return 1;
            }
            else if (code >= (int)Bytecodes.OP_MIN_CODE && code <= (int)Bytecodes.OP_MAX_CODE)
            {
                Bytecodes op = (Bytecodes)code;
                switch (op)
                {
                case Bytecodes.OP_PLUS: return doPlus();
                case Bytecodes.OP_MINUS: return doMinus();
                case Bytecodes.OP_TIMES: return doTimes();
                case Bytecodes.OP_DIVIDE: return doDivide();
                case Bytecodes.OP_GREATER: return doGreater();
                case Bytecodes.OP_LESS: return doLess();
                case Bytecodes.OP_EQUAL: return doEqual();
                case Bytecodes.OP_NOTEQUAL: return doNotEqual();
                case Bytecodes.OP_STORE: return doStore();
                case Bytecodes.OP_DROP: return doDrop();
                case Bytecodes.OP_SWAP: return doSwap();
                case Bytecodes.OP_ROLL: return doRoll();
                case Bytecodes.OP_JUMP: return doJump();
                case Bytecodes.OP_CALL: return doCall();
                case Bytecodes.OP_DUP: return doDuplicate();
                case Bytecodes.OP_IF: return doIf();
                case Bytecodes.OP_IFE: return doIfe();
                case Bytecodes.OP_RECALL: return doRecall();
                case Bytecodes.OP_END: throw new VMachineException(this, "End of code reached.");
                case Bytecodes.OP_NOP: return 1;
                case Bytecodes.OP_AND: return doAnd();
                case Bytecodes.OP_OR: return doOr();
                case Bytecodes.OP_EOR: return doEor();
                case Bytecodes.OP_MOD: return doMod();
                case Bytecodes.OP_BEEP: return doBeep();
                case Bytecodes.OP_CHS: return doChs();
                case Bytecodes.OP_NOT: return doNot();
                case Bytecodes.OP_ARCTAN: return doArcTan();
                case Bytecodes.OP_ABS: return doAbs();
                case Bytecodes.OP_SIN: return doSin();
                case Bytecodes.OP_COS: return doCos();
                case Bytecodes.OP_TAN: return doTan();
                case Bytecodes.OP_SQRT: return doSqrt();
                case Bytecodes.OP_SYNC: return int.MaxValue;
                case Bytecodes.OP_VSTORE: return doVStore();
                case Bytecodes.OP_VRECALL: return doVRecall();
                case Bytecodes.OP_DIST: return doDist();
                case Bytecodes.OP_IFG: return doIfg();
                case Bytecodes.OP_IFEG: return doIfeg();
                case Bytecodes.OP_DEBUG: return doDebug();
                case Bytecodes.OP_INTON: return doIntOn();
                case Bytecodes.OP_INTOFF: return doIntOff();
                case Bytecodes.OP_RTI: return doRti();
                case Bytecodes.OP_SETINT: return doSetInt();
                case Bytecodes.OP_SETPARAM: return doSetParam();
                case Bytecodes.OP_MRB: return doMrb();
                case Bytecodes.OP_DROPALL: return doDropAll();
                case Bytecodes.OP_FLUSHINT: return doFlushInt();
                case Bytecodes.OP_MIN: return doMin();
                case Bytecodes.OP_MAX: return doMax();
                case Bytecodes.OP_ARCCOS: return doArcCos();
                case Bytecodes.OP_ARCSIN: return doArcSin();
                default:
                    if (code >= (int)Bytecodes.OP_ICON_MIN &&
                            code <= (int)Bytecodes.OP_ICON_MAX)
                        return doIcon((Int16)(code - (int)Bytecodes.OP_ICON_MIN));
                    else if (code >= (int)Bytecodes.OP_SND_MIN &&
                            code <= (int)Bytecodes.OP_SND_MAX)
                        return doSnd((Int16)(code - (int)Bytecodes.OP_SND_MIN));
                    break;
                }
            }
            throw new VMachineException(this, "Unidentified instruction " + code.ToString() + ".");
        }

        // Single step, realling just wraps opcode with some checking
        public int step()
        {
            if (program.Count < 2)
                throw new VMachineException(this, "Robot not compiled.");
            else if (pc >= program.Count)
                throw new VMachineException(this, "Tried executing beyond end of program.");

            int cost = opcode(program[pc++]);
            
            if (stack.Count > 100)
                throw new VMachineException(this, "Stack overflow.");
            
            return cost;
        }
    }
}
