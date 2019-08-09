using GameBase.Model;
using System;
using System.Collections.Generic;

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

    public abstract class AbstractPlayer
    {
        public AbstractPlayer(string name)
        {
            Name = name;
        }

        public string Name { get; private set; }
        public event EventHandler<ChooseEventArgs> Chosen;
        public QuartoPiece ChoosePiece(QuartoBoard board, IList<QuartoPiece> pieces)
        {
            var p = InternalChoosePiece(board, pieces);
            Chosen?.Invoke(this, new ChooseEventArgs(p));
            return p;
        }

        public virtual AbstractPlayer Prepare() { return this; }

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
            return Name;
        }

        public virtual void GameOver(GameResult result) { }
    }
}
