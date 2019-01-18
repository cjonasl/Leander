using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using Leander.Nr1;

namespace WebApplication1.Models
{
    public class Resource
    {
        public int Id { get; set; }
        public ResourcesType ResourcesType { get; set; }
        public string Created { get; set; } //In format: yyyy-MM-dd HH:mm:ss
        public string Title { get; set; }
        public string KeyWords { get; set; } //Comma separated list with KeyWord Id:s.
        public string Note { get; set; }
        public int PreviousResource { get; set; }
        public int NextResource { get; set; }
        public string ThumbUpLocation { get; set; } //The location, Page???Menu???Sub???Sub???Tab???, when ResourcesType=ThumbUpLocation, otherwise null
        public string HtmlFile { get; set; } //The html-file (file name full path) when ResourcesType=Html, otherwise null
        public string Files { get; set; } //Files full path (separated with "----- New file -----" when serialized otherwise separated with \n) when ResourcesType=Self, otherwise null
        public string Links { get; set; } //Links (separated with "----- New link -----" when serialized otherwise separated with \n) when ResourcesType=Self, otherwise null. In a link "###" separate value of href-attribute and text to show for the link, for example https://www.expressen.se###Expressen will render like <a href="https://www.expressen.se">Expressen</a>

        public Resource() { }

        public Resource(int id, 
            ResourcesType resourcesType,
            string created,
            string title,
            string keyWords,
            string note, 
            int previousResource,
            int nextResource,
            string thumbUpLocation,
            string htmlFile,
            string files,
            string links)
        {
            this.Id = id;
            this.ResourcesType = resourcesType;
            this.Created = created;
            this.Title = title;
            this.KeyWords = keyWords;
            this.Note = string.IsNullOrEmpty(note) ? null : note;
            this.PreviousResource = previousResource;
            this.NextResource = nextResource;
            this.ThumbUpLocation = string.IsNullOrEmpty(thumbUpLocation) ? null : thumbUpLocation;
            this.HtmlFile = string.IsNullOrEmpty(htmlFile) ? null : htmlFile;
            this.Files = string.IsNullOrEmpty(files) ? null : files;
            this.Links = string.IsNullOrEmpty(links) ? null : links;
        }
    }  

    public static class ResourceUtility
    {
        private const string _fileNameFullPathNextResourceId = "C:\\git_cjonasl\\Leander\\Design Leander\\NextResourceId.txt";

        public static string ReturnResourceDirectory(int id)
        {
            string resourceDirectory;
            int a, b, c, d, e;

            a = id / 1000;

            if ((id % 1000) == 0)
                a--;

            b = 1000 * a;

            if ((id % 1000) == 0)
            {
                d = 9;
            }
            else
            {
                c = id % 1000;

                d = c / 100;

                if ((c % 100) == 0)
                    d--;
            }

            e = b + 100 * d;

            resourceDirectory = string.Format("C:\\git_cjonasl\\Leander\\Resources\\R{0}-{1}\\R{2}-{3}\\R{4}", b + 1, b + 1000, e + 1, e + 100, id);

            return resourceDirectory;
        }

        private static string SerializeResource(Resource resource)
        {
            return string.Format("{0}\r\n\r\n----- New property -----\r\n\r\n{1}\r\n\r\n----- New property -----\r\n\r\n{2}\r\n\r\n----- New property -----\r\n\r\n{3}\r\n\r\n----- New property -----\r\n\r\n{4}\r\n\r\n----- New property -----\r\n\r\n{5}\r\n\r\n----- New property -----\r\n\r\n{6}\r\n\r\n----- New property -----\r\n\r\n{7}\r\n\r\n----- New property -----\r\n\r\n{8}\r\n\r\n----- New property -----\r\n\r\n{9}\r\n\r\n----- New property -----\r\n\r\n{10}\r\n\r\n----- New property -----\r\n\r\n{11}", resource.Id.ToString(), resource.ResourcesType.ToString(), resource.Created, resource.Title, resource.KeyWords, (resource.Note ?? "null"), resource.PreviousResource.ToString(), resource.NextResource.ToString(), (resource.ThumbUpLocation ?? "null"), (resource.HtmlFile ?? "null"), (resource.Files == null ? "null" : resource.Files.Replace("\n", "----- New file -----")), (resource.Links == null ? "null" : resource.Links.Replace("\n", "----- New link -----")));
        }

        private static Resource DeserializeResource(string resource)
        {
            string[] v;
            ResourcesType resourcesType;

            v = resource.Split(new string[] { "\r\n\r\n----- New property -----\r\n\r\n" }, StringSplitOptions.None);

            switch(v[1])
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

            return new Resource(int.Parse(v[0]), resourcesType, v[2], v[3], v[4], (v[5] == "null" ? null : v[5]), int.Parse(v[6]), int.Parse(v[7]), (v[8] == "null" ? null : v[8]), (v[9] == "null" ? null : v[9]), (v[10] == "null" ? null : v[10].Replace("----- New file -----", "\n")), (v[11] == "null" ? null : v[11].Replace("----- New link -----", "\n")));
        }

