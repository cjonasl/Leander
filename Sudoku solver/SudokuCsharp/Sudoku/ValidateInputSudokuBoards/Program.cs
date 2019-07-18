using System;
using System.Collections;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace ValidateInputSudokuBoards
{
    public enum Target
    {
        Row,
        Column,
        Square
    }

    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                int i, index;
                string str = "", msg = null;
                ArrayList inputSudokuBoards = new ArrayList(), sudokuBoardStrings = new ArrayList();
                ArrayList cellsRemainToSet = new ArrayList();
                int[][][] squareCellToRowColumnMapper;
                int[][] workingSudokuBoard = ReturnTwoDimensionalDataStructure(9, 9);
                string[] v = Directory.GetFiles(Directory.GetCurrentDirectory(), "*.txt");
                string[] strArray = new string[1];
                
                i = 0;
                squareCellToRowColumnMapper = ReturnSquareToCellMapper();

                while (msg == null && i < v.Length)
                {
                    cellsRemainToSet.Clear();
                    strArray[0] = v[i];
                    msg = GetInputSudokuBoard(strArray, workingSudokuBoard, cellsRemainToSet);

                    if (msg == null)
                        msg = ValidateSudokuBoard(workingSudokuBoard, squareCellToRowColumnMapper);

                    if (msg == null)
                        str = ReturnSudokuBoardStrings(workingSudokuBoard);

                    index = sudokuBoardStrings.IndexOf(str);

                    if (index >= 0)
                        msg = string.Format("The sudokus in file {0} and {1} are the same!", v[i], v[index]);
                    else
                        sudokuBoardStrings.Add(str);

                    i++;
                }

                if (msg == null)
                    Console.Write("All " + v.Length.ToString() + " input sudokus are correct");
                else
                    Console.Write(string.Format("Error!! {0}", msg));
 
            }
            catch(Exception e)
            {
                Console.Write(string.Format("An exception happened! e.Message: {0}", e.Message));
            }
        }

        public static string ReturnSudokuBoardStrings(int[][] sudokuBoard)
        {
            int row, column;
            StringBuilder sb = new StringBuilder();

            for (row = 1; row <= 9; row++)
            {
                for (column = 1; column <= 9; column++)
                {
                    sb.Append(sudokuBoard[row - 1][column - 1].ToString());
                }
            }

            return sb.ToString();
        }

        private static int ReturnSquare(int row, int column)
        {
            return 1 + (3 * ((row - 1) / 3)) + ((column - 1) / 3);
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
    }
}
