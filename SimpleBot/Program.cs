using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SimpleBot
{
    class Program
    {
        static void Main(string[] args)
        {
            SimpleBot b = new SimpleBot("irc.euirc.org", 6667);

            b.Run();
        }
    }
}
