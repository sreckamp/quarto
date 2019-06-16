using Quarto.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quarto.LearningPlayer
{
    public class LearningPlayerFactory : IPlayerFactory
    {
        public static LearningDataPool Pool = new LearningDataPool();
        public static bool IsLearning = true;
        public Player GetPlayer(int playerNumber)
        {
            return new LearningPlayer(playerNumber, IsLearning, Pool);
        }
    }
}
