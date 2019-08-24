﻿using System;
using System.Collections;
using System.IO;
using System.Text;

namespace Sudoku
{
    class Program
    {
        static void Main(string[] args)
        {
            Sudoku.Run(args);
        }
    }

    public enum Target
    {
        Row,
        Column,
        Square
    }

    public class Sudoku
    {
        /* A total of 22 static methods in the class:
           CopyList
           CopySudokuBoard
           CopyCandidates
           SaveState
           RestoreState
           GetInputSudokuBoard
           CandidateIsAlonePossible
           RemoveNumberIfItExists
           ReturnNumberOfOccurenciesOfNumber
           ReturnTwoDimensionalDataStructure
           ReturnThreeDimensionalDataStructure
           ReturnSquareCellToRowColumnMapper
           ReturnSudokuBoardAsString
           SimulateOneNumber
           CheckIfCanUpdateBestSoFarSudokuBoard (CopySudokuBoard)
           InitCandidates (Dependent on ReturnNumberOfOccurenciesOfNumber)
           TryFindNumberToSetInCellWithCertainty (Dependent on CandidateIsAlonePossible)
           UpdateCandidates (Dependent on RemoveNumberIfItExists)
           ValidateSudokuBoard (Dependent on ReturnNumberOfOccurenciesOfNumber)
           PrintSudokuBoard (Dependent on ReturnSudokuBoardAsString)
           PrintResult (Dependent on PrintSudokuBoard)
           Run (Dependent on GetInputSudokuBoard, ValidateSudokuBoard, ReturnTwoDimensionalDataStructure, SimulateOneNumber, ReturnThreeDimensionalDataStructure, ReturnSquareCellToRowColumnMapper, InitCandidates, TryFindNumberToSetInCellWithCertainty, CopyList, CopySudokuBoard, UpdateCandidates, PrintSudokuBoard)
        */
        public static void Run(string[] args)
        {
            int row = 0, column = 0, number, i;
            int[][] certaintySudokuBoard = null;
            int[][] bestSoFarSudokuBoard = null, workingSudokuBoard = ReturnTwoDimensionalDataStructure(9, 9);
            int[][][] candidates, squareCellToRowColumnMapper, candidatesAfterAddedNumbersWithCertainty = null;
            int maxNumberOfAttemptsToSolveSudoku = 100, numberOfAttemptsToSolveSudoku = 0;
            int numberOfCellsSetInInputSudokuBoard = 0, numberOfCellsSetInBestSoFar = 0, numberOfCandidates;
            int numberOfCandidatesAfterAddedNumbersWithCertainty = 0;
            bool sudokuSolved = false, numbersAddedWithCertaintyAndThenNoCandidates = false;
            Random random = null;
            string msg;
            ArrayList cellsRemainToSet = new ArrayList(), cellsRemainToSetAfterAddedNumbersWithCertainty = null;

            msg = GetInputSudokuBoard(args, workingSudokuBoard, cellsRemainToSet);

            if (msg != null)
            {
                PrintResult(false, args, msg, false, numberOfCellsSetInInputSudokuBoard, numberOfCellsSetInBestSoFar, workingSudokuBoard, bestSoFarSudokuBoard);
                return;
            }

            squareCellToRowColumnMapper = ReturnSquareCellToRowColumnMapper();
            msg = ValidateSudokuBoard(workingSudokuBoard, squareCellToRowColumnMapper);

            if (msg != null)
            {
                PrintResult(false, args, msg, false, numberOfCellsSetInInputSudokuBoard, numberOfCellsSetInBestSoFar, workingSudokuBoard, bestSoFarSudokuBoard);
                return;
            }

            if (cellsRemainToSet.Count == 0)
            {
                PrintResult(false, args, "A complete sudoku was given as input. There is nothing to solve.", true, numberOfCellsSetInInputSudokuBoard, numberOfCellsSetInBestSoFar, workingSudokuBoard, bestSoFarSudokuBoard);
                return;
            }

            candidates = ReturnThreeDimensionalDataStructure(9, 9, 10);
            numberOfCandidates = InitCandidates(workingSudokuBoard, squareCellToRowColumnMapper, candidates);

            if (numberOfCandidates == 0)
            {
                PrintResult(false, args, "It is not possible to add any number to the sudoku.", false, numberOfCellsSetInInputSudokuBoard, numberOfCellsSetInBestSoFar, workingSudokuBoard, bestSoFarSudokuBoard);
                return;
            }

            numberOfCellsSetInInputSudokuBoard = 81 - cellsRemainToSet.Count;

            while (numberOfAttemptsToSolveSudoku < maxNumberOfAttemptsToSolveSudoku && !sudokuSolved && !numbersAddedWithCertaintyAndThenNoCandidates)
            {
                if (numberOfAttemptsToSolveSudoku > 0)
                {
                    RestoreState(cellsRemainToSet, cellsRemainToSetAfterAddedNumbersWithCertainty, numberOfCandidatesAfterAddedNumbersWithCertainty, workingSudokuBoard, certaintySudokuBoard, candidates, candidatesAfterAddedNumbersWithCertainty, out numberOfCandidates);
                }

                while (numberOfCandidates > 0)
                {
                    number = 0;
                    i = 0;

                    while (i < cellsRemainToSet.Count && number == 0)
                    {
                        row = ((int[])cellsRemainToSet[i])[0];
                        column = ((int[])cellsRemainToSet[i])[1];
                        number = TryFindNumberToSetInCellWithCertainty(row, column, candidates, squareCellToRowColumnMapper);
                        i = (number == 0) ? i + 1 : i;
                    }

                    if (number == 0)
                    {
                        if (random == null)
                            random = new Random((int)(DateTime.Now.Ticks % 64765L));

                        SimulateOneNumber(candidates, random, cellsRemainToSet, out i, out number);
                        row = ((int[])cellsRemainToSet[i])[0];
                        column = ((int[])cellsRemainToSet[i])[1];

                        if (certaintySudokuBoard == null)
                        {
                            certaintySudokuBoard = ReturnTwoDimensionalDataStructure(9, 9);
                            cellsRemainToSetAfterAddedNumbersWithCertainty = new ArrayList();
                            candidatesAfterAddedNumbersWithCertainty = ReturnThreeDimensionalDataStructure(9, 9, 10);
                            SaveState(cellsRemainToSet, cellsRemainToSetAfterAddedNumbersWithCertainty, numberOfCandidates, workingSudokuBoard, certaintySudokuBoard, candidates, candidatesAfterAddedNumbersWithCertainty, out numberOfCandidatesAfterAddedNumbersWithCertainty);
                        }
                    }

                    workingSudokuBoard[row - 1][column - 1] = number;
                    cellsRemainToSet.RemoveAt(i);
                    numberOfCandidates -= UpdateCandidates(candidates, squareCellToRowColumnMapper, row, column, number);
                }

                if (cellsRemainToSet.Count == 0)
                {
                    sudokuSolved = true;
                }
                else if (certaintySudokuBoard == null)
                {
                    numbersAddedWithCertaintyAndThenNoCandidates = true;
                    numberOfCellsSetInBestSoFar = 81 - cellsRemainToSet.Count;
                }
                else
                {
                    if (bestSoFarSudokuBoard == null)
                        bestSoFarSudokuBoard = ReturnTwoDimensionalDataStructure(9, 9);

                    numberOfCellsSetInBestSoFar = CheckIfCanUpdateBestSoFarSudokuBoard(numberOfCellsSetInBestSoFar, cellsRemainToSet, workingSudokuBoard, bestSoFarSudokuBoard);
                    numberOfAttemptsToSolveSudoku++;
                }
            }

            PrintResult(true, args, null, sudokuSolved, numberOfCellsSetInInputSudokuBoard, numberOfCellsSetInBestSoFar, workingSudokuBoard, bestSoFarSudokuBoard);
        }

