using GameBase.Model;
using Quarto.Model.Rules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace Quarto.Model
{
    public class QuartoBoard : GameBoard<QuartoPiece, Move>
    {
        private const int ROWS = 4;
        private const int COLUMNS = 4;

        public QuartoBoard() : base(new QuartoPlaceRule(), COLUMNS,ROWS)
        {
            RowQuartoType = new ObservableList<object>();
            ColumnQuartoType = new ObservableList<object>();
            DiagonalQuartoType = new ObservableList<object>();
        }

        public override void Clear()
        {
            base.Clear();
            QuartoRow = QuartoColumn = QuartoDiagonal = -1;
            RowQuartoType.Clear();
            ColumnQuartoType.Clear();
            DiagonalQuartoType.Clear();
            for (var y = 0; y < ROWS; y++)
            {
                for (var x = 0; x < COLUMNS; x++)
                {
                    var m = new Move(x, y);
                    var p = new Placement<QuartoPiece,Move>(new QuartoPiece(), m);
                    Add(p, false);
                }
            }
        }

        protected override void AddAvailableLocations(Placement<QuartoPiece, Move> p)
        {
            if((p?.Piece?.IntValue ?? -1) == 0)
            {
                AvailableLocations.Add(p.Move.Location);
            }
        }

        public bool WouldWin(QuartoPiece piece, Move move)
        {
            var rowCalcs = 0xFF;
            var colCalcs = 0xFF;
            var diagCalcs = 0xFF;

            for (var i = 0; i < 4; i++)
            {
                rowCalcs &= i == move.Location.Y ? piece : this[move.Location.X, i];
                colCalcs &= i == move.Location.X ? piece : this[i, move.Location.Y];
                if(move.Location.X == move.Location.Y)
                {
                    diagCalcs &= i == move.Location.X ? piece : this[i, i];
                }
                else if (move.Location.X == (3 - move.Location.Y))
                {
                    diagCalcs &= i == move.Location.X ? piece : this[i, 3 - i];
                }
                else
                {
                    diagCalcs = 0;
                }
                if ((rowCalcs | colCalcs | diagCalcs) == 0) return false;
            }
            return  true;
        }

        public bool IsWinning()
        {
            var calcs = new int[10] { 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF };
            for (var i = 0; i < 4; i++)
            {
                calcs[8] &= this[i, i];
                calcs[9] &= this[i, 3 - i];
                for (var j = 0; j < 4; j++)
                {
                    // Column
                    calcs[i] &= this[i, j];
                    // Row
                    calcs[4 + i] &= this[j, i];
                }
            }
            for (var i = 0; i < 4; i++)
            {
                if (calcs[i] > 0)
                {
                    ColumnQuartoType.AddRange(QuartoAttribute.FromInt(calcs[i]));
                    QuartoColumn = i;
                }
                if (calcs[4 + i] > 0)
                {
                    RowQuartoType.AddRange(QuartoAttribute.FromInt(calcs[4 + i]));
                    QuartoRow = i;
                }
                if (i < 2 && calcs[8 + i] > 0)
                {
                    DiagonalQuartoType.AddRange(QuartoAttribute.FromInt(calcs[8 + i]));
                    QuartoDiagonal = i;
                }
            }
            return QuartoColumn >= 0 || QuartoRow >= 0 || QuartoDiagonal >= 0;
        }

        public event EventHandler<ChangedValueArgs<int>> QuartoRowChanged;
        private int m_quartoRow = -1;
        public int QuartoRow
        {
            get => m_quartoRow;
            private set
            {
                var old = m_quartoRow;
                m_quartoRow = value;
                QuartoRowChanged?.Invoke(this, new ChangedValueArgs<int>(old, value));
            }
        }
        public ObservableList<object> RowQuartoType { get; private set; }

        public event EventHandler<ChangedValueArgs<int>> QuartoColumnChanged;
        private int m_quartoColumn = -1;
        public int QuartoColumn
        {
            get => m_quartoColumn;
            private set
            {
                var old = m_quartoColumn;
                m_quartoColumn = value;
                QuartoColumnChanged?.Invoke(this, new ChangedValueArgs<int>(old, value));
            }
        }
        public ObservableList<object> ColumnQuartoType { get; private set; }

        public event EventHandler<ChangedValueArgs<int>> QuartoDiagonalChanged;
        private int m_quartoDiagonal = -1;
        public int QuartoDiagonal
        {
            get => m_quartoDiagonal;
            private set
            {
                var old = m_quartoDiagonal;
                m_quartoDiagonal = value;
                QuartoDiagonalChanged?.Invoke(this, new ChangedValueArgs<int>(old, value));
            }
        }
        public ObservableList<object> DiagonalQuartoType { get; private set; }

        /// <summary>
        /// Returns the location where the piece is located.
        /// </summary>
        /// <param name="piece"></param>
        /// <returns></returns>
        public Move GetMove(QuartoPiece piece)
        {
            return (from p in Placements where p.Piece == piece select p.Move).FirstOrDefault();
        }

        public int[,] ToArray()
        {
            var res = new int[4, 4];
            for(var x = MinX; x < MaxX; x++)
            {
                for (var y = MinY; y < MaxY; y++)
                {
                    res[x, y] = this[x, y];
                }
            }
            return res;
        }
    }
}