        public static int ReturnNextResourceId()
        {
            return int.Parse(Utility.ReturnFileContents(_fileNameFullPathNextResourceId));
        }

        private static void UpdateNextResourceId(int currentResourceId)
        {
            int nextResourceId = 1 + currentResourceId;
            Utility.CreateNewFile(_fileNameFullPathNextResourceId, nextResourceId.ToString());
        }

        private static string CheckNextResourceId(int nextResourceId)
        {
            string folder;

            if (nextResourceId > 1)
            {
                folder = ReturnResourceDirectory(nextResourceId - 1);

                if (!Directory.Exists(folder))
                {
                    return string.Format("ERROR!! Directory for previous resourse with Id = {0} does not exist as expected!", nextResourceId - 1);
                }
            }

            folder = ReturnResourceDirectory(nextResourceId);

            if (Directory.Exists(folder))
            {
                return string.Format("ERROR!! Directory for next resourse with Id = {0} exists already!", nextResourceId);
            }

            return null;
        }

        private static string CheckHtmlFile(string htmlFile)
        {
            if (!File.Exists(htmlFile))
            {
                return "ERROR!! The given Html-file does not exist!";
            }
            else
                return null;
        }

        private static string CheckFiles(string files)
        {
            if (string.IsNullOrEmpty(files))
                return null;

            string[] v = files.Split('\n');
            string errorMessage = null;
            int i, n = v.Length;

            i = 0;

            while ((errorMessage == null) && (i < n))
            {
                if (!File.Exists(v[i]))
                {
                    errorMessage = string.Format("ERROR!! File number {0} does not exist", i + 1 );
                }
                else
                {
                    i++;
                }
            }

            return errorMessage;
        }

        private static string CheckLinks(string links)
        {
            if (string.IsNullOrEmpty(links))
                return null;

            string[] u, v;

            u = links.Split('\n');
            string errorMessage = null;
            int i, n = u.Length;

            i = 0;

            while ((errorMessage == null) && (i < n))
            {
                v = u[i].Split(new string[] { "###" }, StringSplitOptions.None);

                if ((v.Length != 2) || (string.IsNullOrEmpty(v[1])))
                {
                    errorMessage = string.Format("ERROR!! No text is given to link number {0}", i + 1);
                }
                else
                {
                    i++;
                }
            }

            return errorMessage;
        }

        public static Resource AddResource(Resource resource, out string errorMessage)  //resource not complete. newResource will be complete
        {
            Resource newResource = null;
            int nextResourceId;
            string resourceSerialized, folder, fileNameFullPath;

            errorMessage = null;

            try
            {
                nextResourceId = ReturnNextResourceId();
                errorMessage = CheckNextResourceId(nextResourceId);

                if (resource.ResourcesType == ResourcesType.Html)
                    errorMessage = CheckHtmlFile(resource.HtmlFile);
                else if (resource.ResourcesType == ResourcesType.Self)
                {
                    errorMessage = CheckFiles(resource.Files);

                    if (errorMessage == null)
                        errorMessage = CheckLinks(resource.Links);
                }

                if (errorMessage == null)
                {
                    newResource = new Resource(nextResourceId, resource.ResourcesType, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), resource.Title, resource.KeyWords, resource.Note, resource.PreviousResource, resource.NextResource, resource.ThumbUpLocation, resource.HtmlFile, resource.Files, resource.Links);
                    folder = ReturnResourceDirectory(nextResourceId);
                    Directory.CreateDirectory(folder);
                    fileNameFullPath = string.Format("{0}\\R{1}.txt", folder, nextResourceId.ToString());
                    resourceSerialized = SerializeResource(newResource);
                    Utility.CreateNewFile(fileNameFullPath, resourceSerialized);
                    UpdateNextResourceId(nextResourceId);
                    ResourcePresentationInSearchUtility.AddResource(newResource);
                }
            }
            catch(Exception e)
            {
                errorMessage = string.Format("ERROR!! An Exception occured in method AddResource! e.Message:\r\n{0}", e.Message);
                return null;
            }

            return newResource;
        }

        public static void UpdateResource(Resource resource, out string errorMessage)
        {
            string resourceSerialized, folder, fileNameFullPath;

            errorMessage = null;

            try
            {
                folder = ReturnResourceDirectory(resource.Id);
                fileNameFullPath = string.Format("{0}\\R{1}.txt", folder, resource.Id.ToString());

                if (!File.Exists(fileNameFullPath))
                {
                    errorMessage = string.Format("ERROR!! The file R{0}.txt does not exist as expected!", resource.Id.ToString());
                    return;
                }

                resourceSerialized = SerializeResource(resource);
                Utility.CreateNewFile(fileNameFullPath, resourceSerialized);
                ResourcePresentationInSearchUtility.UpdateResource(resource);
            }
            catch (Exception e)
            {
                errorMessage = string.Format("ERROR!! An Exception occured in method UpdateResource! e.Message:\r\n{0}", e.Message);
                return;
            }
        }
    }
}