        private static void CopyList(ArrayList from, ArrayList to)
        {
            to.Clear();

            for (int i = 0; i < from.Count; i++)
            {
                to.Add(from[i]);
            }
        }

        private static void CopySudokuBoard(int[][] sudokuBoardFrom, int[][] sudokuBoardTo)
        {
            for (int row = 1; row <= 9; row++)
            {
                for (int column = 1; column <= 9; column++)
                {
                    sudokuBoardTo[row - 1][column - 1] = sudokuBoardFrom[row - 1][column - 1];
                }
            }
        }

        private static void CopyCandidates(int[][][] candidatesFrom, int[][][] candidatesTo)
        {
            for (int row = 1; row <= 9; row++)
            {
                for (int column = 1; column <= 9; column++)
                {
                    for(int i = 0; i < 10; i++)
                    {
                        candidatesTo[row - 1][column - 1][i] = candidatesFrom[row - 1][column - 1][i];
                    }
                }
            }
        }

        private static void SaveState(ArrayList cellsRemainToSet, ArrayList cellsRemainToSetAfterAddedNumbersWithCertainty, int numberOfCandidates, int[][] workingSudokuBoard, int[][] certaintySudokuBoard, int[][][] candidates, int[][][] candidatesAfterAddedNumbersWithCertainty, out int numberOfCandidatesAfterAddedNumbersWithCertainty)
        {
            CopyList(cellsRemainToSet, cellsRemainToSetAfterAddedNumbersWithCertainty);
            CopySudokuBoard(workingSudokuBoard, certaintySudokuBoard);
            CopyCandidates(candidates, candidatesAfterAddedNumbersWithCertainty);
            numberOfCandidatesAfterAddedNumbersWithCertainty = numberOfCandidates;
        }

