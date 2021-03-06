using System;
using System.IO;
using System.Reflection;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using RoboWarX;

namespace RoboWarX.VM
{
    public partial class Interpreter : IRegisterBin
    {
        // Complete map of bytecodes to registers
        private Dictionary<Int16, Register> registerMap;
        // Complete list of registers
        private List<Register> registerList;
        public ReadOnlyCollection<Register> registers { get; private set; }
        
        // Ordered list of interrupts, split at the 1000 threshold
        private SortedList<int, Register> interruptList;
        private SortedList<int, Register> lateInterruptList;
        // Queue of pending interrupts
        // We can't use the Queue type here, because even the queue is kept sorted according to
        // the interrupt's order.
        private SortedList<int, Register> interruptQueue;
        // Interrupts enabled?
        public bool interruptsEnabled { get; private set; }

        // The program
        private List<Int16> program_;
        public ReadOnlyCollection<Int16> program { get; private set; }
        // The program stack
        private Stack<Int16> stack_;
        public IEnumerable<Int16> stack { get { return stack_; } }
        // The program counter
        public Int16 pc { get; private set; }
        
        // Vector storage
        private Int16[] vector;

        public Interpreter()
        {
            registerMap = new Dictionary<Int16, Register>(67);
            registerList = new List<Register>(67);
            registers = new ReadOnlyCollection<Register>(registerList);
            interruptList = new SortedList<int,Register>(11);
            lateInterruptList = new SortedList<int,Register>(3);
            interruptQueue = new SortedList<int,Register>(11);
            interruptsEnabled = false;
            program_ = new List<Int16>(512);
            program = new ReadOnlyCollection<Int16>(program_);
            stack_ = new Stack<Int16>(100);
            pc = 0;
            vector = new Int16[101];
        }

        public Interpreter(Stream source) : this()
        {
            loadProgram(source);
        }
        
        public void addRegister(Register register)
        {
            registerMap.Add(register.code, register);
            registerList.Add(register);
            if (register.order != -1)
            {
                if (register.order < 1000)
                    interruptList.Add(register.order, register);
                else
                    lateInterruptList.Add(register.order, register);
            }
        }

        // Load the program from the stream, byte swapping along the way
        public void loadProgram(Stream source)
        {
            if (program_.Count > 0)
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
                
                program_.Add(op);
                if (program_.Count > (int)Bytecodes.NUM_MAX_CODE)
                    throw new OverflowException("Program exceeds maximum size.");
            }
        }

        public Int16 processInterrupts()
        {
            // Lower than 1000 loop
            foreach (Register reg in interruptList.Values)
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
            Int16 retval = checkPendingInterrupts();
            if (retval != (Int16)Bytecodes.INVALID_CODE)
                return retval;
            // Even with interrupts disabled, the above queue fills up.
            if (!interruptsEnabled)
                return (Int16)Bytecodes.INVALID_CODE;
            
            // 1000 or greater loop
            foreach (Register reg in lateInterruptList.Values)
            {
                // Interrupt target set?
                if (reg.interrupt == -1) continue;
                if (reg.checkInterrupt())
                {
                    fireInterrupt(reg);
                    return reg.code;
                }
            }
            
            return (Int16)Bytecodes.INVALID_CODE;
        }
        
        private Int16 checkPendingInterrupts()
        {
            if (!interruptsEnabled)
                return (Int16)Bytecodes.INVALID_CODE;
            
            // Find the next register and dequeue it.
            while (true)
            {
                if (interruptQueue.Count == 0)
                    return (Int16)Bytecodes.INVALID_CODE;
                
                // Pop
                Register reg = interruptQueue.Values[0];
                interruptQueue.Remove(reg.order);
                
                // Skip if the target was unset
                if (reg.interrupt != -1)
                {
                    fireInterrupt(reg);
                    return reg.code;
                }
            }
        }
        
        private void fireInterrupt(Register reg)
        {
            // The robot should clean up with RTI (or INTON).
            // These instructions will cause the next pending interrupt to dequeue
            interruptsEnabled = false;
            
            try
            {
                stack_.Push(pc);
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
                stack_.Push(code);
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
            
            if (stack_.Count > 100)
                throw new VMachineException(this, "Stack overflow.");
            
            return cost;
        }
    }
}
