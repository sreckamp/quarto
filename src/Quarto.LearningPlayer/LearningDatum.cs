using GameBase.Model;
using Quarto.Model;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quarto.LearningPlayer
{
    [Serializable]
    public class LearningDatum
    {
        public readonly Dictionary<int, Statistics> ChoiceOptions = new Dictionary<int, Statistics>();
        public readonly Dictionary<int, Dictionary<Point, Statistics>> PlacementOptions = new Dictionary<int, Dictionary<Point, Statistics>>();
    }
}
