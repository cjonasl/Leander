using System;
using System.Collections;
using System.IO;
using System.Text;
using System.Threading.Tasks;

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
        public int[][] _workingSudokuBoard, _certaintySudokuBoard, _bestSoFarSudokuBoard;
        public ArrayList _cellsRemainToSet, _cellsRemainToSetAfterAddedNumbersWithCertainty;

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

        public string Init(string[] args)
        {
            string[] rows, columns;
            int row, column, n;

            _certaintySudokuBoard = null;
            _bestSoFarSudokuBoard = null;
            NumberOfCellsSetInBestSoFar = 0;
            _cellsRemainToSet = new ArrayList();
            _cellsRemainToSetAfterAddedNumbersWithCertainty = null;
            CurrentListIndex = -1;
            CurrentRow = -1;
            CurrentColumn = -1;

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

            _workingSudokuBoard = Utility.ReturnTwoDimensionalDataStructure(9, 9);

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

        public string Validate()
        {
            int row, column, square, number;

            for (row = 1; row <= 9; row++)
            {
                for (column = 1; column <= 9; column++)
                {
                    square = 1 + (3 * ((row - 1) / 3)) + ((column - 1) / 3);
                    number = _workingSudokuBoard[row - 1][column - 1];

                    if (number != 0)
                    {
                        if (ReturnNumberOfOccurenciesOfNumber(number, row, Target.Row) > 1)
                        {
                            return string.Format("The input sudoku is incorrect! The number {0} occurs more than once in row {1}", number.ToString(), row.ToString());
                        }
                        else if (ReturnNumberOfOccurenciesOfNumber(number, column, Target.Column) > 1)
                        {
                            return string.Format("The input sudoku is incorrect! The number {0} occurs more than once in column {1}", number.ToString(), column.ToString());
                        }
                        else if (ReturnNumberOfOccurenciesOfNumber(number, square, Target.Square) > 1)
                        {
                            return string.Format("The input sudoku is incorrect! The number {0} occurs more than once in square {1}", number.ToString(), square.ToString());
                        }
                    }
                }
            }

            return null;
        }

        public void SaveState() //Called at most once
        {
            _cellsRemainToSetAfterAddedNumbersWithCertainty = new ArrayList();
            _certaintySudokuBoard = Utility.ReturnTwoDimensionalDataStructure(9, 9);
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
            if (_bestSoFarSudokuBoard == null)
                _bestSoFarSudokuBoard = Utility.ReturnTwoDimensionalDataStructure(9, 9);

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
        public int[][][] _candidates, _candidatesAfterAddedNumbersWithCertainty;
        public int _numberOfCandidates, _numberOfCandidatesAfterAddedNumbersWithCertainty;
        private Random _random;

        public bool HasCandidates
        {
            get { return _numberOfCandidates > 0; }
        }

        public void Init(SudokuBoard sudokuBoard)
        {
            int row, column, square, number, n;

            _candidates = Utility.ReturnThreeDimensionalDataStructure(9, 9, 10);
            _candidatesAfterAddedNumbersWithCertainty = null;
            _random = new Random((int)(DateTime.Now.Ticks % 64765L));
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

        public int TryFindNumberToSetInCellWithCertainty(int row, int column, string[] debugCategory)
        {
            int i, square, numberOfCandidatesInCell, candidate, number = 0;

            square = 1 + (3 * ((row - 1) / 3)) + ((column - 1) / 3);
            numberOfCandidatesInCell = _candidates[row - 1][column - 1][0];

            if (numberOfCandidatesInCell == 1)
            {
                number = _candidates[row - 1][column - 1][1];
                debugCategory[0] = "Alone in cell";
            }
            else if (numberOfCandidatesInCell > 1)
            {
                i = 1;
                while (i <= numberOfCandidatesInCell && number == 0)
                {
                    candidate = _candidates[row - 1][column - 1][i];

                    if (CandidateIsAlonePossible(candidate, row, Target.Row))
                    {
                        number = candidate;
                        debugCategory[0] = "Alone in row";
                    }
                    else if (CandidateIsAlonePossible(candidate, column, Target.Column))
                    {
                        number = candidate;
                        debugCategory[0] = "Alone in column";
                    }
                    else if (CandidateIsAlonePossible(candidate, square, Target.Square))
                    {
                        number = candidate;
                        debugCategory[0] = "Alone in square";
                    }
                    else
                        i++;
                }
            }

            return number;
        }

        public void SaveState() //Called at most once
        {
            _numberOfCandidatesAfterAddedNumbersWithCertainty = _numberOfCandidates;
            _candidatesAfterAddedNumbersWithCertainty = Utility.ReturnThreeDimensionalDataStructure(9, 9, 10);
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

        public void SimulateOneNumber(SudokuBoard sudokuBoard, string[] debugInfo, out int row, out int column, out int index, out int number)
        {
            int tmp, numberOfCandidates, minNumberOfCandidates = 9;
            ArrayList v, debugCellsWithMinNumberOfCandidates;
            string str;

            sudokuBoard.ResetList();
            while(sudokuBoard.NextCellInList())
            {
                numberOfCandidates = _candidates[sudokuBoard.CurrentRow - 1][sudokuBoard.CurrentColumn - 1][0];

                if (numberOfCandidates > 0 && numberOfCandidates < minNumberOfCandidates)
                    minNumberOfCandidates = numberOfCandidates;
            }

            str = "minNumberOfCandidates: " + minNumberOfCandidates.ToString() + "\r\n";

            v = new ArrayList();
            debugCellsWithMinNumberOfCandidates = new ArrayList();

            v = new ArrayList();

            sudokuBoard.ResetList();
            while (sudokuBoard.NextCellInList())
            {
                if (_candidates[sudokuBoard.CurrentRow - 1][sudokuBoard.CurrentColumn - 1][0] == minNumberOfCandidates)
                {
                    v.Add(new int[] { sudokuBoard.CurrentRow, sudokuBoard.CurrentColumn, sudokuBoard.CurrentListIndex });
                    debugCellsWithMinNumberOfCandidates.Add(new int[] { sudokuBoard.CurrentRow, sudokuBoard.CurrentColumn });
                }
            }

            str += "Cells with minNumberOfCandidates (" + v.Count.ToString() + " cells): " + DebugReturnCells(debugCellsWithMinNumberOfCandidates);
            debugInfo[0] = str;

            tmp = _random.Next(0, v.Count);
            row = ((int[])v[tmp])[0];
            column = ((int[])v[tmp])[1];
            index = ((int[])v[tmp])[2];
            number = _candidates[row - 1][column - 1][1 + _random.Next(0, minNumberOfCandidates)];
        }

        private string DebugReturnCells(ArrayList cellsRemainToSet)
        {
            string str = "";

            for (int i = 0; i < cellsRemainToSet.Count; i++)
            {
                if (i > 0)
                {
                    str += " ";
                }

                str += "(" + ((int[])cellsRemainToSet[i])[0].ToString() + ", " + ((int[])cellsRemainToSet[i])[1].ToString() + ")";
            }

            return str;
        }

        /// <summary>
        ///  Returns 1 if number exists, otherwise 0
        /// </summary>
        private int RemoveNumberIfItExists(int[] v, int number)
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
        static void Main(string[] args)
        {
            int row, column, index, number, maxNumberOfAttemptsToSolveSudoku = 100, numberOfAttemptsToSolveSudoku = 0;
            bool atLeastOneSimulation = false, stateHasBeenSaved = false, numbersAddedWithCertaintyAndThenNoCandidates = false;
            SudokuBoard sudokuBoard = new SudokuBoard();
            Candidates candidates = new Candidates();
            ArrayList debugTotalCellsAdded = new ArrayList();
            string debugDirectory, debugString, debugFileNameFullPath;
            string[] debugCategory = new string[1];
            string[] debugInfo = new string[1];
            int debugTry, debugAddNumber, debugSquare;
            int[][][] squareCellToRowColumnMapper;

            squareCellToRowColumnMapper = ReturnSquareCellToRowColumnMapper();

            string msg = sudokuBoard.Init(args);

            if (msg != null)
            {
                PrintResult(false, msg);
                return;
            }

            msg = sudokuBoard.Validate();

            if (msg != null)
            {
                PrintResult(false, msg);
                return;
            }

            if (sudokuBoard.CellsRemainToSet == 0)
            {
                PrintResult(false, "A complete sudoku was given as input. There is nothing to solve.");
                return;
            }

            candidates.Init(sudokuBoard);

            if (!candidates.HasCandidates)
            {
                PrintResult(false, "It is not possible to add any number to the sudoku.");
                return;
            }

            debugDirectory = DebugCreateAndReturnDebugDirectory();
            debugTry = 0;

            while (numberOfAttemptsToSolveSudoku < maxNumberOfAttemptsToSolveSudoku && !sudokuBoard.SudokuSolved && !numbersAddedWithCertaintyAndThenNoCandidates)
            {
                debugTry += 1;
                debugAddNumber = 0;
                debugTotalCellsAdded.Clear();

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
                        number = candidates.TryFindNumberToSetInCellWithCertainty(sudokuBoard.CurrentRow, sudokuBoard.CurrentColumn, debugCategory);
                    }

                    if (number == 0)
                    {
                        candidates.SimulateOneNumber(sudokuBoard, debugInfo, out row, out column, out index, out number);
                        atLeastOneSimulation = true;

                        if (!stateHasBeenSaved)
                        {
                            sudokuBoard.SaveState();
                            candidates.SaveState();
                            stateHasBeenSaved = true;
                        }

                        debugCategory[0] = "Simulated";
                    }
                    else
                    {
                        row = sudokuBoard.CurrentRow;
                        column = sudokuBoard.CurrentColumn;
                        index = sudokuBoard.CurrentListIndex;
                    }

                    debugTotalCellsAdded.Add(new int[] { row, column });

                    debugSquare = 1 + (3 * ((row - 1) / 3)) + ((column - 1) / 3);
                    debugString = "(row, column, square, number, category) = (" + row.ToString() + ", " + column.ToString() + ", " + debugSquare.ToString() + ", " + number.ToString() + ", " + debugCategory[0] + ")\r\n\r\n";
                    debugString += "Total cells added (" + debugTotalCellsAdded.Count.ToString() + " cells): " + DebugReturnCells(debugTotalCellsAdded) + "\r\n\r\n";

                    if (debugCategory[0] == "Simulated")
                    {
                        debugString += debugInfo[0] + "\r\n\r\n";
                    }

                    debugString += "Data before update:\r\n\r\n" + DebugReturnInfo(sudokuBoard._workingSudokuBoard, sudokuBoard._cellsRemainToSet, ReturnNumberOfCandidates(candidates._candidates, true, numberOfAttemptsToSolveSudoku, debugAddNumber), candidates._candidates, squareCellToRowColumnMapper);

                    sudokuBoard.SetNumber(row, column, index, number);
                    candidates.UpdateCandidates(row, column, number);

                    debugString += "\r\nData after update:\r\n\r\n" + DebugReturnInfo(sudokuBoard._workingSudokuBoard, sudokuBoard._cellsRemainToSet, ReturnNumberOfCandidates(candidates._candidates, false, numberOfAttemptsToSolveSudoku, debugAddNumber), candidates._candidates, squareCellToRowColumnMapper);

                    debugAddNumber += 1;
                    debugFileNameFullPath = debugDirectory + "\\" + DebugReturnFileName(debugTry, debugAddNumber);
                    File.WriteAllText(debugFileNameFullPath, debugString);
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

            PrintResult(true, null, args, sudokuBoard);
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

        private static int ReturnNumberOfCandidates(int[][][] candidates, bool ch, int a, int b)
        {
            int n = 0;

            for(int row = 1; row <= 9; row++)
            {
                for (int column = 1; column <= 9; column++)
                {
                    if (candidates[row - 1][column - 1][0] != -1)
                    {
                        n += candidates[row - 1][column - 1][0];
                    }
                }
            }

            if (ch && a > 0 && b == 0)
            {
                int y = n;
            }

            return n;
        }

        private static void PrintSudokuBoard(string[] args, string message, SudokuBoard sudokuBoard)
        {
            string suffix, fileNameFullpath;
            char c;

            if (sudokuBoard.SudokuSolved)
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

            File.WriteAllText(fileNameFullpath, message + "\r\n\r\n" + sudokuBoard.ReturnSudokuBoardAsString());
        }

        private static void PrintResult(bool initialSudokuBoardHasCandidates, string msg, string[] args = null, SudokuBoard sudokuBoard = null)
        {
            if (initialSudokuBoardHasCandidates)
            {
                if (sudokuBoard.SudokuSolved)
                {
                    msg = string.Format("The sudoku was solved. {0} number(s) added to the original {1}.", 81 - sudokuBoard.NumberOfCellsSetInInputSudokuBoard, sudokuBoard.NumberOfCellsSetInInputSudokuBoard);
                }
                else
                {
                    msg = string.Format("The sudoku was partially solved. {0} number(s) added to the original {1}. Unable to set {2} number(s).", sudokuBoard.NumberOfCellsSetInBestSoFar - sudokuBoard.NumberOfCellsSetInInputSudokuBoard, sudokuBoard.NumberOfCellsSetInInputSudokuBoard, 81 - sudokuBoard.NumberOfCellsSetInBestSoFar);
                }

                PrintSudokuBoard(args, msg, sudokuBoard);
            }

            Console.Write(msg);
        }

        private static string DebugCreateAndReturnDebugDirectory()
        {
            string debugDir = "C:\\Sudoku\\Debug\\Run_" + DateTime.Now.ToString("yyyy.MM.dd.HH.mm.ss.fff");
            Directory.CreateDirectory(debugDir);
            return debugDir;
        }

        private static string DebugReturnFileName(int debugTry, int debugAddNumber)
        {
            string s1, s2;

            if (debugTry < 10)
                s1 = "00" + debugTry.ToString();
            else if (debugTry < 100)
                s1 = "0" + debugTry.ToString();
            else
                s1 = debugTry.ToString();

            if (debugAddNumber < 10)
                s2 = "0" + debugAddNumber.ToString();
            else
                s2 = debugAddNumber.ToString();

            return "Try" + s1 + "AddNumber" + s2 + ".txt";
        }

        private static string DebugReturnCells(ArrayList cellsRemainToSet)
        {
            string str = "";

            for (int i = 0; i < cellsRemainToSet.Count; i++)
            {
                if (i > 0)
                {
                    str += " ";
                }

                str += "(" + ((int[])cellsRemainToSet[i])[0].ToString() + ", " + ((int[])cellsRemainToSet[i])[1].ToString() + ")";
            }

            return str;
        }

        private static string DebugReturnCandidates(int row, int column, int[][][] candidates)
        {
            string str = "";
            int n = candidates[row - 1][column - 1][0];

            for (int i = 1; i <= n; i++)
            {
                if (i > 1)
                {
                    str += ", ";
                }

                str += candidates[row - 1][column - 1][i].ToString();
            }

            return str;
        }

        private static void DebugSort(int n, int[] v)
        {
            int tmp;

            for (int i = 0; i < n - 1; i++)
            {
                for (int j = i + 1; j < n; j++)
                {
                    if (v[j] < v[i])
                    {
                        tmp = v[j];
                        v[j] = v[i];
                        v[i] = tmp;
                    }
                }
            }
        }

        private static string DebugReturnAllCandidatesSorted(int[][][] candidates, int[] v, int[][][] squareCellToRowColumnMapper, int t, Target target)
        {
            int i, j, row = 0, column = 0, c, n = 0;
            string str = "";

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

                if (candidates[row - 1][column - 1][0] > 0)
                {
                    c = candidates[row - 1][column - 1][0];

                    for (j = 0; j < c; j++)
                    {
                        v[n] = candidates[row - 1][column - 1][1 + j];
                        n += 1;
                    }
                }
            }

            DebugSort(n, v);

            for (i = 0; i < n; i++)
            {
                if (i > 0)
                {
                    str += ", ";
                }

                str += v[i].ToString();
            }

            if (n > 0)
            {
                str += " (a total of " + n.ToString() + " candidates)";
            }

            return str;
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

        private static string DebugReturnInfo(int[][] sudokuBoard, ArrayList cellsRemainToSet, int numberOfCandidates, int[][][] candidates, int[][][] squareCellToRowColumnMapper)
        {
            string str;
            int row, column, square;
            int[] v = new int[81];

            str = "Sudoku board:\r\n" + ReturnSudokuBoardAsString(sudokuBoard) + "\r\n\r\nCells remain to set (" + cellsRemainToSet.Count.ToString() + " cells): ";
            str += DebugReturnCells(cellsRemainToSet) + "\r\n\r\n";
            str += "Number Of candidates: " + numberOfCandidates.ToString() + "\r\n\r\n";
            str += "Candidates (row, column, square, numberOfCandidate):\r\n";

            for (row = 1; row <= 9; row++)
            {
                for (column = 1; column <= 9; column++)
                {
                    square = 1 + (3 * ((row - 1) / 3)) + ((column - 1) / 3);

                    if (candidates[row - 1][column - 1][0] == -1)
                    {
                        str += "(" + row.ToString() + ", " + column.ToString() + ", " + square.ToString() + ", 0): Already set to " + sudokuBoard[row - 1][column - 1].ToString() + "\r\n";
                    }
                    else
                    {
                        str += "(" + row.ToString() + ", " + column.ToString() + ", " + square.ToString() + ", " + candidates[row - 1][column - 1][0].ToString() + "): " + DebugReturnCandidates(row, column, candidates) + "\r\n";
                    }
                }
            }

            str += "\r\nCandidates in the rows:\r\n";

            for (row = 1; row <= 9; row++)
            {
                str += row.ToString() + ": " + DebugReturnAllCandidatesSorted(candidates, v, squareCellToRowColumnMapper, row, Target.Row) + "\r\n";
            }

            str += "\r\nCandidates in the columns:\r\n";

            for (column = 1; column <= 9; column++)
            {
                str += column.ToString() + ": " + DebugReturnAllCandidatesSorted(candidates, v, squareCellToRowColumnMapper, column, Target.Column) + "\r\n";
            }

            str += "\r\nCandidates in the squares:\r\n";

            for (square = 1; square <= 9; square++)
            {
                str += square.ToString() + ": " + DebugReturnAllCandidatesSorted(candidates, v, squareCellToRowColumnMapper, square, Target.Square) + "\r\n";
            }

            return str;
        }
    }
}
