using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quarto.Model
{
    public enum Height
    {
        Undefined,
        Half,
        Tall,
    }

    public static class HeightExpansions
    {
        private const int OFFSET = 4;

        public static int ToInt(this Height height)
        {
            switch(height)
            {
                case Height.Half:
                    return 1 << OFFSET;
                case Height.Tall:
                    return 2 << OFFSET;
            }
            return 0;
        }
    }
}
