using System;
using System.Collections;
using System.IO;
using System.Text;

namespace GenerateSudokuBoards
{
    public class LogCreationSudokuBoardsMethod2
    {
        private string _basePath, _currentFullSudokuBoard;
        ArrayList arrayList;
        private int[] v;

        public LogCreationSudokuBoardsMethod2(string basePath)
        {
            _basePath = basePath;
            v = new int[40];
            arrayList = new ArrayList();
        }

        public void NewFullSudokuBoard(string currentFullSudokuBoard)
        {
            _currentFullSudokuBoard = currentFullSudokuBoard;
            arrayList.Clear();
        }

        public void NewSimulation()
        {
            for (int i = 0; i < 40; i++)
            {
                v[i] = 0;
            }
        }

        public void Log(int n)
        {
            v[n - 1]++;
        }

        public void CreateLogString()
        {
            int i, k, n = 0;
            StringBuilder sb = new StringBuilder();

            for (i = 0; i < 40; i++)
            {
                n += v[i];
            }

            for (i = 0; i < 40; i++)
            {
                n += v[i];
            }

            sb.Append(string.Format("[{0}] ", n.ToString()));

            for (i = 0; i < 40; i++)
            {
                k = i + 1;
                sb.Append(string.Format("[{0}, {1}] ", k.ToString(), v[i].ToString()));
            }

            arrayList.Add(sb.ToString().TrimEnd());
        }

        public void Print()
        {
            string fileNameFullPath = string.Format("{0}\\File{1}.txt", _basePath, _currentFullSudokuBoard);
            FileStream f;
            StreamWriter w;
            StringBuilder sb;
            int i;

            sb = new StringBuilder();

            for(i = 1; i <= arrayList.Count; i++)
            {
                sb.Append(string.Format("{0}. {1}\r\n", i.ToString(), (string)arrayList[i - 1]));
            }

            f = new FileStream(fileNameFullPath, FileMode.Create, FileAccess.Write);
            w = new StreamWriter(f, Encoding.ASCII);
            w.Write(sb.ToString().TrimEnd());
            w.Flush();
            f.Flush();
            w.Close();
            f.Close();
        }
    }


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
        private static string _basePath;

