using GameBase.Console;
using GameBase.Model;
using Quarto.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quarto.Console
{
    public class ConsoleQuartoPieceView : ConsoleTextBox
    {
        private readonly Placement<QuartoPiece, Move> m_placement;
        //private readonly Move m_move;

        public ConsoleQuartoPieceView(QuartoPiece piece) : this(new Placement<QuartoPiece,Move>(piece, null)) { }
        public ConsoleQuartoPieceView(Placement<QuartoPiece, Move> placement)
        {
            m_placement = placement;
            Width = Height = 2;
            IsWrapText = true;
            Text = toText(Piece);
            ForegroundColor = Piece?.Color == Color.Dark ? ConsoleColor.DarkGray : ConsoleColor.White;
        }

        public QuartoPiece Piece => m_placement?.Piece;

        public int Column => m_placement?.Move?.Location.X ?? -1;
        public int Row => m_placement?.Move?.Location.Y ?? -1;
        private string toText(QuartoPiece piece)
        {
            string text = "";
            if (piece.IntValue > 0)
            {
                foreach (var a in piece.Attributes)
                {
                    text += a.RawValue.ToString().Substring(0, 1);
                }
            }
            else
            {
                text = "    ";
            }
            return text;
        }
    }
}
