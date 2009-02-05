using System;
using System.Collections.Generic;
using RoboWarX;

namespace RoboWarX.VM
{
    // The RoboWar operators. These all operate on the stack, and return their cost.
    public partial class Interpreter
    {
        private int doPlus()
        {
            long val = stack.Pop() + stack.Pop();
            if (val < (int)Bytecodes.NUM_MIN_CODE &&
                val > (int)Bytecodes.NUM_MAX_CODE)
                throw new OutOfBoundsException(this);
            stack.Push((Int16)val);
            return 1;
        }

        private int doMinus()
        {
            Int16 top = stack.Pop();
            long val = stack.Pop() - top;
            if (val < (int)Bytecodes.NUM_MIN_CODE &&
                val > (int)Bytecodes.NUM_MAX_CODE)
                throw new OutOfBoundsException(this);
            stack.Push((Int16)val);
            return 1;
        }

        private int doTimes()
        {
            long val = stack.Pop() * stack.Pop();
            if (val < (int)Bytecodes.NUM_MIN_CODE &&
                val > (int)Bytecodes.NUM_MAX_CODE)
                throw new OutOfBoundsException(this);
            stack.Push((Int16)val);
            return 1;
        }

        private int doDivide()
        {
            Int16 top = stack.Pop();
            long val = stack.Pop() / top;
            if (val < (int)Bytecodes.NUM_MIN_CODE &&
                val > (int)Bytecodes.NUM_MAX_CODE)
                throw new OutOfBoundsException(this);
            stack.Push((Int16)val);
            return 1;
        }

        private int doGreater()
        {
            Int16 top = stack.Pop();
            if (stack.Pop() > top)
                stack.Push(1);
            else
                stack.Push(0);
            return 1;
        }

        private int doLess()
        {
            Int16 top = stack.Pop();
            if (stack.Pop() < top)
                stack.Push(1);
            else
                stack.Push(0);
            return 1;
        }

        private int doEqual()
        {
            if (stack.Pop() == stack.Pop())
                stack.Push(1);
            else
                stack.Push(0);
            return 1;
        }

        private int doNotEqual()
        {
            if (stack.Pop() != stack.Pop())
                stack.Push(1);
            else
                stack.Push(0);
            return 1;
        }

        private int doStore()
        {
            Int16 where = stack.Pop();
            if (where < (int)Bytecodes.REG_MIN_CODE)
                throw new VMachineException(this, "Invalid destination register.");
            ITemplateRegister whereobj = registerMap[where];
            whereobj.value = stack.Pop();
            return 1;
        }

        private int doDrop()
        {
            stack.Pop();
            return 1;
        }

        private int doSwap()
        {
            Int16 val1 = stack.Pop();
            Int16 val2 = stack.Pop();
            stack.Push(val1);
            stack.Push(val2);
            return 1;
        }

        private int doRoll()
        {
            Int16 places = stack.Pop();
            if (places < 0)
                throw new VMachineException(this, "Cannot roll back a negative amount of places.");

            Int16 val = stack.Pop();
            Stack<Int16> elements = new Stack<Int16>(places);

            for (Int16 i = places; i != 0; i--)
                elements.Push(stack.Pop());

            stack.Push(val);

            for (Int16 i = places; i != 0; i--)
                stack.Push(elements.Pop());
            return 1;
        }

        private int doJump()
        {
            Int16 where = stack.Pop();
            if (where < 0 || where >= program.Count)
                throw new VMachineException(this, "Jump destination not in program.");
            pc = where;
            return 1;
        }

        private int doCall()
        {
            Int16 where = stack.Pop();
            if (where < 0 || where >= program.Count)
                throw new VMachineException(this, "Jump destination not in program.");
            stack.Push(pc);
            pc = where;
            return 1;
        }

        private int doDuplicate()
        {
            stack.Push(stack.Peek());
            return 1;
        }

        private int doIf()
        {
            Int16 where = stack.Pop();

            if (stack.Pop() != 0)
            {
                if (where < 0 || where >= program.Count)
                    throw new VMachineException(this, "Jump destination not in program.");
                stack.Push(pc);
                pc = where;
            }
            return 1;
        }

        private int doIfe()
        {
            Int16 where1 = stack.Pop();
            Int16 where2 = stack.Pop();

            if (stack.Pop() != 0)
            {
                if (where2 < 0 || where2 >= program.Count)
                    throw new VMachineException(this, "Jump destination not in program.");
                stack.Push(pc);
                pc = where2;
            }
            else
            {
                if (where1 < 0 || where1 >= program.Count)
                    throw new ArgumentException("Jump destination not in program.");
                stack.Push(pc);
                pc = where1;
            }
            return 1;
        }

