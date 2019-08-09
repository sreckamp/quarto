using System.Collections.Generic;
using GameBase.Model;

namespace Quarto.Model
{
    public class RandomPlayer : AbstractPlayer
    {
        static readonly System.Random s_rnd = new System.Random();
        private int m_chooseDelay;
        private int m_placeDelay;

        public RandomPlayer(string name, int chooseDelay = 0, int placeDelay = 0) : base(name)
        {
            m_chooseDelay = chooseDelay;
            m_placeDelay = placeDelay;
        }

        protected override QuartoPiece InternalChoosePiece(QuartoBoard board, IList<QuartoPiece> pieces)
        {
            if (pieces.Count < 16 && m_chooseDelay > 0)
            {
                System.Threading.Thread.Sleep(m_chooseDelay);
            }
            return pieces[s_rnd.Next(pieces.Count)];
        }

        protected override Move InternalGetPlacement(QuartoBoard board, QuartoPiece piece)
        {
            if (board.AvailableLocations.Count < 16 && m_placeDelay > 0)
            {
                System.Threading.Thread.Sleep(m_placeDelay);
            }
            var l = board.AvailableLocations[s_rnd.Next(board.AvailableLocations.Count)];
            return new Move(l);
        }
    }
}
