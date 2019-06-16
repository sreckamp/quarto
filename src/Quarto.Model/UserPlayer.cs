using Quarto.Model;
using System.Collections.Generic;
using GameBase.Model;
using System.Threading;

namespace Quarto.Model
{
    public class UserPlayer : Player
    {
        private readonly object m_chooseLock = new object();
        private readonly object m_placeLock = new object();

        public UserPlayer(int playerNumber) : base(playerNumber) { }

        protected override QuartoPiece InternalChoosePiece(QuartoBoard board, IList<QuartoPiece> pieces)
        {
            lock (m_chooseLock)
            {
                Monitor.Wait(m_chooseLock);
            }
            var pcs = m_activePiece;
            m_activePiece = null;
            return pcs;
        }

        private QuartoPiece m_activePiece;
        public void TriggerChoice(QuartoPiece piece)
        {
            m_activePiece = piece;
            lock (m_chooseLock)
            {
                Monitor.PulseAll(m_chooseLock);
            }
        }

        protected override Move InternalGetPlacement(QuartoBoard board, QuartoPiece piece)
        {
            var move = new Move(-1, -1);
            do
            {
                lock (m_placeLock)
                {
                    Monitor.Wait(m_placeLock);
                }
                move = m_activeMove;
            } while (!board.AvailableLocations.Contains(move.Location));
            m_activeMove = null;
            return move;
        }

        private Move m_activeMove;

        public void TriggerPlacement(Move move)
        {
            m_activeMove = move;
            lock (m_placeLock)
            {
                Monitor.PulseAll(m_placeLock);
            }
        }
    }
}
