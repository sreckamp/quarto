using GameBase.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quarto.Model
{
    public class QuartoPiece : Piece, IEquatable<QuartoPiece>
    {
        public readonly QuartoAttribute[] Attributes = new QuartoAttribute[4];
        private readonly QuartoAttribute<Color> m_colorAttribute;
        private readonly QuartoAttribute<Shape> m_shapeAttribute;
        private readonly QuartoAttribute<Height> m_heightAttribute;
        private readonly QuartoAttribute<Fill> m_fillAttribute;

        public QuartoPiece(Color color = Color.Undefined, Fill fill = Fill.Undefined, Height height = Height.Undefined, Shape shape = Shape.Undefined)
        {
            IntValue = 0;
            Attributes[(int)QuartoAttributeType.Color] = m_colorAttribute = 
                    new QuartoAttribute<Color>(QuartoAttributeType.Color, color);
            IntValue += m_colorAttribute.IntValue;
            Attributes[(int)QuartoAttributeType.Fill] = m_fillAttribute =
                    new QuartoAttribute<Fill>(QuartoAttributeType.Fill, fill);
            IntValue += m_fillAttribute.IntValue;
            Attributes[(int)QuartoAttributeType.Height] = m_heightAttribute =
                    new QuartoAttribute<Height>(QuartoAttributeType.Height, height);
            IntValue += m_heightAttribute.IntValue;
            Attributes[(int)QuartoAttributeType.Shape] = m_shapeAttribute =
                    new QuartoAttribute<Shape>(QuartoAttributeType.Shape, shape);
            IntValue += m_shapeAttribute.IntValue;
        }

        public Color Color => m_colorAttribute.Value;
        public Fill Fill => m_fillAttribute.Value;
        public Height Height => m_heightAttribute.Value;
        public Shape Shape => m_shapeAttribute.Value;

        public int IntValue { get; private set; }

        public override string ToString()
        {
            var sb = new StringBuilder();
            foreach(var a in Attributes)
            {
                if(!a.RawValue.Equals(Color.Undefined)
                    && !a.RawValue.Equals(Fill.Undefined)
                    && !a.RawValue.Equals(Height.Undefined)
                    && !a.RawValue.Equals(Shape.Undefined))
                {
                    if(sb.Length > 0)
                    {
                        sb.Append(',');
                    }
                    sb.Append(a.RawValue);
                }
            }
            //if (Placement != null)
            //{
            //    if (sb.Length > 0)
            //    {
            //        sb.Append(':');
            //    }
            //    sb.Append(Placement.Location);
            //}
            return sb.ToString();
        }

        public bool IsAttribute<T>(T value) where T : struct, IConvertible
        {
            var type = QuartoAttribute.ValueTypeToQuartoAttributeType(value.GetType());
            foreach (var a in Attributes)
            {
                if (a.Type == type)
                {
                    return ((QuartoAttribute<T>)a).Value.Equals(value);
                }
            }
            return false;
        }

        public override int GetHashCode()
        {
            return IntValue.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as QuartoPiece);
        }

        public bool Equals(QuartoPiece other)
        {
            if (other == null) return false;
            return ((int)this == (int)other);
        }

        public static implicit operator int(QuartoPiece p)
        {
            return (p == null) ? 0 : p.IntValue;
        }

        public static implicit operator QuartoPiece(int i)
        {
            Color color = Color.Undefined;
            Fill fill = Fill.Undefined;
            Height height = Height.Undefined;
            Shape shape = Shape.Undefined;

            var vals = QuartoAttribute.FromInt(i);
            foreach (var a in vals)
            {
                if(a is QuartoAttribute<Color> ca)
                {
                    color = ca.Value;
                }
                else if (a is QuartoAttribute<Fill> fa)
                {
                    fill = fa.Value;
                }
                else if (a is QuartoAttribute<Height> ha)
                {
                    height = ha.Value;
                }
                else if (a is QuartoAttribute<Shape> sa)
                {
                    shape = sa.Value;
                }
            }
            return new QuartoPiece(color, fill, height, shape);
        }
    }
}
