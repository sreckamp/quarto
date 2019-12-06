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
    static class Program
    {
        public static void Main(string[] args)
        {
            var qb = new QuartoBoard();
            qb.Clear();
            var ia = qb.ToArray();
            //var cv = new ConsoleQuartoView(args.Length > 0 ? args[0] : null, args.Length > 1 ? args[1] : null);
            //var cw = new ConsoleWindow(cv);
            //cw.Run();
        }
    }

    internal static class BoardTools
    {
        public static int[] GetHashes(this QuartoBoard board)
        {
            var temp = board.ToArray();
            var res = new int[] { 0, 0, 0, 0 };
            for (var idx = 0; idx < temp.GetLength(0); idx++)
            {
                var negIdx = temp.GetLength(0) - 1 - idx;
//                res[0] = HashCode.Combine(res[0], temp[x,0]);
            }

            return res;
        }

        private static T[,] RotateCW<T>(this T[,] array)
        {
            var res = new T[array.GetLength(1), array.GetLength(0)];
            for(var x = 0; x < array.GetLength(0);x++)
            {
                for (var y = 0; y < array.GetLength(1); y++)
                {
                    res[array.GetLength(1) - 1 - y, x] = array[x, y];
                }

            }
            return res;
        }
    }
}
