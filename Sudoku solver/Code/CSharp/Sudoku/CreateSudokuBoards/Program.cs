using System;
using System.Collections;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace CreateSudokuBoards
{
    public static class Utility
    {
        public static string ToSudokuBoard(this string str)
        {
            StringBuilder sb = new StringBuilder();

            for (int row = 1; row <= 9; row++)
            {
                for (int column = 1; column <= 9; column++)
                {
                    if (row == 9 && column == 9)
                    {
                        sb.Append(str[80]);
                    }
                    else
                    {
                        if (column == 9)
                            sb.Append(str[9 * (row - 1) + (column - 1)] + "\r\n");
                        else
                            sb.Append(str[9 * (row - 1) + (column - 1)] + " ");
                    }
                }
            }

            return sb.ToString();
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            string[] files, v, strArray;
            ArrayList startSudokuBoards, solvedSudokuBoards;
            int i, j, n, numberOfRows, numberOfSudokusToPrintInFile;
            StringBuilder sb;
            string str;
            
            startSudokuBoards = new ArrayList();
            solvedSudokuBoards = new ArrayList();

            files = Directory.GetFiles("C:\\Sudoku\\Sudoku boards");

            n = files.Length;

            for(i = 0; i < n; i++)
            {
                if ((files[i].IndexOf("SudokuBoardsMethod") >= 0) && (files[i].IndexOf("_O_") >= 0))
                {
                    v = File.ReadAllText(files[i]).Trim().Split(new string[] { "\r\n" }, StringSplitOptions.None);

                    numberOfRows = v.Length;

                    if ((numberOfRows % 2) != 0)
                    {
                        throw new Exception("((numberOfRows % 2) != 0)");
                    }

                    j = 0;

                    while (j < numberOfRows)
                    {
                        if (startSudokuBoards.IndexOf(v[j]) >= 0)
                        {
                            throw new Exception("(startSudokuBoards.IndexOf(v[j]) >= 0)");
                        }

                        startSudokuBoards.Add(v[j]);
                        j++;

                        solvedSudokuBoards.Add(v[j]);
                        j++;
                    }
                }
                if ((files[i].IndexOf("SudokuBoardsMethod") >= 0) && (files[i].IndexOf("_S_") >= 0))
                {
                    v = File.ReadAllText(files[i]).Trim().Split(new string[] { "\r\n" }, StringSplitOptions.None);

                    numberOfRows = v.Length;

                    if ((numberOfRows % 2) != 0)
                    {
                        throw new Exception("((numberOfRows % 2) != 0)");
                    }

                    j = 0;

                    while (j < numberOfRows)
                    {
                        str = v[j];

                        strArray = str.Split(' ');

                        if (strArray.Length != 2)
                        {
                            throw new Exception("(strArray.Length != 2)");
                        }

                        if (startSudokuBoards.IndexOf(strArray[1]) >= 0)
                        {
                            throw new Exception("(startSudokuBoards.IndexOf(strArray[1]) >= 0)");
                        }

                        startSudokuBoards.Add(strArray[1]);
                        j++;

                        solvedSudokuBoards.Add(v[j]);
                        j++;
                    }
                }
            }

            if (startSudokuBoards.Count != solvedSudokuBoards.Count)
            {
                throw new Exception("startSudokuBoards.Count != solvedSudokuBoards.Count");
            }

            //numberOfSudokusToPrintInFile = startSudokuBoards.Count;
            numberOfSudokusToPrintInFile = 5000;

            sb = new StringBuilder();

            for (i = 0; i < numberOfSudokusToPrintInFile; i++)
            {
                str = (string)startSudokuBoards[i];
                sb.Append(str.ToSudokuBoard());

                if (i < (numberOfSudokusToPrintInFile - 1))
                {
                    sb.Append("\r\n-- New sudoku --\r\n");
                }
            }

            File.WriteAllText("C:\\Sudoku\\Solve\\StartSudokuBoards.txt", sb.ToString());


            sb.Clear();

            for (i = 0; i < numberOfSudokusToPrintInFile; i++)
            {
                str = (string)solvedSudokuBoards[i];
                sb.Append(str.ToSudokuBoard());

                if (i < (numberOfSudokusToPrintInFile - 1))
                {
                    sb.Append("\r\n-- New sudoku --\r\n");
                }
            }

            File.WriteAllText("C:\\Sudoku\\Solve\\SolvedSudokuBoards.txt", sb.ToString());

            Console.WriteLine("Two files, StartSudokuBoards.txt and SolvedSudokuBoards.txt, were successfully created with " + numberOfSudokusToPrintInFile.ToString() + " sudokus in each.");
        }
    }
}
