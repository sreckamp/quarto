using System.Collections.Generic;
using GameBase.Model;

namespace Quarto.Model
{
    public class RandomPlayer : Player
    {
        static readonly System.Random s_rnd = new System.Random();

        public RandomPlayer(int playerNumber) : base(playerNumber)
        {
        }

        protected override QuartoPiece InternalChoosePiece(QuartoBoard board, IList<QuartoPiece> pieces)
        {
            return pieces[s_rnd.Next(pieces.Count)];
        }

        protected override Move InternalGetPlacement(QuartoBoard board, QuartoPiece piece)
        {
            var l = board.AvailableLocations[s_rnd.Next(board.AvailableLocations.Count)];
            return new Move(l);
        }
    }
}
