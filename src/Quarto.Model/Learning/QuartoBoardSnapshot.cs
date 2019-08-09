using Quarto.Model;
using System.Numerics;

namespace Quarto.Learning
{
    /// <summary>
    /// Model for state of game board (and what is left to choose)
    /// Need to implement a comparison based on rotating the board to reduce states.
    /// </summary>
    public class QuartoBoardSnapshot
    {
        public static BigInteger Calculate(QuartoBoard board)
        {
            var state = new byte[16];
            foreach (var p in board.Placements)
            {
                var tmp = p.Piece.IntValue;
                if (tmp == 0) continue;
                var idx = 0;
                for (int i = 0; i < 4; i++)
                {
                    idx <<= 1;
                    idx |= (tmp & 1);
                    tmp >>= 2;
                }
                byte val = (byte)(0x10 | p.Move.Location.X * 0x04 | p.Move.Location.Y);
                state[idx] = val;
            }
            return new BigInteger(state);
        }
    }
}
