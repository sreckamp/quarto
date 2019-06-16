namespace Quarto.Model
{
    public class UserPlayerFactory : IPlayerFactory
    {
        public virtual Player GetPlayer(int playerNumber)
        {
            return new UserPlayer(playerNumber);
        }
    }
}
