using GameBase.Console;
using GameBase.Model;
using Quarto.Learning;
using Quarto.Model;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Threading;
using SConsole = System.Console;

namespace Quarto.Console
{
    public class ConsoleQuartoView:ConsoleFlowContainer, IRunnable
    {
        private readonly Game m_game;
        private readonly MappingCollection<ConsoleQuartoPieceView, QuartoPiece> m_pieces;
        private readonly MappingCollection<ConsoleQuartoPieceView, Placement<QuartoPiece,Move>> m_placements;
        private readonly ConsoleListBox<ConsoleQuartoPieceView> m_pieceBox;
        private readonly ConsoleGrid<ConsoleQuartoPieceView> m_board;
        private readonly ConsoleTextBox m_details;
        private readonly ConsoleTextBox m_text;
        private readonly string m_player1Name;
        private readonly string m_player2Name;
        private readonly bool m_train;
        private readonly object m_lineEnteredLock = new object();

        public ConsoleQuartoView(string player1Name, string player2Name, bool train = false)
        {
            m_train = train;
            m_player1Name = player1Name ?? "Player 1";
            m_player2Name = player2Name ?? "Player 2";
            SConsole.OutputEncoding = Encoding.Unicode;
            SConsole.CursorVisible = false;
            m_game = new Game();
            m_game.StateChanged += gameStateChanged;
            m_game.ActivePlayerChanged += activePlayerChanged;
            m_game.Board.QuartoRowChanged += quartoChanged;
            m_game.Board.QuartoColumnChanged += quartoChanged;
            m_game.Board.QuartoDiagonalChanged += quartoChanged;
            m_pieces = new MappingCollection<ConsoleQuartoPieceView, QuartoPiece>(m_game.Pieces);
            m_pieceBox = new ConsoleListBox<ConsoleQuartoPieceView>("Pieces")
            {
                ItemsSource = m_pieces,
                IsHorizontalLayout = true
            };
            m_pieceBox.SelectionChanged += selectionChanged;
            m_pieceBox.OverItemChanged += overItemChanged;
            m_placements = new MappingCollection<ConsoleQuartoPieceView, Placement<QuartoPiece, Move>>(m_game.Board.Placements);
            m_board = new ConsoleGrid<ConsoleQuartoPieceView>(4, 4, "Board", (p) => { return p.Column; }, (p) => { return p.Row; })
            {
                ItemsSource = m_placements,
                ForegroundColor = ConsoleColor.Red,
                Padding = new ConsoleSpacing(2, 0)
            };
            m_board.OverItemChanged += overItemChanged;
            var horizFlow = new ConsoleFlowContainer()
            {
                IsHorizontalLayout = true
            };
            m_board.SelectedLocationChanged += pieceLocationChosen;

            m_details = new ConsoleTextBox("Details")
            {
                Margin = new ConsoleSpacing(1, 0, 0, 0)
            };
            m_text = new ConsoleTextBox("Instructions")
            {
                Margin = new ConsoleSpacing(1, 1, 0, 0)
            };
            horizFlow.Children.Add(m_board);
            horizFlow.Children.Add(m_details);
            Children.Add(m_pieceBox);
            Children.Add(horizFlow);
            Children.Add(m_text);
        }

        private void quartoChanged(object sender, ChangedValueArgs<int> e)
        {
            if (e.NewVal >= 0)
            {
                var visible = new List<Point>();
                var result = new StringBuilder();
                for (int i = 0; i < 4; i++)
                {
                    if (m_game.Board.QuartoDiagonal >= 0)
                    {
                        Point p;
                        if (m_game.Board.QuartoDiagonal == 0)
                        {
                            p = new Point(i, i);
                        }
                        else
                        {
                            p = new Point(i, 3 - i);
                        }
                        if (!visible.Contains(p))
                        {
                            visible.Add(p);
                        }
                    }
                    if (m_game.Board.QuartoRow >= 0)
                    {
                        var p = new Point(i, m_game.Board.QuartoRow);
                        if (!visible.Contains(p))
                        {
                            visible.Add(p);
                        }
                    }
                    if (m_game.Board.QuartoColumn >= 0)
                    {
                        var p = new Point(m_game.Board.QuartoColumn, i);
                        if (!visible.Contains(p))
                        {
                            visible.Add(p);
                        }
                    }
                }
                appendWinReason(result, m_game.Board.DiagonalQuartoType, "QUARTO! Diagonal ");
                appendWinReason(result, m_game.Board.RowQuartoType, "QUARTO! Horizontal ");
                appendWinReason(result, m_game.Board.ColumnQuartoType, "QUARTO! Vertical ");
                foreach (var p in m_placements)
                {
                    var loc = new Point(p.Column, p.Row);
                    p.IsVisible = visible.Contains(loc);
                }
                m_details.Text = result.ToString();
            }
        }

