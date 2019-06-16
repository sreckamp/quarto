using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quarto.Model
{
    public enum Shape
    {
        Undefined,
        Round,
        Square,
    }

    public static class ShapeExpansions
    {
        private const int OFFSET = 6;

        public static int ToInt(this Shape shape)
        {
            switch(shape)
            {
                case Shape.Round:
                    return 1 << OFFSET;
                case Shape.Square:
                    return 2 << OFFSET;
            }
            return 0;
        }
    }
}
