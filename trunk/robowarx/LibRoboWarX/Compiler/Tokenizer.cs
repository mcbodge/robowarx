using System;
using System.IO;

namespace RoboWarX.Compiler
{
    public class Tokenizer
    {
        // Characters that do not start a token or comment.
        private const String startDelim = " \r\n\t;,";
        // Characters that end a line.
        private const String lineEnd = "\r\n";
        // Characters that end a token.
        // Not just delimiters, comments aswell.
        private const String endDelim = " \r\n\t;,#{";

        private TextReader input;

        public Tokenizer(TextReader input_)
        {
            input = input_;
        }

        public String next()
        {
            int r;
            Char c = '\0';

start:
            // Find the start of a token or comment.
            while ((r = input.Read()) != -1)
            {
                c = (Char)r;
                if (startDelim.IndexOf(c) < 0)
                    break;
            }
            if (r == -1)    // Stream end.
                return null;

            if (c == '#')   // Comment until line end.
            {
                // Use Peek() here, so anything following the line end
                // doesn't get eaten.
                while ((r = input.Peek()) != -1)
                {
                    c = (Char)r;
                    if (lineEnd.IndexOf(c) >= 0)
                        break;

                    // Gobble up stuff until the line end
                    input.Read();
                }
                if (r == -1)
                    return null;

                goto start;
            }
            else if (c == '{')  // Nested comment.
            {
                uint level = 1;
                while ((r = input.Read()) != -1)
                {
                    c = (Char)r;
                    if (c == '}')
                        level--;
                    else if (c == '{')
                        level++;

                    if (level == 0)
                        break;
                }
                if (r == -1)
                    return null;

                goto start;
            }
            else    // A token
            {
                String token = "" + c;
                // Use Peek() here, so comments directly following
                // a token get parsed.
                while ((r = input.Peek()) != -1)
                {
                    c = (Char)r;
                    if (endDelim.IndexOf(c) >= 0)
                        break;

                    // If it's definitely part of the token, Read() it.
                    token += (Char)input.Read();
                }
                if (token.Length > 0)
                    return token;
            }

            return null;
        }
    }
}
