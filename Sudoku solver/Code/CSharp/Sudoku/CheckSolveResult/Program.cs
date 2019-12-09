using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CheckSolveResult
{
    class Program
    {
        static void Main(string[] args)
        {
            string[] start, end, v = File.ReadAllText("C:\\Sudoku\\Solve\\StartEnd.txt").Split(new string[] { "\r\n" }, StringSplitOptions.None);
            string[] startSudokuBoards = File.ReadAllText("C:\\Sudoku\\Solve\\StartSudokuBoards.txt").Split(new string[] { "\r\n-- New sudoku --\r\n" }, StringSplitOptions.None);
            string[] solvedSudokuBoards = File.ReadAllText("C:\\Sudoku\\Solve\\SolvedSudokuBoards.txt").Split(new string[] { "\r\n-- New sudoku --\r\n" }, StringSplitOptions.None);
            string[] solveResult = File.ReadAllText("C:\\Sudoku\\Solve\\SolveResult.txt").Split(new string[] { "\r\n-- New sudoku --\r\n" }, StringSplitOptions.None);
            string msg;
            bool solvedSuccessfully;


            if ((startSudokuBoards.Length != solvedSudokuBoards.Length) || (startSudokuBoards.Length != solveResult.Length) || (solvedSudokuBoards.Length != solveResult.Length))
            {
                msg = " (The number of rows in the files StartSudokuBoards.txt, SolvedSudokuBoards.txt and SolveResult.txt are not the same!)";
                solvedSuccessfully = false;
            }
            else
            {
                int i, n, same = 0, different = 0;
                StringBuilder sb = new StringBuilder("StartSudoku ExpectedResult SolvedResult\r\n");

                n = solvedSudokuBoards.Length;

                for(i = 0; i < n; i++)
                {
                    if (solvedSudokuBoards[i] == solveResult[i])
                    {
                        same++;
                    }
                    else
                    {
                        different++;

                        if (different > 1)
                        {
                            sb.Append("\r\n-- New sudokus --\r\n");
                        }

                        sb.Append(GetTheThreeSudokus(startSudokuBoards[i], solvedSudokuBoards[i], solveResult[i]));
                        File.WriteAllText("C:\\Sudoku\\Solve\\SudokusThatFailed.txt", sb.ToString());
                    }
                }

                if (different == 0)
                {
                    msg = " (All " + same.ToString() + " sudokus were solved successfully!)";
                    solvedSuccessfully = true;
                }
                else
                {
                    solvedSuccessfully = false;
                    msg = string.Format(" ({0} sudokus were solved successfully and {1} failed)", same.ToString(), different.ToString());
                }
            }

            start = v[0].Split(',');
            end = v[1].Split(',');

            DateTime dtStart = new DateTime(int.Parse(start[0]), int.Parse(start[1]), int.Parse(start[2]), int.Parse(start[3]), int.Parse(start[4]), int.Parse(start[5]));
            DateTime dtEnd = new DateTime(int.Parse(end[0]), int.Parse(end[1]), int.Parse(end[2]), int.Parse(end[3]), int.Parse(end[4]), int.Parse(end[5]));
            TimeSpan ts = dtEnd - dtStart;

            Console.WriteLine("Time: " + ts.TotalSeconds.ToString() + " seconds");
            Console.WriteLine("Solved successfully: " + solvedSuccessfully.ToString() + msg);
            string tmp = Console.ReadLine();
        }

        private static string GetTheThreeSudokus(string s1, string s2, string s3)
        {
            string[] rows1 = s1.Split(new string[] { "\r\n" }, StringSplitOptions.None);
            string[] rows2 = s2.Split(new string[] { "\r\n" }, StringSplitOptions.None);
            string[] rows3 = s3.Split(new string[] { "\r\n" }, StringSplitOptions.None);

            StringBuilder sb = new StringBuilder();

            for(int i = 0; i < 9; i++)
            {
                sb.Append(rows1[i] + "  " + rows2[i] + "  " + rows3[i]);

                if (i < 8)
                {
                    sb.Append("\r\n");
                }
            }

            return sb.ToString();
        }
    }
}