        private static void RestoreState(ArrayList cellsRemainToSet, ArrayList cellsRemainToSetAfterAddedNumbersWithCertainty, int numberOfCandidatesAfterAddedNumbersWithCertainty, int[][] workingSudokuBoard, int[][] certaintySudokuBoard, int[][][] candidates, int[][][] candidatesAfterAddedNumbersWithCertainty, out int numberOfCandidates)
        {
            CopyList(cellsRemainToSetAfterAddedNumbersWithCertainty, cellsRemainToSet);
            CopySudokuBoard(certaintySudokuBoard, workingSudokuBoard);
            CopyCandidates(candidatesAfterAddedNumbersWithCertainty, candidates);
            numberOfCandidates = numberOfCandidatesAfterAddedNumbersWithCertainty;
        }

        private static string GetInputSudokuBoard(string[] args, int[][] sudokuBoard, ArrayList cellsRemainToSet)
        {
            string[] rows, columns;
            int row, column, n;

            if (args.Length == 0)
            {
                return "An input file is not given to the program (first parameter)!";
            }
            else if (args.Length > 2)
            {
                return "At most two parameters may be given to the program!";
            }
            else if (!File.Exists(args[0]))
            {
                return "The given input file in first parameter does not exist!";
            }
            else if (args.Length == 2 && !Directory.Exists(args[1]))
            {
                return "The directory given in second parameter does not exist!";
            }

            string sudokuBoardString = File.ReadAllText(args[0]).Trim().Replace("\r\n", "\n");

            rows = sudokuBoardString.Split(new string[] { "\n" }, StringSplitOptions.None);

            if (rows.Length != 9)
            {
                return "Number of rows in input file are not 9 as expected!";
            }

            for (row = 1; row <= 9; row++)
            {
                columns = rows[row - 1].Split(' ');

                if (columns.Length != 9)
                {
                    return string.Format("Number of columns in input file in row {0} are not 9 as expected!", row.ToString());
                }

                for (column = 1; column <= 9; column++)
                {
                    if (!int.TryParse(columns[column - 1], out n))
                    {
                        return string.Format("The value \"{0}\" in row {1} and column {2} in input file is not a valid integer!", columns[column - 1], row.ToString(), column.ToString());
                    }

                    if (n < 0 || n > 9)
                    {
                        return string.Format("The value \"{0}\" in row {1} and column {2} in input file is not an integer in the interval [0, 9] as expected!", columns[column - 1], row.ToString(), column.ToString());
                    }

                    sudokuBoard[row - 1][column - 1] = n;

                    if (n == 0)
                    {
                        cellsRemainToSet.Add(new int[] { row, column });
                    }
                }
            }

            return null;
        }

        private static bool CandidateIsAlonePossible(int number, int[][][] candidates, int[][][] squareCellToRowColumnMapper, int t, Target target)
        {
            int row = 0, column = 0, n, i, j, numberOfOccurenciesOfNumber = 0;

            for (i = 0; i < 9; i++)
            {
                switch (target)
                {
                    case Target.Row:
                        row = t;
                        column = i + 1;
                        break;
                    case Target.Column:
                        row = i + 1;
                        column = t;
                        break;
                    case Target.Square:
                        row = squareCellToRowColumnMapper[t - 1][i][0];
                        column = squareCellToRowColumnMapper[t - 1][i][1];
                        break;
                }

                n = candidates[row - 1][column - 1][0];

                if (n > 0)
                {
                    for (j = 0; j < n; j++)
                    {
                        if (candidates[row - 1][column - 1][1 + j] == number)
                        {
                            numberOfOccurenciesOfNumber++;

                            if (numberOfOccurenciesOfNumber > 1)
                                return false;
                        }
                    }
                }
            }

            return true;
        }

