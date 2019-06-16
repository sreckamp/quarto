using Quarto.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameBase.Model;
using System.Drawing;
using System.Numerics;

namespace Quarto.LearningPlayer
{
    public class LearningPlayer : RandomPlayer
    {
        private readonly bool m_isLearning;
        private readonly List<KeyValuePair<BigInteger, QuartoPiece>> m_choices = new List<KeyValuePair<BigInteger, QuartoPiece>>();
        private readonly List<KeyValuePair<BigInteger, Placement<QuartoPiece, Move>>> m_placements = new List<KeyValuePair<BigInteger, Placement<QuartoPiece, Move>>>();
        private readonly List<KeyValuePair<BigInteger, QuartoPiece>> m_opponentChoices = new List<KeyValuePair<BigInteger, QuartoPiece>>();
        private readonly List<KeyValuePair<BigInteger, Placement<QuartoPiece, Move>>> m_opponentPlacements = new List<KeyValuePair<BigInteger, Placement<QuartoPiece, Move>>>();
        private readonly LearningDataPool m_pool;
        private QuartoBoardSnapshot m_lastState;
        private QuartoPiece m_lastPiece;
        public LearningPlayer(int playerNumber, bool isLearning, LearningDataPool pool) : base(playerNumber)
        {
            m_pool = pool;
            m_isLearning = isLearning;
        }

        protected override QuartoPiece InternalChoosePiece(QuartoBoard board, IList<QuartoPiece> pieces)
        {
            QuartoPiece choice = null;
            if (m_isLearning)
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
//            m_lastPiece = choice;
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
            foreach(var p in pieces)
            {
                var isSafe = true;
                foreach (var loc in board.AvailableLocations)
                {
                    var m = new Move(loc);
                    if(board.WouldWin(p,m))
                    {
                        isSafe = false;
                        break;
                    }
                }
                if (isSafe) safe.Add(p);
            }
            return safe.Count > 0 ? safe:null;
        }

        protected override Move InternalGetPlacement(QuartoBoard board, QuartoPiece piece)
        {
            Move move = null;
            if (m_isLearning)
            {
                //var last = m_pool.GetBestPlacement(board, piece);
                move = checkForKill(board, piece) ?? base.InternalGetPlacement(board, piece);
                //if (last.X >= 0)
                //{
                //}
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
                    if (board.WouldWin(piece,m))
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
            // new KeyValuePair<QuartoPiece, QuartoPiece>(m_lastState, choice);
            //m_opponentChoices[state] = piece;
            //if (m_lastState != null && m_lastPiece != null)
            //{
            //    Move m = board.GetMove(m_lastPiece);
            //    m_opponentPlacements[m_lastState] = new KeyValuePair<QuartoPiece, Point>(m_lastPiece, m);
            //}
        }

        private Move getBestPlacement(QuartoBoard board, QuartoPiece piece)
        {
            var move = m_pool.GetBestPlacement(board, piece) ?? base.InternalGetPlacement(board, piece);
            return move;
        }

        public override void GameOver(GameResult result)
        {
            m_pool.UpdateChoiceStatistics(m_choices, result);
            //m_pool.UpdateChoiceStatistics(m_opponentChoices, result.Opposite());
            m_pool.UpdatePlacementStatistics(m_placements, result);
            //m_pool.UpdatePlacementStatistics(m_opponentPlacements, result.Opposite());
        }
    }
}
