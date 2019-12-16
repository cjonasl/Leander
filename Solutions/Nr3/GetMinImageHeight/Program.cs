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
            string[] functions;
            string divElement, suffix, title;
            int[] maxHeight;

            string basePath = "C:\\git_cjonasl\\Leander\\Sudoku solver\\Code\\";

            divElement = "divSourceCodeInDifferentLanguage"; // "divCompareLanguages"

            if (divElement == "divSourceCodeInDifferentLanguage")
            {
                suffix = "SCIDL";
                title = "Study the functions/ procedures in the algorithm";

                functions = new string[]
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
            }
            else
            {
                suffix = "CL";
                title = "Compare artifacts of the languages";

                functions = new string[]
                {
                    "Expression",
                    "ForLoop",
                    "ReadWriteTextFromFile",
                    "Statement",               
                    "WhileLoop"
                };
            }

            string[] languages = new string[]
            {
                "CSharp",
                "JavaScript",
                "PHP",
                "Python",
                "Ruby"
            };

            maxHeight = new int[functions.Length];

            sb.Append(string.Format("    <div id=\"{0}\" style=\"display: none;\">\r\n", divElement));
            sb.Append(string.Format("        <div id=\"divLeftMenu{0}\" class=\"leftMenuSourceCodeInDifferentLanguage\" data-active=\"{1}\">\r\n", suffix, functions[0]));
            sb.Append("            <h2 style=\"margin-top: -8px;\">Set a language for all functions:</h2>\r\n");
            sb.Append("            <div style=\"font-family: 'Times New Roman'; margin-bottom: 10px; border-bottom: 1px solid black;\">\r\n");
            sb.Append("                <ul class=\"inlineList\" style=\"position: relative; left: -42px; top: -10px;\">\r\n");

            sb.Append(string.Format("                    <li><input id=\"langCSharp{0}\" type=\"radio\" name=\"langForAllFunctions{0}\" checked onchange=\"setLanguage{0}('CSharp')\" />C#</li>\r\n", suffix));

            for (i = 1; i < languages.Length; i++)
            {
                sb.Append(string.Format("                    <li><input id=\"lang{0}{1}\" type=\"radio\" name=\"langForAllFunctions{1}\" onchange=\"setLanguage{1}('{0}')\" />{0}</li>\r\n", languages[i], suffix));
            }

            sb.Append("                </ul>\r\n");
            sb.Append("            </div>\r\n\r\n");

            sb.Append(string.Format("            <a id=\"aLeftMenu{0}\" href=\"#scroll{0}\" onclick=\"handleLeftMenu('{0}', '{1}')\" class=\"activeLeftMenuItem\">{0}</a>\r\n", functions[0], suffix));

            for (i = 1; i < functions.Length; i++)
            {
                sb.Append(string.Format("            <a id=\"aLeftMenu{0}\" href=\"#scroll{0}\" onclick=\"handleLeftMenu('{0}', '{1}')\">{0}</a>\r\n", functions[i], suffix));
            }

            sb.Append("        </div>\r\n");
            sb.Append("        <div class=\"main\">\r\n");
            sb.Append(string.Format("            <div id=\"scroll{0}\">&nbsp;&nbsp;&nbsp;</div><div>&nbsp;&nbsp;&nbsp;</div><div>&nbsp;&nbsp;&nbsp;</div>\r\n", functions[0]));
            sb.Append(string.Format("            <h1 style=\"font-family: 'Times New Roman';\">{0}</h1>\r\n", title));
            sb.Append("            <div>&nbsp;&nbsp;&nbsp;</div><div>&nbsp;&nbsp;&nbsp;</div><div>&nbsp;&nbsp;&nbsp;</div>\r\n\r\n");

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

            StringBuilder stringBuilder = new StringBuilder();

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

                stringBuilder.Append(functions[i] + ": " + maxheight.ToString() + "\r\n");
                maxHeight[i] = maxheight;
            }

            stringBuilder.Append("\r\n\r\nLanguages:\r\n" + ReturnString(languages));
            stringBuilder.Append("\r\n\r\nFunctions:\r\n" + ReturnString(functions));

            File.WriteAllText("C:\\A\\Log.txt", stringBuilder.ToString().TrimEnd());

            for (i = 0; i < functions.Length; i++)
            {
                sb.Append(string.Format("            <h3 id=\"h3{0}\">{0}</h3>\r\n", functions[i]));
                sb.Append(string.Format("            <div id=\"div{0}\" class=\"languages\" data-active=\"CSharp\">\r\n", functions[i]));

                for (j = 0; j < languages.Length; j++)
                {
                    sb.Append(ReturnAElement(functions[i], languages[j], suffix));
                }

                sb.Append("            </div>\r\n");
                sb.Append(string.Format("            <div class=\"code\" style=\"min-height: {0}px; margin-top: 3px;\"><img id=\"img{1}\" src=\"\" /></div>\r\n", maxHeight[i].ToString(), functions[i]));

                if (i < (functions.Length - 1))
                {
                    sb.Append(string.Format("            <div id=\"scroll{0}\" style=\"position: relative; top: -110px;\"></div>\r\n\r\n", functions[i + 1]));
                }
            }

            sb.Append("        </div>\r\n");
            sb.Append("    </div>\r\n");

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

        private static string ReturnAElement(string fcn, string lang, string suffix)
        {
            if (lang == "CSharp")
                return string.Format("                <a id=\"a{0}CSharp\" href=\"javascript: setImagePath('{0}', 'CSharp', '{1}')\" style=\"color: #6600ff; font-weight: bold;  font-family: Arial; text-decoration: none; font-size: larger;\">C#</a>\r\n", fcn, suffix);
            else
                return string.Format("                <a id=\"a{0}{1}\" href=\"javascript: setImagePath('{0}', '{1}', '{2}')\">{1}</a>\r\n", fcn, lang, suffix);
        }

        private static string ReturnString(string[] v)
        {
            StringBuilder sb = new StringBuilder("v = [");

            for(int i = 0; i < v.Length - 1; i++)
            {
                sb.Append(string.Format("'{0}', ", v[i]));
            }

            sb.Append(string.Format("'{0}'];", v[v.Length - 1]));

            return sb.ToString();
        }
    }
}
