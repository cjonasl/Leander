using System;
using System.Collections;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Leander.Nr1;

namespace CompareFileNamesInTwoFolders
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                string[] v;
                string fileName, currentDirectory, fileNameFullPath;
                int i, totalNumberOfFiles, inBoth, inDir1ButNotInDir2, inDir2ButNotInDir1;
                ArrayList filesDir1, filesDir2, filesDir1ToLower, filesDir2ToLower, filesInBoth, filesInDir1ButNotInDir2, filesInDir2ButNotInDir1;

                if (args.Length != 2)
                {
                    Console.WriteLine("The program should have exactly two parametes (the two folders to compare)!");
                    Console.ReadKey();
                    return;
                }

                if (!Directory.Exists(args[0]))
                {
                    Console.WriteLine(string.Format("The first given directory, \"{0}\" does not exist!", args[0]));
                    Console.ReadKey();
                    return;
                }

                if (!Directory.Exists(args[1]))
                {
                    Console.WriteLine(string.Format("The second given directory, \"{0}\" does not exist!", args[1]));
                    Console.ReadKey();
                    return;
                }

                v = Directory.GetFiles(args[0]);
                filesDir1 = new ArrayList();
                filesDir1ToLower = new ArrayList();

                for (i = 0; i < v.Length; i++)
                {
                    fileName = v[i].Substring(1 + v[i].LastIndexOf("\\"));
                    filesDir1.Add(fileName);
                    filesDir1ToLower.Add(fileName.ToLower());
                }

                v = Directory.GetFiles(args[1]);
                filesDir2 = new ArrayList();
                filesDir2ToLower = new ArrayList();

                for (i = 0; i < v.Length; i++)
                {
                    fileName = v[i].Substring(1 + v[i].LastIndexOf("\\"));
                    filesDir2.Add(fileName);
                    filesDir2ToLower.Add(fileName.ToLower());
                }

                filesInBoth = new ArrayList();
                filesInDir1ButNotInDir2 = new ArrayList();
                filesInDir2ButNotInDir1 = new ArrayList();

                inBoth = 0;
                inDir1ButNotInDir2 = 0;
                inDir2ButNotInDir1 = 0;

                for (i = 0; i < filesDir1ToLower.Count; i++)
                {
                    fileName = (string)filesDir1ToLower[i];

                    if (filesDir2ToLower.IndexOf(fileName) >= 0)
                    {
                        filesInBoth.Add(filesDir1[i]);
                        inBoth++;
                    }
                    else
                    {
                        filesInDir1ButNotInDir2.Add(filesDir1[i]);
                        inDir1ButNotInDir2++;
                    }
                }

                for (i = 0; i < filesDir2ToLower.Count; i++)
                {
                    fileName = (string)filesDir2ToLower[i];

                    if (filesDir1ToLower.IndexOf(fileName) == -1)
                    {
                        filesInDir2ButNotInDir1.Add(filesDir2[i]);
                        inDir2ButNotInDir1++;
                    }
                }

                totalNumberOfFiles = inBoth + inDir1ButNotInDir2 + inDir2ButNotInDir1;

                filesInBoth.Sort();
                filesInDir1ButNotInDir2.Sort();
                filesInDir2ButNotInDir1.Sort();

                Console.WriteLine(string.Format("Dir1: {0}", args[0]));
                Console.WriteLine(string.Format("Dir2: {0}", args[1]));
                Console.WriteLine(string.Format("Total number of files: {0}", totalNumberOfFiles));
                Console.WriteLine(string.Format("In both: {0}", inBoth));
                Console.WriteLine(string.Format("In Dir1, but not in Dir2: {0}", inDir1ButNotInDir2));
                Console.WriteLine(string.Format("In Dir2, but not in Dir1: {0}", inDir2ButNotInDir1));

                Console.WriteLine();
                Console.WriteLine("Files in Dir1, but not in Dir2:");
                for (i = 0; i < filesInDir1ButNotInDir2.Count; i++)
                {
                    Console.WriteLine(filesInDir1ButNotInDir2[i]);
                }

                Console.WriteLine();
                Console.WriteLine("Files in Dir2, but not in Dir1:");
                for (i = 0; i < filesInDir2ButNotInDir1.Count; i++)
                {
                    Console.WriteLine(filesInDir2ButNotInDir1[i]);
                }

                Console.WriteLine();
                Console.WriteLine("Files in both:");
                for (i = 0; i < filesInBoth.Count; i++)
                {
                    Console.WriteLine(filesInBoth[i]);
                }

                currentDirectory = Directory.GetCurrentDirectory();
                fileNameFullPath = string.Format("{0}\\FilesInBoth.txt", currentDirectory);
                Utility.CreateNewFile(fileNameFullPath, string.Format("Dir1: {0}\r\n", args[0]) + string.Format("Dir2: {0}\r\n", args[1]) + Utility.ReturnItems(filesInBoth, "\r\n"));

                currentDirectory = Directory.GetCurrentDirectory();
                fileNameFullPath = string.Format("{0}\\FilesInDir1ButNotInDir2.txt", currentDirectory);
                Utility.CreateNewFile(fileNameFullPath, string.Format("Dir1: {0}\r\n", args[0]) + string.Format("Dir2: {0}\r\n", args[1]) + Utility.ReturnItems(filesInDir1ButNotInDir2, "\r\n"));

                currentDirectory = Directory.GetCurrentDirectory();
                fileNameFullPath = string.Format("{0}\\FilesInDir2ButNotInDir1.txt", currentDirectory);
                Utility.CreateNewFile(fileNameFullPath, string.Format("Dir1: {0}\r\n", args[0]) + string.Format("Dir2: {0}\r\n", args[1]) + Utility.ReturnItems(filesInDir2ButNotInDir1, "\r\n"));

                Console.ReadKey();
            }
            catch (Exception e)
            {
                Console.WriteLine(string.Format("An exception happened! e.Message = {0}", e.Message));
                Console.ReadKey();
                return;
            }
        }
    }
}
