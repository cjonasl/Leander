using System;
using System.Drawing;
using System.IO;
using System.Text;

namespace GetMaxImageHeight
{
    class Program
    {
        static void Main(string[] args)
        {
            StringBuilder sb = new StringBuilder();
            int i, j, maxheight;
            Image img;
            
            string basePath = "C:\\git_cjonasl\\Leander\\Sudoku solver\\Code\\";

            string[] v = new string[]
            {
                "CopyList",
                "CopySudokuBoard",
                "CopyCandidates"
            };

            string[] languages = new string[]
            {
                "CSharp",
                "JavaScript",
                "PHP",
                "Python",
                "Ruby"
            };

            for(i = 0; i < v.Length; i++)
            {
                maxheight = int.MinValue;

                for (j = 0; j < v.Length; j++)
                {
                    img = Image.FromFile(basePath + languages[j] + "\\" + v[i] + ".png");

                    if (maxheight < img.Height)
                    {
                        maxheight = img.Height;
                    }
                }

                sb.Append(v[i] + ": " + maxheight.ToString() + "\r\n");
            }

            File.WriteAllText("C:\\A\\MaxImageHeights.txt", sb.ToString().TrimEnd());
        }
    }
}
