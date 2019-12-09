using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GetSolveStat
{
    class Program
    {
        static void Main(string[] args)
        {
            string initialSudokoBoard = File.ReadAllText(args[0]);
            int numberOfTimesToSolve = int.Parse(args[1]);
            string result = Sudoku.Sudoku.GetSolveStat(initialSudokoBoard, numberOfTimesToSolve);

            if (result == null)
            {
                Console.WriteLine("The sudoku can not be solved uniquely.");
            }
            else
            {
                Console.WriteLine(result);
            }

            string str = Console.ReadLine();
        }
    }
}
