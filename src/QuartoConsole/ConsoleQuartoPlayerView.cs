using GameBase.Model;
using Quarto.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quarto.Console
{
    public class ConsoleQuartoPlayerView
    {
        private readonly AbstractPlayer m_player;

        public ConsoleQuartoPlayerView(AbstractPlayer player)
        {
            m_player = player;
        }

        public void TriggerChoice(QuartoPiece piece)
        {
            if (m_player is UserPlayer up)
            {
                up.TriggerChoice(piece);
            }
        }

        public void TriggerPlacement(Move move)
        {
            if (m_player is UserPlayer up)
            {
                up.TriggerPlacement(move);
            }
        }
    }
}
