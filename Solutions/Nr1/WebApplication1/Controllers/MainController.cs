using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Web;
using System.Text;
using System.Web.Mvc;
using WebApplication1.Models;
using Leander.Nr1;

namespace WebApplication1.Controllers
{
    public enum PageEntity
    {
        Text,
        Icon,
        Title,
        Tab
    }


    public class MainController : Controller
    {
        private string[] GetTabNames(string baseFileName)
        {
            string[] tabNames;
            string fileNameFullPath;
            bool stop;
            int i;

            tabNames = new string[] { null, null, null, null, null, null, null, null, null, null };

            stop = false;
            i = 1;
            while ((i <= 10) && (!stop))
            {
                fileNameFullPath = string.Format("C:\\git_cjonasl\\Leander\\Solutions\\Nr1\\WebApplication1\\Tab\\{0}{1}.txt", baseFileName, i.ToString());

                if (System.IO.File.Exists(fileNameFullPath))
                {
                    tabNames[i - 1] = Utility.ReturnFileContents(fileNameFullPath);
                }
                else
                {
                    stop = true;
                }

                i++;
            }

            return tabNames;
        }

        private string GetPageEntity(PageEntity pageEntity, string baseFileName)
        {
            string fileNameFullPath = string.Format("C:\\git_cjonasl\\Leander\\Solutions\\Nr1\\WebApplication1\\{0}\\{1}", pageEntity.ToString(), baseFileName);

            if (System.IO.File.Exists(fileNameFullPath))
                return Utility.ReturnFileContents(fileNameFullPath);
            else
                return null;         
        }

        private DataDefaultLocation GetDataForNewLocation(Location location)
        {
            string baseFileName, icon, title, text;
            DataDefaultLocation dataDefaultLocation = new DataDefaultLocation();
            string[] tabNames;
            bool stop;
            int i;

            baseFileName = string.Format("Page{0}Menu{1}Sub{2}Sub{3}.txt", location.Page, location.Menu, location.Sub1, location.Sub2);
            icon = GetPageEntity(PageEntity.Icon, baseFileName);
            title = GetPageEntity(PageEntity.Title, baseFileName);

            baseFileName = string.Format("Page{0}Menu{1}Sub{2}Sub{3}Tab{4}.txt", location.Page, location.Menu, location.Sub1, location.Sub2, location.Tab);
            text = GetPageEntity(PageEntity.Text, baseFileName);         

            if (!string.IsNullOrEmpty(icon))
                dataDefaultLocation.Icon = icon;

            if (!string.IsNullOrEmpty(title))
                dataDefaultLocation.Title = title;

            if (!string.IsNullOrEmpty(text))
            {
                dataDefaultLocation.Width = text.Substring(0, 5).Trim();
                dataDefaultLocation.Height = text.Substring(5, 5).Trim();
                dataDefaultLocation.Text = text.Substring(10);
            }

            if ((location.NewLocationByChangeOfTab.HasValue) && (!location.NewLocationByChangeOfTab.Value)) //Not need to update the tabs
            {
                baseFileName = string.Format("Page{0}Menu{1}Sub{2}Sub{3}Tab", location.Page, location.Menu, location.Sub1, location.Sub2);
                tabNames = GetTabNames(baseFileName);
                stop = false;
                i = 0;
                while ((i < 10) && (!stop))
                {
                    if (!string.IsNullOrEmpty(tabNames[i]))
                    {
                        dataDefaultLocation.Tab[i] = tabNames[i];
                    }
                    else
                    {
                        stop = true;
                    }

                    i++;
                }
            }

            return dataDefaultLocation;
        }

        public ViewResult Default(Location location)
        {
            return View(location);
        }

