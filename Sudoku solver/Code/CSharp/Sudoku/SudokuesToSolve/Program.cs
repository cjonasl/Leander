using System;
using System.Collections;
using System.IO;
using System.Text;

namespace SudokuesToSolve
{
    class Program
    {
        static void Main(string[] args)
        {
            int i, j, n, maxNumberOfSudokus = 50;
            string str;
            StringBuilder sb = new StringBuilder();

            ArrayList sudokus = new ArrayList();
            ArrayList files = new ArrayList();

            for(i = 0; i < 10; i++)
            {
                files.Add(new ArrayList());
            }

            //Level 1
            ((ArrayList)files[0]).Add("SudokuBoardsMethod2_O_39.txt");
            ((ArrayList)files[0]).Add("SudokuBoardsMethod2_O_40.txt");

            //Level 2
            ((ArrayList)files[1]).Add("SudokuBoardsMethod2_O_37.txt");
            ((ArrayList)files[1]).Add("SudokuBoardsMethod2_O_38.txt");

            //Level 3
            ((ArrayList)files[2]).Add("SudokuBoardsMethod2_O_35.txt");
            ((ArrayList)files[2]).Add("SudokuBoardsMethod2_O_36.txt");

            //Level 4
            ((ArrayList)files[3]).Add("SudokuBoardsMethod2_O_33.txt");
            ((ArrayList)files[3]).Add("SudokuBoardsMethod2_O_34.txt");

            //Level 5
            ((ArrayList)files[4]).Add("SudokuBoardsMethod2_O_30.txt");
            ((ArrayList)files[4]).Add("SudokuBoardsMethod2_O_31.txt");
            ((ArrayList)files[4]).Add("SudokuBoardsMethod2_O_32.txt");

            //Level 6
            ((ArrayList)files[5]).Add("SudokuBoardsMethod2_O_30.txt");
            ((ArrayList)files[5]).Add("SudokuBoardsMethod2_O_31.txt");

            //Level 7
            ((ArrayList)files[6]).Add("SudokuBoardsMethod2_S_29.txt");
            ((ArrayList)files[6]).Add("SudokuBoardsMethod2_O_28.txt");

            //Level 8
            ((ArrayList)files[7]).Add("SudokuBoardsMethod2_S_27.txt");
            ((ArrayList)files[7]).Add("SudokuBoardsMethod2_S_28.txt");

            //Level 9
            ((ArrayList)files[8]).Add("SudokuBoardsMethod2_S_25.txt");
            ((ArrayList)files[8]).Add("SudokuBoardsMethod2_S_26.txt");

            //Level 10
            ((ArrayList)files[9]).Add("SudokuBoardsMethod2_S_22.txt");
            ((ArrayList)files[9]).Add("SudokuBoardsMethod2_S_23.txt");
            ((ArrayList)files[9]).Add("SudokuBoardsMethod2_S_24.txt");

            for (i = 0; i < 10; i++)
            {
                sudokus.Clear();
                GetSudokus((ArrayList)files[i], sudokus);

                n = Math.Min(sudokus.Count, maxNumberOfSudokus);
                Console.WriteLine(n);

                for (j = 0; j < n; j++)
                { 
                    sb.Append(string.Format("window.sudoku.boards[{0}].push('{1}');\r\n", i.ToString(), (string)sudokus[j]));
                }
            }

            str = File.ReadAllText("C:\\git_cjonasl\\Leander\\Sudoku solver\\Code\\sudoku.js");
            File.WriteAllText("C:\\git_cjonasl\\Leander\\Sudoku solver\\Code\\sudoku_New.js", str.Replace("#####REPLACE_SUDOKUS_TO_SOLVE#####", sb.ToString().TrimEnd()));
        }

        public static void GetSudokus(ArrayList files, ArrayList sudokus)
        {
            int i, j, n = files.Count;
            string fileName;
            bool isSimulation;
            string[] v1, v2;

            for (i = 0; i < files.Count; i++)
            {
                fileName = (string)files[i];
                isSimulation = (fileName.IndexOf("_S_") >= 0);
                v1 = File.ReadAllText("C:\\Sudoku\\Sudoku boards\\" + fileName).Trim().Split(new string[] { "\r\n" }, StringSplitOptions.None);

                for(j = 0; j < v1.Length; j++)
                {
                    if ((j % 2) == 0)
                    {
                        if (isSimulation)
                        {
                            v2 = v1[j].Split(' ');

                            if (v2.Length != 2)
                            {
                                throw new Exception("(v2.Length != 2)");
                            }

                            sudokus.Add(v2[1]);
                        }
                        else
                        {
                            sudokus.Add(v1[j]);
                        }
                    }
                }
            }
        }
    }
}
