using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections;
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
        Tab,
        LocationResource
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

        public ViewResult Default(Location location)
        {
            int nextResourceId;

            nextResourceId = ResourceUtility.ReturnNextResourceId();
 
            ViewBag.MaxResourceId = nextResourceId - 1;

            return View(location);
        }

        private DocumentReadyDataForNonDefaultLocation GetDocumentReadyDataForNonDefaultLocation(Location location)
        {
            string icon, title, baseFileName, previousCurrentNextResource;
            string[] tabNames, v;
            bool stop;
            int i;
            DocumentReadyDataForNonDefaultLocation data;

            data = new DocumentReadyDataForNonDefaultLocation();

            baseFileName = string.Format("Page{0}Menu{1}Sub{2}Sub{3}.txt", location.Page, location.Menu, location.Sub1, location.Sub2);

            icon = GetPageEntity(PageEntity.Icon, baseFileName);
            title = GetPageEntity(PageEntity.Title, baseFileName);

            if (!string.IsNullOrEmpty(icon))
                data.Icon = icon;

            if (!string.IsNullOrEmpty(title))
                data.Title = title;

            baseFileName = string.Format("Page{0}Menu{1}Sub{2}Sub{3}Tab{4}.txt", location.Page, location.Menu, location.Sub1, location.Sub2, location.Tab);
            previousCurrentNextResource = GetPageEntity(PageEntity.LocationResource, baseFileName);

            if (!string.IsNullOrEmpty(previousCurrentNextResource))
            {
                v = previousCurrentNextResource.Split(' ');
                data.PreviousResource = int.Parse(v[0]);
                data.CurrentResource = int.Parse(v[1]);
                data.NextResource = int.Parse(v[2]);
            }

            baseFileName = string.Format("Page{0}Menu{1}Sub{2}Sub{3}Tab", location.Page, location.Menu, location.Sub1, location.Sub2);
            tabNames = GetTabNames(baseFileName);
            stop = false;
            i = 0;
            while ((i < 10) && (!stop))
            {
                if (!string.IsNullOrEmpty(tabNames[i]))
                {
                    data.Tab[i] = tabNames[i];
                }
                else
                {
                    stop = true;
                }

                i++;
            }

            return data;
        }

        private DataDefaultLocation GetDefaultDataForNewLocation(Location location)
        {
            DocumentReadyDataForNonDefaultLocation data = GetDocumentReadyDataForNonDefaultLocation(location);
            DataDefaultLocation dataDefaultLocation = new DataDefaultLocation(data);
            string text = GetPageEntity(PageEntity.Text, string.Format("Page{0}Menu{1}Sub{2}Sub{3}Tab{4}.txt", location.Page, location.Menu, location.Sub1, location.Sub2, location.Tab));

            if (!string.IsNullOrEmpty(text))
            {
                dataDefaultLocation.Width = text.Substring(0, 5).Trim();
                dataDefaultLocation.Height = text.Substring(5, 5).Trim();
                dataDefaultLocation.Text = text.Substring(10);
            }

            return dataDefaultLocation;
        }

        public ActionResult NewLocation(Location location)
        {
            string locationStr = string.Format("Page{0}Menu{1}Sub{2}Sub{3}Tab{4}", location.Page, location.Menu, location.Sub1, location.Sub2, location.Tab);

            switch (locationStr)
            {
                case "Page1Menu0Sub0Sub0Tab1":
                    return View("Page1Menu0Sub0Sub0Tab1", GetDocumentReadyDataForNonDefaultLocation(location));
                case "Page1Menu0Sub0Sub0Tab2":
                    ViewBag.LlistWithKeyWords = KeyWordUtility.GetKeyWords();
                    return View("Page1Menu0Sub0Sub0Tab2", GetDocumentReadyDataForNonDefaultLocation(location));
                default:
                    return Json(GetDefaultDataForNewLocation(location), JsonRequestBehavior.AllowGet);
            }
        }

        private void HandleSaveOfPageEntity(PageEntity pageEntity, int page, int menu, int sub1, int sub2, int tab, string fileContent)
        {
            string baseFileName, fileNameFullPath;

            if ((pageEntity == PageEntity.Icon) || (pageEntity == PageEntity.Title)) //Same icon and title for all 10 tabs
                baseFileName = string.Format("Page{0}Menu{1}Sub{2}Sub{3}.txt", page, menu, sub1, sub2);
            else
                baseFileName = string.Format("Page{0}Menu{1}Sub{2}Sub{3}Tab{4}.txt", page, menu, sub1, sub2, tab);

            fileNameFullPath = string.Format("C:\\git_cjonasl\\Leander\\Solutions\\Nr1\\WebApplication1\\{0}\\{1}", pageEntity.ToString(), baseFileName);
            Utility.CreateNewFile(fileNameFullPath, fileContent);
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
                else if ((command.Cmd == "icon") || (command.Cmd == "title") || ((command.Cmd.Length >= 4) && (command.Cmd.Substring(0, 3) == "tab")))
                {
                    if ((command.Cmd == "icon") && !Utility.IconExists(command.Val))
                        return Json("ERROR!! The icon does not exist!!", JsonRequestBehavior.AllowGet);

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
                else if ((command.Cmd == "nr") || (command.Cmd == "er")) //New resource or edit resource
                {
                    LoadResourceData loadResourceData = new LoadResourceData();

                    loadResourceData.ArrayWithKeyWordShort = KeyWordUtility.ReturnArrayWithKeyWordShort();

                    if (command.Cmd == "nr")
                    {
                        ResourcesType resourcesType;

                        switch(command.Val)
                        {
                            case "ThumbUpLocation":
                                resourcesType = ResourcesType.ThumbUpLocation;
                                break;
                            case "Html":
                                resourcesType = ResourcesType.Html;
                                break;
                            case "Self":
                                resourcesType = ResourcesType.Self;
                                break;
                            default:
                                return Json("ERROR!! Incorrect ResourcesType!", JsonRequestBehavior.AllowGet);
                        }

                        loadResourceData.Resource = new Resource(0, resourcesType, "", "", "", "", 0, 0, "", "", "", "");
                    }
                    else
                    {
                        loadResourceData.Resource = ResourceUtility.ReturnResource(int.Parse(command.Val), out errorMessage);

                        if (errorMessage != null)
                            return Json(string.Format("ERROR!!\r\n", errorMessage), JsonRequestBehavior.AllowGet);
                    }
                   
                    return Json(loadResourceData, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception e)
            {
                return Json(string.Format("ERROR!! An Exception happened! e.Message:\r\n{0}", e.Message), JsonRequestBehavior.AllowGet);
            }

            return Json("Success", JsonRequestBehavior.AllowGet);
        }

        public ViewResult SearchResource(string searchTerm)
        {
            int index1, index2;
            string[] u;
            ArrayList v;

            List<ResourcePresentationInSearch> list = ResourcePresentationInSearchUtility.GetResources();

            ViewBag.SearchTerm = searchTerm;

            if (searchTerm == "a asc")
                return View(list);
            else if (searchTerm == "a")
                return View(list.OrderByDescending(x => x.Id).ToList());
            else
            {
                index1 = searchTerm.IndexOf("ka(");

                if (index1 >= 0)
                {
                    index2 = searchTerm.IndexOf(')', 3 + index1);
                    v = new ArrayList(searchTerm.Substring(3 + index1, index2 - index1 - 3).Split(','));
                    list = list.Where(x => Utility.PhrasesInArrayListAreAllPresentInCommaSeparatedListWithPhrases(v, x.KeyWords)).ToList();
                }

                index1 = searchTerm.IndexOf("ko(");

                if (index1 >= 0)
                {
                    index2 = searchTerm.IndexOf(')', 3 + index1);
                    v = new ArrayList(searchTerm.Substring(3 + index1, index2 - index1 - 3).Split(','));
                    list = list.Where(x => Utility.AtLeastOnePhraseInArrayListIsPresentInCommaSeparatedListWithPhrases(v, x.KeyWords)).ToList();
                }

                index1 = searchTerm.IndexOf("ta(");

                if (index1 >= 0)
                {
                    index2 = searchTerm.IndexOf(')', 3 + index1);
                    v = new ArrayList(searchTerm.Substring(3 + index1, index2 - index1 - 3).Split(','));
                    list = list.Where(x => Utility.PhrasesInArrayListAreAllPresentInString(v, x.Title)).ToList();
                }

                index1 = searchTerm.IndexOf("to(");

                if (index1 >= 0)
                {
                    index2 = searchTerm.IndexOf(')', 3 + index1);
                    v = new ArrayList(searchTerm.Substring(3 + index1, index2 - index1 - 3).Split(','));
                    list = list.Where(x => Utility.AtLeastOnePhraseInArrayListIsPresentInString(v, x.Title)).ToList();
                }

                index1 = searchTerm.IndexOf("c(");

                if (index1 >= 0)
                {
                    index2 = searchTerm.IndexOf(')', 2 + index1);
                    u = searchTerm.Substring(2 + index1, index2 - index1 - 2).Split(',');
                    list = list.Where(x => Utility.DateTimeFulfillRequirement(u[0], u[1], x.Created)).ToList();
                }
            }

            return View(list);
        }

        public JsonResult SaveKeyWord(KeyWord keyWord)
        {
            string errorMessage;
            KeyWord newKeyWord;

            if (!keyWord.Id.HasValue)
                newKeyWord = KeyWordUtility.AddKeyWord(keyWord, out errorMessage);
            else
                newKeyWord = KeyWordUtility.EditKeyWord(keyWord, out errorMessage);

            if (errorMessage == null)
                return Json(newKeyWord, JsonRequestBehavior.AllowGet);
            else
                return Json(errorMessage, JsonRequestBehavior.AllowGet);
        }

        public JsonResult SaveResource(Resource resource)
        {
            string errorMessage;
            Resource newResource = null;

            if (resource.Id == 0)
                newResource = ResourceUtility.AddResource(resource, out errorMessage);
            else
                newResource = ResourceUtility.EditResource(resource, out errorMessage);

            if (errorMessage == null)
                return Json(newResource, JsonRequestBehavior.AllowGet);        
            else
                return Json(errorMessage, JsonRequestBehavior.AllowGet);
        }
    }
}