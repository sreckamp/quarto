using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quarto.Model
{
    public enum Color
    {
        Undefined,
        Dark,
        Light,
    }

    public static class ColorExpansions
    {
        private const int OFFSET = 0;

        public static int ToInt(this Color color)
        {
            switch(color)
            {
                case Color.Dark:
                    return 1 << OFFSET;
                case Color.Light:
                    return 2 << OFFSET;
            }
            return 0;
        }
    }
}
