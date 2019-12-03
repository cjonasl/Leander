﻿using System;
using System.Collections;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace GenerateSudokuBoards
{
    public static class Utility
    {
        public static string ToSudokuBoard(this string str)
        {
            StringBuilder sb = new StringBuilder();

            for(int row = 1; row <= 9; row++)
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
        private static string _basePath = "C:\\git_cjonasl\\Leander\\Sudoku solver\\";

        static void Main(string[] args)
        {
            DateTime start, end;
            TimeSpan ts;
            double numberOfSecondsToRun = int.Parse(args[0]);
            bool errorFound = false;
            string solvedSudokoBoard, result;
            int solvedInitialEmptySudokuBoard, newFullSudokuBoards, existedAlreadyFullSudokuBoards, n = 0, i;
            string[] initialSudokuBoards = new string[30];
            ArrayList listFullSudokuBoards;

            solvedInitialEmptySudokuBoard = 0;
            newFullSudokuBoards = 0;
            existedAlreadyFullSudokuBoards = 0;

            listFullSudokuBoards = ReturnExistingFullSudokuBoards();

            start = DateTime.Now;
            end = DateTime.Now;
            ts = end - start;

            while (ts.TotalSeconds < numberOfSecondsToRun && !errorFound)
            {
                result = Sudoku.Sudoku.SolveInitialEmptySudokuBoard(out solvedSudokoBoard, initialSudokuBoards);

                if (!result.StartsWith("The sudoku was solved."))
                {
                    Log(start, args[0], result, solvedInitialEmptySudokuBoard, newFullSudokuBoards, existedAlreadyFullSudokuBoards);
                    errorFound = true;
                }
                else
                {
                    solvedInitialEmptySudokuBoard++;

                    if (listFullSudokuBoards.IndexOf(solvedSudokoBoard) == -1)
                    {
                        newFullSudokuBoards++;
                        AddFullSudokuBoard(solvedSudokoBoard);

                        i = 11;

                        while (i <= 40 && !errorFound)
                        {
                            result = Sudoku.Sudoku.GetSolveStat(initialSudokuBoards[i - 11].ToSudokuBoard(), solvedSudokoBoard);

                            if (result != null)
                            {
                                if (!result.StartsWith("ERROR"))
                                {
                                    result = AddSudokuBoard(initialSudokuBoards[i - 11], solvedSudokoBoard, result, i);

                                    if (result.StartsWith("ERROR"))
                                        errorFound = true;
                                }
                                else
                                {
                                    Log(start, args[0], result, solvedInitialEmptySudokuBoard, newFullSudokuBoards, existedAlreadyFullSudokuBoards);
                                    errorFound = true;
                                }
                            }

                            i++;
                        }
                    }
                    else
                    {
                        existedAlreadyFullSudokuBoards++;
                    }

                    n++;

                    if ((n % 25) == 0)
                    {
                        Console.WriteLine(solvedInitialEmptySudokuBoard.ToString() + "\t" + newFullSudokuBoards.ToString() + "\t" + existedAlreadyFullSudokuBoards.ToString());
                    }

                    end = DateTime.Now;
                    ts = end - start;
                }
            }

            if (!errorFound)
            {
                Log(start, args[0], null, solvedInitialEmptySudokuBoard, newFullSudokuBoards, existedAlreadyFullSudokuBoards);
            }
        }

        private static ArrayList ReturnExistingFullSudokuBoards()
        {
            FileStream f;
            StreamReader r;
            string[] strArray;

            if (!File.Exists(string.Format("{0}{1}", _basePath, "Sudoku boards\\FullSudokuBoards.txt")))
            {
                return new ArrayList();
            }
            else
            {
                f = new FileStream(string.Format("{0}{1}", _basePath, "Sudoku boards\\FullSudokuBoards.txt"), FileMode.Open, FileAccess.Read);
                r = new StreamReader(f, Encoding.ASCII);
                strArray = r.ReadToEnd().Split(new string[] { "\r\n" }, StringSplitOptions.None);
                return new ArrayList(strArray);
            }
        }

        private static string AddSudokuBoard(string sudokuBoard, string solvedSudokoBoard, string solveStat, int numberOfIntegers)
        {
            FileStream f;
            StreamWriter w;

            string c, fileNameFullPath1, fileNameFullPath2;

            if (solveStat == "O")
            {
                c = "O"; //Only ordinary methods needed
            }
            else
            {
                c = "S"; //At least one simulation needed
            }

            fileNameFullPath1 = string.Format("{0}Sudoku boards\\SudokuBoardsMethod1_{1}_{2}.txt", _basePath, c, numberOfIntegers.ToString());
            fileNameFullPath2 = string.Format("{0}Sudoku boards\\SudokuBoardsMethod2_{1}_{2}.txt", _basePath, c, numberOfIntegers.ToString());

            if (File.Exists(fileNameFullPath1) && SudokuBoardExistsAlready(sudokuBoard, fileNameFullPath1, c))
            {
                return "ERROR!! Was about to add a sudoku board in file " + fileNameFullPath1 + ", but it exist already in that file, which it should not";
            }

            if (File.Exists(fileNameFullPath2) && (SudokuBoardExistsAlready(sudokuBoard, fileNameFullPath2, c)))
            {
                return "Success";
            }

            if (!File.Exists(fileNameFullPath1))
                f = new FileStream(fileNameFullPath1, FileMode.Create, FileAccess.Write);
            else
                f = new FileStream(fileNameFullPath1, FileMode.Append, FileAccess.Write);

            w = new StreamWriter(f, Encoding.ASCII);

            if (c == "O")
                w.WriteLine(sudokuBoard);
            else
                w.WriteLine(solveStat + " " + sudokuBoard);

            w.WriteLine(solvedSudokoBoard);

            w.Flush();
            f.Flush();
            w.Close();
            f.Close();

            return "Success";
        }

        private static bool SudokuBoardExistsAlready(string sudokuBoard, string fileNameFullPath, string c)
        {
            FileStream f;
            StreamReader r;
            string[] v;
            ArrayList a;
          
            f = new FileStream(fileNameFullPath, FileMode.Open, FileAccess.Read);
            r = new StreamReader(f, Encoding.ASCII);
            v = r.ReadToEnd().Split(new string[] { "\r\n" }, StringSplitOptions.None);
            r.Close();
            f.Close();

            a = new ArrayList();

            if (c == "O") //Only ordinary methods
            {
                for (int i = 0; i < v.Length; i++)
                {
                    if ((i % 2) == 0)
                    {
                        a.Add(v[i]);
                    }
                }
            }
            else //At least one simulation
            {
                string[] strArray;

                for (int i = 0; i < v.Length; i++)
                {
                    if ((i % 2) == 0)
                    {
                        strArray = v[i].Split(' ');

                        if (strArray.Length != 2)
                        {
                            throw new Exception("(strArray.Length != 2) in SudokuBoardExistsAlready");
                        }

                        a.Add(strArray[1]);
                    }
                }
            }

            return a.IndexOf(sudokuBoard) >= 0;
        }

        private static void AddFullSudokuBoard(string sudokuBoard)
        {
            FileStream f;
            StreamWriter w;
            string fileNameFullPath = string.Format("{0}{1}", _basePath, "Sudoku boards\\FullSudokuBoards.txt");

            if (!File.Exists(fileNameFullPath))     
                f = new FileStream(fileNameFullPath, FileMode.Create, FileAccess.Write);       
            else        
                f = new FileStream(fileNameFullPath, FileMode.Append, FileAccess.Write);

            w = new StreamWriter(f, Encoding.ASCII);
            w.WriteLine(sudokuBoard);

            w.Flush();
            f.Flush();
            w.Close();
            f.Close();
        }

        private static void Log(DateTime start, string numberOfSecondsToRun, string errorMessage, int solvedInitialEmptySudokuBoard, int newFullSudokuBoards, int existedAlreadyFullSudokuBoards)
        {
            FileStream f;
            StreamWriter w;
            string fileNameFullPath;
            string runString = string.Format("{0}__{1}", start.ToString("yyyy-MM-dd_HH.mm.ss.fff"), numberOfSecondsToRun);
            bool addHeader = false;

            fileNameFullPath = string.Format("{0}{1}", _basePath, "Sudoku boards\\Log.txt");

            if (!File.Exists(fileNameFullPath))
            {
                f = new FileStream("C:\\git_cjonasl\\Leander\\Sudoku solver\\Sudoku boards\\Log.txt", FileMode.Create, FileAccess.Write);
                addHeader = true;
            }
            else
                f = new FileStream("C:\\git_cjonasl\\Leander\\Sudoku solver\\Sudoku boards\\Log.txt", FileMode.Append, FileAccess.Write);

            w = new StreamWriter(f, Encoding.ASCII);

            if (addHeader)
                w.WriteLine("Run\tSolvedInitialEmptySudokuBoard\tNewFullSudokuBoards\tExistedAlreadyFullSudokuBoards");

            if (errorMessage != null)
            {
                w.WriteLine(string.Format("{0}\t{1}\t[2}\t{3}\r\n{4}", runString, solvedInitialEmptySudokuBoard.ToString(), newFullSudokuBoards.ToString(), existedAlreadyFullSudokuBoards.ToString(), errorMessage));
            }
            else
            {
                w.WriteLine(string.Format("{0}\t{1}\t{2}\t{3}", runString, solvedInitialEmptySudokuBoard.ToString(), newFullSudokuBoards.ToString(), existedAlreadyFullSudokuBoards.ToString()));
            }

            w.Flush();
            f.Flush();
            w.Close();
            f.Close();
        }
    }
}
