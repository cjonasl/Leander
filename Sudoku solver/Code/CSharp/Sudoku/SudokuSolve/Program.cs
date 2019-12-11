using System;
using System.IO;
using System.Text;

namespace SudokuSolve
{
    class Program
    {
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

            result = Sudoku.Sudoku.Run(sudokuArray);

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

                for(i = 0; i < n; i++)
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