        /// <summary>
        ///  Returns 1 if number exists, otherwise 0
        /// </summary>
        private static int RemoveNumberIfItExists(int[] v, int number)
        {
            int i, n, index = -1, returnValue = 0;

            n = v[0];
            i = 1;
            while (i <= n && index == -1)
            {
                if (v[i] == number)
                {
                    index = i;
                    returnValue = 1;
                }
                else
                    i++;
            }

            if (index != -1)
            {
                while (index + 1 <= n)
                {
                    v[index] = v[index + 1];
                    index++;
                }

                v[0]--;
            }

            return returnValue;
        }

        private static int ReturnNumberOfOccurenciesOfNumber(int[][] sudokuBoard, int[][][] squareCellToRowColumnMapper, int number, int t, Target target) //t refers to a row, column or square
        {
            int row = 0, column = 0, n = 0;

            for (int i = 0; i < 9; i++)
            {
                switch (target)
                {
                    case Target.Row:
                        row = t;
                        column = i + 1;
                        break;
                    case Target.Column:
                        row = i + 1;
                        column = t;
                        break;
                    case Target.Square:
                        row = squareCellToRowColumnMapper[t - 1][i][0];
                        column = squareCellToRowColumnMapper[t - 1][i][1];
                        break;
                }

                if (sudokuBoard[row - 1][column - 1] == number)
                    n++;
            }

            return n;
        }

        private static int[][] ReturnTwoDimensionalDataStructure(int m, int n)
        {
            int[][] v;
            int i;

            v = new int[m][];

            for (i = 0; i < m; i++)
            {
                v[i] = new int[n];
            }

            return v;
        }

        private static int[][][] ReturnThreeDimensionalDataStructure(int l, int m, int n)
        {
            int[][][] v;
            int i, j;

            v = new int[l][][];

            for (i = 0; i < l; i++)
            {
                v[i] = new int[m][];
            }

            for (i = 0; i < l; i++)
            {
                for (j = 0; j < m; j++)
                {
                    v[i][j] = new int[n];
                }
            }

            return v;
        }

        private static int[][][] ReturnSquareCellToRowColumnMapper()
        {
            int[][][] v;
            int[] index;
            int i, row, column, square;

            v = ReturnThreeDimensionalDataStructure(9, 9, 2);

            index = new int[9];

            for (i = 0; i < 9; i++)
            {
                index[i] = 0;
            }

            for (row = 1; row <= 9; row++)
            {
                for (column = 1; column <= 9; column++)
                {
                    square = 1 + (3 * ((row - 1) / 3)) + ((column - 1) / 3);
                    v[square - 1][index[square - 1]][0] = row;
                    v[square - 1][index[square - 1]][1] = column;
                    index[square - 1]++;
                }
            }

            return v;
        }

        private static string ReturnSudokuBoardAsString(int[][] sudokuBoard)
        {
            int row, column;
            StringBuilder sb = new StringBuilder();

            for (row = 1; row <= 9; row++)
            {
                if (row > 1)
                    sb.Append("\r\n");

                for (column = 1; column <= 9; column++)
                {
                    if (column == 1)
                        sb.Append(sudokuBoard[row - 1][column - 1].ToString());
                    else
                        sb.Append(string.Format(" {0}", sudokuBoard[row - 1][column - 1].ToString()));
                }
            }

            return sb.ToString();
        }

        private static void SimulateOneNumber(int[][][] candidates, Random random, ArrayList cellsRemainToSet, out int index, out int number)
        {
            int tmp, row, column, i, numberOfCandidates, minNumberOfCandidates = 9;
            ArrayList v;

            for (i = 0; i < cellsRemainToSet.Count; i++)
            {
                row = ((int[])cellsRemainToSet[i])[0];
                column = ((int[])cellsRemainToSet[i])[1];
                numberOfCandidates = candidates[row - 1][column - 1][0];

                if (numberOfCandidates > 0 && numberOfCandidates < minNumberOfCandidates)
                    minNumberOfCandidates = numberOfCandidates;
            }

            v = new ArrayList();

            for (i = 0; i < cellsRemainToSet.Count; i++)
            {
                row = ((int[])cellsRemainToSet[i])[0];
                column = ((int[])cellsRemainToSet[i])[1];

                if (candidates[row - 1][column - 1][0] == minNumberOfCandidates)
                    v.Add(i);
            }

            tmp = random.Next(0, v.Count);
            index = (int)v[tmp];
            row = ((int[])cellsRemainToSet[index])[0];
            column = ((int[])cellsRemainToSet[index])[1];
            number = candidates[row - 1][column - 1][1 + random.Next(0, minNumberOfCandidates)];
        }

