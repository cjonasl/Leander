using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Leander.Nr1;

namespace WebApplication1.Models
{
    public class ResourcePresentationInSearch
    {
        public int Id { get; set; }
        public ResourcesType ResourcesType { get; set; }
        public DateTime Created { get; set; }
        public string Title { get; set; }
        public string KeyWords { get; set; }
        

        public ResourcePresentationInSearch() { }

        public ResourcePresentationInSearch(int id, ResourcesType resourcesType, DateTime created, string title, string keyWords)
        {
            this.Id = id;
            this.ResourcesType = resourcesType;
            this.Created = created;
            this.Title = title;
            this.KeyWords = keyWords;        
        }
    }

    public static class ResourcePresentationInSearchUtility
    {
        private const string _fileNameFullPathToResources = "C:\\git_cjonasl\\Leander\\Design Leander\\Resources.txt";

        private static string SerializeResourcePresentationInSearch(ResourcePresentationInSearch resource)
        {
            return string.Format("{0}\r\n\r\n----- New property -----\r\n\r\n{1}\r\n\r\n----- New property -----\r\n\r\n{2}\r\n\r\n----- New property -----\r\n\r\n{3}\r\n\r\n----- New property -----\r\n\r\n{4}", resource.Id.ToString(), resource.ResourcesType.ToString(), resource.Created.ToString("yyyy-MM-dd HH:mm:ss"), resource.Title, resource.KeyWords);
        }

        private static ResourcePresentationInSearch DeserializeResource(string resource)
        {
            string[] v;
            ResourcesType resourcesType;
            DateTime created;
            int id;

            v = resource.Split(new string[] { "\r\n\r\n----- New property -----\r\n\r\n" }, StringSplitOptions.None);

            id = int.Parse(v[0]);

            switch (v[1])
            {
                case "ThumbUpLocation":
                    resourcesType = ResourcesType.ThumbUpLocation;
                    break;
                case "Html":
                    resourcesType = ResourcesType.Html;
                    break;
                default:
                    resourcesType = ResourcesType.Self;
                    break;
            }

            created = Utility.ReturnDateTimeFromString(v[2]);

            return new ResourcePresentationInSearch(id, resourcesType, created, v[3], v[4]);
        }

        public static List<ResourcePresentationInSearch> ReturnListWithAllResourcePresentationInSearch()
        {
            string[] list;
            List<ResourcePresentationInSearch> listWithResourcePresentationInSearch;
            int i;

            list = Utility.ReturnFileContents(_fileNameFullPathToResources).Split(new string[] { "\r\n\r\n---------- New resource ----------\r\n\r\n" }, StringSplitOptions.None);

            listWithResourcePresentationInSearch = new List<ResourcePresentationInSearch>();

            if (!string.IsNullOrEmpty(list[0].Trim()))
            {
                for (i = 0; i < list.Length; i++)
                {
                    listWithResourcePresentationInSearch.Add(DeserializeResource(list[i]));
                }
            }

            return listWithResourcePresentationInSearch;
        }

        public static void SaveListWithResourcePresentationInSearch(List<ResourcePresentationInSearch> listWithResourcePresentationInSearch)
        {
            int i;
            StringBuilder sb;

            sb = new StringBuilder();

            for(i = 0; i < listWithResourcePresentationInSearch.Count; i++)
            {
                if (i == 0)
                {
                    sb.Append(SerializeResourcePresentationInSearch(listWithResourcePresentationInSearch[0]));
                }
                else
                {
                    sb.Append(string.Format("{0}{1}", "\r\n\r\n---------- New resource ----------\r\n\r\n", SerializeResourcePresentationInSearch(listWithResourcePresentationInSearch[i])));
                }
            }

            Utility.CreateNewFile(_fileNameFullPathToResources, sb.ToString());
        }

        public static void AddResource(Resource resource)
        {
            string commaSeparatedListWithKeyWords, serializedResourcePresentationInSearch;
            ResourcePresentationInSearch resourcePresentationInSearch;
            DateTime created;

            created = Utility.ReturnDateTimeFromString(resource.Created);
            commaSeparatedListWithKeyWords = KeyWordUtility.ReturnCommaSeparatedListWithKeyWords(resource.KeyWords);

            resourcePresentationInSearch = new ResourcePresentationInSearch(resource.Id, resource.ResourcesType, created, resource.Title, commaSeparatedListWithKeyWords);
            serializedResourcePresentationInSearch = SerializeResourcePresentationInSearch(resourcePresentationInSearch);

            if (resource.Id > 1)
                Utility.AppendToFile(_fileNameFullPathToResources, string.Format("{0}{1}", "\r\n\r\n---------- New resource ----------\r\n\r\n", serializedResourcePresentationInSearch));
            else
                Utility.AppendToFile(_fileNameFullPathToResources, serializedResourcePresentationInSearch);
        }

        public static void UpdateResource(Resource resource)
        {
            string commaSeparatedListWithKeyWords;
            bool updateOfResourceFileNeeded = false;
            List<ResourcePresentationInSearch> listWithResourcePresentationInSearch;

            commaSeparatedListWithKeyWords = KeyWordUtility.ReturnCommaSeparatedListWithKeyWords(resource.KeyWords);

            listWithResourcePresentationInSearch = ReturnListWithAllResourcePresentationInSearch();

            if (listWithResourcePresentationInSearch[resource.Id - 1].KeyWords != commaSeparatedListWithKeyWords)
            {
                updateOfResourceFileNeeded = true;
                listWithResourcePresentationInSearch[resource.Id - 1].KeyWords = commaSeparatedListWithKeyWords;
            }

            if (listWithResourcePresentationInSearch[resource.Id - 1].Title != resource.Title)
            {
                updateOfResourceFileNeeded = true;
                listWithResourcePresentationInSearch[resource.Id - 1].Title = resource.Title;
            }

            if (updateOfResourceFileNeeded)
                SaveListWithResourcePresentationInSearch(listWithResourcePresentationInSearch);
        }

