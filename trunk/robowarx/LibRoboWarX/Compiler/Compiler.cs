using System;
using System.IO;
using System.Reflection;
using System.Collections.Generic;
using RoboWarX;

namespace RoboWarX.Compiler
{
    public class Compiler
    {
        private Tokenizer tokenizer;
        private Stream output;
        private Dictionary<String, ITemplateRegister> registers;

        public Compiler(TextReader input, Stream output)
        {
            tokenizer = new Tokenizer(input);
            this.output = output;
            registers = new Dictionary<String, ITemplateRegister>(67);
        }

        // Add the provided register to the registers attribute
        public void loadRegister(ITemplateRegister register)
        {
            String[] names;
            
            FieldInfo finfo = register.GetType().GetField("name");
            if (finfo == null)
            {
                finfo = register.GetType().GetField("names");
                if (finfo == null)
                    throw new ArgumentException("Register requires a name or names field");
                
                names = finfo.GetValue(register) as String[];
            }
            else
                names = new String[1] { finfo.GetValue(register) as String };
            
            foreach (String name in names)
            {
                if (registers.ContainsKey(name))
                    throw new ArgumentException("Register already loaded");

                registers.Add(name, register);
            }
        }

        // Load registers implemented by DLLs in the current directory
        public void loadDefaults()
        {
            Util.loadDefaults(loadRegister);
        }

