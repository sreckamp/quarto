using GameBase.Console;
using GameBase.Model;
using Quarto.Model;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quarto.Console
{
    class Program
    {
        public static void Main(string[] args)
        {
            var cv = new ConsoleQuartoView(args.Length > 0 ? args[0] : null, args.Length > 1 ? args[1] : null);
            var cw = new ConsoleWindow(cv);
            cw.Run();
        }
    }
}
