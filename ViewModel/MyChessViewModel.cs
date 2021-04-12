using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using MyChess.Common;
using MyChess.Controls;
using MyChess.Controls.Pieces;
using MyChess.Helper;
using MyChess.Model;

namespace MyChess.ViewModel
{
    public class MyChessViewModel
    {
        private readonly Grid GameGrid;

        private readonly ChessMenuUserControl Menu;

        private readonly ChessBoardUserControl ChessBoard;

        private readonly EngineOutputControl EngineOutput;

        #region Commands

        private void New()
        {
            // pawn
            for (int i = 0; i < 8; i++)
            {
                ChessBoard.SetPiece(1, i, new Pawn(ChessConstants.Color.White));
                ChessBoard.SetPiece(6, i, new Pawn(ChessConstants.Color.Black));
            }

            // rook
            ChessBoard.SetPiece(0, 0, new Rook(ChessConstants.Color.White));
            ChessBoard.SetPiece(0, 7, new Rook(ChessConstants.Color.White));

            ChessBoard.SetPiece(7, 0, new Rook(ChessConstants.Color.Black));
            ChessBoard.SetPiece(7, 7, new Rook(ChessConstants.Color.Black));

            // bishop 
            ChessBoard.SetPiece(0, 2, new Bishop(ChessConstants.Color.White));
            ChessBoard.SetPiece(0, 5, new Bishop(ChessConstants.Color.White));

            ChessBoard.SetPiece(7, 2, new Bishop(ChessConstants.Color.Black));
            ChessBoard.SetPiece(7, 5, new Bishop(ChessConstants.Color.Black));

            // knight
            ChessBoard.SetPiece(0, 1, new Knight(ChessConstants.Color.White));
            ChessBoard.SetPiece(0, 6, new Knight(ChessConstants.Color.White));

            ChessBoard.SetPiece(7, 1, new Knight(ChessConstants.Color.Black));
            ChessBoard.SetPiece(7, 6, new Knight(ChessConstants.Color.Black));

            // queen
            ChessBoard.SetPiece(0, 3, new Queen(ChessConstants.Color.White));
            ChessBoard.SetPiece(7, 3, new Queen(ChessConstants.Color.Black));

            // king
            ChessBoard.SetPiece(0, 4, new King(ChessConstants.Color.White));
            ChessBoard.SetPiece(7, 4, new King(ChessConstants.Color.Black));

        }

        private void Clear()
        {
            for(int row=0; row<8; row++)
                for(int column=0; column<8;column++)
                    ChessBoard.SetPiece(row,column, null);
        }

        #endregion
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

                case ChessConstants.QuitCommand:
                    System.Windows.Application.Current.Shutdown();
                    break;
            }
            
        }

        public MyChessViewModel(Grid gameGrid)
        {
            GameGrid = gameGrid;

            #region Board

            RowDefinition menuRowDefinition = new RowDefinition {Height = GridLength.Auto};
            GameGrid.RowDefinitions.Add(menuRowDefinition);

            RowDefinition chessBoRowDefinitionRowDefinition = new RowDefinition();
            GameGrid.RowDefinitions.Add(chessBoRowDefinitionRowDefinition);

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

            #region Menu
            Menu.SetEventHandler(Command);
            #endregion

        }
    }
}
