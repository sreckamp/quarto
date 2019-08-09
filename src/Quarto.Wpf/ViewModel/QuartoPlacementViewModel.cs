using Quarto.Model;
using GameBase.WPF.ViewModel;
using GameBase.Model;

namespace Quarto.WPF.ViewModel
{
    public class QuartoPlacementViewModel : AbstractPlacementViewModel<QuartoPiece,Move>
    {
        public QuartoPlacementViewModel(QuartoPiece piece) : this(new Placement<QuartoPiece, Move>(piece, null)) { }
        public QuartoPlacementViewModel(Placement<QuartoPiece,Move> placement) : base(placement) { }

        public QuartoPiece Piece => base.m_placement?.Piece as QuartoPiece;
        public Fill Fill => Piece?.Fill ?? Fill.Undefined;
        public Shape Shape => Piece?.Shape ?? Shape.Undefined;
        public Height Height => Piece?.Height ?? Height.Undefined;
        public Color Color => Piece?.Color ?? Color.Undefined;

        protected override Move GetMove(int locationX, int locationY)
        {
            return new Move(locationX, locationY);
        }
    }
}
