using System;
using System.IO;
using Leander.Nr1;

namespace LogRequestResponse
{
    class Program
    {
        static void Main(string[] args)
        {
            int i, index;
            string configFolderFullPath, folder, fileContents, fileNameFullPath, errorMessage, fileNameFullPathToGlobalAsaxCs, state, fileContentsLeander;
            string[] stringArray;

            try
            {
                if (args.Length != 1)
                {
                    Console.WriteLine("There should be exactly one parameter to the program, the config-folder full name");
                    Console.ReadKey();
                    return;
                }

                configFolderFullPath = args[0].Trim();

                if (!Directory.Exists(configFolderFullPath))
                {
                    Console.WriteLine("The given config-folder does not exist!");
                    Console.ReadKey();
                    return;
                }

                folder = configFolderFullPath + "\\RequestResponse";

                if (!Directory.Exists(folder))
                {
                    Console.WriteLine(string.Format("Error!! The following folder does not exist!", folder));
                    Console.ReadKey();
                    return;
                }

                stringArray = new string[] { "Config.txt", "State.txt", "Leander.cs", "ShowFiles.exe" };

                for (i = 0; i < 4; i++)
                {
                    fileNameFullPath = configFolderFullPath + "\\" + stringArray[i];

                    if (!File.Exists(fileNameFullPath))
                    {
                        Console.WriteLine(string.Format("The following file does not exist as expected: {0}", fileNameFullPath));
                        Console.ReadKey();
                        return;
                    }
                }

                fileNameFullPath = configFolderFullPath + "\\Config.txt";

                ReadConfigFile(fileNameFullPath, configFolderFullPath, out fileNameFullPathToGlobalAsaxCs, out errorMessage);

                if (!string.IsNullOrEmpty(errorMessage))
                {
                    Console.WriteLine(errorMessage);
                    Console.ReadKey();
                    return;
                }

                fileNameFullPath = configFolderFullPath + "\\State.txt";
                state = Utility.ReturnFileContents(fileNameFullPath);

                if (state != "LogCodeNotAdded" && state != "LogCodeAddedToGlobalAsaxCs")
                {
                    Console.WriteLine(string.Format("Incorrect value, \"{0}\", of state in the file {1}! It should be \"LogCodeAddedToGlobalAsaxCs\" or \"LogCodeNotAdded\"", state, fileNameFullPath));
                    Console.ReadKey();
                    return;
                }

                if (state == "LogCodeNotAdded")
                {
                    fileNameFullPath = configFolderFullPath + "\\Global.asax.cs";

                    if (File.Exists(fileNameFullPath))
                    {
                        Console.WriteLine(string.Format("The following file should not exist when state is \"LogCodeNotAdded\": {0}", fileNameFullPath));
                        Console.ReadKey();
                        return;
                    }

                    fileContents = Utility.ReturnFileContents(fileNameFullPathToGlobalAsaxCs);

                    if (fileContents.IndexOf("CarlJonasLeander") >= 0)
                    {
                        Console.WriteLine(string.Format("The following file should not contain \"CarlJonasLeander\" when state is \"LogCodeNotAdded\": {0}", fileNameFullPathToGlobalAsaxCs));
                        Console.ReadKey();
                        return;
                    }

                    fileContents = Utility.ReturnFileContents(fileNameFullPathToGlobalAsaxCs);
                    fileContentsLeander = Utility.ReturnFileContents(configFolderFullPath + "\\Leander.cs");

                    index = fileContents.IndexOf("namespace ");

                    if (index == -1)
                    {
                        Console.WriteLine(string.Format("Can not find \"namespace \" in the following file: {0}", fileNameFullPathToGlobalAsaxCs));
                        Console.ReadKey();
                        return;
                    }
                    else
                    {
                        index = Utility.ReturnIndex(fileContents, false, index + 10, "{", 1);

                        if (index == -1)
                        {
                            Console.WriteLine("Can not insert contents in Leander.cs into Global.asax.cs!");
                            Console.ReadKey();
                            return;
                        }
                        else
                        {
                            fileContents = Utility.InsertText(fileContents, "\r\n" + fileContentsLeander, index + 1);
                        }
                    }

                    index = fileContents.IndexOf("Application_EndRequest");

                    if (index >= 0)
                    {
                        index = Utility.ReturnIndex(fileContents, false, index + 24, "{", 1);

                        if (index == -1)
                        {
                            Console.WriteLine("Unable to insert \"CarlJonasLeander.ApplicationEndRequest(HttpContext.Current.Request, this.Context);\" in Application_EndRequest()!");
                            Console.ReadKey();
                            return;
                        }
                        else
                        {
                            fileContents = Utility.InsertText(fileContents, "\r\n            CarlJonasLeander.ApplicationEndRequest(HttpContext.Current.Request, this.Context);", index + 1);
                        }
                    }
                    else
                    {
                        index = fileContents.IndexOf("public class MvcApplication");

                        if (index == -1)
                        {
                            Console.WriteLine("Unable to insert \"CarlJonasLeander.ApplicationEndRequest(HttpContext.Current.Request, this.Context);\" in Application_EndRequest()!");
                            Console.ReadKey();
                            return;
                        }
                        else
                        {
                            index = Utility.ReturnIndex(fileContents, false, index + 27, "{", 1);

                            if (index == -1)
                            {
                                Console.WriteLine("Unable to insert \"CarlJonasLeander.ApplicationEndRequest(HttpContext.Current.Request, this.Context);\" in Application_EndRequest()!");
                                Console.ReadKey();
                                return;
                            }

                            fileContents = Utility.InsertText(fileContents, "\r\n        protected void Application_EndRequest()\r\n        {\r\n            CarlJonasLeander.ApplicationEndRequest(HttpContext.Current.Request, this.Context);\r\n        }\r\n\r\n", index + 1);
                        }
                    }

                    index = fileContents.IndexOf("Application_BeginRequest");
                    
                    if (index >= 0)
                    {
                        index = Utility.ReturnIndex(fileContents, false, index + 24, "{", 1);

                        if (index == -1)
                        {
                            Console.WriteLine("Unable to insert \"CarlJonasLeander.ApplicationBeginRequest(HttpContext.Current.Response, this.Context);\" in Application_BeginRequest()!");
                            Console.ReadKey();
                            return;
                        }
                        else
                        {
                            fileContents = Utility.InsertText(fileContents, "\r\n            CarlJonasLeander.ApplicationBeginRequest(HttpContext.Current.Response, this.Context);", index + 1);
                        }
                    }
                    else
                    {
                        index = fileContents.IndexOf("public class MvcApplication");

                        if (index == -1)
                        {
                            Console.WriteLine("Unable to insert \"CarlJonasLeander.ApplicationBeginRequest(HttpContext.Current.Response, this.Context);\" in Application_BeginRequest()!");
                            Console.ReadKey();
                            return;
                        }
                        else
                        {
                            index = Utility.ReturnIndex(fileContents, false, index + 27, "{", 1);

                            if (index == -1)
                            {
                                Console.WriteLine("Unable to insert \"CarlJonasLeander.ApplicationBeginRequest(HttpContext.Current.Request, this.Context);\" in Application_BeginRequest()!");
                                Console.ReadKey();
                                return;
                            }

                            fileContents = Utility.InsertText(fileContents, "\r\n        protected void Application_BeginRequest()\r\n        {\r\n            CarlJonasLeander.ApplicationBeginRequest(HttpContext.Current.Response, this.Context);\r\n        }\r\n\r\n", index + 1);
                        }
                    }

                    File.Move(fileNameFullPathToGlobalAsaxCs, configFolderFullPath + "\\Global.asax.cs");
                    Utility.CreateNewFile(fileNameFullPathToGlobalAsaxCs, fileContents);

                    fileNameFullPath = configFolderFullPath + "\\State.txt";
                    Utility.CreateNewFile(fileNameFullPath, "LogCodeAddedToGlobalAsaxCs");

                    Console.WriteLine("State was successfullt moved to \"LogCodeAddedToGlobalAsaxCs\".");
                    Console.ReadKey();
                }
                else
                {
                    fileContents = Utility.ReturnFileContents(fileNameFullPathToGlobalAsaxCs);

                    if (fileContents.IndexOf("CarlJonasLeander") == -1)
                    {
                        Console.WriteLine(string.Format("The following file does not contain \"CarlJonasLeander\" as expected: {0}", fileNameFullPathToGlobalAsaxCs));
                        Console.ReadKey();
                        return;
                    }


                    fileNameFullPath = configFolderFullPath + "\\Global.asax.cs";

                    if (!File.Exists(fileNameFullPath))
                    {
                        Console.WriteLine(string.Format("The following file does not exist as expected: {0}", fileNameFullPath));
                        Console.ReadKey();
                        return;
                    }

                    File.Delete(fileNameFullPathToGlobalAsaxCs);
                    File.Move(fileNameFullPath, fileNameFullPathToGlobalAsaxCs);

                    fileNameFullPath = configFolderFullPath + "\\State.txt";
                    Utility.CreateNewFile(fileNameFullPath, "LogCodeNotAdded");

                    Console.WriteLine("State was successfullt moved to \"LogCodeNotAdded\".");
                    Console.ReadKey();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(string.Format("An exception happened! e.Message = {0}", e.Message));
                Console.ReadKey();
                return;
            }
        }

        private static void ReadConfigFile(string fileNameFullPath, string configFolderFullPath, out string fileNameFullPathToGlobalAsaxCs, out string errorMessage)
        {
            string fileContents, folder;
            string[] v, w;
            bool foundErrorOnRow3 = false;
            int i, n;

            fileNameFullPathToGlobalAsaxCs = null;
            errorMessage = null;

            try
            {
                fileContents = Utility.ReturnFileContents(fileNameFullPath);

                v = fileContents.Split(new string[] { "\r\n" }, StringSplitOptions.None);

                if (v.Length != 3)
                {
                    errorMessage = string.Format("Number of rows in file {0} is not exactly 3 as expected!", fileNameFullPath);
                    return;
                }

                fileNameFullPathToGlobalAsaxCs = v[0];

                if (!File.Exists(fileNameFullPathToGlobalAsaxCs))
                {
                    errorMessage = string.Format("The given Global.asax.cs-file, \"{0}\", does not exist!", fileNameFullPathToGlobalAsaxCs);
                    return;
                }

                folder = string.Format("{0}\\RequestResponse", configFolderFullPath);

                if (v[1] != folder)
                {
                    errorMessage = string.Format("The 2nd row in config file is not \"{0}\" as expected!", folder);
                    return;
                }

                w = v[2].Split(' ');

                if (w.Length == 12)
                {
                    i = 0;

                    while (i < 12 && !foundErrorOnRow3)
                    {
                        if (!int.TryParse(w[i], out n))
                            foundErrorOnRow3 = true;

                        i++;
                    }
                }
                else
                    foundErrorOnRow3 = true;

                if (foundErrorOnRow3)
                {
                    errorMessage = "The 3rd row in config file does not contain 12 non-negative integers, blank separated, as expected!";
                    return;
                }
            }
            catch (Exception e)
            {
                errorMessage = string.Format("An exception happened in method ReadConfigFile! e.Message = {0}", e.Message);
            }
        }
    }
}
