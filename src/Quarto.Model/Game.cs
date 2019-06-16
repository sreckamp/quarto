using GameBase.Model;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quarto.Model
{
    public enum GameState
    {
        Choose,
        Place,
        Lock,
        Win,
    }

    public class Game
    {
        private readonly IPlayerFactory m_playerFactory;
        private readonly Player[] m_players = new Player[2];
        public int ChooseDelay;
        public int PlaceDelay;

        public Game(IPlayerFactory playerFactory = null, int chooseDelay = 0, int placeDelay = 0)
        {
            m_playerFactory = playerFactory ?? new UserPlayerFactory();
            ChooseDelay = chooseDelay;
            PlaceDelay = placeDelay;
            Board = new QuartoBoard();
            Pieces = new ObservableList<QuartoPiece>();
        }

        public QuartoBoard Board { get; private set; }
        public ObservableList<QuartoPiece> Pieces { get; private set; }
        public event EventHandler<ChangedValueArgs<Player>> ActivePlayerChanged;
        public Player ActivePlayer => m_players[activeIdx];
        public Player OtherPlayer => m_players[(activeIdx + 1) % 2];

        private int m_activeIdx = 0;
        private int activeIdx
        {
            get => m_activeIdx;
            set
            {
                var old = ActivePlayer;
                m_activeIdx = value;
                ActivePlayerChanged?.Invoke(this, new ChangedValueArgs<Player>(old, ActivePlayer));
            }
        }

        public event EventHandler<ChangedValueArgs<GameState>> StateChanged;
        private GameState m_state;
        public GameState State
        {
            get { return m_state; }
            set
            {
                var old = m_state;
                m_state = value;
                StateChanged?.Invoke(this, new ChangedValueArgs<GameState>(old, value));
            }
        }

        public Player Play()
        {
            resetGame();
            activeIdx = 0;
            bool tie;
            do
            {
                if (Pieces.Count < 16 && !(ActivePlayer is UserPlayer) && PlaceDelay > 0)
                {
                    System.Threading.Thread.Sleep(PlaceDelay);
                }
                State = GameState.Choose;
                var pcs = ActivePlayer.ChoosePiece(Board, Pieces);
                State = GameState.Lock;
                if (!(ActivePlayer is UserPlayer) && ChooseDelay > 0)
                {
                    System.Threading.Thread.Sleep(ChooseDelay);
                }
                activeIdx = (activeIdx + 1) % 2;
                State = GameState.Place;
                var m = ActivePlayer.GetPlacement(Board, pcs);
                State = GameState.Lock;
                Board.Add(new Placement<QuartoPiece, Move>(pcs, m));
                Pieces.Remove(pcs);
            } while ((tie = !Board.IsWinning()) && Pieces.Count > 0);
            State = GameState.Win;
            if (tie)
            {
                ActivePlayer.GameOver(GameResult.Draw);
                OtherPlayer.GameOver(GameResult.Draw);
                return null;
            }
            else
            {
                ActivePlayer.GameOver(GameResult.Win);
                OtherPlayer.GameOver(GameResult.Lose);
                return ActivePlayer;
            }
        }

        private void resetGame()
        {
            Board.Clear();
            Pieces.Clear();
            foreach (Shape s in Enum.GetValues(typeof(Shape)))
            {
                if (s == Shape.Undefined) continue;
                foreach (Height h in Enum.GetValues(typeof(Height)))
                {
                    if (h == Height.Undefined) continue;
                    foreach (Fill f in Enum.GetValues(typeof(Fill)))
                    {
                        if (f == Fill.Undefined) continue;
                        foreach (Color c in Enum.GetValues(typeof(Color)))
                        {
                            if (c == Color.Undefined) continue;
                            var p = new QuartoPiece(c, f, h, s);
                            Pieces.Add(p);
                        }
                    }
                }
            }
            for(int i = 0; i < m_players.Length; i++)
            {
                m_players[i] = m_playerFactory.GetPlayer(i + 1);
                m_players[i].Prepare();
            }
        }
    }
}