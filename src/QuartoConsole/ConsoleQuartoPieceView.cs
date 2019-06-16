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
        private readonly Move m_move;
        public ConsoleQuartoPieceView(Placement<QuartoPiece, Move> placement) : this(placement?.Piece, placement?.Move)
        { }

        public ConsoleQuartoPieceView(QuartoPiece piece) : this(piece, null) { }
        public ConsoleQuartoPieceView(QuartoPiece piece, Move move)
        {
            Piece = piece;
            m_move = move;
            Width = Height = 2;
            IsWrapText = true;
            Text = toText(piece);
            ForegroundColor = piece.Color == Color.Dark ? ConsoleColor.DarkGray : ConsoleColor.White;
        }

        public QuartoPiece Piece { get; private set; }

        public int Column => m_move?.Location.X ?? -1;
        public int Row => m_move?.Location.Y ?? -1;
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
