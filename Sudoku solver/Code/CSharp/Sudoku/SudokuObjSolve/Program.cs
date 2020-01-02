using System;
using System.Collections;
using System.IO;
using System.Text;

namespace SudokuObj
{
    public enum Target
    {
        Row,
        Column,
        Square
    }

    static class Utility
    {
        private static int[][][] _squareCellToRowColumnMapper;

        static Utility()
        {
            int[] index;
            int i, row, column, square;

            _squareCellToRowColumnMapper = ReturnThreeDimensionalDataStructure(9, 9, 2);

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
                    _squareCellToRowColumnMapper[square - 1][index[square - 1]][0] = row;
                    _squareCellToRowColumnMapper[square - 1][index[square - 1]][1] = column;
                    index[square - 1]++;
                }
            }
        }

        public static int ReturnRow(int square, int cell)
        {
            return _squareCellToRowColumnMapper[square - 1][cell][0];
        }

        public static int ReturnColumn(int square, int cell)
        {
            return _squareCellToRowColumnMapper[square - 1][cell][1];
        }

        public static int[][] ReturnTwoDimensionalDataStructure(int m, int n)
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

        public static int[][][] ReturnThreeDimensionalDataStructure(int l, int m, int n)
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
    }

    class SudokuBoard
    {
        private int[][] _workingSudokuBoard, _certaintySudokuBoard, _bestSoFarSudokuBoard;
        private ArrayList _cellsRemainToSet, _cellsRemainToSetAfterAddedNumbersWithCertainty;

        public int NumberOfCellsSetInInputSudokuBoard { get; private set; }
        public int NumberOfCellsSetInBestSoFar { get; private set; }

        public int CurrentListIndex
        {
            get; private set;
        }

        public int CurrentRow
        {
            get; private set;
        }

        public int CurrentColumn
        {
            get; private set;
        }

        public int CellsRemainToSet
        {
            get { return _cellsRemainToSet.Count; }
        }

        public bool SudokuSolved
        {
            get { return _cellsRemainToSet.Count == 0; }
        }

        public string SetWorkingSudokuBoard(int index, string[] sudokuArray)
        {
            string[] rows, columns;
            int row, column, n;

            NumberOfCellsSetInBestSoFar = 0;
            CurrentListIndex = -1;
            CurrentRow = -1;
            CurrentColumn = -1;
            _cellsRemainToSet.Clear();
            _cellsRemainToSetAfterAddedNumbersWithCertainty.Clear();

            string sudokuBoardString = sudokuArray[index].Trim().Replace("\r\n", "\n");

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

                    _workingSudokuBoard[row - 1][column - 1] = n;

                    if (n == 0)
                    {
                        _cellsRemainToSet.Add(new int[] { row, column });
                    }
                }
            }

            NumberOfCellsSetInInputSudokuBoard = 81 - _cellsRemainToSet.Count;

            return null;
        }

        public void Init()
        {
            _cellsRemainToSet = new ArrayList();
            _cellsRemainToSetAfterAddedNumbersWithCertainty = new ArrayList();
            _workingSudokuBoard = Utility.ReturnTwoDimensionalDataStructure(9, 9);
            _certaintySudokuBoard = Utility.ReturnTwoDimensionalDataStructure(9, 9);
            _bestSoFarSudokuBoard = Utility.ReturnTwoDimensionalDataStructure(9, 9);   
        }

        public void SaveState() //Called at most once
        {
            CopySudokuBoard(_workingSudokuBoard, _certaintySudokuBoard);
            CopyList(_cellsRemainToSet, _cellsRemainToSetAfterAddedNumbersWithCertainty);
        }

        public void RestoreState()
        {
            CopySudokuBoard(_certaintySudokuBoard, _workingSudokuBoard);
            CopyList(_cellsRemainToSetAfterAddedNumbersWithCertainty, _cellsRemainToSet);
        }

        public void CheckIfCanUpdateBestSoFarSudokuBoard()
        {
            if (NumberOfCellsSetInBestSoFar < (81 - _cellsRemainToSet.Count))
            {
                NumberOfCellsSetInBestSoFar = 81 - _cellsRemainToSet.Count;
                CopySudokuBoard(_workingSudokuBoard, _bestSoFarSudokuBoard);
            }
        }

        public void ResetList()
        {
            CurrentListIndex = -1;
        }

        public bool NextCellInList()
        {
            CurrentListIndex++;

            if (CurrentListIndex < _cellsRemainToSet.Count)
            {
                CurrentRow = ((int[])_cellsRemainToSet[CurrentListIndex])[0];
                CurrentColumn = ((int[])_cellsRemainToSet[CurrentListIndex])[1];
                return true;
            }
            else
            {
                CurrentRow = -1;
                CurrentColumn = -1;
                return false;
            }
        }

        public bool IsSet(int row, int column)
        {
            return _workingSudokuBoard[row - 1][column - 1] != 0;
        }

        public int ReturnNumberOfOccurenciesOfNumber(int number, int t, Target target) //t refers to a row, column or square
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
                        row = Utility.ReturnRow(t, i);
                        column = Utility.ReturnColumn(t, i);
                        break;
                }

                if (_workingSudokuBoard[row - 1][column - 1] == number)
                    n++;
            }

            return n;
        }

        public void SetNumber(int row, int column, int index, int number)
        {
            _workingSudokuBoard[row - 1][column - 1] = number;
            _cellsRemainToSet.RemoveAt(index);
        }

        public string ReturnSudokuBoardAsString()
        {
            int[][] sudokuBoard;
            int row, column;
            StringBuilder sb = new StringBuilder();

            if (_cellsRemainToSet.Count == 0 || _bestSoFarSudokuBoard == null)
                sudokuBoard = _workingSudokuBoard;
            else
                sudokuBoard = _bestSoFarSudokuBoard;

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

        private void CopySudokuBoard(int[][] sudokuBoardFrom, int[][] sudokuBoardTo)
        {
            for (int row = 1; row <= 9; row++)
            {
                for (int column = 1; column <= 9; column++)
                {
                    sudokuBoardTo[row - 1][column - 1] = sudokuBoardFrom[row - 1][column - 1];
                }
            }
        }

        private void CopyList(ArrayList from, ArrayList to)
        {
            to.Clear();

            for (int i = 0; i < from.Count; i++)
            {
                to.Add(from[i]);
            }
        }
    }

    class Candidates
    {
        private int[][][] _candidates, _candidatesAfterAddedNumbersWithCertainty;
        private int _numberOfCandidates, _numberOfCandidatesAfterAddedNumbersWithCertainty;
        private Random _random;

        public bool HasCandidates
        {
            get { return _numberOfCandidates > 0; }
        }

        public void Init()
        {
            _candidates = Utility.ReturnThreeDimensionalDataStructure(9, 9, 10);
            _random = new Random((int)(DateTime.Now.Ticks % 64765L));
            _candidatesAfterAddedNumbersWithCertainty = Utility.ReturnThreeDimensionalDataStructure(9, 9, 10);
        }

        public void SetCandidates(SudokuBoard sudokuBoard)
        {
            int row, column, square, number, n;

            _numberOfCandidates = 0;

            for (row = 1; row <= 9; row++)
            {
                for (column = 1; column <= 9; column++)
                {
                    square = 1 + (3 * ((row - 1) / 3)) + ((column - 1) / 3);

                    if (sudokuBoard.IsSet(row, column))
                    {
                        _candidates[row - 1][column - 1][0] = -1; //Indicates that the cell is set already
                    }
                    else
                    {
                        n = 0;
                        this._candidates[row - 1][column - 1][0] = 0; //Number of candidates is set in index 0

                        for (number = 1; number <= 9; number++)
                        {
                            if (
                                (sudokuBoard.ReturnNumberOfOccurenciesOfNumber(number, row, Target.Row) == 0) &&
                                (sudokuBoard.ReturnNumberOfOccurenciesOfNumber(number, column, Target.Column) == 0) &&
                                (sudokuBoard.ReturnNumberOfOccurenciesOfNumber(number, square, Target.Square) == 0)
                                )
                            {
                                n++;
                                _candidates[row - 1][column - 1][0] = n;
                                _candidates[row - 1][column - 1][n] = number;
                                _numberOfCandidates++;
                            }
                        }
                    }
                }
            }
        }

        public int TryFindNumberToSetInCellWithCertainty(int row, int column)
        {
            int i, square, numberOfCandidatesInCell, candidate, number = 0;

            square = 1 + (3 * ((row - 1) / 3)) + ((column - 1) / 3);
            numberOfCandidatesInCell = _candidates[row - 1][column - 1][0];

            if (numberOfCandidatesInCell == 1)
            {
                number = _candidates[row - 1][column - 1][1];
            }
            else if (numberOfCandidatesInCell > 1)
            {
                i = 1;
                while (i <= numberOfCandidatesInCell && number == 0)
                {
                    candidate = _candidates[row - 1][column - 1][i];

                    if (CandidateIsAlonePossible(candidate, row, Target.Row) ||
                        CandidateIsAlonePossible(candidate, column, Target.Column) ||
                        CandidateIsAlonePossible(candidate, square, Target.Square))
                        number = candidate;
                    else
                        i++;
                }
            }

            return number;
        }

        public void SaveState() //Called at most once
        {
            _numberOfCandidatesAfterAddedNumbersWithCertainty = _numberOfCandidates;
            Copy(_candidates, _candidatesAfterAddedNumbersWithCertainty);
        }

        public void RestoreState()
        {
            _numberOfCandidates = _numberOfCandidatesAfterAddedNumbersWithCertainty;
            Copy(_candidatesAfterAddedNumbersWithCertainty, _candidates);
        }

        public void UpdateCandidates(int row, int column, int number)
        {
            int i, r, c, square, totalNumberOfCandidatesRemoved;

            totalNumberOfCandidatesRemoved = _candidates[row - 1][column - 1][0]; //Remove all candidates in that cell
            _candidates[row - 1][column - 1][0] = -1; //Indicates that the cell is set already

            square = 1 + (3 * ((row - 1) / 3)) + ((column - 1) / 3);

            for (c = 1; c <= 9; c++)
            {
                if (c != column && _candidates[row - 1][c - 1][0] > 0)
                {
                    totalNumberOfCandidatesRemoved += RemoveNumberIfItExists(_candidates[row - 1][c - 1], number);
                }
            }

            for (r = 1; r <= 9; r++)
            {
                if (r != row && _candidates[r - 1][column - 1][0] > 0)
                {
                    totalNumberOfCandidatesRemoved += RemoveNumberIfItExists(_candidates[r - 1][column - 1], number);
                }
            }

            for (i = 0; i < 9; i++)
            {
                r = Utility.ReturnRow(square, i);
                c = Utility.ReturnColumn(square, i);

                if (r != row && c != column && _candidates[r - 1][c - 1][0] > 0)
                {
                    totalNumberOfCandidatesRemoved += RemoveNumberIfItExists(_candidates[r - 1][c - 1], number);
                }
            }

            _numberOfCandidates -= totalNumberOfCandidatesRemoved;
        }

        public void SimulateOneNumber(SudokuBoard sudokuBoard, out int row, out int column, out int index, out int number)
        {
            int tmp, numberOfCandidates, minNumberOfCandidates = 9;
            ArrayList v;

            sudokuBoard.ResetList();
            while(sudokuBoard.NextCellInList())
            {
                numberOfCandidates = _candidates[sudokuBoard.CurrentRow - 1][sudokuBoard.CurrentColumn - 1][0];

                if (numberOfCandidates > 0 && numberOfCandidates < minNumberOfCandidates)
                    minNumberOfCandidates = numberOfCandidates;
            }

            v = new ArrayList();

            sudokuBoard.ResetList();
            while (sudokuBoard.NextCellInList())
            {
                if (_candidates[sudokuBoard.CurrentRow - 1][sudokuBoard.CurrentColumn - 1][0] == minNumberOfCandidates)
                    v.Add(new int[] { sudokuBoard.CurrentRow, sudokuBoard.CurrentColumn, sudokuBoard.CurrentListIndex });
            }

            tmp = _random.Next(0, v.Count);
            row = ((int[])v[tmp])[0];
            column = ((int[])v[tmp])[1];
            index = ((int[])v[tmp])[2];
            number = _candidates[row - 1][column - 1][1 + _random.Next(0, minNumberOfCandidates)];
        }

        /// <summary>
        ///  Returns 1 if number exists, otherwise 0
        /// </summary>
        private int RemoveNumberIfItExists(int[] v, int number)
        {
            int i = 1, index = -1, returnValue = 0;

            while (i <= v[0] && number >= v[i] && index == -1) //The numbers in v are in increasing order
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
                while (index + 1 <= v[0])
                {
                    v[index] = v[index + 1];
                    index++;
                }

                v[0]--;
            }

            return returnValue;
        }

        private bool CandidateIsAlonePossible(int number, int t, Target target)
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
                        row = Utility.ReturnRow(t, i);
                        column = Utility.ReturnColumn(t, i);
                        break;
                }

                n = _candidates[row - 1][column - 1][0];

                if (n > 0)
                {
                    for (j = 0; j < n; j++)
                    {
                        if (_candidates[row - 1][column - 1][1 + j] == number)
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

        private void Copy(int[][][] candidatesFrom, int[][][] candidatesTo)
        {
            for (int row = 1; row <= 9; row++)
            {
                for (int column = 1; column <= 9; column++)
                {
                    for (int i = 0; i < 10; i++)
                    {
                        candidatesTo[row - 1][column - 1][i] = candidatesFrom[row - 1][column - 1][i];
                    }
                }
            }
        }
    }

    class SudokuSolver
    {
        static string Run(string[] sudokuArray)
        {
            int row, column, index, number, maxNumberOfAttemptsToSolveSudoku = 1000, numberOfAttemptsToSolveSudoku = 0;
            bool atLeastOneSimulation = false, stateHasBeenSaved = false, numbersAddedWithCertaintyAndThenNoCandidates = false;
            SudokuBoard sudokuBoard = new SudokuBoard();
            Candidates candidates = new Candidates();
            int currentSudokuToSolve, numberOfSudokusToSolve = sudokuArray.Length;
            string result = "S";

            sudokuBoard.Init();
            candidates.Init();

            currentSudokuToSolve = 1;

            while (currentSudokuToSolve <= numberOfSudokusToSolve && result == "S")
            {
                numberOfAttemptsToSolveSudoku = 0;
                atLeastOneSimulation = false;
                stateHasBeenSaved = false;
                numbersAddedWithCertaintyAndThenNoCandidates = false;
                sudokuBoard.SetWorkingSudokuBoard(currentSudokuToSolve - 1, sudokuArray);
                candidates.SetCandidates(sudokuBoard);

                while (numberOfAttemptsToSolveSudoku < maxNumberOfAttemptsToSolveSudoku && !sudokuBoard.SudokuSolved && !numbersAddedWithCertaintyAndThenNoCandidates)
                {
                    if (numberOfAttemptsToSolveSudoku > 0)
                    {
                        sudokuBoard.RestoreState();
                        candidates.RestoreState();
                    }

                    while (candidates.HasCandidates)
                    {
                        number = 0;
                        sudokuBoard.ResetList();

                        while (number == 0 && sudokuBoard.NextCellInList())
                        {
                            number = candidates.TryFindNumberToSetInCellWithCertainty(sudokuBoard.CurrentRow, sudokuBoard.CurrentColumn);
                        }

                        if (number == 0)
                        {
                            candidates.SimulateOneNumber(sudokuBoard, out row, out column, out index, out number);
                            atLeastOneSimulation = true;

                            if (!stateHasBeenSaved)
                            {
                                sudokuBoard.SaveState();
                                candidates.SaveState();
                                stateHasBeenSaved = true;
                            }
                        }
                        else
                        {
                            row = sudokuBoard.CurrentRow;
                            column = sudokuBoard.CurrentColumn;
                            index = sudokuBoard.CurrentListIndex;
                        }

                        sudokuBoard.SetNumber(row, column, index, number);
                        candidates.UpdateCandidates(row, column, number);
                    }

                    if (!sudokuBoard.SudokuSolved)
                    {
                        if (!atLeastOneSimulation)
                            numbersAddedWithCertaintyAndThenNoCandidates = true;
                        else
                        {
                            sudokuBoard.CheckIfCanUpdateBestSoFarSudokuBoard();
                            numberOfAttemptsToSolveSudoku++;
                        }
                    }
                }

                result = ProcessResult(sudokuBoard, currentSudokuToSolve - 1, sudokuArray);

                if ((currentSudokuToSolve % 500) == 0)
                {
                    Console.Write("\r" + currentSudokuToSolve.ToString());
                }

                currentSudokuToSolve++;
            }

            return result;
        }

        private static string ProcessResult(SudokuBoard sudokuBoard, int index, string[] sudokuArray)
        {
            string msg;

            if (sudokuBoard.SudokuSolved)
            {
                msg = "S";
            }
            else
            {
                msg = string.Format("The sudoku was partially solved. {0} number(s) added to the original {1}. Unable to set {2} number(s).", sudokuBoard.NumberOfCellsSetInBestSoFar - sudokuBoard.NumberOfCellsSetInInputSudokuBoard, sudokuBoard.NumberOfCellsSetInInputSudokuBoard, 81 - sudokuBoard.NumberOfCellsSetInBestSoFar);
            }

            sudokuArray[index] = sudokuBoard.ReturnSudokuBoardAsString();

            return msg;
        }

        static void Main(string[] args)
        {
            DateTime dt = DateTime.Now;
            string start, end, result;
            StringBuilder sb;
            string[] sudokuArray;
            int i, n;

            start = string.Format("{0},{1},{2},{3},{4},{5}\r\n", dt.Year.ToString(), dt.Month.ToString(), dt.Day.ToString(), dt.Hour, dt.Minute.ToString(), dt.Second.ToString());

            sudokuArray = File.ReadAllText("C:\\Sudoku\\Solve\\StartSudokuBoards.txt").Split(new string[] { "\r\n-- New sudoku --\r\n" }, StringSplitOptions.None);

            i = 0;
            n = sudokuArray.Length;

            result = Run(sudokuArray);

            if (result != "S")
            {
                File.WriteAllText("C:\\Sudoku\\Solve\\Error.txt", result);
            }
            else
            {
                dt = DateTime.Now;
                end = string.Format("{0},{1},{2},{3},{4},{5}", dt.Year.ToString(), dt.Month.ToString(), dt.Day.ToString(), dt.Hour, dt.Minute.ToString(), dt.Second.ToString());
                File.WriteAllText("C:\\Sudoku\\Solve\\StartEnd.txt", start + end);

                sb = new StringBuilder();

                for (i = 0; i < n; i++)
                {
                    if (i == (n - 1))
                        sb.Append(sudokuArray[i]);
                    else
                        sb.Append(sudokuArray[i] + "\r\n-- New sudoku --\r\n");
                }

                File.WriteAllText("C:\\Sudoku\\Solve\\SolveResult.txt", sb.ToString());
            }
        }
    }
}
