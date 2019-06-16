using Quarto.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameBase.Model;
using System.Threading;
using System.Windows.Input;
using GalaSoft.MvvmLight.CommandWpf;

namespace Quarto.WPF.ViewModel
{
    public class PlayerViewModel
    {
        private readonly Player m_player;

        public PlayerViewModel(Player player)
        {
            m_player = player;
        }

        public void TriggerChoice(QuartoPiece piece)
        {
            if(m_player is UserPlayer up)
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