        // Read text from input, compile, write binary to output
        public void compile()
        {
            // The program, in memory
            List<Int16> program = new List<Int16>(512);
            // Known labels
            Dictionary<String, Int16> labels = new Dictionary<String, Int16>();
            // Forward declarations to labels leave their program position here, so they can be
            // updated once the label is found
            Dictionary<String, List<Int16>> unresolved = new Dictionary<String, List<Int16>>();
            // Current token
            String token;

            while ((token = tokenizer.next()) != null)
            {
                token = token.ToUpper();

                // Reliable place to check for program length.
                if (program.Count >= Int16.MaxValue)
                    throw new CompilerException("Program exceeds maximum length");

                // Check for a number.
                if (isNumeric(token))
                {
                    try
                    {
                        Int16 val = Convert.ToInt16(token);
                        if (val < (Int16)Bytecodes.NUM_MIN_CODE || val > (Int16)Bytecodes.NUM_MAX_CODE)
                            throw new OverflowException();

                        program.Add(val);
                    }
                    catch (OverflowException)
                    {
                        throw new CompilerException("Number value out of range");
                    }

                    continue;
                }

                // Check for a label.
                if (token.Length > 1 && token[token.Length - 1] == ':')
                {
                    String label = token.Remove(token.Length - 1);
                    if (label.Length > 20)
                        throw new CompilerException("Label name exceeds maximum length");

                    labels[label] = (Int16)program.Count;

                    // Clean up forward references for this label
                    if (unresolved.ContainsKey(label))
                    {
                        List<Int16> fixups = unresolved[label];
                        foreach (Int16 fixup in fixups)
                            program[fixup] = (Int16)program.Count;
                        unresolved.Remove(label);
                    }

                    continue;
                }

                // Check for a register reference
                bool indirect = false;
                if (token.Length > 1 && token[token.Length - 1] == '\'')
                {
                    // Snip off the quote indicating a reference, but mark it as such
                    token = token.Remove(token.Length - 1);
                    indirect = true;
                }
                else
                {
                    bool found = true;
                    
                    // Check for operators
                    switch (token)
                    {
                        case "+": program.Add((Int16)Bytecodes.OP_PLUS); break;
                        case "-": program.Add((Int16)Bytecodes.OP_MINUS); break;
                        case "*": program.Add((Int16)Bytecodes.OP_TIMES); break;
                        case "/": program.Add((Int16)Bytecodes.OP_DIVIDE); break;
                        case "=": program.Add((Int16)Bytecodes.OP_EQUAL); break;
                        case "!": program.Add((Int16)Bytecodes.OP_NOTEQUAL); break;
                        case ">": program.Add((Int16)Bytecodes.OP_GREATER); break;
                        case "<": program.Add((Int16)Bytecodes.OP_LESS); break;
                        case "ABS": program.Add((Int16)Bytecodes.OP_ABS); break;
                        case "AND": program.Add((Int16)Bytecodes.OP_AND); break;
                        case "ARCCOS": program.Add((Int16)Bytecodes.OP_ARCCOS); break;
                        case "ARCSIN": program.Add((Int16)Bytecodes.OP_ARCSIN); break;
                        case "ARCTAN": program.Add((Int16)Bytecodes.OP_ARCTAN); break;
                        case "BEEP": program.Add((Int16)Bytecodes.OP_BEEP); break;
                        case "CALL": program.Add((Int16)Bytecodes.OP_CALL); break;
                        case "CHS": program.Add((Int16)Bytecodes.OP_CHS); break;
                        case "COS":
                        case "COSINE": program.Add((Int16)Bytecodes.OP_COS); break;
                        case "DEBUG":
                        case "DEBUGGER": program.Add((Int16)Bytecodes.OP_DEBUG); break;
                        case "DIST": program.Add((Int16)Bytecodes.OP_DIST); break;
                        case "DROP": program.Add((Int16)Bytecodes.OP_DROP); break;
                        case "DROPALL": program.Add((Int16)Bytecodes.OP_DROPALL); break;
                        case "DUP":
                        case "DUPLICATE": program.Add((Int16)Bytecodes.OP_DUP); break;
                        case "FLUSHINT": program.Add((Int16)Bytecodes.OP_FLUSHINT); break;
                        case "IF": program.Add((Int16)Bytecodes.OP_IF); break;
                        case "IFE": program.Add((Int16)Bytecodes.OP_IFE); break;
                        case "IFEG": program.Add((Int16)Bytecodes.OP_IFEG); break;
                        case "IFG": program.Add((Int16)Bytecodes.OP_IFG); break;
                        case "INTOFF": program.Add((Int16)Bytecodes.OP_INTOFF); break;
                        case "INTON": program.Add((Int16)Bytecodes.OP_INTON); break;
                        case "JUMP":
                        case "RETURN": program.Add((Int16)Bytecodes.OP_JUMP); break;
                        case "MAX": program.Add((Int16)Bytecodes.OP_MAX); break;
                        case "MIN": program.Add((Int16)Bytecodes.OP_MIN); break;
                        case "MOD": program.Add((Int16)Bytecodes.OP_MOD); break;
                        case "MRB": program.Add((Int16)Bytecodes.OP_MRB); break;
                        case "NOP": program.Add((Int16)Bytecodes.OP_NOP); break;
                        case "NOT": program.Add((Int16)Bytecodes.OP_NOT); break;
                        case "OR": program.Add((Int16)Bytecodes.OP_OR); break;
                        case "PRINT": program.Add((Int16)Bytecodes.OP_PRINT); break;
                        case "ROLL": program.Add((Int16)Bytecodes.OP_ROLL); break;
                        case "RTI": program.Add((Int16)Bytecodes.OP_RTI); break;
                        case "SETINT": program.Add((Int16)Bytecodes.OP_SETINT); break;
                        case "SETPARAM": program.Add((Int16)Bytecodes.OP_SETPARAM); break;
                        case "SIN":
                        case "SINE": program.Add((Int16)Bytecodes.OP_SIN); break;
                        // FIXME: implement StoWarnings directive
                        case "STO":
                        case "STORE": program.Add((Int16)Bytecodes.OP_STORE); break;
                        case "SQRT": program.Add((Int16)Bytecodes.OP_SQRT); break;
                        case "SWAP": program.Add((Int16)Bytecodes.OP_SWAP); break;
                        case "SYNC": program.Add((Int16)Bytecodes.OP_SYNC); break;
                        case "TAN":
                        case "TANGENT": program.Add((Int16)Bytecodes.OP_TAN); break;
                        case "VSTORE": program.Add((Int16)Bytecodes.OP_VSTORE); break;
                        case "VRECALL": program.Add((Int16)Bytecodes.OP_VRECALL); break;
                        case "XOR":
                        case "EOR": program.Add((Int16)Bytecodes.OP_EOR); break;
                        case "ICON0":
                        case "ICON1":
                        case "ICON2":
                        case "ICON3":
                        case "ICON4":
                        case "ICON5":
                        case "ICON6":
                        case "ICON7":
                        case "ICON8":
                        case "ICON9":
                            program.Add((Int16)((int)Bytecodes.OP_ICON_MIN + token[4] - '0'));
                            break;
                        case "SND0":
                        case "SND1":
                        case "SND2":
                        case "SND3":
                        case "SND4":
                        case "SND5":
                        case "SND6":
                        case "SND7":
                        case "SND8":
                        case "SND9":
                            program.Add((Int16)((int)Bytecodes.OP_SND_MIN + token[3] - '0'));
                            break;
                        default:
                            found = false;
                            break;
                    }
                    
                    if (found)
                        continue;
                }
                
                // Check for registers
                if (registers.ContainsKey(token))
                {
                    FieldInfo finfo = registers[token].GetType().GetField("code");
                    if (finfo == null)
                        throw new ArgumentException("Register requires a code field");
                    Int16 code = (Int16)finfo.GetValue(registers[token]);

                    program.Add(code);
                }
                // Check for register references
                else if (indirect)
                {
                    // Must be a register but didn't match anything before
                    throw new CompilerException("No such register: " + token);
                }
                else
                {
                    // Check for a label
                    if (labels.ContainsKey(token))
                        program.Add(labels[token]);
                    else
                    {
                        // Assume a forward reference to a label, resolve later
                        if (!unresolved.ContainsKey(token))
                            unresolved[token] = new List<Int16>();
                        unresolved[token].Add((Int16)program.Count);
                        program.Add((Int16)Bytecodes.INVALID_CODE);
                    }
                    continue;
                }
                
                // Add the register dereference op if necessary
                if (!indirect)
                    program.Add((Int16)Bytecodes.OP_RECALL);
            }

            // Finish up
            
            // Error on forward references to labels that didn't end up resolved
            if (unresolved.Count > 0)
                throw new CompilerException("Unresolved labels remaining after compilation: " + 
                                            unresolved.Keys.ToString());

            // Add program end op
            program.Add((Int16)Bytecodes.OP_END);

            // Final check for program length
            if (program.Count >= Int16.MaxValue)
                throw new CompilerException("Program exceeds maximum length");

            // Write to output, byteswapping if necessary
            foreach (Int16 op in program)
            {
                byte[] bytes = BitConverter.GetBytes(op);
                if (BitConverter.IsLittleEndian)
                    output.Write(new byte[] {bytes[1], bytes[0]}, 0, 2);
                else
                    output.Write(bytes, 0, 2);
            }
        }

        // This version of isNumeric adheres to RoboWar's syntax
        private static bool isNumeric(String token)
        {
            int idx = 0;

            if (token[0] == '+' || token[0] == '-')
            {
                if (token.Length == 1)
                    return false;
                idx++;
            }

            while (idx < token.Length)
            {
                if (!Char.IsDigit(token[idx]))
                    return false;
                idx++;
            }

            return true;
        }
    }
}
