using System;

namespace RoboWarX.Compiler
{
    public class CompilerException : Exception
    {
        public CompilerException(string msg)
            : base(msg)
        {
        }
    }
}
