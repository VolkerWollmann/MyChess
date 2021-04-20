using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using MyChess.Common;
using MyChess.Controls;
using MyChess.Helper;
using MyChessEngine;

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
            Position.AllPositions().ForEach(position =>
            {
                ChessBoard.SetPiece(position, ChessEngine.GetPiece(position));
            });
            

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
                ChessEngine.ExecuteMove(move);
                UpdateBoard();
                Move response = ChessEngine.CalculateMove();
                ChessEngine.ExecuteMove(response);
                UpdateBoard();

                EngineOutput.Text = ChessEngine.Message;

            }
        }

        private void Test()
        {
            ChessEngine.Test();

        }

        private void Command(object sender, ChessMenuEventArgs e)
        {
            EngineOutput.Text = "Command " + e.Tag + " " + DateTime.Now.ToString(CultureInfo.InvariantCulture);
            switch (e.Tag)
            {
                case ChessGameConstants.NewCommand:
                    New();
                    break;

                case ChessGameConstants.ClearCommand:
                    Clear();
                    break;

                case ChessGameConstants.MoveCommand:
                    Move();
                    EngineOutput.Text = ChessEngine.Message;
                    break;

                case ChessGameConstants.QuitCommand:
                    Application.Current.Shutdown();
                    break;

                case ChessGameConstants.Test1Command:
                    Test();
                    EngineOutput.Text = ChessEngine.Message;
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
            ChessEngine = new ChessEngine(EngineType.EngineType1);
            #endregion

            #region Menu
            Menu.SetEventHandler(Command);
            ChessBoard.SetEventHandler(Command);
            #endregion

        }
    }
}