        private int doRecall()
        {
            Int16 what = stack.Pop();
            if (what < (int)Bytecodes.REG_MIN_CODE)
                throw new VMachineException(this, "Invalid register.");
            ITemplateRegister whatobj = registerMap[what];
            stack.Push(whatobj.value);
            return 1;
        }

        private int doAnd()
        {
            if ((stack.Pop() != 0) & (stack.Pop() != 0))
                stack.Push(1);
            else
                stack.Push(0);
            return 1;
        }

        private int doOr()
        {
            if ((stack.Pop() != 0) | (stack.Pop() != 0))
                stack.Push(1);
            else
                stack.Push(0);
            return 1;
        }

        private int doEor()
        {
            if ((stack.Pop() != 0) ^ (stack.Pop() != 0))
                stack.Push(1);
            else
                stack.Push(0);
            return 1;
        }

        private int doMod()
        {
            Int16 top = stack.Pop();
            long val = stack.Pop() % top;
            stack.Push((Int16)val);
            return 1;
        }

        private int doBeep()
        {
            /* FIXME
            if (gPrefs.soundFlag) SysBeep(1);
            */
            return 1;
        }

        private int doChs()
        {
            stack.Push((Int16) (stack.Pop() * -1));
            return 1;
        }

        private int doNot()
        {
            if (stack.Pop() == 0)
                stack.Push(1);
            else
                stack.Push(0);
            return 1;
        }

        private int doArcTan()
        {
            double y = stack.Pop();
            double x = stack.Pop();
            stack.Push((Int16) ((450.5 - Math.Atan2(y, x) * Constants.RAD_TO_DEG) % 360));
            return 1;
        }

        private int doAbs()
        {
            stack.Push(Math.Abs(stack.Pop()));
            return 1;
        }

        private int doSin()
        {
            Int16 hyp = stack.Pop();
            Int16 ang = stack.Pop();
            int sgn = 1;
            if (ang < 0)
            {
                ang *= -1;
                sgn = -1;
            }
            stack.Push((Int16)(sgn * hyp * Util.Sin((ang + 270) % 360)));
            return 1;
        }

        private int doCos()
        {
            Int16 hyp = stack.Pop();
            Int16 ang = stack.Pop();
            if (ang < 0)
                ang *= -1;
            stack.Push((Int16)(hyp * Util.Cos((ang + 270) % 360)));
            return 1;
        }

        private int doTan()
        {
            Int16 hyp = stack.Pop();
            Int16 ang = stack.Pop();
            int sgn = 1;
            if (ang < 0)
            {
                ang *= -1;
                sgn = -1;
            }
            stack.Push((Int16)(sgn * hyp * Util.Tan((ang + 270) % 360)));
            return 1;
        }

        private int doSqrt()
        {
            stack.Push((Int16)Math.Sqrt(stack.Pop()));
            return 1;
        }

        private int doIcon(short which)
        {
            /* FIXME
            who->icon = which;
            cycleNum--;
            */
            return 0;
        }

        /* FIXME
        private int doPrint()
        {
            Str255 msg,msg2;
            short itemHit;
            DialogPtr myDialog;
            ModalFilterUPP filterUPP;
            
            if (who->stackPtr < 1) robotError("Stack underflow",true);
            else {
                PtoCstr(who->name);
                
                // - Set message 2 first so we can use msg1 as a temp name with NumToString
                //strcpy( (char*)msg2, "Prints ");
                //NumToString( who->stack[who->stackPtr-1], msg);
                //strAppendP( (char*)msg2, msg);
                sprintf ((char*)msg2,"Prints %d",who->stack[who->stackPtr-1]);
                
                //strcpy( (char*)msg, "Robot ");
                //strAppendP( (char*)msg, who->name);
                //strAppend( (char*)msg, ":");
                sprintf ((char*)msg,"Robot %s:",who->name);
                CtoPstr((char*)who->name);
                
                CtoPstr((char*)msg);
                CtoPstr((char*)msg2);
                
                messageTime = TickCount();
                filterUPP = NewModalFilterProc(&timeOutFilter);
                myDialog = GetNewDialog(PrintDlogID,NULL,(WindowPtr)-1);
                ParamText(msg,msg2,noName,noName);
                installButtonOutline(myDialog,5);
                do {
                    if (isTournament) ModalDialog(filterUPP,&itemHit);
                    else ModalDialog(NULL,&itemHit);
                } while (itemHit > 2);
                if (itemHit == 2) {
                    isBattle = 0;
                    isTournament = 0;
                }
                DisposeDialog(myDialog);
                DisposeRoutineDescriptor(filterUPP);
                
                cycleNum--; // Prints top of stack in zero cycles, for debugging
            }
            return 1;
        }
        */

        private int doVStore()
        {
            Int16 where = stack.Pop();
            Int16 what = stack.Pop();
            if (where >= 0 && where <= 100)
                vector[where] = what;
            return 1;
        }