        private static int CheckIfCanUpdateBestSoFarSudokuBoard(int numberOfCellsSetInBestSoFar, ArrayList cellsRemainToSet, int[][] workingSudokuBoard, int[][] bestSoFarSudokuBoard)
        {
            int retVal = numberOfCellsSetInBestSoFar; //Default

            if (numberOfCellsSetInBestSoFar < (81 - cellsRemainToSet.Count))
            {
                retVal = 81 - cellsRemainToSet.Count;
                CopySudokuBoard(workingSudokuBoard, bestSoFarSudokuBoard);
            }

            return retVal;
        }

        private static int InitCandidates(int[][] sudokuBoard, int[][][] squareCellToRowColumnMapper, int[][][] candidates)
        {
            int row, column, square, number, numberOfCandidates, n;

            numberOfCandidates = 0;

            for (row = 1; row <= 9; row++)
            {
                for (column = 1; column <= 9; column++)
                {
                    square = 1 + (3 * ((row - 1) / 3)) + ((column - 1) / 3);

                    if (sudokuBoard[row - 1][column - 1] != 0)
                    {
                        candidates[row - 1][column - 1][0] = -1; //Indicates that the cell is set already
                    }
                    else
                    {
                        n = 0;
                        candidates[row - 1][column - 1][0] = 0; //Number of candidates is set in index 0

                        for (number = 1; number <= 9; number++)
                        {
                            if (
                                (ReturnNumberOfOccurenciesOfNumber(sudokuBoard, squareCellToRowColumnMapper, number, row, Target.Row) == 0) &&
                                (ReturnNumberOfOccurenciesOfNumber(sudokuBoard, squareCellToRowColumnMapper, number, column, Target.Column) == 0) &&
                                (ReturnNumberOfOccurenciesOfNumber(sudokuBoard, squareCellToRowColumnMapper, number, square, Target.Square) == 0)
                                )
                            {
                                n++;
                                candidates[row - 1][column - 1][0] = n;
                                candidates[row - 1][column - 1][n] = number;
                                numberOfCandidates++;
                            }
                        }
                    }
                }
            }

            return numberOfCandidates;
        }

        private static int TryFindNumberToSetInCellWithCertainty(int row, int column, int[][][] candidates, int[][][] squareCellToRowColumnMapper)
        {
            int i, square, numberOfCandidatesInCell, candidate, number = 0;

            square = 1 + (3 * ((row - 1) / 3)) + ((column - 1) / 3);
            numberOfCandidatesInCell = candidates[row - 1][column - 1][0];

            if (numberOfCandidatesInCell == 1)
            {
                number = candidates[row - 1][column - 1][1];
            }
            else if (numberOfCandidatesInCell > 1)
            {
                i = 1;
                while (i <= numberOfCandidatesInCell && number == 0)
                {
                    candidate = candidates[row - 1][column - 1][i];

                    if (CandidateIsAlonePossible(candidate, candidates, squareCellToRowColumnMapper, row, Target.Row) ||
                        CandidateIsAlonePossible(candidate, candidates, squareCellToRowColumnMapper, column, Target.Column) ||
                        CandidateIsAlonePossible(candidate, candidates, squareCellToRowColumnMapper, square, Target.Square))
                        number = candidate;
                    else
                        i++;
                }
            }

            return number;
        }

        private static int UpdateCandidates(int[][][] candidates, int[][][] squareCellToRowColumnMapper, int row, int column, int number)
        {
            int i, r, c, square, totalNumberOfCandidatesRemoved;

            totalNumberOfCandidatesRemoved = candidates[row - 1][column - 1][0]; //Remove all candidates in that cell
            candidates[row - 1][column - 1][0] = -1; //Indicates that the cell is set already

            square = 1 + (3 * ((row - 1) / 3)) + ((column - 1) / 3);

            for (c = 1; c <= 9; c++)
            {
                if (c != column && candidates[row - 1][c - 1][0] > 0)
                {
                    totalNumberOfCandidatesRemoved += RemoveNumberIfItExists(candidates[row - 1][c - 1], number);
                }
            }

            for (r = 1; r <= 9; r++)
            {
                if (r != row && candidates[r - 1][column - 1][0] > 0)
                {
                    totalNumberOfCandidatesRemoved += RemoveNumberIfItExists(candidates[r - 1][column - 1], number);
                }
            }

            for (i = 0; i < 9; i++)
            {
                r = squareCellToRowColumnMapper[square - 1][i][0];
                c = squareCellToRowColumnMapper[square - 1][i][1];

                if (r != row && c != column && candidates[r - 1][c - 1][0] > 0)
                {
                    totalNumberOfCandidatesRemoved += RemoveNumberIfItExists(candidates[r - 1][c - 1], number);
                }
            }

            return totalNumberOfCandidatesRemoved;
        }

