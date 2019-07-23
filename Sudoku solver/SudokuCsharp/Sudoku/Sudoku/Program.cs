﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Sudoku
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args[2] == "1")
                SudokuMain.Sudoku.Run(args);
            else if (args[2] == "2")
                SudokuDebug.Sudoku.Run(args);
            else
                SudokuDebugOld.Sudoku.Run(args);

            //SudokuMarch2019.Sudoku.Run(args);
        }
    }
}