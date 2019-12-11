using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrintBytesInFile
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length != 1)
            {
                Console.WriteLine("Exactly one parameter, file name full path, must be given to the program!");
            }
            else if (!File.Exists(args[0]))
            {
                Console.WriteLine("The given file does not exist!");
            }
            else
            {
                byte[] v = File.ReadAllBytes(args[0]);

                for(int i = 0; i < v.Length; i++)
                {
                    Console.WriteLine(v[i]);
                }
            }
        }
    }
}