        private int doVRecall()
        {
            Int16 where = stack.Pop();
            if (where >= 0 && where <= 100)
                stack.Push(vector[where]);
            else
                stack.Push(0);
            return 1;
        }

        private int doDist()
        {
            Int16 a = stack.Pop();
            Int16 b = stack.Pop();
            stack.Push((Int16) Math.Sqrt(a * a + b * b));
            return 1;
        }

        private int doIfg()
        {
            Int16 where = stack.Pop();

            if (stack.Pop() != 0)
            {
                if (where < 0 || where >= program.Count)
                    throw new VMachineException(this, "Jump destination not in program.");
                pc = where;
            }
            return 1;
        }

        private int doIfeg()
        {
            Int16 where2 = stack.Pop();
            Int16 where1 = stack.Pop();

            if (stack.Pop() != 0)
            {
                if (where2 < 0 || where2 >= program.Count)
                    throw new VMachineException(this, "Jump destination not in program.");
                pc = where2;
            }
            else
            {
                if (where1 < 0 || where1 >= program.Count)
                    throw new VMachineException(this, "Jump destination not in program.");
                pc = where1;
            }
            return 1;
        }

        private int doDebug()
        {
            /* FIXME
            if (rob+useDebugger == who) {
                drawDebuggerInfo();
                pausedFlag = 1;
                cycleNum = who->hardware.processorSpeed; // syncronize
                setButtonsPaused();
            }
            else cycleNum--;
            */
            return int.MaxValue; // Pause to end of chronon
        }

        private int doIntOn()
        {
            interruptsEnabled = true;
            checkPendingInterrupts();
            return 1;
        }

        private int doIntOff()
        {
            interruptsEnabled = false;
            return 1;
        }

        private int doRti()
        {
            doJump();
            doIntOn();
            return 2;
        }

        private int doSetInt()
        {
            Int16 where = stack.Pop();
            if (where < (int)Bytecodes.REG_MIN_CODE)
                throw new VMachineException(this, "Illegal interrupt name.");
            ITemplateRegister whereobj = registerMap[where];
            Int16 target = stack.Pop();
            if (target < 0 || target >= program.Count)
                throw new VMachineException(this, "Interrupt destination not in program.");
            whereobj.interrupt = target;
            return 1;
        }

        private int doSetParam()
        {
            Int16 where = stack.Pop();
            if (where < (int)Bytecodes.REG_MIN_CODE)
                throw new VMachineException(this, "Illegal interrupt name.");
            ITemplateRegister whereobj = registerMap[where];
            whereobj.param = stack.Pop();
            return 1;
        }

        private int doFlushInt()
        {
            interruptQueue.Clear();
            return 1;
        }

        private int doSnd(short which)
        {
            /* FIXME
            playSound(which,who->number);
            cycleNum--;
            */
            return 0;
        }

        private int doMrb()
        {
            /* FIXME
            short itemHit;
            DialogPtr myDialog;
            ModalFilterUPP filterUPP;
            
            if (isTournament && officialFlag) robotError ("No undocumented commands in tournaments",true);
            else {
                myDialog = GetNewDialog(MRBDlogID, NULL, (WindowPtr)-1);
                installButtonOutline(myDialog,3);
                messageTime = TickCount();
                
                do {
                    filterUPP =  NewModalFilterProc(&timeOutFilter);
                    ModalDialog(filterUPP, &itemHit);
                    DisposeRoutineDescriptor(filterUPP);
                } while (itemHit != 1);
                
                DisposeDialog(myDialog);
            }
            */
            return 1;
        }

        private int doDropAll()
        {
            stack.Clear();
            return 1;
        }

        private int doMax()
        {
            Int16 a = stack.Pop();
            Int16 b = stack.Pop();
            stack.Push(Math.Max(a, b));
            return 1;
        }

        private int doMin()
        {
            Int16 a = stack.Pop();
            Int16 b = stack.Pop();
            stack.Push(Math.Min(a, b));
            return 1;
        }

        private int doArcCos()
        {
            double val = (double)stack.Pop() / (double)stack.Pop();
            if (val < -1 || val > 1)
                throw new VMachineException(this, "-1 ≤ Num / Denom ≤ 1 for arccos.");
            
            val = Math.Acos(val);
            stack.Push((Int16)(val * Constants.RAD_TO_DEG));
            return 1;
        }

        private int doArcSin()
        {
            double val = (double)stack.Pop() / (double)stack.Pop();
            if (val < -1 || val > 1)
                throw new VMachineException(this, "-1 ≤ Num / Denom ≤ 1 for arcsin.");
            
            val = Math.Asin(val);
            stack.Push((Int16)(val * Constants.RAD_TO_DEG));
            return 1;
        }
    }
}
