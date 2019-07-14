using System;
using System.Collections;
using System.IO;
using System.Text;

namespace SudokuMarch2019
{
    public static class Sudoku
    {
        private const int _maxNumberOfSimulations = 10;

        private static int[][][] _squareToCellMapper;
        private static int[][][] _candidates;     
        private static Random _random;
        private static int _numberOfCandidates;
        private static ArrayList _cellsRemainToSet; //List with two-tuples of int


        public static void Run(string[] args)
        {
            int[][] sudokuBoardCertainty, sudokuBoardTmp, sudokuBoardBestSoFar;
            int numberOfCellsSetInInputSudokuBoard, numberOfCellsSetInBestSoFar, numberOfSimulations, candidate, row, column, index;
            FileStream fileStream;
            StreamReader streamReader;
            StreamWriter streamWriter;
            FileInfo fi;
            string inputFolder, errorMessage, sudokuBoardString, message, fileNameFullPath;

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

            inputFolder = (new FileInfo(args[0])).DirectoryName;

            fileStream = new FileStream(args[0], FileMode.Open, FileAccess.Read);
            streamReader = new StreamReader(fileStream, Encoding.ASCII);
            sudokuBoardString = streamReader.ReadToEnd().Trim();
            streamReader.Close();
            fileStream.Close();

            _squareToCellMapper = ReturnSquareToCellMapper();
            sudokuBoardCertainty = ReturnTwoDimensionalDataStructure(9, 9);
            sudokuBoardTmp = ReturnTwoDimensionalDataStructure(9, 9);
            sudokuBoardBestSoFar = ReturnTwoDimensionalDataStructure(9, 9);

            if (!TryToInitSudokuBoard(sudokuBoardString, sudokuBoardCertainty, out errorMessage))
            {
                Console.WriteLine(string.Format("The given input file is incorrect! {0}", errorMessage));            
                return;
            }

            if (!ValidateSudokuRule(sudokuBoardCertainty, out errorMessage))
            {
                Console.WriteLine(string.Format("The given input file is incorrect! {0}", errorMessage));
                return;
            }

            _candidates = ReturnThreeDimensionalDataStructure(9, 9, 10);
            CalculateCandidateStructure(sudokuBoardCertainty);
            numberOfCellsSetInInputSudokuBoard = 81 - _cellsRemainToSet.Count;
            numberOfSimulations = 0;
            _random = new Random((int)(DateTime.Now.Ticks % 64765L));
            numberOfCellsSetInBestSoFar = 0;
            index = 0;

            if (_cellsRemainToSet.Count == 0)
            {
                Console.WriteLine("The sudoku is solved already.");
                return;
            }
            else if (_numberOfCandidates == 0)
            {
                Console.WriteLine("Not possible to add any number to the sudoku.");
                return;
            }
        
            while (_cellsRemainToSet.Count > 0)
            {
                if (index == _cellsRemainToSet.Count)
                {
                    if (numberOfSimulations == 0 && _numberOfCandidates == 0)
                    {
                        numberOfCellsSetInBestSoFar = 81 - _cellsRemainToSet.Count;
                        CopySudokuBoard(sudokuBoardCertainty, sudokuBoardBestSoFar);
                        break;
                    }

                    if (numberOfSimulations == 0)
                    {
                        CopySudokuBoard(sudokuBoardCertainty, sudokuBoardTmp);
                        numberOfSimulations++;
                    }

                    if (_numberOfCandidates > 0)
                    {
                        SimulateOneCandidate(out row, out column, out candidate);
                        SetNewCellAndUpdateStructure(sudokuBoardTmp, row, column, candidate, -1);
                    }
                    else
                    {
                        if ((81 - _cellsRemainToSet.Count) > numberOfCellsSetInBestSoFar)
                        {
                            numberOfCellsSetInBestSoFar = 81 - _cellsRemainToSet.Count;
                            CopySudokuBoard(sudokuBoardTmp, sudokuBoardBestSoFar);
                        }

                        CopySudokuBoard(sudokuBoardCertainty, sudokuBoardTmp);
                        CalculateCandidateStructure(sudokuBoardTmp);
                        numberOfSimulations++;
                    }

                    if (numberOfSimulations > _maxNumberOfSimulations)
                        break;
              
                    index = 0;
                }
                    
                row = ((int[])_cellsRemainToSet[index])[0];
                column = ((int[])_cellsRemainToSet[index])[1];

                if (CanSetCell(row, column, out candidate))
                {
                    if (numberOfSimulations == 0)
                        SetNewCellAndUpdateStructure(sudokuBoardCertainty, row, column, candidate, index);
                    else
                        SetNewCellAndUpdateStructure(sudokuBoardTmp, row, column, candidate, index);

                    index = 0;              
                }
                else          
                    index++;            
            }

            if (_cellsRemainToSet.Count == 0 && numberOfSimulations == 0) //Sudoku solved in first try          
                sudokuBoardString = ReturnSudokuBoardsAsString(sudokuBoardCertainty);
            else if (_cellsRemainToSet.Count == 0 && numberOfSimulations > 0) //Sudoku after simulation
                sudokuBoardString = ReturnSudokuBoardsAsString(sudokuBoardTmp);
            else
                sudokuBoardString = ReturnSudokuBoardsAsString(sudokuBoardBestSoFar); //Sudoku only partially solved

            fi = new FileInfo(args[0]);
            fileNameFullPath = string.Format("{0}\\Result.txt", fi.DirectoryName);

            if (_cellsRemainToSet.Count == 0)
                message = string.Format("Sudoku was solved. Numbers added = {0}. The following output file was created: {1}", 81 - numberOfCellsSetInInputSudokuBoard, fileNameFullPath);
            else
                message = string.Format("Sudoku was partially solved. Numbers added = {0}. Numbers remaining = {1}. The following output file was created: {2}", numberOfCellsSetInBestSoFar - numberOfCellsSetInInputSudokuBoard, 81 - numberOfCellsSetInBestSoFar, fileNameFullPath);

            fileStream = new FileStream(fileNameFullPath, FileMode.Create, FileAccess.Write);
            streamWriter = new StreamWriter(fileStream, Encoding.ASCII);
            streamWriter.Write(sudokuBoardString);
            streamWriter.Flush();
            fileStream.Flush();
            streamWriter.Close();
            fileStream.Close();

            Console.WriteLine(message);
        }