        public ActionResult NewLocation(Location location)
        {
            string locationStr;
            DataDefaultLocation dataDefaultLocation;
            ModelDataPageWithKeyWords modelDataPageWithKeyWords;

            locationStr = string.Format("Page{0}Menu{1}Sub{2}Sub{3}Tab{4}", location.Page, location.Menu, location.Sub1, location.Sub2, location.Tab);

            switch (locationStr)
            {
                case "Page1Menu0Sub0Sub0Tab1":
                    return View("Page1Menu0Sub0Sub0Tab1", GetIconTitleTabs(location));
                case "Page1Menu0Sub0Sub0Tab2":
                    modelDataPageWithKeyWords = new ModelDataPageWithKeyWords();
                    modelDataPageWithKeyWords.IconTitleTabs = GetIconTitleTabs(location);
                    modelDataPageWithKeyWords.listWithKeyWords = KeyWordUtility.GetKeyWords();
                    return View("Page1Menu0Sub0Sub0Tab2", modelDataPageWithKeyWords);
                default:
                    dataDefaultLocation = GetDataForNewLocation(location);
                    return Json(dataDefaultLocation, JsonRequestBehavior.AllowGet);
            }
        }

        private IconTitleTabs GetIconTitleTabs(Location location)
        {
            string icon, title, baseFileName;
            string[] tabNames;
            bool stop;
            int i;
            IconTitleTabs iconTitleTabs;

            iconTitleTabs = new IconTitleTabs();

            baseFileName = string.Format("Page{0}Menu{1}Sub{2}Sub{3}.txt", location.Page, location.Menu, location.Sub1, location.Sub2);

            icon = GetPageEntity(PageEntity.Icon, baseFileName);
            title = GetPageEntity(PageEntity.Title, baseFileName);

            if (!string.IsNullOrEmpty(icon))
                iconTitleTabs.Icon = icon;

            if (!string.IsNullOrEmpty(title))
                iconTitleTabs.Title = title;

            baseFileName = string.Format("Page{0}Menu{1}Sub{2}Sub{3}Tab", location.Page, location.Menu, location.Sub1, location.Sub2);
            tabNames = GetTabNames(baseFileName);
            stop = false;
            i = 0;
            while ((i < 10) && (!stop))
            {
                if (!string.IsNullOrEmpty(tabNames[i]))
                {
                    iconTitleTabs.Tab[i] = tabNames[i];
                }
                else
                {
                    stop = true;
                }

                i++;
            }

            return iconTitleTabs;
        }

        private void HandleSaveOfPageEntity(PageEntity pageEntity, int page, int menu, int sub1, int sub2, int tab, string fileContent)
        {
            string baseFileName, fileNameFullPathText;

            if ((pageEntity == PageEntity.Icon) || (pageEntity == PageEntity.Title)) //Same icon and title for all 10 tabs
                baseFileName = string.Format("Page{0}Menu{1}Sub{2}Sub{3}.txt", page, menu, sub1, sub2);
            else
                baseFileName = string.Format("Page{0}Menu{1}Sub{2}Sub{3}Tab{4}.txt", page, menu, sub1, sub2, tab);

            fileNameFullPathText = string.Format("C:\\git_cjonasl\\Leander\\Solutions\\Nr1\\WebApplication1\\{0}\\{1}", pageEntity.ToString(), baseFileName);
            Utility.CreateNewFile(fileNameFullPathText, fileContent);
        }

        [ValidateInput(false)]
        public JsonResult SaveTextArea(LocationExtension locationExtension)
        {
            try
            {
                HandleSaveOfPageEntity(PageEntity.Text, locationExtension.Page, locationExtension.Menu, locationExtension.Sub1, locationExtension.Sub2, locationExtension.Tab, locationExtension.Width.PadRight(5) + locationExtension.Height.PadRight(5) + locationExtension.Text);
            }
            catch(Exception e)
            {
                return Json(string.Format("ERROR!! An Exception happened! e.Message:\r\n", e.Message), JsonRequestBehavior.AllowGet);
            }

            return Json("Text saved", JsonRequestBehavior.AllowGet);
        }

