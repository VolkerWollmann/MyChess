﻿using System;
using System.Linq;
using MyChessEngine.Pieces;

namespace MyChessEngine
{
    public class ChessEngine : IChessEngine
    {
        private readonly Board Board;
        public ChessEngineConstants.Color ColorToMove { get; set; }

        public IPiece  GetPiece(Position position)
        {
            return Board[position];
        }

        public ChessEngine Copy()
        {
            return new ChessEngine(Board.Copy(), ChessEngineConstants.NextColorToMove(ColorToMove));
        }


        public void New()
        {
            // pawn
            string[] whitePawnPositions = new[] { "A2", "B2", "C2", "D2", "E2", "F2", "G2", "H2"};
            string[] blackPawnPositions = new[] { "A7", "B7", "C7", "D7", "E7", "F7", "G7", "H7" };
            for (int i = 0; i < 8; i++)
            {
                Board[whitePawnPositions[i]] = new Pawn(ChessEngineConstants.Color.White);
                Board[blackPawnPositions[i]] = new Pawn(ChessEngineConstants.Color.Black);
            }

            // rook
            Board["A1"] = new Rook(ChessEngineConstants.Color.White);
            Board["H1"] = new Rook(ChessEngineConstants.Color.White);
            Board["A8"] = new Rook(ChessEngineConstants.Color.Black);
            Board["H8"] = new Rook(ChessEngineConstants.Color.Black);


            // bishop 
            Board["C1"] = new Bishop(ChessEngineConstants.Color.White);
            Board["F1"] = new Bishop(ChessEngineConstants.Color.White);
            Board["C8"] = new Bishop(ChessEngineConstants.Color.Black);
            Board["F8"] = new Bishop(ChessEngineConstants.Color.Black);


            // knight
            Board["B1"] = new Knight(ChessEngineConstants.Color.White);
            Board["G1"] = new Knight(ChessEngineConstants.Color.White);
            Board["B8"] = new Knight(ChessEngineConstants.Color.Black);
            Board["G8"] = new Knight(ChessEngineConstants.Color.Black);


            // queen
            Board["D1"] = new Queen(ChessEngineConstants.Color.White);
            Board["D8"] = new Queen(ChessEngineConstants.Color.Black);
            

            // king
            Board["E1"] = new King(ChessEngineConstants.Color.White);
            Board["E8"] = new King(ChessEngineConstants.Color.Black);

            ColorToMove = ChessEngineConstants.Color.White;
        }

        public void Clear()
        {
            Board.Clear();
        }

        private void CalculateMove()
        {

            DateTime time1 = DateTime.Now;
            Board copy = Board.Copy();

            var allMoves = copy.GetAllPieces(ColorToMove).Select((piece => piece.GetMoves())).SelectMany(move2 => move2).ToList();

            foreach (Move move in allMoves)
            {
                Board copy2 = copy.Copy();
                copy2.ExecuteMove(move);
            }

            TimeSpan ts = DateTime.Now.Subtract(time1);


            _Message = ts.ToString() + Environment.NewLine;

            foreach (Move move3 in allMoves)
            {
                _Message = _Message + " " + move3 + Environment.NewLine;
            }
        }

        public bool ExecuteMove(Move move)
        {
            Board.ExecuteMove(move);

            ColorToMove = ChessEngineConstants.NextColorToMove(ColorToMove);

            CalculateMove();

            return true;
        }

        public void Test()
        {
            var allMoves = this.Board.GetAllPieces(ColorToMove).Select( (piece => piece.GetMoves())).SelectMany( move => move).ToList();
            
            foreach (Move move3 in allMoves)
            {
                _Message = _Message + " " + move3 + Environment.NewLine;
            }
            
        }

        private string _Message;

        public string Message => _Message;

        public ChessEngine()
        {
            Board = new Board();
        }
        private ChessEngine(Board board, ChessEngineConstants.Color colorToMove)
        {
            Board = board;
            ColorToMove = colorToMove;
        }
    }
}
