using GameBase.Model.Rules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameBase.Model;

namespace Quarto.Model.Rules
{
    public class QuartoPlaceRule : IPlaceRule<QuartoPiece, Move>
    {
        public bool Applies(IGameBoard<QuartoPiece, Move> board, QuartoPiece piece, Move move)
        {
            return true;
        }

        public bool Fits(IGameBoard<QuartoPiece, Move> board, QuartoPiece piece, Move move)
        {
            return board.AvailableLocations.Contains(move.Location);
        }
    }
}
