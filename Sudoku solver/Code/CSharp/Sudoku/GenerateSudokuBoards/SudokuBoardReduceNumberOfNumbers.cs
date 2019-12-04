using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenerateSudokuBoards
{
    public class SudokuBoardReduceNumberOfNumbers
    {
        private int[][] _sudokuBoard;
        private int[][] _possibleNumbersToRemoveFromSudokuBoard;
        private int _cellsRemainToSet;
        private Random _random;

        public int CellsRemainToSet
        {
            get { return _cellsRemainToSet; }
        }

        public SudokuBoardReduceNumberOfNumbers()
        {
            _sudokuBoard = ReturnTwoDimensionalDataStructure(9, 9);
            _possibleNumbersToRemoveFromSudokuBoard = ReturnTwoDimensionalDataStructure(9, 9);
            _random = new Random((int)(DateTime.Now.Ticks % 62866L));
        }

        public void Init(string sudokuBoardString)
        {
            GetInputSudokuBoard(sudokuBoardString, _sudokuBoard, out _cellsRemainToSet);
            CopySudokuBoard(_sudokuBoard, _possibleNumbersToRemoveFromSudokuBoard);
        }

        public void Reduce(int row, int column)
        {
            _sudokuBoard[row - 1][column - 1] = 0;
            CopySudokuBoard(_sudokuBoard, _possibleNumbersToRemoveFromSudokuBoard);
            _cellsRemainToSet++;
        }

        public string ReturnReducedSudokuBoard(out int row, out int column)
        {
            int r, c, numberOfStepsBackwards, numberOfStepsForward, numbersInSudokuBoardNotZero;
            int rowBackwards, columnBackwards, rowForward, columnForward;
            bool foundBackwards, foundForward;
            string reducedSudokuBoardAsString;

            row = column = 0;

            r = _random.Next(1, 10);
            c = _random.Next(1, 10);

            numbersInSudokuBoardNotZero = GetNumberOfNumbersInSudokuBoardNotZero(_possibleNumbersToRemoveFromSudokuBoard);

            if (numbersInSudokuBoardNotZero == 0)
                return null;

            if (_possibleNumbersToRemoveFromSudokuBoard[r - 1][c - 1] != 0)
            {
                row = r;
                column = c;
                _possibleNumbersToRemoveFromSudokuBoard[r - 1][c - 1] = 0;
            }
            else
            {
                GoBackwards(r, c, out numberOfStepsBackwards, out rowBackwards, out columnBackwards, out foundBackwards);
                GoForward(r, c, out numberOfStepsForward, out rowForward, out columnForward, out foundForward);

                if (foundBackwards && !foundForward)
                {
                    row = rowBackwards;
                    column = columnBackwards;
                }
                else if (!foundBackwards && foundForward)
                {
                    row = rowForward;
                    column = columnForward;
                }
                else if (foundBackwards && foundForward)
                {
                    if (numberOfStepsBackwards < numberOfStepsForward)
                    {
                        row = rowBackwards;
                        column = columnBackwards;
                    }
                    else if (numberOfStepsBackwards > numberOfStepsForward)
                    {
                        row = rowForward;
                        column = columnForward;
                    }
                    else
                    {
                        int n = _random.Next(0, 2);

                        if (n == 0)
                        {
                            row = rowBackwards;
                            column = columnBackwards;
                        }
                        else
                        {
                            row = rowForward;
                            column = columnForward;
                        }
                    }
                }
                else
                {
                    throw new Exception("Can not find row and column in ReturnReducedSudokuBoard!!");
                }
            }

            reducedSudokuBoardAsString = ReturnSudokuBoardAsString(_sudokuBoard, row, column);

            return reducedSudokuBoardAsString;
        }

        private string GetInputSudokuBoard(string sudokuBoardString, int[][] sudokuBoard, out int cellsRemainToSet)
        {
            string[] rows, columns;
            int row, column, n;

            cellsRemainToSet = 0;

            rows = sudokuBoardString.Split(new string[] { "\r\n" }, StringSplitOptions.None);

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
                        cellsRemainToSet++;
                    }
                }
            }

            return null;
        }

        private int[][] ReturnTwoDimensionalDataStructure(int m, int n)
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

        private int GetNumberOfNumbersInSudokuBoardNotZero(int[][] sudokuBoard)
        {
            int n = 0;

            for (int row = 1; row <= 9; row++)
            {
                for (int column = 1; column <= 9; column++)
                {
                    if (sudokuBoard[row - 1][column - 1] != 0)
                    {
                        n++;
                    }
                }
            }

            return n;
        }

        private string ReturnSudokuBoardAsString(int[][] sudokuBoard, int r, int c)
        {
            int row, column, n;
            StringBuilder sb = new StringBuilder();

            for (row = 1; row <= 9; row++)
            {
                if (row > 1)
                    sb.Append("\r\n");

                for (column = 1; column <= 9; column++)
                {
                    if (row == r && column == c)
                    {
                        n = 0;
                    }
                    else
                    {
                        n = sudokuBoard[row - 1][column - 1];
                    }


                    if (column == 1)
                        sb.Append(n.ToString());
                    else
                        sb.Append(string.Format(" {0}", n.ToString()));
                }
            }

            return sb.ToString();
        }

        private void GoBackwards(int row, int column, out int numberOfSteps, out int r, out int c, out bool found)
        {
            found = false;

            numberOfSteps = 0;
            r = row;
            c = column;

            while (!found && r != 0)
            {
                if (c == 1)
                {
                    c = 9;
                    r--;
                }
                else
                {
                    c--;
                }

                if (r != 0)
                {
                    if (_possibleNumbersToRemoveFromSudokuBoard[r - 1][c - 1] != 0)
                    {
                        found = true;
                    }

                    numberOfSteps++;
                }
            }     
        }

        private void GoForward(int row, int column, out int numberOfSteps, out int r, out int c, out bool found)
        {
            found = false;

            numberOfSteps = 0;
            r = row;
            c = column;

            while (!found && r != 10)
            {
                if (c == 9)
                {
                    c = 1;
                    r++;
                }
                else
                {
                    c++;
                }

                if (r != 10)
                {
                    if (_possibleNumbersToRemoveFromSudokuBoard[r - 1][c - 1] != 0)
                    {
                        found = true;
                    }

                    numberOfSteps++;
                }
            }
        }
    }
}
