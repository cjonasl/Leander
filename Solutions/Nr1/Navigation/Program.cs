using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Leander.Nr1
{
    public class Menu
    {
        public string Name { get; set; }
        public List<Menu> SubMenu { get; set; }
        public bool SubMenuIsVertical { get; set; }
        public Menu Parent { get; set; }

        public string FullName
        {
            get
            {
                if (Parent == null)
                    return Name;
                else
                    return Parent.Name + Name;
            }
        }

        public Menu(string name, Menu parent, string[] subMenuItems)
        {
            this.Name = name;
            SubMenu = new List<Menu>();

            if (subMenuItems == null)
                this.SubMenuIsVertical = true;
            else
            {
                this.SubMenuIsVertical = false;

                for (int i = 0; i < subMenuItems.Length; i++)
                {
                    SubMenu.Add(new Menu(subMenuItems[i], this, null));
                }
            }
                
            this.Parent = parent;        
        }

        public bool CanAddSubMenuItem(string newMenuItemName)
        {
            ArrayList v = new ArrayList();
            int index;

            for(int i = 0; i < SubMenu.Count; i++)
            {
                v.Add(SubMenu[i].Name.ToLower());
            }

            index = v.IndexOf(newMenuItemName.ToLower());

            return (index == -1);
        }

        public void Render(StreamWriter sw, string align)
        {
            string str;

            if (!SubMenuIsVertical)
            {
                StringBuilder sb = new StringBuilder(this.Name);

                for(int i = 0; i < this.SubMenu.Count; i++)
                {
                    sb.Append(" " + this.SubMenu[i].Name);
                }

                sw.WriteLine(string.Format("{0}<li>{1}</li>", align, sb.ToString()));
            }
            else
            {
                if (this.Parent != null)
                {
                    sw.WriteLine(string.Format("{0}<li>", align));
                    sw.WriteLine(string.Format("{0}    <ul>{1}", align, this.Name));
                    str = "        ";
                }
                else
                {
                    sw.WriteLine(string.Format("{0}<ul>{1}", align, this.Name));
                    str = "    ";
                }

                for (int i = 0; i < this.SubMenu.Count; i++)
                {
                    this.SubMenu[i].Render(sw, str + align);
                }

                if (this.Parent != null)
                {
                    sw.WriteLine(string.Format("{0}    </ul>", align));
                    sw.WriteLine(string.Format("{0}</li>", align));
                }
                else
                {
                    sw.WriteLine(string.Format("{0}</ul>", align));
                }
            }
        }
    }


    public class Program
    {
        private static Stack<Menu> _parent;
        private static List<Menu> _pages;
        private static Menu _currentParent;
        private static int _expectedIndex;

        static void Main(string[] args)
        {
            string fileContents, menuItemName, fileNameFullPathToOutputHtmlFile, errorMessage;
            string[] rows, subMenuItemNames;
            bool errorFound = false;
            ArrayList allowAbleStartIndexes = new ArrayList(new int[] { 0, 4, 8, 12 });
            int startIndex = -1,  previousStartIndex = 0, i = 0;

            _parent = new Stack<Menu>();
            _pages = new List<Menu>();
            _currentParent = null;
            _expectedIndex = 0;

            if (args.Length == 0)
            {
                Utility.Print("First parameter, name (full path) to navigation file is not given!");
                return;
            }

            if (args.Length == 2)
            {
                fileNameFullPathToOutputHtmlFile = args[1];

                if (!File.Exists(fileNameFullPathToOutputHtmlFile))
                {
                    Utility.Print("Incorrect 2nd parameter, the file does not exist!");
                    return;
                }

                if (!fileNameFullPathToOutputHtmlFile.EndsWith(".html"))
                {
                    Utility.Print("Incorrect 2nd parameter, the file name does not end with .html as expected!");
                    return;
                }
            }
            else
            {
                fileNameFullPathToOutputHtmlFile = null;
            }

            fileContents = Utility.ReturnFileContents(args[0], out errorMessage);

            if (errorMessage != null)
            {
                Utility.Print(errorMessage);
                return;
            }

            rows = fileContents.Split(new string[] { "\r\n" }, StringSplitOptions.None);

            while ((i < rows.Length) && (!errorFound))
            {
                Utility.NavigationRowIsCorrect(rows[i].TrimStart(), i + 1, out errorMessage);

                if (errorMessage != null)
                    errorFound = true;
                else
                {
                    startIndex = Utility.ReturnStartIndex(rows[i], i + 1, allowAbleStartIndexes, out errorMessage);

                    if (startIndex == 0)
                    {
                        previousStartIndex = 0;
                        _currentParent = null;
                    }

                    if (errorMessage != null)
                        errorFound = true;
                    else
                    {
                        if ((_expectedIndex > -1) && (startIndex != _expectedIndex))
                        {
                            Utility.Print(string.Format("Start index on row {0} is incorrect! It is expected to be {1}", i + 1, _expectedIndex));
                            errorFound = true;
                        }
                        else
                        {
                            if ((startIndex - previousStartIndex) > 4)
                            {
                                Utility.Print(string.Format("Start index on row {0} is incorrect!", i + 1));
                                errorFound = true;
                            }
                            else
                            {
                                menuItemName = Utility.ReturnMenuItemName(rows[i].TrimStart(), i + 1, out subMenuItemNames, out errorMessage);

                                if (errorMessage == null)
                                {
                                    if ((startIndex == 0) && (_parent.Count > 0))
                                    {
                                        _parent.Clear();
                                    }

                                    AddSubMenuToParent(i + 1, startIndex, previousStartIndex, menuItemName, subMenuItemNames, out errorMessage);
                                }
                                else
                                    errorFound = true;
                            }
                        }
                    }
                }

                previousStartIndex = startIndex;
                i++;
            }

            if (errorMessage != null)
            {
                Utility.Print(errorMessage);
                return;
            }

            if (fileNameFullPathToOutputHtmlFile != null)
                CreateOutputHtmlFile(fileNameFullPathToOutputHtmlFile);
        }

        public static void AddSubMenuToParent(int rowNr, int startIndex, int previousStartIndex,  string menuItemName, string[] subMenuItemNames, out string errorMessage)
        {
            Menu menu = new Menu(menuItemName, _currentParent, subMenuItemNames);
            int n = startIndex - previousStartIndex;

            errorMessage = null;

            if (n < 0)
            {
                _parent.Pop();

                if (n == -8)
                {
                    _parent.Pop();
                }

                _currentParent = _parent.Peek();
            }

            if ((startIndex != 0) &&(!_currentParent.CanAddSubMenuItem(menuItemName)))
            {
                errorMessage = string.Format("Error on row {0}! Can not add submenu item {1} to menu {2}, because there is already a sub menu item with that name!", rowNr.ToString(), menuItemName, _currentParent.FullName);
                return;
            }

            if (startIndex == 0)
                _pages.Add(menu);
            else
                _currentParent.SubMenu.Add(menu);

            if (subMenuItemNames == null)
            {           
                _currentParent = menu;      
                _parent.Push(menu);
            }

            if (subMenuItemNames == null)
                _expectedIndex = startIndex + 4;
            else
                _expectedIndex = -1;
        }

        public static void CreateOutputHtmlFile(string fileNameFullpath)
        {
            FileStream fileStream = new FileStream(fileNameFullpath, FileMode.Create, FileAccess.Write);
            StreamWriter streamWriter = new StreamWriter(fileStream, Encoding.UTF8);

            streamWriter.WriteLine("<!DOCTYPE html>");
            streamWriter.WriteLine();
            streamWriter.WriteLine("<html lang='en' xmlns='http://www.w3.org/1999/xhtml'>");
            streamWriter.WriteLine("    <head>");
            streamWriter.WriteLine("        <meta charset='utf-8' />");
            streamWriter.WriteLine("        <title>Test Navigation program</title>");
            streamWriter.WriteLine("    </head>");
            streamWriter.WriteLine("    <body>");

            for(int i = 0; i < _pages.Count; i++)
            {
                _pages[i].Render(streamWriter, "        ");

                if (i < (_pages.Count - 1))
                {
                    streamWriter.WriteLine();
                    streamWriter.WriteLine("    <hr>");
                    streamWriter.WriteLine();
                }
            }

            streamWriter.WriteLine("    </body>");
            streamWriter.WriteLine("</html>");

            streamWriter.Flush();
            fileStream.Flush();
            streamWriter.Close();
            fileStream.Close();
        }
    }
}
