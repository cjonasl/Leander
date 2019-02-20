using System;
using System.Collections;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Sudoku
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length != 2)
            {
                Console.Write("There must be 2 arguments to the program. First 0 or 1 (0=debug version and 1=non debug version) and then file name full path to sudoku to be solved.");
                return;
            }
            else if (args[0] != "0" && args[0] != "1")
            {
                Console.Write("First argument to the program must be 0 (debug version) or 1 (non debug version).");
                return;
            }

            if (args[0] == "0")
                SudokuDebugVersion.Sudoku.Solve(new string[] { args[1] });
            else
                Sudoku.Solve(new string[] { args[1] });
        }
    }
}