        private static int ReturnSquare(int row, int column)
        {
            return 1 + (3 * ((row - 1) / 3)) + ((column - 1) / 3);
        }

        private static bool CanSetCell(int row, int column, out int candidate)
        {
            int i, square, numberOfCandidates;

            candidate = 0;
            square = ReturnSquare(row, column);
            numberOfCandidates = _candidates[row - 1][column - 1][0];

            if (numberOfCandidates == 1)
            {
                candidate = _candidates[row - 1][column - 1][1]; //Alone candidate in cell
            }
            else if (numberOfCandidates > 1)
            {
                i = 0;

                while (i < numberOfCandidates && candidate == 0)
                {
                    if (NumberIsAloneCandidateInRow(row, _candidates[row - 1][column - 1][1 + i]))
                        candidate = _candidates[row - 1][column - 1][1 + i];
                    else if (NumberIsAloneCandidateInColumn(column, _candidates[row - 1][column - 1][1 + i]))
                        candidate = _candidates[row - 1][column - 1][1 + i];
                    else if (NumberIsAloneCandidateInSquare(square, _candidates[row - 1][column - 1][1 + i]))
                        candidate = _candidates[row - 1][column - 1][1 + i];
                    else
                        i++;
                }
            }

            return (candidate != 0) ? true : false;
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

        private static int ReturnNumberOfOccurenciesOfNumberInSquare(int[][] sudokuBoard, int square, int number)
        {
            int row, column, i, n = 0;

            for (i = 0; i < 9; i++)
            {
                row = _squareToCellMapper[square - 1][i][0];
                column = _squareToCellMapper[square - 1][i][1];

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

        private static bool ValidateSudokuRule(int[][] sudokuBoard, out string errorMessage)
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
                        else if (ReturnNumberOfOccurenciesOfNumberInSquare(sudokuBoard, square, number) > 1)
                        {
                            errorMessage = string.Format("The number {0} occurs more than once in square {1}", number.ToString(), square.ToString());
                            return false;
                        }
                    }
                }
            }

            return true;
        }

