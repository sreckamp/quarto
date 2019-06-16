using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quarto.Model
{
    public class QuartoAttribute
    {
        public QuartoAttributeType Type { get; protected set; }
        public int IntValue { get; protected set;  }
        public object RawValue { get; protected set; }
        public static QuartoAttributeType ValueTypeToQuartoAttributeType(Type type)
        {
            if (typeof(Color).IsAssignableFrom(type))
            {
                return QuartoAttributeType.Color;
            }
            if (typeof(Fill).IsAssignableFrom(type))
            {
                return QuartoAttributeType.Fill;
            }
            if (typeof(Height).IsAssignableFrom(type))
            {
                return QuartoAttributeType.Height;
            }
            if (typeof(Shape).IsAssignableFrom(type))
            {
                return QuartoAttributeType.Shape;
            }
            return QuartoAttributeType.Unknown;
        }

        internal static List<object> FromInt(int v)
        {
            var attributes = new List<object>();
            foreach (Shape s in Enum.GetValues(typeof(Shape)))
            {
                var val = new QuartoAttribute<Shape>(QuartoAttributeType.Shape, s).IntValue;
                if ((v & val) > 0)
                {
                    attributes.Add(s);
                }
            }
            foreach (Height h in Enum.GetValues(typeof(Height)))
            {
                var val = new QuartoAttribute<Height>(QuartoAttributeType.Height, h).IntValue;
                if ((v & val) > 0)
                {
                    attributes.Add(h);
                }
            }
            foreach (Fill f in Enum.GetValues(typeof(Fill)))
            {
                var val = new QuartoAttribute<Fill>(QuartoAttributeType.Fill, f).IntValue;
                if ((v & val) > 0)
                {
                    attributes.Add(f);
                }
            }
            foreach (Color c in Enum.GetValues(typeof(Color)))
            {
                var val = new QuartoAttribute<Color>(QuartoAttributeType.Color, c).IntValue;
                if ((v & val) > 0)
                {
                    attributes.Add(c);
                }
            }
            return attributes;
        }
    }

    public class QuartoAttribute<T> : QuartoAttribute where T : struct, IConvertible
    {
        public QuartoAttribute(QuartoAttributeType type, T value)
        {
            if (!typeof(T).IsEnum) throw new ArgumentException("T must be an enumerated type");
            Type = type;
            RawValue = value;
            IntValue = Convert.ToInt32(Value) << (2 * (int)Type);
        }

        public T Value => (T)RawValue;
        public override string ToString()
        {
            return Value.ToString();
        }

        public static implicit operator int(QuartoAttribute<T> a)
        {
            return (a == null) ? 0 : a.IntValue;
        }
    }
}