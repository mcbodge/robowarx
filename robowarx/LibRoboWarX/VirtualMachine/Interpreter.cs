using System;
using System.IO;
using System.Reflection;
using System.Collections.Generic;
using RoboWarX;

namespace RoboWarX.VM
{
    public partial class Interpreter
    {
        // Complete map of bytecodes to registers
        private Dictionary<Int16, ITemplateRegister> registerMap;
        // Complete list of registers
        private List<ITemplateRegister> registerList;
        
        // Ordered list of interrupts, split at the 1000 threshold
        private SortedList<int, ITemplateRegister> interruptList;
        private SortedList<int, ITemplateRegister> lateInterruptList;
        // Queue of pending interrupts
        // We can't use the Queue type here, because even the queue is kept sorted according to
        // the interrupt's order.
        private SortedList<int, ITemplateRegister> interruptQueue;
        // Interrupts enabled?
        private bool interruptsEnabled;

        // The program
        internal List<Int16> program;
        // The program stack
        internal Stack<Int16> stack;
        // The program counter
        internal Int16 pc;
        
        // Vector storage
        private Int16[] vector;

        public Interpreter()
        {
            registerMap = new Dictionary<Int16, ITemplateRegister>(67);
            registerList = new List<ITemplateRegister>(67);
            interruptList = new SortedList<int,ITemplateRegister>(11);
            lateInterruptList = new SortedList<int,ITemplateRegister>(3);
            interruptQueue = new SortedList<int,ITemplateRegister>(11);
            interruptsEnabled = false;
            program = new List<Int16>(512);
            stack = new Stack<Int16>(100);
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
            Int16 code = register.code;

            if (registerMap.ContainsKey(code))
                throw new ArgumentException("Register already loaded.");

            registerMap.Add(code, register);
            registerList.Add(register);
            if (register.order != -1)
            {
                if (register.order < 1000)
                    interruptList.Add(register.order, register);
                else
                    lateInterruptList.Add(register.order, register);
            }
        }

        // Load registers implemented by DLLs in the current directory
        public void loadDefaults()
        {
            Util.loadDefaults(loadRegister);
        }

        public void processInterrupts()
        {
            // Lower than 1000 loop
            foreach (ITemplateRegister reg in interruptList.Values)
            {
                // Interrupt target set?
                if (reg.interrupt == -1) continue;
                // Interrupt already queued?
                if (interruptQueue.ContainsValue(reg)) continue;
                // Perform the actual check
                if (reg.checkInterrupt())
                    interruptQueue.Add(reg.order, reg);
            }
            
            // If an interrupt fires here, then late interrupts do not receive our love.
            if (checkPendingInterrupts())
                return;
            // Even with interrupts disabled, the above queue fills up.
            if (!interruptsEnabled)
                return;
            
            // 1000 or greater loop
            foreach (ITemplateRegister reg in lateInterruptList.Values)
            {
                // Interrupt target set?
                if (reg.interrupt == -1) continue;
                if (reg.checkInterrupt())
                {
                    fireInterrupt(reg);
                    return;
                }
            }
        }
        
        private bool checkPendingInterrupts()
        {
            if (!interruptsEnabled) return false;
            
            // Find the next register and dequeue it.
            while (true)
            {
                if (interruptQueue.Count == 0) return false;
                
                // Pop
                ITemplateRegister reg = interruptQueue.Values[0];
                interruptQueue.Remove(reg.order);
                
                // Skip if the target was unset
                if (reg.interrupt != -1)
                {
                    fireInterrupt(reg);
                    return true;
                }
            }
        }
        
        private void fireInterrupt(ITemplateRegister reg)
        {
            // The robot should clean up with RTI (or INTON).
            // These instructions will cause the next pending interrupt to dequeue
            interruptsEnabled = false;
            
            try
            {
                stack.Push(pc);
            }
            catch (StackOverflowException e)
            {
                throw new StackOverflowException("Interrupt caused stack overflow", e);
            }
            
            pc = reg.interrupt;
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
