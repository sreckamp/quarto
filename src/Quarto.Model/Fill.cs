using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quarto.Model
{
    public enum Fill
    {
        Undefined,
        Open,
        Filled,
    }

    public static class FillExpansions
    {
        private const int OFFSET = 2;

        public static int ToInt(this Fill fill)
        {
            switch (fill)
            {
                case Fill.Open:
                    return 1 << OFFSET;
                case Fill.Filled:
                    return 2 << OFFSET;
            }
            return 0;
        }
    }
}
