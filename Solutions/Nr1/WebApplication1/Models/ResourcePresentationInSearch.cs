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

        private static string SerializeResource(ResourcePresentationInSearch resource)
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

        public static List<ResourcePresentationInSearch> GetResources()
        {
            string[] list;
            List<ResourcePresentationInSearch> listWithResources;
            int i;

            list = Utility.ReturnFileContents(_fileNameFullPathToResources).Split(new string[] { "\r\n\r\n---------- New resource ----------\r\n\r\n" }, StringSplitOptions.None);

            listWithResources = new List<ResourcePresentationInSearch>();

            if (!string.IsNullOrEmpty(list[0].Trim()))
            {
                for (i = 0; i < list.Length; i++)
                {
                    listWithResources.Add(DeserializeResource(list[i]));
                }
            }

            return listWithResources;
        }

        public static void SaveListWithResources(List<ResourcePresentationInSearch> listWithResources)
        {
            int i;
            StringBuilder sb;

            sb = new StringBuilder();

            for(i = 0; i < listWithResources.Count; i++)
            {
                if (i == 0)
                {
                    sb.Append(SerializeResource(listWithResources[0]));
                }
                else
                {
                    sb.Append(string.Format("{0}{1}", "\r\n\r\n---------- New resource ----------\r\n\r\n", SerializeResource(listWithResources[0])));
                }
            }

            Utility.CreateNewFile(_fileNameFullPathToResources, sb.ToString());
        }

        public static void AddResource(Resource resource)
        {
            string commaSeparatedListWithKeyWords, serializedResource;
            ResourcePresentationInSearch resourcePresentationInSearch;
            DateTime created;

            created = Utility.ReturnDateTimeFromString(resource.Created);
            commaSeparatedListWithKeyWords = KeyWordUtility.ReturnCommaSeparatedListWithKeyWords(resource.KeyWords);

            resourcePresentationInSearch = new ResourcePresentationInSearch(resource.Id, resource.ResourcesType, created, resource.Title, commaSeparatedListWithKeyWords);

            serializedResource = SerializeResource(resourcePresentationInSearch);

            Utility.AppendToFile(_fileNameFullPathToResources, serializedResource);
        }

        public static void UpdateResource(Resource resource)
        {
            string commaSeparatedListWithKeyWords;
            ResourcePresentationInSearch resourcePresentationInSearch;
            List<ResourcePresentationInSearch> listWithResources;
            DateTime created;

            created = Utility.ReturnDateTimeFromString(resource.Created);
            commaSeparatedListWithKeyWords = KeyWordUtility.ReturnCommaSeparatedListWithKeyWords(resource.KeyWords);

            resourcePresentationInSearch = new ResourcePresentationInSearch(resource.Id, resource.ResourcesType, created, resource.Title, commaSeparatedListWithKeyWords);

            listWithResources = GetResources();

            listWithResources[resource.Id - 1] = resourcePresentationInSearch;

            SaveListWithResources(listWithResources);
        }
    }
}