namespace Quarto.Model
{
    public class RandomPlayerFactory : IPlayerFactory
    {
        public virtual Player GetPlayer(int playerNumber)
        {
            return new RandomPlayer(playerNumber);
        }
    }
}