        static void Main(string[] args)
        {
            DateTime start, end;
            TimeSpan ts;
            double numberOfSecondsToRun = int.Parse(args[0]);
            bool errorFound = false, continueProcessSolvedSudokoBoard, solvedWithOnlyOrdinaryMethodsFound;
            string solvedSudokoBoard, result, testSudoKuBoard, simulatedTestSudoKuBoard = "", simulatedSolveStat = "";
            int solvedInitialEmptySudokuBoard, newFullSudokuBoards, existedAlreadyFullSudokuBoards, i;
            int row, column, rowO, columnO, rowS, columnS, numberOfNumbersSetInSudokuBoard, prc;
            string[] initialSudokuBoards = new string[30];
            ArrayList listFullSudokuBoards;
            SudokuBoardReduceNumberOfNumbers sudokuBoardReduceNumberOfNumbers;
            LogCreationSudokuBoardsMethod2 logCreationSudokuBoardsMethod2;

            _basePath = args[1];

            solvedInitialEmptySudokuBoard = 0;
            newFullSudokuBoards = 0;
            existedAlreadyFullSudokuBoards = 0;

            listFullSudokuBoards = ReturnExistingFullSudokuBoards();

            sudokuBoardReduceNumberOfNumbers = new SudokuBoardReduceNumberOfNumbers();
            logCreationSudokuBoardsMethod2 = new LogCreationSudokuBoardsMethod2("C:\\Sudoku\\Log");

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
                    prc = Convert.ToInt32(100.0 * (ts.TotalSeconds / numberOfSecondsToRun));

                    Console.WriteLine(string.Format("solvedInitialEmptySudokuBoard nr. {0} {1}%", solvedInitialEmptySudokuBoard.ToString(), prc.ToString()));

                    if (listFullSudokuBoards.IndexOf(solvedSudokoBoard) == -1)
                    {
                        newFullSudokuBoards++;
                        AddFullSudokuBoard(solvedSudokoBoard);

                        i = 11;

                        while (i <= 40 && !errorFound)
                        {
                            Console.Write("\rSolve stat method 1 " + i.ToString());

                            result = Sudoku.Sudoku.GetSolveStat(initialSudokuBoards[i - 11].ToSudokuBoard(), solvedSudokoBoard);

                            if (result != null)
                            {
                                if (!result.StartsWith("ERROR"))
                                {
                                    result = AddSudokuBoard(initialSudokuBoards[i - 11], solvedSudokoBoard, result, i, true);

                                    if (result.StartsWith("ERROR"))
                                    {
                                        Log(start, args[0], result, solvedInitialEmptySudokuBoard, newFullSudokuBoards, existedAlreadyFullSudokuBoards);
                                        errorFound = true;
                                    }
                                }
                                else
                                {
                                    Log(start, args[0], result, solvedInitialEmptySudokuBoard, newFullSudokuBoards, existedAlreadyFullSudokuBoards);
                                    errorFound = true;
                                }
                            }

                            i++;
                        }

                        if (!errorFound)
                        {
                            Console.WriteLine("\r\nStart with method 2");

                            logCreationSudokuBoardsMethod2.NewFullSudokuBoard(solvedSudokoBoard);

                            for (i = 0; i < 10; i++)
                            {
                                Console.Write("\rReduce sudoku board " + (i + 1).ToString());
                                logCreationSudokuBoardsMethod2.NewSimulation();
                                sudokuBoardReduceNumberOfNumbers.Init(solvedSudokoBoard.ToSudokuBoard());
                                continueProcessSolvedSudokoBoard = true;

                                while (continueProcessSolvedSudokoBoard)
                                {
                                    testSudoKuBoard = sudokuBoardReduceNumberOfNumbers.ReturnReducedSudokuBoard(out row, out column);

                                    solvedWithOnlyOrdinaryMethodsFound = false;
                                    rowO = columnO = rowS = columnS = 0;

                                    while (testSudoKuBoard != null && !solvedWithOnlyOrdinaryMethodsFound)
                                    {
                                        result = Sudoku.Sudoku.GetSolveStat(testSudoKuBoard, solvedSudokoBoard);

                                        if (result != null && result.StartsWith("ERROR"))
                                        {
                                            Log(start, args[0], result, solvedInitialEmptySudokuBoard, newFullSudokuBoards, existedAlreadyFullSudokuBoards);
                                            return;
                                        }

                                        if (result != null)
                                        {
                                            if (result == "O")
                                            {
                                                solvedWithOnlyOrdinaryMethodsFound = true;
                                                rowO = row;
                                                columnO = column;
                                            }
                                            else
                                            {
                                                if (rowS == 0)
                                                {
                                                    rowS = row;
                                                    columnS = column;
                                                    simulatedTestSudoKuBoard = testSudoKuBoard;
                                                    simulatedSolveStat = result;
                                                }
                                            }
                                        }

                                        if (!solvedWithOnlyOrdinaryMethodsFound)
                                            testSudoKuBoard = sudokuBoardReduceNumberOfNumbers.ReturnReducedSudokuBoard(out row, out column);
                                    }

                                    if (solvedWithOnlyOrdinaryMethodsFound || rowS != 0)
                                    {
                                        if (!solvedWithOnlyOrdinaryMethodsFound)
                                        {
                                            testSudoKuBoard = simulatedTestSudoKuBoard;
                                            result = simulatedSolveStat;
                                        }

                                        numberOfNumbersSetInSudokuBoard = 81 - (sudokuBoardReduceNumberOfNumbers.CellsRemainToSet + 1); //+1 because sudokuBoardReduceNumberOfNumbers.CellsRemainToSet will be increased by one in the call sudokuBoardReduceNumberOfNumbers.Reduce below

                                        if (numberOfNumbersSetInSudokuBoard <= 40)
                                        {
                                            result = AddSudokuBoard(testSudoKuBoard.Replace("\r\n", "").Replace(" ", ""), solvedSudokoBoard, result, numberOfNumbersSetInSudokuBoard, false);

                                            if (result == "Added")
                                            {
                                                logCreationSudokuBoardsMethod2.Log(numberOfNumbersSetInSudokuBoard);
                                            }
                                            else if (result.StartsWith("ERROR"))
                                            {
                                                Log(start, args[0], result, solvedInitialEmptySudokuBoard, newFullSudokuBoards, existedAlreadyFullSudokuBoards);
                                                errorFound = true;
                                                continueProcessSolvedSudokoBoard = false;
                                            }
                                        }

                                        if (solvedWithOnlyOrdinaryMethodsFound)
                                        {
                                            sudokuBoardReduceNumberOfNumbers.Reduce(rowO, columnO);
                                        }
                                        else
                                        {
                                            sudokuBoardReduceNumberOfNumbers.Reduce(rowS, columnS);
                                        }
                                    }
                                    else
                                    {
                                        continueProcessSolvedSudokoBoard = false;
                                    }
                                }

                                logCreationSudokuBoardsMethod2.CreateLogString();
                            }

                            logCreationSudokuBoardsMethod2.Print();
                        }
                    }
                    else
                    {
                        existedAlreadyFullSudokuBoards++;
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

            if (!File.Exists(string.Format("{0}\\{1}", _basePath, "Sudoku boards\\FullSudokuBoards.txt")))
            {
                return new ArrayList();
            }
            else
            {
                f = new FileStream(string.Format("{0}\\{1}", _basePath, "Sudoku boards\\FullSudokuBoards.txt"), FileMode.Open, FileAccess.Read);
                r = new StreamReader(f, Encoding.ASCII);
                strArray = r.ReadToEnd().Split(new string[] { "\r\n" }, StringSplitOptions.None);
                r.Close();
                f.Close();
                return new ArrayList(strArray);
            }
        }

        private static string AddSudokuBoard(string sudokuBoard, string solvedSudokoBoard, string solveStat, int numberOfIntegers, bool isMethod1)
        {
            FileStream f;
            StreamWriter w;
            bool existsAlreadyInMethod1File, existsAlreadyInMethod2File;

            string c, fileNameFullPath, fileNameFullPath1, fileNameFullPath2;

            if (solveStat == "O")
            {
                c = "O"; //Only ordinary methods needed
            }
            else
            {
                c = "S"; //At least one simulation needed
            }

            fileNameFullPath1 = string.Format("{0}\\Sudoku boards\\SudokuBoardsMethod1_{1}_{2}.txt", _basePath, c, numberOfIntegers.ToString());
            fileNameFullPath2 = string.Format("{0}\\Sudoku boards\\SudokuBoardsMethod2_{1}_{2}.txt", _basePath, c, numberOfIntegers.ToString());

            if (isMethod1)
                fileNameFullPath = fileNameFullPath1;
            else
                fileNameFullPath = fileNameFullPath2;

            if (File.Exists(fileNameFullPath1) && SudokuBoardExistsAlready(sudokuBoard, fileNameFullPath1, c))
                existsAlreadyInMethod1File = true;
            else
                existsAlreadyInMethod1File = false;

            if (File.Exists(fileNameFullPath2) && SudokuBoardExistsAlready(sudokuBoard, fileNameFullPath2, c))
                existsAlreadyInMethod2File = true;
            else
                existsAlreadyInMethod2File = false;


            if (isMethod1 && existsAlreadyInMethod1File)
            {
                return "ERROR!! Was about to add a sudoku board in file " + fileNameFullPath + ", but it exist already in that file, which it should not";
            }

            if (existsAlreadyInMethod1File || existsAlreadyInMethod2File)
            {
                return "Exists already";
            }

            if (!File.Exists(fileNameFullPath))
                f = new FileStream(fileNameFullPath, FileMode.Create, FileAccess.Write);
            else
                f = new FileStream(fileNameFullPath, FileMode.Append, FileAccess.Write);

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

            return "Added";
        }

        private static bool SudokuBoardExistsAlready(string sudokuBoard, string fileNameFullPath, string c)
        {
            FileStream f;
            StreamReader r;
            string[] v;
            ArrayList a;
            string str;
          
            f = new FileStream(fileNameFullPath, FileMode.Open, FileAccess.Read);
            r = new StreamReader(f, Encoding.ASCII);
            str = r.ReadToEnd().Trim();
            v = str.Split(new string[] { "\r\n" }, StringSplitOptions.None);
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
                            File.WriteAllText("C:\\Sudoku\\abc.txt", str);

                            throw new Exception("(strArray.Length != 2) in SudokuBoardExistsAlready.");
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
            string fileNameFullPath = string.Format("{0}\\{1}", _basePath, "Sudoku boards\\FullSudokuBoards.txt");

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

            fileNameFullPath = string.Format("{0}\\{1}", _basePath, "Sudoku boards\\Log.txt");

            if (!File.Exists(fileNameFullPath))
            {

                f = new FileStream(fileNameFullPath, FileMode.Create, FileAccess.Write);
                addHeader = true;
            }
            else
                f = new FileStream(fileNameFullPath, FileMode.Append, FileAccess.Write);

            w = new StreamWriter(f, Encoding.ASCII);

            if (addHeader)
                w.WriteLine("Run\tSolvedInitialEmptySudokuBoard\tNewFullSudokuBoards\tExistedAlreadyFullSudokuBoards");

            if (errorMessage != null)
            {
                w.WriteLine(string.Format("{0}\t{1}\t{2}\t{3}\r\n{4}", runString, solvedInitialEmptySudokuBoard.ToString(), newFullSudokuBoards.ToString(), existedAlreadyFullSudokuBoards.ToString(), errorMessage));
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
