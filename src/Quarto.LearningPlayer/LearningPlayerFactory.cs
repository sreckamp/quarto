using Quarto.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quarto.LearningPlayer
{
    public class LearningPlayerFactory : PlayerFactory
    {
        public static LearningDataPool Pool = new LearningDataPool();
        public Player GetPlayer(int playerNumber)
        {
            return new LearningPlayer(playerNumber, Pool);
        }
    }
}
