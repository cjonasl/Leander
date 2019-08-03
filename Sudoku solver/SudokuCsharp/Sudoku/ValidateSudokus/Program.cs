using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace ValidateSudokus
{
    class Program
    {
        public enum Target
        {
            Row,
            Column,
            Square
        }

        static void Main(string[] args)
        {
            try
            {
                DateTime min, max, tmp;
                TimeSpan ts;
                int i, n, milliSeconds, total = 0, solved = 0, partiallySolved = 0, error = 0;
                FileStream fileStream;
                StreamWriter streamWriter;
                int[][][] squareCellToRowColumnMapper;
                StringBuilder sb = new StringBuilder();
                string fileName;
                int[][] sudokuBoard = ReturnTwoDimensionalDataStructure(9, 9);

                squareCellToRowColumnMapper = ReturnSquareToCellMapper();

                min = new DateTime(2100, 1, 1);
                max = new DateTime(1900, 1, 1);

                string currentDirectory = Directory.GetCurrentDirectory();

                string[] v = Directory.GetFiles(currentDirectory, "*.txt");

                for (i = 0; i < v.Length; i++)
                {
                    fileName = (new FileInfo(v[i])).Name;

                    if (fileName.ToLower() != "result.txt")
                    {
                        GetSudoku(v[i], sudokuBoard, out tmp);

                        if (min > tmp)
                            min = tmp;

                        if (max < tmp)
                            max = tmp;

                        n = ValidateSudokuBoard(sudokuBoard, squareCellToRowColumnMapper);

                        switch(n)
                        {
                            case 0:
                                solved++;
                                sb.Append(string.Format("{0}: Solved\r\n", fileName));
                                break;
                            case 1:
                                partiallySolved++;
                                sb.Append(string.Format("{0}: Partially solved\r\n", fileName));
                                break;
                            default:
                                error++;
                                sb.Append(string.Format("{0}: Error\r\n", fileName));
                                break;
                        }
                    }
                }

                total = solved + partiallySolved + error;

                ts = max - min;
                milliSeconds = 1000 * ts.Seconds + ts.Milliseconds;

                sb.Append(string.Format("\r\nSolved: {0}\r\n", solved.ToString()));
                sb.Append(string.Format("Partially solved: {0}\r\n", partiallySolved.ToString()));
                sb.Append(string.Format("Error: {0}\r\n", error.ToString()));
                sb.Append(string.Format("Total: {0}\r\n", total.ToString()));
                sb.Append(string.Format("Time: {0} milliseconds\r\n", milliSeconds.ToString()));

                fileStream = new FileStream(currentDirectory + "\\Result.txt", FileMode.Create, FileAccess.Write);
                streamWriter = new StreamWriter(fileStream, Encoding.ASCII);
                streamWriter.Write(sb.ToString().TrimEnd());
                streamWriter.Flush();
                fileStream.Flush();
                streamWriter.Close();
                fileStream.Close();

                Console.WriteLine(string.Format("Solved: {0}", solved.ToString()));
                Console.WriteLine(string.Format("Partially solved: {0}", partiallySolved.ToString()));
                Console.WriteLine(string.Format("Error: {0}", error.ToString()));
                Console.WriteLine(string.Format("Total: {0}", total.ToString()));
                Console.WriteLine(string.Format("Time: {0} milliseconds", milliSeconds.ToString()));
            }
            catch (Exception e)
            {
                Console.Write(string.Format("An exception happened! e.Message: {0}", e.Message));
            }
        }

        private static void GetSudoku(string fileNameFullPath, int[][] sudokuBoard, out DateTime d)
        {
            int index;
            string sudokuBoardString;
            string[] rows, columns;
            int row, column;
            FileStream f = new FileStream(fileNameFullPath, FileMode.Open, FileAccess.Read);
            StreamReader r = new StreamReader(f, Encoding.ASCII);
            string str = r.ReadToEnd();
            r.Close();
            f.Close();

            index = str.IndexOf("\r\n\r\n");
            sudokuBoardString = str.Substring(4 + index);

            rows = sudokuBoardString.Split(new string[] { "\n" }, StringSplitOptions.None);

            for (row = 1; row <= 9; row++)
            {
                columns = rows[row - 1].Split(' ');

                for (column = 1; column <= 9; column++)
                {
                    sudokuBoard[row - 1][column - 1] = int.Parse(columns[column - 1]);
                }
            }

            index = fileNameFullPath.ToLower().IndexOf("solved_");

            index += 7;
            int year = int.Parse(fileNameFullPath.Substring(index, 4));

            index += 5;
            int month = int.Parse(fileNameFullPath.Substring(index, 2));

            index += 3;
            int day = int.Parse(fileNameFullPath.Substring(index, 2));

            index += 3;
            int hour = int.Parse(fileNameFullPath.Substring(index, 2));

            index += 3;
            int minute = int.Parse(fileNameFullPath.Substring(index, 2));

            index += 3;
            int second = int.Parse(fileNameFullPath.Substring(index, 2));

            index += 3;
            int millisecond = int.Parse(fileNameFullPath.Substring(index, 3));

            d = new DateTime(year, month, day, hour, minute, second, millisecond);
        }

        private static int ReturnSquare(int row, int column)
        {
            return 1 + (3 * ((row - 1) / 3)) + ((column - 1) / 3);
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

        /// <summary>
        /// Return 0 if solved, 1 if partially solved and 2 if error
        /// </summary>
        private static int ValidateSudokuBoard(int[][] sudokuBoard, int[][][] squareCellToRowColumnMapper)
        {
            int row, column, square, number, returnValue = 0;

            for (row = 1; row <= 9; row++)
            {
                for (column = 1; column <= 9; column++)
                {
                    square = ReturnSquare(row, column);
                    number = sudokuBoard[row - 1][column - 1];

                    if (number == 0)
                        returnValue = 1;
                    else if (
                            (number < 0) ||
                            (number > 9) ||
                            (ReturnNumberOfOccurenciesOfNumber(sudokuBoard, squareCellToRowColumnMapper, number, row, Target.Row) > 1) ||
                            (ReturnNumberOfOccurenciesOfNumber(sudokuBoard, squareCellToRowColumnMapper, number, column, Target.Column) > 1) ||
                            (ReturnNumberOfOccurenciesOfNumber(sudokuBoard, squareCellToRowColumnMapper, number, square, Target.Square) > 1)
                           )
                            return 2;
                }
            }

            return returnValue;
        }
    }
}
