using System;
using System.Collections;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Sudoku
{
    class Program
    {
        private const int _maxNumberOfSimulations = 10;
        private static Random _random;
        private static int _numberOfCandidates;


        static void Main(string[] args)
        {
            int[][][] squareToCellMapper, candidates;
            int[][] sudokuBoardCertainty, sudokuBoardTmp, sudokuBoardFinal;
            int numberOfCellsSetInInputSudokuBoard, numberOfCellsSetInFinalSudoku, numberOfSimulations, numberOfCandidates, candidate, i, row, column, square, index;
            FileStream fileStream;
            StreamReader streamReader;
            StreamWriter streamWriter;
            FileInfo fi;
            string errorMessage, sudokuBoardString, message, fileNameFullPath;
            ArrayList cellsRemainToSet; //List with two-tuples of int
            int debugIteration;

            if (args.Length == 0)
            {
                Console.WriteLine("An input file is not given!");
                return;
            }
            else if (!File.Exists(args[0]))
            {
                Console.WriteLine("The given input file does not exist!");
                return;
            }

            fileStream = new FileStream(args[0], FileMode.Open, FileAccess.Read);
            streamReader = new StreamReader(fileStream, Encoding.ASCII);
            sudokuBoardString = streamReader.ReadToEnd();
            streamReader.Close();
            fileStream.Close();

            squareToCellMapper = ReturnSquareToCellMapper();
            sudokuBoardCertainty = ReturnTwoDimensionalDataStructure(9, 9);
            sudokuBoardTmp = ReturnTwoDimensionalDataStructure(9, 9);
            sudokuBoardFinal = ReturnTwoDimensionalDataStructure(9, 9);

            if (!TryToInitSudokuBoard(sudokuBoardString, sudokuBoardCertainty, out errorMessage))
            {
                Console.WriteLine(string.Format("The given input file is incorrect! {0}", errorMessage));
                return;
            }

            if (!ValidateSudokuRule(sudokuBoardCertainty, squareToCellMapper, out errorMessage))
            {
                Console.WriteLine(string.Format("The given input file is incorrect! {0}", errorMessage));
                return;
            }

            candidates = ReturnThreeDimensionalDataStructure(9, 9, 10);
            InitCandidateStructure(sudokuBoardCertainty, candidates, squareToCellMapper, out cellsRemainToSet);
            numberOfCellsSetInInputSudokuBoard = 81 - cellsRemainToSet.Count;
            numberOfSimulations = 0;
            _random = new Random((int)(DateTime.Now.Ticks % 64765L));
            numberOfCellsSetInFinalSudoku = 0;
            index = 0;

            if (cellsRemainToSet.Count == 0)
            {
                Console.WriteLine("The sudoku is solved already.");
                return;
            }
            else if (_numberOfCandidates == 0)
            {
                Console.WriteLine("Not possible to add any number to the sudoku.");
                return;
            }

            debugIteration = 0;
            while (cellsRemainToSet.Count > 0)
            {
                if (index == cellsRemainToSet.Count)
                {
                    if (numberOfSimulations == 0 && _numberOfCandidates == 0)
                    {
                        numberOfCellsSetInFinalSudoku = 81 - cellsRemainToSet.Count;
                        CopySudokuBoard(sudokuBoardTmp, sudokuBoardFinal);
                        break;
                    }

                    if (numberOfSimulations == 0)
                    {
                        CopySudokuBoard(sudokuBoardCertainty, sudokuBoardTmp);
                        numberOfSimulations++;
                    }

                    if (_numberOfCandidates > 0)
                    {
                        SimulateOneCandidate(candidates, out row, out column, out candidate);
                        Debug.Register(debugIteration, sudokuBoardTmp, numberOfSimulations, index, _numberOfCandidates, cellsRemainToSet, candidates, row, column, candidate, 1);
                        SetNewCell(sudokuBoardTmp, row, column, candidate, candidates, squareToCellMapper, cellsRemainToSet, -1);      
                    }
                    else
                    {
                        if ((81 - cellsRemainToSet.Count) > numberOfCellsSetInFinalSudoku)
                        {
                            numberOfCellsSetInFinalSudoku = 81 - cellsRemainToSet.Count;
                            CopySudokuBoard(sudokuBoardTmp, sudokuBoardFinal);
                        }

                        CopySudokuBoard(sudokuBoardCertainty, sudokuBoardTmp);
                        InitCandidateStructure(sudokuBoardTmp, candidates, squareToCellMapper, out cellsRemainToSet);
                        numberOfSimulations++;
                    }

                    if (numberOfSimulations > _maxNumberOfSimulations)
                        break;
              
                    index = 0;
                }
                    
                row = ((int[])cellsRemainToSet[index])[0];
                column = ((int[])cellsRemainToSet[index])[1];
                square = ReturnSquare(row, column);

                candidate = 0;
                numberOfCandidates = candidates[row - 1][column - 1][0];

                if (numberOfCandidates == 1)
                {
                    candidate = candidates[row - 1][column - 1][1]; //Alone candidate in cell
                }
                else if (numberOfCandidates > 1)
                {
                    i = 0;

                    while (i < numberOfCandidates && candidate == 0)
                    {
                        if (NumberIsAloneCandidateInRow(row, candidates[row - 1][column - 1][1 + i], candidates))                
                            candidate = candidates[row - 1][column - 1][1 + i];                   
                        else if (NumberIsAloneCandidateInColumn(column, candidates[row - 1][column - 1][1 + i], candidates))               
                            candidate = candidates[row - 1][column - 1][1 + i];               
                        else if (NumberIsAloneCandidateInSquare(square, candidates[row - 1][column - 1][1 + i], candidates, squareToCellMapper))                    
                            candidate = candidates[row - 1][column - 1][1 + i];
                        else                 
                            i++;
                    }
                }

                debugIteration++;
                //Debug.PrintCandidates(candidates, debugIteration);

                if (candidate != 0)
                {
                    if (numberOfSimulations == 0)
                    {
                        Debug.Register(debugIteration, sudokuBoardCertainty, numberOfSimulations, index, _numberOfCandidates, cellsRemainToSet, candidates, row, column, candidate, 0);
                        SetNewCell(sudokuBoardCertainty, row, column, candidate, candidates, squareToCellMapper, cellsRemainToSet, index);
                    }
                    else
                    {
                        Debug.Register(debugIteration, sudokuBoardTmp, numberOfSimulations, index, _numberOfCandidates, cellsRemainToSet, candidates, row, column, candidate, 0);
                        SetNewCell(sudokuBoardTmp, row, column, candidate, candidates, squareToCellMapper, cellsRemainToSet, index);
                    }

                    index = 0;              
                }
                else
                {
                    if (numberOfSimulations == 0)
                    {
                        Debug.Register(debugIteration, sudokuBoardCertainty, numberOfSimulations, index, _numberOfCandidates, cellsRemainToSet, candidates, 0, 0, 0, 0);
                    }
                    else
                    {
                        Debug.Register(debugIteration, sudokuBoardTmp, numberOfSimulations, index, _numberOfCandidates, cellsRemainToSet, candidates, 0, 0, 0, 0);
                    }

                    index++;
                }
            }

            if (cellsRemainToSet.Count == 0 && numberOfSimulations == 0) //Sudoku solved in first try          
                sudokuBoardString = ReturnSudokuBoardsAsString(sudokuBoardCertainty);
            else if (cellsRemainToSet.Count == 0 && numberOfSimulations > 0) //Sudoku after simulation
                sudokuBoardString = ReturnSudokuBoardsAsString(sudokuBoardTmp);
            else
                sudokuBoardString = ReturnSudokuBoardsAsString(sudokuBoardFinal); //Sudoku only partially solved

            fi = new FileInfo(args[0]);
            fileNameFullPath = string.Format("{0}\\Result.txt", fi.DirectoryName);

            if (cellsRemainToSet.Count == 0)
                message = string.Format("Sudoku was solved. Numbers added = {0}. The following output file was created: {1}", 81 - numberOfCellsSetInInputSudokuBoard, fileNameFullPath);
            else
                message = string.Format("Sudoku was partially solved. Numbers added = {0}. Numbers remaining = {1}. The following output file was created: {2}", numberOfCellsSetInFinalSudoku - numberOfCellsSetInInputSudokuBoard, 81 - numberOfCellsSetInFinalSudoku, fileNameFullPath);

            fileStream = new FileStream(fileNameFullPath, FileMode.Create, FileAccess.Write);
            streamWriter = new StreamWriter(fileStream, Encoding.ASCII);
            streamWriter.Write(sudokuBoardString);
            streamWriter.Flush();
            fileStream.Flush();
            streamWriter.Close();
            fileStream.Close();

            Console.WriteLine(message);

            Debug.CreateNewFile("C:\\AAA\\Debug.txt", Debug.sb.ToString());
        }

        private static int ReturnSquare(int row, int column)
        {
            return 1 + (3 * ((row - 1) / 3)) + ((column - 1) / 3);
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

        private static int ReturnNumberOfOccurenciesOfNumberInRow(int[][] sudokuBoard, int row, int number)
        {
            int column, n = 0;

            for(column = 1; column <= 9; column++)
            {
                if (sudokuBoard[row - 1][column - 1] == number)
                    n++;
            }

            return n;
        }

        private static int ReturnNumberOfOccurenciesOfNumberInColumn(int[][] sudokuBoard, int column, int number)
        {
            int row, n = 0;

            for (row = 1; row <= 9; row++)
            {
                if (sudokuBoard[row - 1][column - 1] == number)
                    n++;
            }

            return n;
        }

        private static int ReturnNumberOfOccurenciesOfNumberInSquare(int[][] sudokuBoard, int[][][] squareToCellMapper, int square, int number)
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

            for(row = 1; row <= 9; row++)
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

        private static bool TryToInitSudokuBoard(string sudokuBoardString, int[][] sudokuBoard, out string errorMessage)
        {
            string[] rows, columns;
            int row, column, n;

            errorMessage = null;

            rows = sudokuBoardString.Split(new string[] { "\r\n" }, StringSplitOptions.None);

            if (rows.Length != 9)
            {
                errorMessage = "Number of rows are not 9 as expected!";
                return false;
            }

            for(row = 1; row <= 9; row++)
            {
                columns = rows[row - 1].Split(' ');

                if (columns.Length != 9)
                {
                    errorMessage = string.Format("Number of columns in row {0} is not 9 as expected!", row.ToString());
                    return false;
                }

                for (column = 1; column <= 9; column++)
                {
                    if (!int.TryParse(columns[column - 1], out n))
                    {
                        errorMessage = string.Format("The value, \"{0}\", in row {1} and column {2} is not a valid integer!", columns[column - 1], row.ToString(), column.ToString());
                        return false;
                    }

                    if (n < 0 || n > 9)
                    {
                        errorMessage = string.Format("The value, \"{0}\", in row {1} and column {2} is not an integer in range [0,9] as expected!", columns[column - 1], row.ToString(), column.ToString());
                        return false;
                    }

                    sudokuBoard[row - 1][column - 1] = n;
                }
            }

            return true;
        }

        private static bool ValidateSudokuRule(int[][] sudokuBoard, int[][][] squareToCellMapper, out string errorMessage)
        {
            int row, column, square, number;

            errorMessage = null;

            for (row = 1; row <= 9; row++)
            {
                for (column = 1; column <= 9; column++)
                {
                    square = ReturnSquare(row, column);

                    number = sudokuBoard[row - 1][column - 1];

                    if (number != 0)
                    {
                        if (ReturnNumberOfOccurenciesOfNumberInRow(sudokuBoard, row, number) > 1)
                        {
                            errorMessage = string.Format("The number {0} occurs more than once in row {1}", number.ToString(), row.ToString());
                            return false;
                        }
                        else if (ReturnNumberOfOccurenciesOfNumberInColumn(sudokuBoard, column, number) > 1)
                        {
                            errorMessage = string.Format("The number {0} occurs more than once in column {1}", number.ToString(), column.ToString());
                            return false;
                        }
                        else if (ReturnNumberOfOccurenciesOfNumberInSquare(sudokuBoard, squareToCellMapper, square, number) > 1)
                        {
                            errorMessage = string.Format("The number {0} occurs more than once in square {1}", number.ToString(), square.ToString());
                            return false;
                        }
                    }
                }
            }

            return true;
        }

        private static void InitCandidateStructure(int[][] sudokuBoard, int[][][] candidates, int[][][] squareToCellMapper, out ArrayList cellsRemainToSet)
        {
            int row, column, square, number, n;

            cellsRemainToSet = new ArrayList();
            _numberOfCandidates = 0;

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
                        cellsRemainToSet.Add(new int[] { row, column });
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
                                _numberOfCandidates++;
                            }
                        }
                    }
                }
            }
        }

        private static void RemoveCandidateIfItExists(int[] v, int number)
        {
            int i, n, index = -1;

            n = v[0];
            i = 1;
            while (i <= n && index == -1)
            {
                if (v[i] == number)
                    index = i;
                else
                    i++;
            }

            if (index != -1)
            {
                while (index + 1 <= n)
                {
                    v[index] = v[index + 1];
                    index++;
                }

                v[0]--;

                _numberOfCandidates--;
            }
        }

        private static bool NumberIsAloneCandidateInRow(int row, int number, int[][][] candidates)
        {
            int column, n, i, numberOfOccurenciesOfNumber = 0;

            for(column = 1; column <= 9; column++)
            {
                n = candidates[row - 1][column - 1][0];

                if (n > 0)
                {
                    for(i = 1; i <= n; i++)
                    {
                        if (candidates[row - 1][column - 1][i] == number)
                        {
                            numberOfOccurenciesOfNumber++;

                            if (numberOfOccurenciesOfNumber > 1)
                                return false;
                        }
                    }
                }
            }

            return true;
        }

        private static bool NumberIsAloneCandidateInColumn(int column, int number, int[][][] candidates)
        {
            int row, n, i, numberOfOccurenciesOfNumber = 0;

            for (row = 1; row <= 9; row++)
            {
                n = candidates[row - 1][column - 1][0];

                if (n > 0)
                {
                    for (i = 1; i <= n; i++)
                    {
                        if (candidates[row - 1][column - 1][i] == number)
                        {
                            numberOfOccurenciesOfNumber++;

                            if (numberOfOccurenciesOfNumber > 1)
                                return false;
                        }
                    }
                }
            }

            return true;
        }

        private static bool NumberIsAloneCandidateInSquare(int square, int number, int[][][] candidates, int[][][] squareToCellMapper)
        {
            int row, column, n, i, j, numberOfOccurenciesOfNumber = 0;

            for (i = 0; i < 9; i++)
            {
                row = squareToCellMapper[square - 1][i][0];
                column = squareToCellMapper[square - 1][i][1];
                n = candidates[row - 1][column - 1][0];

                if (n > 0)
                {
                    for (j = 1; j <= n; j++)
                    {
                        if (candidates[row - 1][column - 1][j] == number)
                        {
                            numberOfOccurenciesOfNumber++;

                            if (numberOfOccurenciesOfNumber > 1)
                                return false;

                            break;
                        }
                    }
                }
            }

            return true;
        }

        private static void RemoveCandidateInCurrentRowColumnSquare(int[][][] candidates, int[][][] squareToCellMapper, int row, int column, int candidate)
        {
            int i, r, c, square;

            square = ReturnSquare(row, column);

            for (i = 1; i <= 9; i++)
            {
                if ((i != column) && (candidates[row - 1][i - 1][0] > 0))
                {
                    RemoveCandidateIfItExists(candidates[row - 1][i - 1], candidate);
                }
            }

            for (i = 1; i <= 9; i++)
            {
                if ((i != row) && (candidates[i - 1][column - 1][0] > 0))
                {
                    RemoveCandidateIfItExists(candidates[i - 1][column - 1], candidate);
                }
            }

            for (i = 0; i < 9; i++)
            {
                r = squareToCellMapper[square - 1][i][0];
                c = squareToCellMapper[square - 1][i][1];

                if ((r != row) && (c != column) && (candidates[r - 1][c - 1][0] > 0))
                {
                    RemoveCandidateIfItExists(candidates[r - 1][c - 1], candidate);
                }
            }
        }

        private static void SetNewCell(int[][] sudokuBoard, int row, int column, int candidate, int[][][] candidates, int[][][] squareToCellMapper, ArrayList cellsRemainToSet, int index)
        {
            int i, r, c;
            RemoveCandidateInCurrentRowColumnSquare(candidates, squareToCellMapper, row, column, candidate);
            sudokuBoard[row - 1][column - 1] = candidate;
            _numberOfCandidates -= candidates[row - 1][column - 1][0]; //Remove all candidates in that cell
            candidates[row - 1][column - 1][0] = -1; //Indicates that the cell is set already


            i = 0;
            while (index == -1) //index == -1 if candidate was simulated, otherwise not
            {
                r = ((int[])cellsRemainToSet[i])[0];
                c = ((int[])cellsRemainToSet[i])[1];

                if (r == row && c == column)
                    index = i;

                i++;
            }

            cellsRemainToSet.RemoveAt(index);
        }

        private static void CopySudokuBoard(int[][] sudokuBoardFrom, int[][] sudokuBoardTo)
        {
            int row, column;

            for (row = 1; row <= 9; row++)
            {
                for (column = 1; column <= 9; column++)
                {
                    sudokuBoardTo[row - 1][column - 1] = sudokuBoardFrom[row - 1][column - 1];
                }
            }
        }

        private static string ReturnSudokuBoardsAsString(int[][] sudokuBoard)
        {
            int row, column;
            StringBuilder sb = new StringBuilder();
           
            for(row = 1; row <= 9; row++)
            {
                if (row > 1)
                    sb.Append("\r\n");

                for (column = 1; column <= 9; column++)
                {
                    if (column == 1)
                        sb.Append(sudokuBoard[row - 1][column - 1].ToString());
                    else
                        sb.Append(string.Format(" {0}", sudokuBoard[row - 1][column - 1].ToString()));
                }
            }

            return sb.ToString();
        }

        private static void SimulateOneCandidate(int[][][] candidates, out int row, out int column, out int candidate)
        {
            int i, j, index, minNumberOfCandidates = 9;
            ArrayList v;

            for(i = 0; i < 9; i++)
            {
                for (j = 0; j < 9; j++)
                {
                    if ((candidates[i][j][0] > 0) && (candidates[i][j][0] < minNumberOfCandidates))
                        minNumberOfCandidates = candidates[i][j][0];
                }
            }

            v = new ArrayList();

            for (i = 0; i < 9; i++)
            {
                for (j = 0; j < 9; j++)
                {
                    if (candidates[i][j][0] == minNumberOfCandidates)
                        v.Add(new int[] { i, j });
                }
            }

            index = _random.Next(0, v.Count);
            row = 1 + ((int[])v[index])[0];
            column = 1 + ((int[])v[index])[1];
            index = _random.Next(0, minNumberOfCandidates);
            candidate = candidates[row - 1][column - 1][1 + index];
        }
    }
}