        public JsonResult ExecuteCommand(Command command)
        {
            string fileNameFullPath, fileContents, newfileContents, searchTerm = "", newTerm = "", errorMessage;
            int startIndexSearchTerm, index;
            PageEntity pageEntity;

            try
            {
                if ((command.Cmd == "menu") || (command.Cmd == "sub1") || (command.Cmd == "sub2"))
                {
                    fileNameFullPath = string.Format("C:\\git_cjonasl\\Leander\\Solutions\\Nr1\\WebApplication1\\Views\\Shared\\_Layout{0}.cshtml", command.Page.ToString());
                    fileContents = Utility.ReturnFileContents(fileNameFullPath, out errorMessage);

                    if (errorMessage != null)
                        return Json(string.Format("ERROR!!\r\n", errorMessage), JsonRequestBehavior.AllowGet);

                    switch(command.Cmd)
                    {
                        case "menu":
                            searchTerm = string.Format("<span class='title' data-location='Menu{0}'>", command.Menu.ToString());
                            break;
                        case "sub1":
                            searchTerm = string.Format("<span class='title' data-location='Menu{0}Sub{1}'>", command.Menu.ToString(), command.Sub1.ToString());
                            break;
                        case "sub2":
                            searchTerm = string.Format("<span class='title' data-location='Menu{0}Sub{1}Sub{2}'>", command.Menu.ToString(), command.Sub1.ToString(), command.Sub1.ToString());
                            break;
                    }

                    if (!Utility.IsSearchTermUniqueInString(fileContents, searchTerm, out startIndexSearchTerm, out errorMessage))
                        return Json(string.Format("ERROR!!\r\n", errorMessage), JsonRequestBehavior.AllowGet);

                    index = fileContents.IndexOf("</span>", startIndexSearchTerm + searchTerm.Length - 1);

                    if (index == -1)
                        return Json("ERROR!! Can't find </span> after search term!", JsonRequestBehavior.AllowGet);

                    searchTerm = fileContents.Substring(startIndexSearchTerm, index + 7 - startIndexSearchTerm);

                    switch (command.Cmd)
                    {
                        case "menu":
                            newTerm = string.Format("<span class='title' data-location='Menu{0}'>{1}</span>", command.Menu.ToString(), command.Val);
                            break;
                        case "sub1":
                            newTerm = string.Format("<span class='title' data-location='Menu{0}Sub{1}'>{2}</span>", command.Menu.ToString(), command.Sub1.ToString(), command.Val);
                            break;
                        case "sub2":
                            newTerm = string.Format("<span class='title' data-location='Menu{0}Sub{1}Sub{2}'>{3}</span>", command.Menu.ToString(), command.Sub1.ToString(), command.Sub1.ToString(), command.Val);
                            break;
                    }

                    newfileContents = fileContents.Replace(searchTerm, newTerm);
                    Utility.CreateNewFile(fileNameFullPath, newfileContents);
                }
                else if ((command.Cmd == "icon") || (command.Cmd == "title") || (command.Cmd.Substring(0, 3) == "tab"))
                {
                    if ((command.Cmd == "icon") && !Utility.IconExists(command.Val))
                        return Json("The icon does not exist!!" , JsonRequestBehavior.AllowGet);

                    switch(command.Cmd)
                    {
                        case "icon":
                            pageEntity = PageEntity.Icon;
                            break;
                        case "title":
                            pageEntity = PageEntity.Title;
                            break;
                        default:
                            pageEntity = PageEntity.Tab;
                            break;
                    }

                    HandleSaveOfPageEntity(pageEntity, command.Page, command.Menu, command.Sub1, command.Sub2, command.Tab, command.Val);
                }
            }
            catch (Exception e)
            {
                return Json(string.Format("ERROR!! An Exception happened! e.Message:\r\n", e.Message), JsonRequestBehavior.AllowGet);
            }

            return Json("ok", JsonRequestBehavior.AllowGet);
        }

        public ViewResult SearchResource(string searchTerm)
        {
            List<ResourcePresentationInSearch> list = new List<ResourcePresentationInSearch>()
            {
                new ResourcePresentationInSearch(10, "Jonas abc", "JavaScript, C#", new DateTime(2018, 9, 3, 2, 2, 2)),
                new ResourcePresentationInSearch(5, "Karlstad heja", "music, adhoc, By a gift", new DateTime(2018, 10, 21, 20, 34, 55)),
                new ResourcePresentationInSearch(8, "Bangkok next", "travel", new DateTime(2019, 1, 4, 15, 17, 53)),
                new ResourcePresentationInSearch(6, "Work as consultant", "consultant, C#", new DateTime(2018, 2, 15, 23, 34, 35)),
                new ResourcePresentationInSearch(22, "Leisure in USA", "Washington, tennis, jQuery", new DateTime(2018, 5, 5, 5, 2, 47))
            };

            ViewBag.SearchTerm = searchTerm;

            return View(list);
        }
    }
}