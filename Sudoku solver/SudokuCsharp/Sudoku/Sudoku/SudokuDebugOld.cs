﻿using System;
using System.Collections;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace SudokuDebugOld
{
    public enum Target
    {
        Row,
        Column,
        Square
    }

    public class Sudoku
    {
        public static void Run(string[] args)
        {
            int[][] workingSudokuBoard, certaintySudokuBoard, bestSoFarSudokuBoard;
            int[][][] candidates, squareCellToRowColumnMapper;
            int maxNumberOfAttemptsToSolveSudoku, numberOfAttemptsToSolveSudoku;
            int numberOfCellsSetInInputSudokuBoard, numberOfCellsSetInBestSoFar, numberOfCandidates;
            int row, column, square, number, numberOfCandidatesInCell, i, j;
            bool sudokuSolved, numbersAddedWithCertaintyAndThenNoCandidates, foundNumberToSet;
            Random random;
            string errorMessage, message;
            ArrayList cellsRemainToSet, cellsRemainToSetAfterAddedNumbersWithCertainty; //Lists with two-tuples of int
            SudokuDebug sudokuDebug = new SudokuDebug();
            string category = "", fileNameFullPath;

            //Init
            row = column = number = numberOfCellsSetInBestSoFar = 0;
            certaintySudokuBoard = null;
            bestSoFarSudokuBoard = ReturnTwoDimensionalDataStructure(9, 9);
            cellsRemainToSetAfterAddedNumbersWithCertainty = null;
            maxNumberOfAttemptsToSolveSudoku = 10;
            numberOfAttemptsToSolveSudoku = 0;
            sudokuSolved = false;
            numbersAddedWithCertaintyAndThenNoCandidates = false;
            workingSudokuBoard = ReturnTwoDimensionalDataStructure(9, 9);
            cellsRemainToSet = new ArrayList();           
            random = new Random((int)(DateTime.Now.Ticks % 64765L));

            errorMessage = GetInputSudokuBoard(args, workingSudokuBoard, cellsRemainToSet);

            if (errorMessage != null)
            {
                Console.Write(errorMessage);
                return;
            }

            squareCellToRowColumnMapper = ReturnSquareToCellMapper();
            errorMessage = ValidateSudokuBoard(workingSudokuBoard, squareCellToRowColumnMapper);

            if (errorMessage != null)
            {
                Console.Write(errorMessage);
                return;
            }

            if (cellsRemainToSet.Count == 0)
            {
                Console.Write("A complete sudoku was given as input. There is nothing to solve.");
                return;
            }

            candidates = ReturnThreeDimensionalDataStructure(9, 9, 10);
            numberOfCandidates = InitCandidates(workingSudokuBoard, squareCellToRowColumnMapper, candidates);

            if (numberOfCandidates == 0)
            {
                Console.WriteLine("It is not possible to add any number to the sudoku.");
                return;
            }

            numberOfCellsSetInInputSudokuBoard = 81 - cellsRemainToSet.Count;

            while (numberOfAttemptsToSolveSudoku < maxNumberOfAttemptsToSolveSudoku && !sudokuSolved && !numbersAddedWithCertaintyAndThenNoCandidates)
            {
                if (numberOfAttemptsToSolveSudoku > 0)
                {
                    CopySudokuBoard(certaintySudokuBoard, workingSudokuBoard);
                    Copy(cellsRemainToSetAfterAddedNumbersWithCertainty, cellsRemainToSet);
                    numberOfCandidates = InitCandidates(workingSudokuBoard, squareCellToRowColumnMapper, candidates);
                }

                while (numberOfCandidates > 0)
                {
                    foundNumberToSet = false;
                    i = 0;

                    while (i < cellsRemainToSet.Count && !foundNumberToSet)
                    {
                        row = ((int[])cellsRemainToSet[i])[0];
                        column = ((int[])cellsRemainToSet[i])[1];
                        square = ReturnSquare(row, column);
                        numberOfCandidatesInCell = candidates[row - 1][column - 1][0];

                        if (numberOfCandidatesInCell == 1)
                        {
                            number = candidates[row - 1][column - 1][1];
                            foundNumberToSet = true;
                            category = "Alone in cell";
                        }
                        else
                        {
                            j = 1;
                            while (j <= numberOfCandidatesInCell && !foundNumberToSet)
                            {
                                number = candidates[row - 1][column - 1][j];

                                if (NumberIsAloneCandidate(number, candidates, squareCellToRowColumnMapper, row, Target.Row))
                                {
                                    foundNumberToSet = true;
                                    category = "Alone possible in row";
                                }
                                else if (NumberIsAloneCandidate(number, candidates, squareCellToRowColumnMapper, column, Target.Column))
                                {
                                    foundNumberToSet = true;
                                    category = "Alone possible in column";
                                }
                                else if (NumberIsAloneCandidate(number, candidates, squareCellToRowColumnMapper, square, Target.Square))
                                {
                                    foundNumberToSet = true;
                                    category = "Alone possible in square";
                                }
                                else
                                    j++;
                            }
                        }

                        if (!foundNumberToSet)
                            i++;
                    }

                    if (!foundNumberToSet)
                    {
                        SimulateOneNumber(candidates, random, cellsRemainToSet, out i, out number);
                        row = ((int[])cellsRemainToSet[i])[0];
                        column = ((int[])cellsRemainToSet[i])[1];

                        if (certaintySudokuBoard == null)
                        {
                            certaintySudokuBoard = ReturnTwoDimensionalDataStructure(9, 9);
                            cellsRemainToSetAfterAddedNumbersWithCertainty = new ArrayList();
                            CopySudokuBoard(workingSudokuBoard, certaintySudokuBoard);
                            Copy(cellsRemainToSet, cellsRemainToSetAfterAddedNumbersWithCertainty);
                        }

                        category = "Simulated";
                    }

                    fileNameFullPath = sudokuDebug.Log(1 + numberOfAttemptsToSolveSudoku, row, column, number, category, workingSudokuBoard, candidates, numberOfCandidates, cellsRemainToSet);
                    workingSudokuBoard[row - 1][column - 1] = number;
                    cellsRemainToSet.RemoveAt(i);
                    numberOfCandidates -= UpdateCandidates(candidates, squareCellToRowColumnMapper, row, column, number);
                    sudokuDebug.Log(fileNameFullPath, workingSudokuBoard, candidates, numberOfCandidates, cellsRemainToSet);
                }

                if (numberOfCellsSetInBestSoFar < (81 - cellsRemainToSet.Count))
                {
                    numberOfCellsSetInBestSoFar = 81 - cellsRemainToSet.Count;
                    CopySudokuBoard(workingSudokuBoard, bestSoFarSudokuBoard);
                }

                if (cellsRemainToSet.Count == 0)
                    sudokuSolved = true;
                else if (certaintySudokuBoard == null)
                    numbersAddedWithCertaintyAndThenNoCandidates = true;
                else
                    numberOfAttemptsToSolveSudoku++;
            }

            if (numberOfCellsSetInBestSoFar == 81)
                message = string.Format("The sudoku was solved. {0} numbers added to the original {1}", 81 - numberOfCellsSetInInputSudokuBoard, numberOfCellsSetInInputSudokuBoard);
            else
                message = string.Format("The sudoku was partially solved. {0} numbers added to the original {1}. Unable to set {2} number(s).", numberOfCellsSetInBestSoFar - numberOfCellsSetInInputSudokuBoard, numberOfCellsSetInInputSudokuBoard, 81 - numberOfCellsSetInBestSoFar);

            PrintSudokuBoard(numberOfCellsSetInBestSoFar == 81 ? true : false, args, message, bestSoFarSudokuBoard);

            Console.Write(message);
        }


        private static void CopySudokuBoard(int[][] sudokuBoardFrom, int[][] sudokuBoardTo)
        {
            int row, column;

            for (row = 1; row <= 9; row++)
            {
                for (column = 1; column <= 9; column++)
                {
                    sudokuBoardTo[row - 1][column - 1] = sudokuBoardFrom[row - 1][column - 1];
                }
            }
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

        private static string GetInputSudokuBoard(string[] args, int[][] sudokuBoard, ArrayList cellsRemainToSet)
        {
            string[] rows, columns;
            int row, column, n;

            if (args.Length == 0)
            {
                return "An input file is not given to the program!";
            }
            else if (!File.Exists(args[0]))
            {
                return "The given input file does not exist!";
            }

            FileStream fileStream = new FileStream(args[0], FileMode.Open, FileAccess.Read);
            StreamReader streamReader = new StreamReader(fileStream, Encoding.ASCII);
            string sudokuBoardString = streamReader.ReadToEnd().Trim().Replace("\r\n", "\n");
            streamReader.Close();
            fileStream.Close();

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
                        return string.Format("The value, \"{0}\", in row {1} and column {2} in input file is not a valid integer!", columns[column - 1], row.ToString(), column.ToString());
                    }

                    if (n < 0 || n > 9)
                    {
                        return string.Format("The value, \"{0}\", in row {1} and column {2} in input file is not an integer in the interval [0,9] as expected!", columns[column - 1], row.ToString(), column.ToString());
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

        private static int ReturnSquare(int row, int column)
        {
            return 1 + (3 * ((row - 1) / 3)) + ((column - 1) / 3);
        }

        private static int[][][] ReturnSquareToCellMapper()
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
                    square = ReturnSquare(row, column);
                    v[square - 1][index[square - 1]][0] = row;
                    v[square - 1][index[square - 1]][1] = column;
                    index[square - 1]++;
                }
            }

            return v;
        }

        private static int ReturnNumberOfOccurenciesOfNumber(int[][] sudokuBoard, int[][][] squareCellToRowColumnMapper, int number, int t, Target target) //t refers to a row, column or square
        {
            int row = 0, column = 0, n = 0;

            for (int i = 0; i < 9; i++)
            {
                switch(target)
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

        private static string ValidateSudokuBoard(int[][] sudokuBoard, int[][][] squareCellToRowColumnMapper)
        {
            int row, column, square, number;

            for (row = 1; row <= 9; row++)
            {
                for (column = 1; column <= 9; column++)
                {
                    square = ReturnSquare(row, column);

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

        private static int InitCandidates(int[][] sudokuBoard, int[][][] squareCellToRowColumnMapper, int[][][] candidates)
        {
            int row, column, square, number, numberOfCandidates, n;

            numberOfCandidates = 0;

            for (row = 1; row <= 9; row++)
            {
                for (column = 1; column <= 9; column++)
                {
                    square = ReturnSquare(row, column);

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

        private static bool NumberIsAloneCandidate(int number, int[][][] candidates, int[][][] squareCellToRowColumnMapper, int t, Target target)
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

                if (n != -1)
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

        private static int UpdateCandidates(int[][][] candidates, int[][][] squareCellToRowColumnMapper, int row, int column, int number)
        {
            int i, r, c, square, totalNumberOfCandidatesRemoved;

            totalNumberOfCandidatesRemoved = candidates[row - 1][column - 1][0]; //Remove all candidates in that cell
            candidates[row - 1][column - 1][0] = -1; //Indicates that the cell is set already

            square = ReturnSquare(row, column);

            for (i = 1; i <= 9; i++)
            {
                if ((i != column) && (candidates[row - 1][i - 1][0] > 0))
                {
                    totalNumberOfCandidatesRemoved += RemoveNumberIfItExists(candidates[row - 1][i - 1], number);
                }
            }

            for (i = 1; i <= 9; i++)
            {
                if ((i != row) && (candidates[i - 1][column - 1][0] > 0))
                {
                    totalNumberOfCandidatesRemoved += RemoveNumberIfItExists(candidates[i - 1][column - 1], number);
                }
            }

            for (i = 0; i < 9; i++)
            {
                r = squareCellToRowColumnMapper[square - 1][i][0];
                c = squareCellToRowColumnMapper[square - 1][i][1];

                if ((r != row) && (c != column) && (candidates[r - 1][c - 1][0] > 0))
                {
                    totalNumberOfCandidatesRemoved += RemoveNumberIfItExists(candidates[r - 1][c - 1], number);
                }
            }

            return totalNumberOfCandidatesRemoved;
        }

        private static void Copy(ArrayList from, ArrayList to)
        {
            to.Clear();

            for(int i = 0; i < from.Count; i++)
            {
                to.Add(from[i]);
            }
        }

        private static string ReturnSudokuBoardsAsString(int[][] sudokuBoard)
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

        private static void PrintSudokuBoard(bool solved, string[] args, string message, int[][] sudokuBoard)
        {
            FileStream fileStream;
            StreamWriter streamWriter;
            string suffix, fileNameFullpath;
            char c;

            if (solved)
                suffix = (new FileInfo(args[0])).Name + string.Format("__Solved_{0}.txt", DateTime.Now.ToString("yyyy.MM.dd.HH.mm.ss.fff"));
            else
                suffix = (new FileInfo(args[0])).Name + string.Format("__Partially_solved_{0}.txt", DateTime.Now.ToString("yyyy.MM.dd.HH.mm.ss.fff"));

            if (args.Length >= 2 && Directory.Exists(args[1]))
            {
                c = args[1].Trim()[args[1].Trim().Length - 1];
                fileNameFullpath = args[1].Trim() + ((c == '\\') ? "" : "\\") + (new FileInfo(args[0])).Name + suffix;
            }
            else
                fileNameFullpath = args[0] + suffix;

            fileStream = new FileStream(fileNameFullpath, FileMode.Create, FileAccess.Write);
            streamWriter = new StreamWriter(fileStream, Encoding.ASCII);
            streamWriter.WriteLine(message);
            streamWriter.WriteLine();
            streamWriter.Write(ReturnSudokuBoardsAsString(sudokuBoard));
            streamWriter.Flush();
            fileStream.Flush();
            streamWriter.Close();
            fileStream.Close();
        }
    }

    public class SudokuDebug
    {
        private int[][][] _squareCellToRowColumnMapper;
        private string _debugDirectory;
        private int _tryNumber;
        private int _numberAdded;


        public SudokuDebug()
        {
            _squareCellToRowColumnMapper = ReturnSquareToCellMapper();
            _debugDirectory = "C:\\SudokuDebug\\";
            _tryNumber = 0;
            _numberAdded = 0;
        }

        public string Log(int tryNumber, int row, int column, int number, string category, int[][] sudokuBoard, int[][][] candidates, int numberOfCandidates, ArrayList cellsRemainToSet)
        {
            FileStream fileStream;
            StreamWriter streamWriter;
            string fileNameFullpath;

            if (_tryNumber != tryNumber)
            {
                _tryNumber = tryNumber;
                _numberAdded = 1;
            }
            else
                _numberAdded++;

            fileNameFullpath = string.Format("{0}Try{1}AddNumber{2}.txt", _debugDirectory, tryNumber.ToString(), _numberAdded.ToString());

            fileStream = new FileStream(fileNameFullpath, FileMode.Create, FileAccess.Write);
            streamWriter = new StreamWriter(fileStream, Encoding.ASCII);

            streamWriter.WriteLine(string.Format("(row, column, number, category) = ({0}, {1}, {2}, {3})", row.ToString(), column.ToString(), number.ToString(), category));

            streamWriter.WriteLine();
            streamWriter.WriteLine("Data before update:");
            streamWriter.WriteLine();
            Log(streamWriter, sudokuBoard, candidates, numberOfCandidates, cellsRemainToSet);
            streamWriter.Flush();
            fileStream.Flush();
            streamWriter.Close();
            fileStream.Close();

            return fileNameFullpath;
        }

        public void Log(string fileNameFullpath, int[][] sudokuBoard, int[][][] candidates, int numberOfCandidates, ArrayList cellsRemainToSet)
        {
            FileStream fileStream;
            StreamWriter streamWriter;

            fileStream = new FileStream(fileNameFullpath, FileMode.Append, FileAccess.Write);
            streamWriter = new StreamWriter(fileStream, Encoding.ASCII);

            streamWriter.WriteLine();
            streamWriter.WriteLine("Data after update:");
            streamWriter.WriteLine();
            Log(streamWriter, sudokuBoard, candidates, numberOfCandidates, cellsRemainToSet);
            streamWriter.Flush();
            fileStream.Flush();
            streamWriter.Close();
            fileStream.Close();
        }

        private void Log(StreamWriter streamWriter, int[][] sudokuBoard, int[][][] candidates, int numberOfCandidates, ArrayList cellsRemainToSet)
        {
            streamWriter.WriteLine("Sudoku board:");
            streamWriter.WriteLine(ReturnSudokuBoardsAsString(sudokuBoard));
            streamWriter.WriteLine();
            streamWriter.WriteLine(string.Format("Cells remain to set ({0} cells): {1}", cellsRemainToSet.Count.ToString(), ReturnCellsRemainToSet(cellsRemainToSet)));
            streamWriter.WriteLine();
            streamWriter.WriteLine("Number Of candidates: " + numberOfCandidates.ToString());
            streamWriter.WriteLine();
            streamWriter.WriteLine("Candidates:");
            streamWriter.WriteLine(ReturnCandidatesAsString(candidates, sudokuBoard));
            streamWriter.WriteLine();
            streamWriter.WriteLine(ReturnAllCandidatesAsString(candidates));
            streamWriter.WriteLine();
        }

        private string ReturnCellsRemainToSet(ArrayList cellsRemainToSet)
        {
            int i, row, column;
            StringBuilder sb = new StringBuilder();
 
            for(i = 0; i < cellsRemainToSet.Count; i++)
            {
                row = ((int[])cellsRemainToSet[i])[0];
                column = ((int[])cellsRemainToSet[i])[1];

                if (i == 0)
                    sb.Append(string.Format("({0}, {1})", row.ToString(), column.ToString()));
                else
                    sb.Append(string.Format(" ({0}, {1})", row.ToString(), column.ToString()));
            }

            return sb.ToString();
        }

        private string ReturnSudokuBoardsAsString(int[][] sudokuBoard)
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

        private int[][][] ReturnThreeDimensionalDataStructure(int l, int m, int n)
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

        private int ReturnSquare(int row, int column)
        {
            return 1 + (3 * ((row - 1) / 3)) + ((column - 1) / 3);
        }

        private int[][][] ReturnSquareToCellMapper()
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
                    square = ReturnSquare(row, column);
                    v[square - 1][index[square - 1]][0] = row;
                    v[square - 1][index[square - 1]][1] = column;
                    index[square - 1]++;
                }
            }

            return v;
        }

        private string ReturnCandidates(int[] v)
        {
            int i, n = v[0];
            StringBuilder sb = new StringBuilder();

            for (i = 0; i < n; i++)
            {
                if (i == 0)
                    sb.Append(v[i + 1].ToString());
                else
                    sb.Append(string.Format(", {0}", v[i + 1].ToString()));
            }

            return sb.ToString();
        }

        private string ReturnCandidatesAsString(int[][][] candidates, int[][] sudokuBoard)
        {
            int row, column;
            string candidatesString;
            StringBuilder sb = new StringBuilder();

            for (row = 1; row <= 9; row++)
            {
                for (column = 1; column <= 9; column++)
                {
                    if (sudokuBoard[row - 1][column - 1] == 0)
                    {
                        candidatesString = ReturnCandidates(candidates[row - 1][column - 1]);
                        sb.Append(string.Format("({0}, {1}): {2}\r\n", row, column, candidatesString));
                    }
                    else
                    {
                        if (candidates[row - 1][column - 1][0] != -1)
                        {
                            throw new Exception("(candidates[row - 1][column - 1][0] != -1)");
                        }

                        sb.Append(string.Format("({0}, {1}): Set already to {2}\r\n", row, column, sudokuBoard[row - 1][column - 1]));
                    }
                }
            }

            return sb.ToString().TrimEnd();
        }

        private string ReturnAllCandidatesAsString(int[][][] candidates)
        {
            int row, column, square;
            StringBuilder sb = new StringBuilder();

            sb.Append("Candidates in the rows:\r\n");

            for(row = 1; row <= 9; row++)
            {
                sb.Append(string.Format("{0}: {1}\r\n", row, ReturnAllCandidatesInRow(row, candidates)));
            }

            sb.Append("\r\nCandidates in the columns:\r\n");

            for (column = 1; column <= 9; column++)
            {
                sb.Append(string.Format("{0}: {1}\r\n", column, ReturnAllCandidatesInColumn(column, candidates)));
            }

            sb.Append("\r\nCandidates in the squares:\r\n");

            for (square = 1; square <= 9; square++)
            {
                sb.Append(string.Format("{0}: {1}\r\n", square, ReturnAllCandidatesInSquare(square, candidates)));
            }

            return sb.ToString().TrimEnd();
        }

        private string ReturnAllCandidatesInRow(int row, int[][][] candidates)
        {
            int column, i, n;
            ArrayList arrayList = new ArrayList();
            StringBuilder sb = new StringBuilder();

            for (column = 1; column <= 9; column++)
            {
                if (candidates[row - 1][column - 1][0] != -1)
                {
                    n = candidates[row - 1][column - 1][0];

                    for(i = 0; i < n; i++)
                    {
                        arrayList.Add(candidates[row - 1][column - 1][1 + i]);
                    }
                }
            }

            arrayList.Sort();
            sb.Clear();

            for(i = 0; i < arrayList.Count; i++)
            {
                if (i == 0)
                    sb.Append(arrayList[i].ToString());
                else
                    sb.Append(string.Format(", {0}", arrayList[i].ToString()));
            }

            sb.Append(string.Format(" (A total of {0} candidates)", arrayList.Count.ToString()));

            return sb.ToString();
        }

        private string ReturnAllCandidatesInColumn(int column, int[][][] candidates)
        {
            int row, i, n;
            ArrayList arrayList = new ArrayList();
            StringBuilder sb = new StringBuilder();

            for (row = 1; row <= 9; row++)
            {
                if (candidates[row - 1][column - 1][0] != -1)
                {
                    n = candidates[row - 1][column - 1][0];

                    for (i = 0; i < n; i++)
                    {
                        arrayList.Add(candidates[row - 1][column - 1][1 + i]);
                    }
                }
            }

            arrayList.Sort();
            sb.Clear();

            for (i = 0; i < arrayList.Count; i++)
            {
                if (i == 0)
                    sb.Append(arrayList[i].ToString());
                else
                    sb.Append(string.Format(", {0}", arrayList[i].ToString()));
            }

            sb.Append(string.Format(" (A total of {0} candidates)", arrayList.Count.ToString()));

            return sb.ToString();
        }

        private string ReturnAllCandidatesInSquare(int square, int[][][] candidates)
        {
            int row, column, i, j, n;
            ArrayList arrayList = new ArrayList();
            StringBuilder sb = new StringBuilder();

            for (i = 1; i <= 9; i++)
            {
                row = _squareCellToRowColumnMapper[square - 1][i - 1][0];
                column = _squareCellToRowColumnMapper[square - 1][i - 1][1];

                if (candidates[row - 1][column - 1][0] != -1)
                {
                    n = candidates[row - 1][column - 1][0];

                    for (j = 0; j < n; j++)
                    {
                        arrayList.Add(candidates[row - 1][column - 1][1 + j]);
                    }
                }
            }

            arrayList.Sort();
            sb.Clear();

            for (i = 0; i < arrayList.Count; i++)
            {
                if (i == 0)
                    sb.Append(arrayList[i].ToString());
                else
                    sb.Append(string.Format(", {0}", arrayList[i].ToString()));
            }

            sb.Append(string.Format(" (A total of {0} candidates)", arrayList.Count.ToString()));

            return sb.ToString();
        }
    }
}
