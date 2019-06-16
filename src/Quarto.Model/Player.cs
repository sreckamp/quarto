using GameBase.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quarto.Model
{
    public class ChooseEventArgs : EventArgs
    {
        public ChooseEventArgs(QuartoPiece piece)
        {
            ChosenPiece = piece;
        }

        public QuartoPiece ChosenPiece { get; private set; }
    }

    public class PlacementEventArgs : EventArgs
    {
        public PlacementEventArgs(Move move)
        {
            Move = move;
        }

        public Move Move { get; private set; }
    }

    public class Player
    {
        private readonly int m_playerNumber;
        public Player(int playerNumber)
        {
            m_playerNumber = playerNumber;
        }

        public event EventHandler<ChooseEventArgs> Chosen;
        public QuartoPiece ChoosePiece(QuartoBoard board, IList<QuartoPiece> pieces)
        {
            var p = InternalChoosePiece(board, pieces);
            Chosen?.Invoke(this, new ChooseEventArgs(p));
            return p;
        }

        public virtual void Prepare() { }

        protected virtual QuartoPiece InternalChoosePiece(QuartoBoard board, IList<QuartoPiece> pieces)
        {
            return null;
        }

        public event EventHandler<PlacementEventArgs> Placed;
        public Move GetPlacement(QuartoBoard board, QuartoPiece piece)
        {
            var m = InternalGetPlacement(board, piece);
            Placed?.Invoke(this, new PlacementEventArgs(m));
            return m;
        }
        protected virtual Move InternalGetPlacement(QuartoBoard board, QuartoPiece piece)
        {
            return null;
        }

        public override string ToString()
        {
            return string.Format("Player {0}", m_playerNumber);
        }

        public virtual void GameOver(GameResult result) { }
    }
}
