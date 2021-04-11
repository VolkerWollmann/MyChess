﻿using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using MyChess.Controls;
using MyChess.Helper;

namespace MyChess.ViewModel
{
    public class MyChessViewModel
    {
        private readonly Grid GameGrid;

        private readonly ChessMenuUserControl Menu;

        private readonly ChessBoardUserControl ChessBoard;

        private readonly EngineOutputControl EngineOutput;

        private void TestCommand(object sender, ChessMenuEventArgs e)
        {
            EngineOutput.Text = "Command "  + e.Tag + " " + DateTime.Now.ToString(CultureInfo.InvariantCulture);

            ChessBoard.SetPiece();
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
            Menu.SetEventHandler(TestCommand);
            #endregion

        }
    }
}
