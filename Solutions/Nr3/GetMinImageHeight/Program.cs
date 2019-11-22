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
            string str1, str2, str3, fileNameFullPath, errorMessage = "";
            bool errorFound;
            FileStream fileStream;
            StreamReader streamReader;
            StreamWriter streamWriter;
            Image img;
            
            string basePath = "C:\\git_cjonasl\\Leander\\Sudoku solver\\Code\\";

            string[] functions = new string[]
            {
                "CopyList",
                "CopySudokuBoard",
                "CopyCandidates",
                "SaveState",
                "RestoreState",
                "GetInputSudokuBoard",
                "CandidateIsAlonePossible",
                "RemoveNumberIfItExists",
                "ReturnNumberOfOccurenciesOfNumber",
                "ReturnTwoDimensionalDataStructure",
                "ReturnThreeDimensionalDataStructure",
                "ReturnSquareCellToRowColumnMapper",
                "ReturnSudokuBoardAsString",
                "SimulateOneNumber",
                "CheckIfCanUpdateBestSoFarSudokuBoard",
                "InitCandidates",
                "TryFindNumberToSetInCellWithCertainty",
                "UpdateCandidates",
                "ValidateSudokuBoard",
                "PrintSudokuBoard",
                "PrintResult",
                "Run"
            };

            int[] maxHeight = new int[22];

            string[] languages = new string[]
            {
                "CSharp",
                "JavaScript",
                "PHP",
                "Python",
                "Ruby"
            };

            errorFound = false;
            i = 0;

            while (i < functions.Length && !errorFound)
            {
                j = 0;

                while (j < languages.Length && !errorFound)
                {
                    fileNameFullPath = basePath + languages[j] + "\\" + functions[i] + ".png";

                    if (!File.Exists(fileNameFullPath))
                    {
                        errorMessage = "The following file does not exist: " + fileNameFullPath;
                        errorFound = true;
                    }

                    j++;
                }

                i++;
            }

            if (errorFound)
            {
                Console.Write(errorMessage);
                return;
            }

            for (i = 0; i < functions.Length; i++)
            {
                maxheight = int.MinValue;

                for (j = 0; j < languages.Length; j++)
                {
                    fileNameFullPath = basePath + languages[j] + "\\" + functions[i] + ".png";
                    img = Image.FromFile(fileNameFullPath);

                    if (maxheight < img.Height)
                    {
                        maxheight = img.Height;
                    }
                }

                sb.Append(functions[i] + ": " + maxheight.ToString() + "\r\n");
                maxHeight[i] = maxheight;
            }

            File.WriteAllText("C:\\A\\MaxImageHeights.txt", sb.ToString().TrimEnd());

            sb.Clear();

            for (i = 0; i < functions.Length; i++)
            {
                sb.Append(ReturnHElement(functions[i]));
                sb.Append(ReturnDivElement(functions[i]));

                for(j = 0; j < languages.Length; j++)
                {
                    sb.Append(ReturnAElement(functions[i], languages[j]));
                }

                sb.Append("        </div>\r\n");

                sb.Append(ReturnAnotherDivElement(functions[i], maxHeight[i]));
                sb.Append("\r\n");
            }

            fileStream = new FileStream("C:\\git_cjonasl\\Leander\\Sudoku solver\\Code\\SudokuSolver.html", FileMode.Open, FileAccess.Read);
            streamReader = new StreamReader(fileStream, Encoding.UTF8);
            str1 = streamReader.ReadToEnd();
            streamReader.Close();
            fileStream.Close();

            str2 = sb.ToString().TrimEnd();

            str3 = str1.Replace("#####REPLACE#####", str2);

            fileStream = new FileStream("C:\\git_cjonasl\\Leander\\Sudoku solver\\Code\\SudokuSolver_New.html", FileMode.Create, FileAccess.Write);
            streamWriter = new StreamWriter(fileStream, Encoding.UTF8);
            streamWriter.Write(str3);
            streamWriter.Flush();
            fileStream.Flush();
            streamWriter.Close();
            fileStream.Close();
        }

        private static string ReturnHElement(string fcn)
        {
            if (fcn == "CopyList")
                return string.Format("        <h3 id=\"h3{0}\" style=\"margin-top: 5px;\">{0}</h3>\r\n", fcn);
            else
                return string.Format("        <h3 id=\"h3{0}\">{0}</h3>\r\n", fcn);
        }

        private static string ReturnDivElement(string fcn)
        {
            return string.Format("        <div id=\"div{0}\" class=\"languages\" data-active=\"CSharp\">\r\n", fcn);
        }

        private static string ReturnAElement(string fcn, string lang)
        {
            if (lang == "CSharp")
                return string.Format("            <a id=\"a{0}CSharp\" href=\"javascript: setImagePath('{0}', 'CSharp')\" style=\"color: #6600ff; font-weight: bold;  font-family: Arial; text-decoration: none; font-size: larger;\">C#</a>\r\n", fcn);
            else
                return string.Format("            <a id=\"a{0}{1}\" href=\"javascript: setImagePath('{0}', '{1}')\">{1}</a>\r\n", fcn, lang);
        }

        private static string ReturnAnotherDivElement(string fcn, int height)
        {
            return string.Format("        <div class=\"code\" style=\"min-height: {1}px; margin-top: 3px;\"><img id=\"img{0}\" src=\"\" /></div>\r\n", fcn, height.ToString());
        }
    }
}
