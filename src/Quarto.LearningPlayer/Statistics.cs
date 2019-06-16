using GameBase.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quarto.LearningPlayer
{
    [Serializable]
    public class Statistics
    {
        public bool WinningMove = false;
        public int Wins;
        public float WinPercent => (float)Wins / (float)Total;
        public int Losses;
        public float LossPercent => (float)Losses / (float)Total;
        public int Draws;
        public float DrawPercent => (float)Draws / (float)Total;
        public int Total => Wins + Losses + Draws;


        public void AddResult(GameResult result, bool isWin)
        {
            switch (result)
            {
                case GameResult.Draw: Draws++; break;
                case GameResult.Win: Wins++; break;
                case GameResult.Lose: Losses++; break;
            }
            WinningMove = isWin;
        }

        public override string ToString()
        {
            return string.Format("[{0}-{1}-{2}]", Wins, Losses, Draws);
        }
    }
}
