using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quarto.Model
{
    public interface IPlayerFactory
    {
        Player GetPlayer(int playerNumber);
    }
}