        private static string appendWinReason(StringBuilder sb, IEnumerable<object> results, string text = "")
        {
            if (sb.Length > 0)
            {
                sb.AppendLine();
            }
            var i = 0;
            foreach (var r in results)
            {
                sb.Append(i++ > 0 ? " & " : text);
                sb.Append(r);
            }
            return sb.ToString();
        }
        private void overItemChanged(object sender, ChangedValueArgs<ConsoleQuartoPieceView> e)
        {
            if (e.NewVal?.Piece != null)
            {
                m_details.Text = e.NewVal.Piece.ToString();
            }
        }

        private void pieceLocationChosen(object sender, ChangedValueArgs<Point> e)
        {
            if(e.NewVal.X >= 0 && e.NewVal.Y >=0)
            {
                ActivePlayerModel.TriggerPlacement(new Move(e.NewVal));
            }
        }

        private void gameStateChanged(object sender, ChangedValueArgs<GameState> e)
        {
            m_pieceBox.IsEnabled = e.NewVal == GameState.Choose;
            m_board.IsEnabled = e.NewVal == GameState.Place;
            if (m_pieceBox.IsEnabled)
            {
                m_text.Text = $"{m_game.ActivePlayer}, choose the piece for {m_game.OtherPlayer} to place.";
            }
            else if (m_board.IsEnabled)
            {
                m_text.Text = $"{m_game.ActivePlayer}, choose the target location.";
            }
            else
            {
                m_text.Text = string.Empty;
            }
        }

        private void selectionChanged(object sender, ChangedValueArgs<ConsoleQuartoPieceView> e)
        {
            if(e.NewVal != null)
            {
                ActivePlayerModel.TriggerChoice(e.NewVal.Piece);
            }
        }

        private void activePlayerChanged(object sender, ChangedValueArgs<AbstractPlayer> e)
        {
            getPlayerView(e.NewVal);
        }

        private ConsoleQuartoPlayerView getPlayerView(AbstractPlayer p)
        {
            if (!m_playerModels.ContainsKey(p))
            {
                m_playerModels[p] = new ConsoleQuartoPlayerView(p);
                if(!(p is UserPlayer))
                {
                    p.Chosen += pieceChosen;
                    p.Placed += piecePlaced;
                }
            }
            return m_playerModels[p];
        }

        private readonly Dictionary<AbstractPlayer, ConsoleQuartoPlayerView> m_playerModels = new Dictionary<AbstractPlayer, ConsoleQuartoPlayerView>();
        public ConsoleQuartoPlayerView ActivePlayerModel
        {
            get => getPlayerView(m_game.ActivePlayer);
        }

        private void piecePlaced(object sender, PlacementEventArgs e)
        {
            m_pieceBox.SelectedItem = null;
        }

        private void pieceChosen(object sender, ChooseEventArgs e)
        {
            m_pieceBox.SelectedItem = m_pieces[e.ChosenPiece];
        }

        public void Run()
        {
            if(m_train)
            {
                var lp = new LearningPlayer("trainee","default.qdp");
                lp.Train(50000);
            }
            var player1 = getPlayer(m_player1Name);
            var player2 = getPlayer(m_player2Name);
            while (true)
            {
                m_details.Text = string.Empty;
                m_text.Text = string.Empty;
                var winner = m_game.Play(player1, player2);
                if(winner != null)
                {
                    m_text.Text = $"{winner} Wins!";
                }
                else
                {
                    m_text.Text = "Tie Game!";
                }
                m_text.Text += "  Press Enter to restart.";
                lock (m_lineEnteredLock)
                {
                    Monitor.Wait(m_lineEnteredLock);
                }
            }
        }

        protected override bool ComponentLineEntered(string line)
        {
            var res = base.ComponentLineEntered(line);
            lock (m_lineEnteredLock)
            {
                Monitor.PulseAll(m_lineEnteredLock);
            }
            return res;
        }

        private static AbstractPlayer getPlayer(string name)
        {
            AbstractPlayer player = null;
            if(name?.StartsWith("~") ?? false)
            {
                name = name.Substring(1);
                player = new RandomPlayer(name, 2000, 2000);
            }
            else if (name?.StartsWith("?") ?? false)
            {
                name = name.Substring(1);
                player = new LearningPlayer(name, "default.qdp", 2000, 2000);
            }
            else
            {
                player = new UserPlayer(name);
            }
            return player;
        }
    }
}