        public static void RegenerateResourceFile(out string message)
        {
            List<Resource> listWithAllResources;
            ResourcePresentationInSearch resourcePresentationInSearch;
            string commaSeparatedListWithKeyWords, serializedResourcePresentationInSearch, errorMessage;
            DateTime created;
            StringBuilder sb;
            int i;

            message = null;

            try
            {
                listWithAllResources = ResourceUtility.ReturnListWithResources(ResourcesType.All, out errorMessage);

                if (errorMessage != null)
                {
                    message = errorMessage;
                    return;
                }

                sb = new StringBuilder();

                for(i = 0; i < listWithAllResources.Count; i++)
                {
                    created = Utility.ReturnDateTimeFromString(listWithAllResources[i].Created);
                    commaSeparatedListWithKeyWords = KeyWordUtility.ReturnCommaSeparatedListWithKeyWords(listWithAllResources[i].KeyWords);
                    resourcePresentationInSearch = new ResourcePresentationInSearch(listWithAllResources[i].Id, listWithAllResources[i].ResourcesType, created, listWithAllResources[i].Title, commaSeparatedListWithKeyWords);
                    serializedResourcePresentationInSearch = SerializeResourcePresentationInSearch(resourcePresentationInSearch);

                    if (i > 0)
                        sb.Append(string.Format("{0}{1}", "\r\n\r\n---------- New resource ----------\r\n\r\n", serializedResourcePresentationInSearch));
                    else
                        sb.Append(serializedResourcePresentationInSearch);
                }

                Utility.CreateNewFile(_fileNameFullPathToResources, sb.ToString());

                message = string.Format("Resources.txt was successfully regenerated with {0} resources.", listWithAllResources.Count.ToString());
            }
            catch (Exception e)
            {
                message = string.Format("ERROR!! An Exception occured in method RegenerateResourceFile! e.Message:\r\n{0}", e.Message);
                return;
            }
        }

        public static void CheckResourceFile(out string message)
        {
            string commaSeparatedListWithKeyWords, errorMessage;
            List<Resource> listWithAllResources;
            List<ResourcePresentationInSearch> listWithResourcePresentationInSearch;
            int id;

            message = null;

            try
            {
                listWithAllResources = ResourceUtility.ReturnListWithResources(ResourcesType.All, out errorMessage);

                if (errorMessage != null)
                {
                    message = errorMessage;
                    return;
                }

                listWithResourcePresentationInSearch = ReturnListWithAllResourcePresentationInSearch();

                if (listWithResourcePresentationInSearch.Count != listWithAllResources.Count)
                {
                    message = string.Format("Number of resource in Resources.txt, {0} resources, is not the same as the actual number of resources, {1} resources!", listWithResourcePresentationInSearch.Count.ToString(), listWithAllResources.Count.ToString());
                    return;
                }

                id = 1;

                while ((id <= listWithResourcePresentationInSearch.Count) && (message == null))
                {
                    if (listWithResourcePresentationInSearch[id - 1].Id != id)
                    {
                        message = string.Format("ResourcePresentationInSearch number {0} in file Resources.txt does not have id = {1} as expected", id.ToString(), id.ToString());
                    }
                    else if (listWithResourcePresentationInSearch[id - 1].ResourcesType != listWithAllResources[id - 1].ResourcesType)
                    {
                        message = string.Format("ResourcePresentationInSearch number {0} in file Resources.txt does not have resource type = {1} as expected", id.ToString(), listWithAllResources[id - 1].ResourcesType.ToString());
                    }
                    else if (listWithResourcePresentationInSearch[id - 1].Created.ToString("yyyy-MM-dd HH:mm:ss") != listWithAllResources[id - 1].Created)
                    {
                        message = string.Format("ResourcePresentationInSearch number {0} in file Resources.txt does not have created date = {1} as expected", id.ToString(), listWithAllResources[id - 1].Created);
                    }
                    else if (listWithResourcePresentationInSearch[id - 1].Title != listWithAllResources[id - 1].Title)
                    {
                        message = string.Format("ResourcePresentationInSearch number {0} in file Resources.txt does not have title = {1} as expected", id.ToString(), listWithAllResources[id - 1].Title);
                    }

                    if (message == null)
                    {
                        commaSeparatedListWithKeyWords = KeyWordUtility.ReturnCommaSeparatedListWithKeyWords(listWithAllResources[id - 1].KeyWords);

                        if (commaSeparatedListWithKeyWords != listWithResourcePresentationInSearch[id - 1].KeyWords)
                        {
                            message = string.Format("ResourcePresentationInSearch number {0} in file Resources.txt does not have key words = {1} as expected", id.ToString(), commaSeparatedListWithKeyWords);
                        }
                    }

                    id++;
                }

                if (message == null)
                {
                    message = string.Format("All {0} ResourcePresentationInSearch in file Resources.txt are correct!", listWithResourcePresentationInSearch.Count.ToString());
                }
            }
            catch (Exception e)
            {
                message = string.Format("ERROR!! An Exception occured in method CheckResourceFile! e.Message:\r\n{0}", e.Message);
                return;
            }
        }
    }
}