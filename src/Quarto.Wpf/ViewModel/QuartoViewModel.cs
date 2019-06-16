using GameBase.Model;
using GameBoard.WPF.ViewModel;
using Quarto.Model;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Threading;
using System.Drawing;
using System.Windows.Input;
using GalaSoft.MvvmLight.CommandWpf;
using GameBase.WPF;
using GameBoard.Model;

namespace Quarto.WPF.ViewModel
{
    public class QuartoViewModel : INotifyPropertyChanged
    {
        private readonly object m_restartLock = new object();
        private readonly Game m_game;

        public QuartoViewModel()
        {
            m_game = new Game();
            m_game.StateChanged += m_game_StateChanged;
            m_game.Board.QuartoColumnChanged += board_QuartoColumnChanged;
            m_game.Board.QuartoDiagonalChanged += board_QuartoDiagonalChanged;
            m_game.Board.QuartoRowChanged += board_QuartoRowChanged;
            AvailablePieces = new DispatchedMappingCollection<QuartoPlacementViewModel, QuartoPiece>(m_game.Pieces);
            BoardPieces = new DispatchedMappingCollection<QuartoPlacementViewModel, Placement<QuartoPiece,Move>>(m_game.Board.Placements);
            FloatingPieces = new ObservableList<QuartoPlacementViewModel>();
        }

        private void board_QuartoRowChanged(object sender, ChangedValueArgs<int> e)
        {
            notifyPropertyChanged(nameof(QuartoRow));
            RowText = attributesToString(m_game.Board.RowQuartoType);
        }

        private void board_QuartoDiagonalChanged(object sender, ChangedValueArgs<int> e)
        {
            notifyPropertyChanged(nameof(QuartoDiagonal));
            DiagonalText = attributesToString(m_game.Board.DiagonalQuartoType);
        }

        private void board_QuartoColumnChanged(object sender, ChangedValueArgs<int> e)
        {
            notifyPropertyChanged(nameof(QuartoColumn));
            ColumnText = attributesToString(m_game.Board.ColumnQuartoType);
        }

        private string attributesToString(IList<object> attributes)
        {
            var sb = new StringBuilder();
            foreach (var a in attributes)
            {
                if (sb.Length > 0)
                {
                    sb.Append(" & ");
                }
                sb.Append(a);
            }
            return sb.ToString().ToUpper();
        }

        private void m_game_StateChanged(object sender, ChangedValueArgs<GameState> e)
        {
            if(e.OldVal != e.NewVal)
            {
                if(e.NewVal == GameState.Choose)
                {
                    ActionText = string.Format("{0}, Choose a piece for {1} to place.", m_game.ActivePlayer, m_game.OtherPlayer);
                }
                else if (e.NewVal == GameState.Place)
                {
                    ActionText = string.Format("{0}, Place the piece.", m_game.ActivePlayer);
                }
            }
            notifyPropertyChanged(nameof(GameState));
        }

        public IObservableList<QuartoPlacementViewModel> AvailablePieces { get; private set; }
        public IObservableList<QuartoPlacementViewModel> BoardPieces { get; private set; }
        public IObservableList<QuartoPlacementViewModel> FloatingPieces { get; private set; }
        public int QuartoColumn => m_game.Board.QuartoColumn;
        private string m_columnText = "";
        public string ColumnText
        {
            get => m_columnText;
            set
            {
                m_columnText = value;
                notifyPropertyChanged(nameof(ColumnText));
            }
        }
        public int QuartoDiagonal => m_game.Board.QuartoDiagonal;
        private string m_diagonalText = "";
        public string DiagonalText
        {
            get => m_diagonalText;
            set
            {
                m_diagonalText = value;
                notifyPropertyChanged(nameof(DiagonalText));
            }
        }
        public int QuartoRow => m_game.Board.QuartoRow;
        private string m_rowText = "";
        public string RowText
        {
            get => m_rowText;
            set
            {
                m_rowText = value;
                notifyPropertyChanged(nameof(RowText));
            }
        }

        public GameState GameState => m_game.State;

        private string m_actionText;
        public string ActionText
        {
            get => m_actionText;
            set { m_actionText = value; notifyPropertyChanged(nameof(ActionText)); }
        }

        private Dictionary<Player, PlayerViewModel> m_playerModels = new Dictionary<Player, PlayerViewModel>();
        public PlayerViewModel ActivePlayerModel
        {
            get
            {
                var ap = m_game.ActivePlayer;
                if (!m_playerModels.ContainsKey(ap))
                {
                    m_playerModels[ap] = new PlayerViewModel(ap);
                }
                return m_playerModels[ap];
            }
        }
        public QuartoPlacementViewModel ActivePiece
        {
            get
            {
                if (FloatingPieces.Count > 0)
                {
                    return FloatingPieces[0];
                }
                return null;
            }
            set
            {
                FloatingPieces.Clear();
                if (value != null)
                {
                    FloatingPieces.Add(value);
                }
                notifyPropertyChanged(nameof(ActivePiece));
            }
        }

        internal void Run()
        {
            while(true)
            {
                m_playerModels.Clear();
                var winner = m_game.Play();
                if(winner != null)
                {
                    ActionText = string.Format("{0} Wins!", winner);
                }
                else
                {
                    ActionText = "Tie Game!";
                }
                ActionText += "  Click anywhere to restart.";
                lock (m_restartLock)
                {
                    Monitor.Wait(m_restartLock);
                }
            }
        }

        public ICommand ChooseCommand => new RelayCommand<object>(choose, canChoose);

        private bool canChoose(object obj)
        {
            return (m_game.State == GameState.Choose);
        }

        private void choose(object obj)
        {
            ActivePlayerModel?.TriggerChoice(ActivePiece.Piece);
        }

        public ICommand PlaceCommand => new RelayCommand<object>(place, canPlace);
        private bool canPlace(object obj)
        {
            return (m_game.State == GameState.Place);
        }

        private void place(object obj)
        {
            ActivePlayerModel?.TriggerPlacement(ActivePiece?.Move);
        }

        public ICommand MoveCommand => new RelayCommand<GridCellRoutedEventArgs>(move, canMove);
        private bool canMove(GridCellRoutedEventArgs args)
        {
            return (m_game.State == GameState.Place);
        }

        private void move(GridCellRoutedEventArgs args)
        {
            if (m_game.State == GameState.Place && ActivePiece != null)
            {
                var m = new Move(args.Cell);
                ActivePiece.Move = m;
            }
        }

        public ICommand RestartCommand => new RelayCommand<GridCellRoutedEventArgs>(restart, canRestart);
        private bool canRestart(object obj)
        {
            return (m_game.State == GameState.Win);
        }

        private void restart(object obj)
        {
            lock (m_restartLock)
            {
                Monitor.PulseAll(m_restartLock);
            }
        }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        private void notifyPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        #endregion
    }
}