        private static void CalculateCandidateStructure(int[][] sudokuBoard)
        {
            int row, column, square, number, n;

            _cellsRemainToSet = new ArrayList();
            _numberOfCandidates = 0;

            for (row = 1; row <= 9; row++)
            {
                for (column = 1; column <= 9; column++)
                {
                    square = ReturnSquare(row, column);

                    if (sudokuBoard[row - 1][column - 1] != 0)
                    {
                        _candidates[row - 1][column - 1][0] = -1; //Indicates that the cell is set already
                    }
                    else
                    {
                        _cellsRemainToSet.Add(new int[] { row, column });
                        n = 0;
                        _candidates[row - 1][column - 1][0] = 0; //Number of candidates is set in index 0

                        for (number = 1; number <= 9; number++)
                        {
                            if ( 
                                (ReturnNumberOfOccurenciesOfNumberInRow(sudokuBoard, row, number) == 0) &&
                                (ReturnNumberOfOccurenciesOfNumberInColumn(sudokuBoard, column, number) == 0) &&
                                (ReturnNumberOfOccurenciesOfNumberInSquare(sudokuBoard, square, number) == 0)
                                )
                            {
                                n++;
                                _candidates[row - 1][column - 1][0] = n;
                                _candidates[row - 1][column - 1][n] = number;
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

        private static bool NumberIsAloneCandidateInRow(int row, int number)
        {
            int column, n, i, numberOfOccurenciesOfNumber = 0;

            for(column = 1; column <= 9; column++)
            {
                n = _candidates[row - 1][column - 1][0];

                if (n > 0)
                {
                    for(i = 1; i <= n; i++)
                    {
                        if (_candidates[row - 1][column - 1][i] == number)
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

        private static bool NumberIsAloneCandidateInColumn(int column, int number)
        {
            int row, n, i, numberOfOccurenciesOfNumber = 0;

            for (row = 1; row <= 9; row++)
            {
                n = _candidates[row - 1][column - 1][0];

                if (n > 0)
                {
                    for (i = 1; i <= n; i++)
                    {
                        if (_candidates[row - 1][column - 1][i] == number)
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

        private static bool NumberIsAloneCandidateInSquare(int square, int number)
        {
            int row, column, n, i, j, numberOfOccurenciesOfNumber = 0;

            for (i = 0; i < 9; i++)
            {
                row = _squareToCellMapper[square - 1][i][0];
                column = _squareToCellMapper[square - 1][i][1];
                n = _candidates[row - 1][column - 1][0];

                if (n > 0)
                {
                    for (j = 1; j <= n; j++)
                    {
                        if (_candidates[row - 1][column - 1][j] == number)
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

        private static void RemoveCandidateInCurrentRowColumnSquare(int row, int column, int candidate)
        {
            int i, r, c, square;

            square = ReturnSquare(row, column);

            for (i = 1; i <= 9; i++)
            {
                if ((i != column) && (_candidates[row - 1][i - 1][0] > 0))
                {
                    RemoveCandidateIfItExists(_candidates[row - 1][i - 1], candidate);
                }
            }

            for (i = 1; i <= 9; i++)
            {
                if ((i != row) && (_candidates[i - 1][column - 1][0] > 0))
                {
                    RemoveCandidateIfItExists(_candidates[i - 1][column - 1], candidate);
                }
            }

            for (i = 0; i < 9; i++)
            {
                r = _squareToCellMapper[square - 1][i][0];
                c = _squareToCellMapper[square - 1][i][1];

                if ((r != row) && (c != column) && (_candidates[r - 1][c - 1][0] > 0))
                {
                    RemoveCandidateIfItExists(_candidates[r - 1][c - 1], candidate);
                }
            }
        }

        private static void SetNewCellAndUpdateStructure(int[][] sudokuBoard, int row, int column, int candidate, int index)
        {
            int i, r, c;

            RemoveCandidateInCurrentRowColumnSquare(row, column, candidate);
            sudokuBoard[row - 1][column - 1] = candidate;
            _numberOfCandidates -= _candidates[row - 1][column - 1][0]; //Remove all candidates in that cell
            _candidates[row - 1][column - 1][0] = -1; //Indicates that the cell is set already

            i = 0;
            while (index == -1) //index == -1 if candidate was simulated, otherwise not
            {
                r = ((int[])_cellsRemainToSet[i])[0];
                c = ((int[])_cellsRemainToSet[i])[1];

                if (r == row && c == column)
                    index = i;

                i++;
            }

            _cellsRemainToSet.RemoveAt(index);
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

        private static void SimulateOneCandidate(out int row, out int column, out int candidate)
        {
            int r, c, index, minNumberOfCandidates = 9;
            ArrayList v;

            for(r = 1; r <= 9; r++)
            {
                for (c = 1; c <= 9; c++)
                {
                    if ((_candidates[r - 1][c - 1][0] > 0) && (_candidates[r - 1][c - 1][0] < minNumberOfCandidates))
                        minNumberOfCandidates = _candidates[r - 1][c - 1][0];
                }
            }

            v = new ArrayList();

            for (r = 1; r <= 9; r++)
            {
                for (c = 1; c <= 9; c++)
                {
                    if (_candidates[r - 1][c - 1][0] == minNumberOfCandidates)
                        v.Add(new int[] { r, c });
                }
            }

            index = _random.Next(0, v.Count);
            row = ((int[])v[index])[0];
            column = ((int[])v[index])[1];
            index = _random.Next(0, minNumberOfCandidates);
            candidate = _candidates[row - 1][column - 1][1 + index];
        }


        private static string ReturnSudokuBoardsAsString(int[][] sudokuBoard)
        {
            int row, column;
            StringBuilder sb = new StringBuilder();

            for (row = 1; row <= 9; row++)
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
    }
}
