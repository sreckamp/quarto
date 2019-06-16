using GameBase.Model;
using Quarto.Model;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace Quarto.LearningPlayer
{
    [Serializable]
    public class LearningDataPool
    {
        public Dictionary<BigInteger, LearningDatum> LearnedData = new Dictionary<BigInteger, LearningDatum>();

        public void UpdateChoiceStatistics(List<KeyValuePair<BigInteger, QuartoPiece>> plays, GameResult result)
        {
            int idx = 0;
            foreach (var kvp in plays)
            {
                if (!LearnedData.ContainsKey(kvp.Key))
                {
                    LearnedData[kvp.Key] = new LearningDatum();
                }
                if(!LearnedData[kvp.Key].ChoiceOptions.ContainsKey(kvp.Value))
                {
                    LearnedData[kvp.Key].ChoiceOptions[kvp.Value] = new Statistics();
                }
                LearnedData[kvp.Key].ChoiceOptions[kvp.Value].AddResult(result, idx++ == plays.Count - 1 && result == GameResult.Lose);
            }
        }

        public QuartoPiece ChooseBestPiece(QuartoBoard board, IList<QuartoPiece> pieces)
        {
            var state = QuartoBoardSnapshot.Calculate(board);
            if (LearnedData.ContainsKey(state))
            {
                QuartoPiece choice = null;
                float best = 0;
                foreach (var kvp in LearnedData[state].ChoiceOptions)
                {
                    //Don't want to give them a winning piece
                    if (kvp.Value.WinningMove) continue;
                    if(kvp.Value.WinPercent > best)
                    {
                        best = kvp.Value.WinPercent;
                        choice = kvp.Key;
                    }
                }
                return choice;
            }
            return null;
        }

        public void UpdatePlacementStatistics(List<KeyValuePair<BigInteger, Placement<QuartoPiece, Move>>> plays, GameResult result)
        {
            int idx = 0;
            foreach (var kvp in plays)
            {
                if (!LearnedData.ContainsKey(kvp.Key))
                {
                    LearnedData[kvp.Key] = new LearningDatum();
                }
                if (!LearnedData[kvp.Key].PlacementOptions.ContainsKey(kvp.Value.Piece))
                {
                    LearnedData[kvp.Key].PlacementOptions[kvp.Value.Piece] = new Dictionary<Point, Statistics>();
                }
                if (!LearnedData[kvp.Key].PlacementOptions[kvp.Value.Piece].ContainsKey(kvp.Value.Move))
                {
                    LearnedData[kvp.Key].PlacementOptions[kvp.Value.Piece][kvp.Value.Move] = new Statistics();
                }
                LearnedData[kvp.Key].PlacementOptions[kvp.Value.Piece][kvp.Value.Move].AddResult(result, idx++ == plays.Count - 1 && result == GameResult.Win);
            }
        }

        public Move GetBestPlacement(QuartoBoard board, QuartoPiece piece)
        {
            Move m = null;
            var state = QuartoBoardSnapshot.Calculate(board);
            if (LearnedData.ContainsKey(state) && LearnedData[state].PlacementOptions.ContainsKey(piece))
            {
                float best = 0;
                foreach (var kvp in LearnedData[state].PlacementOptions[piece])
                {
                    if (kvp.Value.WinningMove)
                    {
                        m = kvp.Key;
                        break;
                    }
                    if (kvp.Value.WinPercent > best)
                    {
                        best = kvp.Value.WinPercent;
                        m = kvp.Key;
                    }
                }
            }
            return m;
        }

        public static LearningDataPool Read(string path)
        {
            var formatter = new BinaryFormatter();
            var stream = new FileStream(path, FileMode.Open, FileAccess.Read);
            var pool = formatter.Deserialize(stream) as LearningDataPool;
            stream.Close();
            return pool;
        }

        public static void Write(string path, LearningDataPool pool)
        {
            var formatter = new BinaryFormatter();
            var stream = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.None);
            formatter.Serialize(stream, pool);
            stream.Close();
        }
    }
}
