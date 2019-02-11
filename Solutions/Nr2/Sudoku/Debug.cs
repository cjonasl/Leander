using System;
using System.Collections;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Sudoku
{
    public static class Debug
    {
        public static StringBuilder sb = new StringBuilder("[Iteration number, Simulation, Number of items in cellsRemainToSet, index, Number of candidates, Number of candidates calculated 1, Number of candidates calculated 2, Row set (0 if not), Column set (0 if not), Candidate set (0 if not), Candidate was simulated]");
        public static int[][][] candidates = ReturnThreeDimensionalDataStructure(9, 9, 10);
        public static int[][][] squareToCellMapper = ReturnSquareToCellMapper();

        public static string ReturnMap(int[][] v)
        {
            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < 9; i++)
            {
                sb.Append(string.Format("[{0},{1}] ", v[i][0], v[i][1]));
            }

            return sb.ToString().TrimEnd();
        }

        public static int[][][] ReturnThreeDimensionalDataStructure(int l, int m, int n)
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

        public static string ReturnFullMapp(int[][][] v)
        {
            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < 9; i++)
            {
                sb.Append(ReturnMap(v[i]) + "\r\n");
            }

            return sb.ToString().TrimEnd();
        }

        public static void CreateNewFile(string fileNameFullPath, string fileContent)
        {
            FileStream fileStream = new FileStream(fileNameFullPath, FileMode.Create, FileAccess.Write);
            StreamWriter streamWriter = new StreamWriter(fileStream, Encoding.UTF8);
            streamWriter.Write(fileContent);
            streamWriter.Flush();
            fileStream.Flush();
            streamWriter.Close();
            fileStream.Close();
        }

        public static int ReturnNumberOfCandidates(int[][][] candidates)
        {
            int i, j, n = 0;

            for(i = 0; i < 9; i++)
            {
                for (j = 0; j < 9; j++)
                {
                    if (candidates[i][j][0] > 0)
                        n += candidates[i][j][0];
                }
            }

            return n;
        }

        public static int ReturnSquare(int row, int column)
        {
            return 1 + (3 * ((row - 1) / 3)) + ((column - 1) / 3);
        }

        public static int ReturnNumberOfOccurenciesOfNumberInRow(int[][] sudokuBoard, int row, int number)
        {
            int column, n = 0;

            for (column = 1; column <= 9; column++)
            {
                if (sudokuBoard[row - 1][column - 1] == number)
                    n++;
            }

            return n;
        }

        public static int ReturnNumberOfOccurenciesOfNumberInColumn(int[][] sudokuBoard, int column, int number)
        {
            int row, n = 0;

            for (row = 1; row <= 9; row++)
            {
                if (sudokuBoard[row - 1][column - 1] == number)
                    n++;
            }

            return n;
        }

        public static int ReturnNumberOfOccurenciesOfNumberInSquare(int[][] sudokuBoard, int[][][] squareToCellMapper, int square, int number)
        {
            int row, column, i, n = 0;

            for (i = 0; i < 9; i++)
            {
                row = squareToCellMapper[square - 1][i][0];
                column = squareToCellMapper[square - 1][i][1];

                if (sudokuBoard[row - 1][column - 1] == number)
                    n++;
            }

            return n;
        }

        public static int[][][] ReturnSquareToCellMapper()
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

        public static void InitCandidateStructure(int[][] sudokuBoard, int[][][] candidates, int[][][] squareToCellMapper)
        {
            int row, column, square, number, n;

            for (row = 1; row <= 9; row++)
            {
                for (column = 1; column <= 9; column++)
                {
                    square = ReturnSquare(row, column);

                    if (sudokuBoard[row - 1][column - 1] != 0)
                    {
                        candidates[row - 1][column - 1][0] = -1; //Indicates that the cell is set already
                    }
                    else
                    {
                        n = 0;
                        candidates[row - 1][column - 1][0] = 0; //Number of candidates is set in index 0

                        for (number = 1; number <= 9; number++)
                        {
                            if (
                                (ReturnNumberOfOccurenciesOfNumberInRow(sudokuBoard, row, number) == 0) &&
                                (ReturnNumberOfOccurenciesOfNumberInColumn(sudokuBoard, column, number) == 0) &&
                                (ReturnNumberOfOccurenciesOfNumberInSquare(sudokuBoard, squareToCellMapper, square, number) == 0)
                                )
                            {
                                n++;
                                candidates[row - 1][column - 1][0] = n;
                                candidates[row - 1][column - 1][n] = number;
                            }
                        }
                    }
                }
            }
        }

        public static void Register(int iteration, int[][] sudokuBoard, int numberOfSimulations, int index, int numberOfCandidates, ArrayList cellsRemainToSet, int[][][] candidatesAAA, int row, int column, int candidate, int candidateWasSimulated)
        {
            string iterationStr = iteration.ToString().PadLeft(3, '0');
            string numberOfSimulationsStr = numberOfSimulations.ToString();
            string numberOfItemsInCellsRemainToSet = cellsRemainToSet.Count.ToString().PadLeft(2, '0');
            string indexStr = index.ToString().PadLeft(2, '0');
            string numberOfCandidatesStr = numberOfCandidates.ToString().PadLeft(3, '0');
            string numberOfCandidatesCalculated1 = ReturnNumberOfCandidates(candidatesAAA).ToString().PadLeft(3, '0');

            InitCandidateStructure(sudokuBoard, Debug.candidates, Debug.squareToCellMapper);
            string numberOfCandidatesCalculated2 = ReturnNumberOfCandidates(Debug.candidates).ToString().PadLeft(3, '0');

            string rowStr = row.ToString();
            string columnStr = column.ToString();
            string candidateStr = candidate.ToString();
            string candidateWasSimulatedStr = candidateWasSimulated.ToString();
            string indicatorStr;

            if (row != 0 && candidateWasSimulated == 0)
                indicatorStr = "<---------- Candidate set with certainty ----------";
            else if (row != 0 && candidateWasSimulated == 1)
                indicatorStr = "<---------- Candidate set with simulation ----------";
            else
                indicatorStr = "";

            string str = string.Format("\r\n[{0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8}, {9}, {10}, {11}]",
                iterationStr,
                numberOfSimulationsStr,
                numberOfItemsInCellsRemainToSet,
                indexStr,
                numberOfCandidatesStr,
                numberOfCandidatesCalculated1,
                numberOfCandidatesCalculated2,
                rowStr,
                columnStr,
                candidateStr,
                candidateWasSimulatedStr,
                indicatorStr
                );

            sb.Append(str);
        }

        public static string ReturnCandidates(int[] v)
        {
            StringBuilder sb = new StringBuilder();
            int i, n = v[0];

            for(i = 0; i < n; i++)
            {
                if (i == 0)
                    sb.Append(v[1 + i].ToString());
                else
                    sb.Append(", " + v[1 + i].ToString());
            }

            return sb.ToString();
        }

        public static void PrintCandidates(int[][][] candidates, int iteration)
        {
            int row, column;
            StringBuilder sb = new StringBuilder();
            string iterationStr = iteration.ToString().PadLeft(3, '0');

            for (row = 1; row <= 9; row++)
            {
                for (column = 1; column <= 9; column++)
                {
                    if (candidates[row - 1][column - 1][0] > 0)
                        sb.Append(string.Format("[{0}][{1}]: {2}\r\n", row.ToString(), column.ToString(), ReturnCandidates(candidates[row - 1][column - 1])));
                    else
                        sb.Append(string.Format("[{0}][{1}]:\r\n", row.ToString(), column.ToString()));
                }
            }

            CreateNewFile(string.Format("C:\\AAA\\CandidatesIteration{0}.txt", iterationStr), sb.ToString());
        }
    }
}
