using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Sudoku
{
    class Program
    {
        private static int[][][] _squareToCellMapper;
        private static int[][] _sudokuBoardCertainty, _sudokuBoardTmp, _sudokuBoardFinal;



        static void Main(string[] args)
        {
            _squareToCellMapper = ReturnSquareToCellMapper();
            _sudokuBoardCertainty = ReturnTwoDimensionalDataStructure(9, 9);
            _sudokuBoardTmp = ReturnTwoDimensionalDataStructure(9, 9);
            _sudokuBoardFinal = ReturnTwoDimensionalDataStructure(9, 9);

        }

        public static int ReturnSquare(int row, int column)
        {
            return 1 + (3 * ((row - 1) / 3)) + ((column - 1) / 3);
        }

        public static int[][] ReturnTwoDimensionalDataStructure(int m, int n)
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
    }
}
