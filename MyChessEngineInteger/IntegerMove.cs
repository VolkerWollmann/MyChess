﻿using System;
using System.Collections.Generic;
using System.Text;
using MyChessEngineBase;
using MyChessEngineBase.Rating;

namespace MyChessEngineInteger
{
    public class IntegerMove
    {
        public int StartRow;
        public int StartColumn;
        public int EndRow;
        public int EndColumn;
        public MoveType MoveType;

        public bool IsAMove { get; } = true;

        public BoardRating Rating { get; set; }


        public IntegerMove(int startRow, int startColumn, int endRow, int endColumn, MoveType moveType, BoardRating boardRating)
        {
            StartRow = startRow;
            StartColumn = startColumn;
            EndRow = endRow;
            EndColumn = endColumn;
            MoveType = moveType;
            Rating = boardRating;
        }

        public IntegerMove(int startRow, int startColumn, int endRow, int endColumn) : this(startRow, startColumn,
            endRow, endColumn, MoveType.Normal, null)
        {

        }

        public IntegerMove(Move move) : this(move.Start.Row, move.Start.Column, move.End.Row, move.End.Column,
            move.Type, move.Rating)
        {

        }

    }
}
