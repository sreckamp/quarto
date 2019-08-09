using GameBase.Model;
using Quarto.Model;
using System.Collections.Generic;
using System.Numerics;

namespace Quarto.Learning
{
    public class LearningPlayer : RandomPlayer
    {
        private readonly List<KeyValuePair<BigInteger, QuartoPiece>> m_choices = new List<KeyValuePair<BigInteger, QuartoPiece>>();
        private readonly List<KeyValuePair<BigInteger, Placement<QuartoPiece, Move>>> m_placements = new List<KeyValuePair<BigInteger, Placement<QuartoPiece, Move>>>();
        private readonly List<KeyValuePair<BigInteger, QuartoPiece>> m_opponentChoices = new List<KeyValuePair<BigInteger, QuartoPiece>>();
        private readonly List<KeyValuePair<BigInteger, Placement<QuartoPiece, Move>>> m_opponentPlacements = new List<KeyValuePair<BigInteger, Placement<QuartoPiece, Move>>>();
        private readonly LearningDataPool m_pool;
        public LearningPlayer(string name, string poolPath, int chooseDelay = 0, int placeDelay = 0) : base(name, chooseDelay, placeDelay)
        {
            m_pool = LearningDataPool.Get(poolPath);
        }

        private bool m_isTraining = false;

        protected override QuartoPiece InternalChoosePiece(QuartoBoard board, IList<QuartoPiece> pieces)
        {
            QuartoPiece choice = null;
            if (m_isTraining)
            {
                choice = base.InternalChoosePiece(board, getSafePieces(board, pieces) ?? pieces);
                storeChoice(board, pieces, choice);
            }
            else
            {
                choice = chooseBestPiece(board, pieces);
            }
            return choice;
        }

        private void storeChoice(QuartoBoard board, IList<QuartoPiece> pieces, QuartoPiece choice)
        {
            var state = QuartoBoardSnapshot.Calculate(board);
            m_choices.Add(new KeyValuePair<BigInteger, QuartoPiece>(state, choice));
        }

        private QuartoPiece chooseBestPiece(QuartoBoard board, IList<QuartoPiece> pieces)
        {
            return m_pool.ChooseBestPiece(board, getSafePieces(board, pieces) ?? pieces) ??
                base.InternalChoosePiece(board, pieces);
        }

        private IList<QuartoPiece> getSafePieces(QuartoBoard board, IList<QuartoPiece> pieces)
        {
            var safe = new List<QuartoPiece>();
            foreach (var p in pieces)
            {
                var isSafe = true;
                foreach (var loc in board.AvailableLocations)
                {
                    var m = new Move(loc);
                    if (board.WouldWin(p, m))
                    {
                        isSafe = false;
                        break;
                    }
                }
                if (isSafe) safe.Add(p);
            }
            return safe.Count > 0 ? safe : null;
        }

        protected override Move InternalGetPlacement(QuartoBoard board, QuartoPiece piece)
        {
            Move move = null;
            if (m_isTraining)
            {
                move = checkForKill(board, piece) ?? base.InternalGetPlacement(board, piece);
                storePlacement(board, piece, move);
            }
            else
            {
                move = getBestPlacement(board, piece);
            }
            return move;
        }

        private Move checkForKill(QuartoBoard board, QuartoPiece piece)
        {
            Move kill = null;
            if (board.Placements.Count >= 3)
            {
                foreach (var loc in board.AvailableLocations)
                {
                    var m = new Move(loc);
                    if (board.WouldWin(piece, m))
                    {
                        kill = m;
                        break;
                    }
                }
            }
            return kill;
        }

        private void storePlacement(QuartoBoard board, QuartoPiece piece, Move move)
        {
            var state = QuartoBoardSnapshot.Calculate(board);
            m_placements.Add(new KeyValuePair<BigInteger, Placement<QuartoPiece, Move>>(state, new Placement<QuartoPiece, Move>(piece, move)));
        }

        private Move getBestPlacement(QuartoBoard board, QuartoPiece piece)
        {
            var move = m_pool.GetBestPlacement(board, piece) ?? base.InternalGetPlacement(board, piece);
            return move;
        }

        public override void GameOver(GameResult result)
        {
            m_pool.UpdateChoiceStatistics(m_choices, result);
            m_pool.UpdatePlacementStatistics(m_placements, result);
        }

        public void Train(int gameCount)
        {
            var lGame = new Game();
            m_isTraining = true;
            for (int i = 0; i < gameCount; i++)
            {
                lGame.Play(this, this);
            }
            m_isTraining = false;
            m_pool.Write();
        }
    }
}
