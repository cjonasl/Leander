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
            string start, end, result = "The sudoku was solved.";
            StringBuilder sb;
            string[] v;
            int i, n;

            start = string.Format("{0},{1},{2},{3},{4},{5}\r\n", dt.Year.ToString(), dt.Month.ToString(), dt.Day.ToString(), dt.Hour, dt.Minute.ToString(), dt.Second.ToString());

            v = File.ReadAllText("C:\\Sudoku\\Solve\\StartSudokuBoards.txt").Split(new string[] { "\r\n-- New sudoku --\r\n" }, StringSplitOptions.None);

            args = new string[] { "C:\\Sudoku\\Solve\\DummySudoku.txt" };

            i = 0;
            n = v.Length;

            while (i < n && result.StartsWith("The sudoku was solved."))
            {
                result = Sudoku.Sudoku.Run(args, i, v);

                if ((i % 25) == 0)
                {
                    Console.Write("\r" + i.ToString());
                }

                i++;
            }

            if (!result.StartsWith("The sudoku was solved."))
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
                        sb.Append(v[i]);
                    else
                        sb.Append(v[i] + "\r\n-- New sudoku --\r\n");
                }

                File.WriteAllText("C:\\Sudoku\\Solve\\SolveResult.txt", sb.ToString());
            }
        }
    }
}