        private static string ValidateSudokuBoard(int[][] sudokuBoard, int[][][] squareCellToRowColumnMapper)
        {
            int row, column, square, number;

            for (row = 1; row <= 9; row++)
            {
                for (column = 1; column <= 9; column++)
                {
                    square = 1 + (3 * ((row - 1) / 3)) + ((column - 1) / 3);
                    number = sudokuBoard[row - 1][column - 1];

                    if (number != 0)
                    {
                        if (ReturnNumberOfOccurenciesOfNumber(sudokuBoard, squareCellToRowColumnMapper, number, row, Target.Row) > 1)
                        {
                            return string.Format("The input sudoku is incorrect! The number {0} occurs more than once in row {1}", number.ToString(), row.ToString());
                        }
                        else if (ReturnNumberOfOccurenciesOfNumber(sudokuBoard, squareCellToRowColumnMapper, number, column, Target.Column) > 1)
                        {
                            return string.Format("The input sudoku is incorrect! The number {0} occurs more than once in column {1}", number.ToString(), column.ToString());
                        }
                        else if (ReturnNumberOfOccurenciesOfNumber(sudokuBoard, squareCellToRowColumnMapper, number, square, Target.Square) > 1)
                        {
                            return string.Format("The input sudoku is incorrect! The number {0} occurs more than once in square {1}", number.ToString(), square.ToString());
                        }
                    }
                }
            }

            return null;
        }

        private static void PrintSudokuBoard(bool solved, string[] args, string message, int[][] sudokuBoard)
        {
            string suffix, fileNameFullpath;
            char c;

            if (solved)
                suffix = string.Format("__Solved_{0}.txt", DateTime.Now.ToString("yyyy.MM.dd.HH.mm.ss.fff"));
            else
                suffix = string.Format("__Partially_solved_{0}.txt", DateTime.Now.ToString("yyyy.MM.dd.HH.mm.ss.fff"));

            if (args.Length == 2)
            {
                c = args[1].Trim()[args[1].Trim().Length - 1];
                fileNameFullpath = args[1].Trim() + ((c == '\\') ? "" : "\\") + (new FileInfo(args[0])).Name + suffix;
            }
            else
                fileNameFullpath = args[0] + suffix;

            File.WriteAllText(fileNameFullpath, message + "\r\n\r\n" + ReturnSudokuBoardAsString(sudokuBoard));
        }

        private static void PrintResult(bool initialSudokuBoardHasCandidates, string[] args, string msg, bool sudokuSolved, int numberOfCellsSetInInputSudokuBoard, int numberOfCellsSetInBestSoFar, int[][] workingSudokuBoard, int[][] bestSoFarSudokuBoard)
        {
            if (initialSudokuBoardHasCandidates)
            {
                if (sudokuSolved)
                {
                    msg = string.Format("The sudoku was solved. {0} number(s) added to the original {1}.", 81 - numberOfCellsSetInInputSudokuBoard, numberOfCellsSetInInputSudokuBoard);
                }
                else
                {
                    msg = string.Format("The sudoku was partially solved. {0} number(s) added to the original {1}. Unable to set {2} number(s).", numberOfCellsSetInBestSoFar - numberOfCellsSetInInputSudokuBoard, numberOfCellsSetInInputSudokuBoard, 81 - numberOfCellsSetInBestSoFar);
                }

                if (sudokuSolved || bestSoFarSudokuBoard == null)
                {
                    PrintSudokuBoard(sudokuSolved, args, msg, workingSudokuBoard);
                }
                else
                {
                    PrintSudokuBoard(sudokuSolved, args, msg, bestSoFarSudokuBoard);
                }
            }

            Console.Write(msg);
        }
    }
}
