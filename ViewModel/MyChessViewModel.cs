using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using MyChess.Common;
using MyChess.Controls;
using MyChess.Helper;
using MyChess.Model;

namespace MyChess.ViewModel
{
    public class MyChessViewModel
    {
        private readonly Grid GameGrid;

        private readonly ChessMenuUserControl Menu;

        private readonly ChessBoardUserControl ChessBoard;

        private readonly IChessEngine ChessEngine;

        private readonly EngineOutputControl EngineOutput;


        private void UpdateBoard()
        {
            IPiece[,] board = ChessEngine.GetBoard();

            for (int row = 0; row < ChessConstants.Length; row++)
            for (int column = 0; column < ChessConstants.Length; column++)
            {
                ChessBoard.SetPiece(row, column, board[row, column]);
            }

            ChessBoard.SetField(null);
        }

        #region Commands

        private void New()
        {
            ChessEngine.New();
            UpdateBoard();
        }

        private void Clear()
        {
           ChessEngine.Clear();
           UpdateBoard();
        }

        private void Move()
        {
            var move = ChessBoard.GetMove();
            if (move != null)
            {
                ChessEngine.ExecuteMove(move.Item1, move.Item2, move.Item3, move.Item4);
                UpdateBoard();
            }
        }
        
        private void Command(object sender, ChessMenuEventArgs e)
        {
            EngineOutput.Text = "Command " + e.Tag + " " + DateTime.Now.ToString(CultureInfo.InvariantCulture);
            switch (e.Tag)
            {
                case ChessConstants.NewCommand:
                    New();
                    break;

                case ChessConstants.ClearCommand:
                    Clear();
                    break;

                case ChessConstants.MoveCommand:
                    Move();
                    break;

                case ChessConstants.QuitCommand:
                    Application.Current.Shutdown();
                    break;
            }
            
        }
        #endregion

        public MyChessViewModel(Grid gameGrid)
        {
            GameGrid = gameGrid;

            #region Board

            RowDefinition menuRowDefinition = new RowDefinition {Height = GridLength.Auto};
            GameGrid.RowDefinitions.Add(menuRowDefinition);

            RowDefinition chessBoardRowDefinitionRowDefinition = new RowDefinition();
            GameGrid.RowDefinitions.Add(chessBoardRowDefinitionRowDefinition);

            RowDefinition engineOutputRowDefinition = new RowDefinition {Height = new GridLength(100)};
            GameGrid.RowDefinitions.Add(engineOutputRowDefinition);

            Menu = new ChessMenuUserControl();
            GameGrid.Children.Add(Menu);
            Grid.SetRow(Menu, 0);

            ChessBoard = new ChessBoardUserControl();
            GameGrid.Children.Add(ChessBoard);
            Grid.SetRow(ChessBoard, 1);

            EngineOutput = new EngineOutputControl();
            GameGrid.Children.Add(EngineOutput);
            Grid.SetRow(EngineOutput, 2);
            #endregion

            #region Engine
            ChessEngine = new ChessEngine();
            #endregion

            #region Menu
            Menu.SetEventHandler(Command);
            ChessBoard.SetEventHandler(Command);
            #endregion

        }
    }
}
