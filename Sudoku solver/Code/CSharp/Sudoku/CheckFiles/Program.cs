using System;
using System.Collections;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace CheckFiles
{
    class Program
    {
        static void Main(string[] args)
        {
            int i, j, index;
            ArrayList sudokus = new ArrayList();
            string[] fileNamesFullPath = Directory.GetFiles("C:\\Sudoku\\Sudoku boards");
            string fileNameShort;
            int[] v = new int[81];

            for (i = 0; i < fileNamesFullPath.Length; i++)
            {
                index = fileNamesFullPath[i].LastIndexOf("\\");
                fileNameShort = fileNamesFullPath[i].Substring(1 + index);

                if (fileNameShort.StartsWith("SudokuBoardsMethod"))
                {
                    sudokus.Clear();
                    GetSudokus(fileNamesFullPath[i], sudokus);

                    for(j = 0; j < 81; j++)
                    {
                        v[j] = 0;
                    }

                    for (j = 0; j < sudokus.Count; j++)
                    {
                        v[ReturnNumberOfNonZeroIntegers((string)sudokus[j])]++;
                    }

                    Console.WriteLine(fileNameShort + ": " + ReturnStr(v));
                }

            }
        }

        public static void GetSudokus(string fileNamesFullPath, ArrayList sudokus)
        {
            int i;
            bool isSimulation;
            string[] v1, v2;

            isSimulation = (fileNamesFullPath.IndexOf("_S_") >= 0);
            v1 = File.ReadAllText(fileNamesFullPath).Trim().Split(new string[] { "\r\n" }, StringSplitOptions.None);

            for (i = 0; i < v1.Length; i++)
            {
                if ((i % 2) == 0)
                {
                    if (isSimulation)
                    {
                        v2 = v1[i].Split(' ');

                        if (v2.Length != 2)
                        {
                            throw new Exception("(v2.Length != 2)");
                        }

                        sudokus.Add(v2[1]);
                    }
                    else
                    {
                        sudokus.Add(v1[i]);
                    }
                }
            }
        }

        public static int ReturnNumberOfNonZeroIntegers(string sudoku)
        {
            int i, m, numberOfNonZeroIntegers = 0, n = sudoku.Length;

            if (n != 81)
            {
                throw new Exception("(n != 81)");
            }

            for (i = 0; i < n; i++)
            {
                m = int.Parse(sudoku.Substring(i, 1));

                if (m < 0 || m > 9)
                {
                    throw new Exception("(m < 0 || m > 9)");
                }

                if (m != 0)
                {
                    numberOfNonZeroIntegers++;
                }
            }

            return numberOfNonZeroIntegers;
        }

        public static string ReturnStr(int[] v)
        {
            StringBuilder sb = new StringBuilder();

            for(int i = 0; i < 81; i++)
            {
                if (v[i] != 0)
                {
                    sb.Append(string.Format("[{0},{1}]", i.ToString(), v[i].ToString()));
                }
            }

            return sb.ToString();
        }
    }
}
