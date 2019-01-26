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
        public string HtmlFileText { get; set; } //A tmp property to store the source code for the html-file when ResourcesType = Html and when send Resource to the client (not serialized and deserialized)
        public int WidthIframe { get; set; } //A tmp property to store the source code for the html-file when ResourcesType = Html and when send Resource to the client (not serialized and deserialized)
        public int HeightIframe { get; set; } //A tmp property to store the source code for the html-file when ResourcesType = Html and when send Resource to the client (not serialized and deserialized)
        public int WidthTextarea { get; set; } //A tmp property to store the source code for the html-file when ResourcesType = Html and when send Resource to the client (not serialized and deserialized)
        public int HeightTextarea { get; set; } //A tmp property to store the source code for the html-file when ResourcesType = Html and when send Resource to the client (not serialized and deserialized)

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
        private const string _basePath = "C:\\git_cjonasl\\Leander\\Solutions\\Nr1\\WebApplication1\\";
        private const string _fileNameFullPathNextResourceId = "C:\\git_cjonasl\\Leander\\Design Leander\\NextResourceId.txt";

        private static string SerializeHtmlResourceIframeTextAreaDimensions(int[] v)
        {
            return string.Format("{0} {1} {2} {3}", v[0].ToString(), v[1].ToString(), v[2].ToString(), v[3].ToString());
        }

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
            return string.Format("{0}\r\n\r\n----- New property -----\r\n\r\n{1}\r\n\r\n----- New property -----\r\n\r\n{2}\r\n\r\n----- New property -----\r\n\r\n{3}\r\n\r\n----- New property -----\r\n\r\n{4}\r\n\r\n----- New property -----\r\n\r\n{5}\r\n\r\n----- New property -----\r\n\r\n{6}\r\n\r\n----- New property -----\r\n\r\n{7}\r\n\r\n----- New property -----\r\n\r\n{8}\r\n\r\n----- New property -----\r\n\r\n{9}\r\n\r\n----- New property -----\r\n\r\n{10}\r\n\r\n----- New property -----\r\n\r\n{11}\r\n\r\n----- New property -----\r\n\r\n{11}", resource.Id.ToString(), resource.ResourcesType.ToString(), resource.Created, resource.Title, resource.KeyWords, (resource.Note ?? "null"), resource.PreviousResource.ToString(), resource.NextResource.ToString(), (resource.ThumbUpLocation ?? "null"), (resource.HtmlFile ?? "null"), (resource.Files == null ? "null" : resource.Files.Replace("\n", "----- New file -----")), (resource.Links == null ? "null" : resource.Links.Replace("\n", "----- New link -----")));
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

        public static Resource GetResource(int id, out string errorMessage)
        {
            string resourceDirectory, firstRow, firstRowTemplate, fileNameFullPath, resourceSerialized;
            Resource resourceDeserialized;
            int ifw, ifh, tw, th;

            errorMessage = null;

            resourceDirectory = ReturnResourceDirectory(id);
            fileNameFullPath = string.Format("{0}\\R{1}.txt", resourceDirectory, id.ToString());

            if (!File.Exists(fileNameFullPath))
            {
                errorMessage = string.Format("ERROR!! The resource R{0} does not exist!", id.ToString());
                return null;
            }

            resourceSerialized = Utility.ReturnFileContents(fileNameFullPath);
            resourceDeserialized = DeserializeResource(resourceSerialized);

            if (resourceDeserialized.ResourcesType == ResourcesType.Html)
            {
                resourceDeserialized.HtmlFileText = Utility.ReturnFileContents(_basePath + resourceDeserialized.HtmlFile.Replace('/', '\\'));
                Utility.ReturnTextExceptFirstRow(resourceDeserialized.HtmlFileText, out firstRow);
                 
                if (!Utility.CheckFirstRowInHtmlResource(firstRow, false, out firstRowTemplate, out ifw, out ifh, out tw, out th, out errorMessage))
                {
                    return null;
                }
                else
                {
                    resourceDeserialized.WidthIframe = ifw;
                    resourceDeserialized.HeightIframe = ifh;
                    resourceDeserialized.WidthTextarea = tw;
                    resourceDeserialized.HeightTextarea = th;
                }
            }

            return resourceDeserialized;
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

        private static bool CheckNextResourceId(int nextResourceId, out string errorMessage)
        {
            string folder;

            errorMessage = null;

            if (nextResourceId > 1)
            {
                folder = ReturnResourceDirectory(nextResourceId - 1);

                if (!Directory.Exists(folder))
                {
                    errorMessage = string.Format("ERROR!! Directory for previous resourse with Id = {0} does not exist as expected!", nextResourceId - 1);
                    return false;
                }
            }

            folder = ReturnResourceDirectory(nextResourceId);

            if (Directory.Exists(folder))
            {
                errorMessage = string.Format("ERROR!! Directory for next resourse with Id = {0} exists already!", nextResourceId);
                return false;
            }

            return true;
        }

        private static bool CheckHtmlFile(string htmlFile, out string errorMessage)
        {
            string fileContents, fileNameFullPath;

            errorMessage = null;

            fileNameFullPath = _basePath + htmlFile.Replace('/', '\\');

            if (!File.Exists(fileNameFullPath))
            {
                errorMessage = "ERROR!! The given Html-file does not exist!";
                return false;
            }
            else
            {
                fileContents = Utility.ReturnFileContents(fileNameFullPath);

                if (!fileContents.StartsWith("<!DOCTYPE html>"))
                {
                    errorMessage = "ERROR!! The given Html-file does not start with \"<!DOCTYPE html>\" as expected!";
                    return false;
                }
            }

            return true;
        }

        private static bool CheckFiles(string files, out string errorMessage)
        {
            errorMessage = null;

            if (string.IsNullOrEmpty(files))
                return true;

            string[] v = files.Split('\n');
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

            if (errorMessage == null)
                return true;
            else
                return false;
        }

        private static bool CheckLinks(string links, out string errorMessage)
        {
            errorMessage = null;

            if (string.IsNullOrEmpty(links))
                return true;

            string[] u, v;

            u = links.Split('\n');
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

            if (errorMessage == null)
                return true;
            else
                return false;
        }

        private static bool ValidateResource(Resource resource, out string errorMessage)
        {
            errorMessage = null;

            if (resource.ResourcesType == ResourcesType.Html)
            {
                if (!CheckHtmlFile(resource.HtmlFile, out errorMessage))
                    return false;
            }

            if (resource.ResourcesType == ResourcesType.Self)
            {
                if (!CheckFiles(resource.Files, out errorMessage))
                    return false;

                if (!CheckLinks(resource.Links, out errorMessage))
                    return false;
            }

            return true;
        }

        public static Resource AddResource(Resource resource, out string errorMessage)  //resource not complete. newResource will be complete
        {
            Resource newResource = null;
            int nextResourceId;
            string resourceSerialized, folder, fileNameFullPath, fileText, textExceptFirstRow, firstRow;

            errorMessage = null;

            try
            {
                nextResourceId = ReturnNextResourceId();

                if (!CheckNextResourceId(nextResourceId, out errorMessage))
                    return null;

                if (!ValidateResource(resource, out errorMessage))
                    return null;

                if (resource.ResourcesType == ResourcesType.Html)
                {
                    fileText = Utility.ReturnFileContents(_basePath + resource.HtmlFile.Replace('/', '\\'));
                    textExceptFirstRow = Utility.ReturnTextExceptFirstRow(fileText, out firstRow);
                    Utility.CreateNewFile(_basePath + resource.HtmlFile.Replace('/', '\\'), "<!DOCTYPE html> <!-- iframe dimension: [1000,600] textarea dimension: [1000px,500px] -->\r\n" + textExceptFirstRow);
                }

                newResource = new Resource(nextResourceId, resource.ResourcesType, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), resource.Title, resource.KeyWords, resource.Note, resource.PreviousResource, resource.NextResource, resource.ThumbUpLocation, resource.HtmlFile, resource.Files, resource.Links);
                folder = ReturnResourceDirectory(nextResourceId);
                Directory.CreateDirectory(folder);
                fileNameFullPath = string.Format("{0}\\R{1}.txt", folder, nextResourceId.ToString());
                resourceSerialized = SerializeResource(newResource);
                Utility.CreateNewFile(fileNameFullPath, resourceSerialized);
                UpdateNextResourceId(nextResourceId);
                ResourcePresentationInSearchUtility.AddResource(newResource);

                if (resource.ResourcesType == ResourcesType.ThumbUpLocation)
                {
                    fileNameFullPath = string.Format("C:\\git_cjonasl\\Leander\\Solutions\\Nr1\\WebApplication1\\LocationResource\\{0}.txt", resource.ThumbUpLocation);
                    Utility.CreateNewFile(fileNameFullPath, string.Format("{0} {1} {2}", newResource.PreviousResource, newResource.Id, newResource.NextResource));
                }
            }
            catch(Exception e)
            {
                errorMessage = string.Format("ERROR!! An Exception occured in method AddResource! e.Message:\r\n{0}", e.Message);
                return null;
            }

            return newResource;
        }

        public static Resource EditResource(Resource resource, out string errorMessage)
        {
            string resourceSerialized, folder, fileNameFullPath;

            errorMessage = null;

            try
            {
                if (!ValidateResource(resource, out errorMessage))
                    return null;

                folder = ReturnResourceDirectory(resource.Id);
                fileNameFullPath = string.Format("{0}\\R{1}.txt", folder, resource.Id.ToString());

                if (!File.Exists(fileNameFullPath))
                {
                    errorMessage = string.Format("ERROR!! The file R{0}.txt does not exist as expected!", resource.Id.ToString());
                    return null;
                }

                resourceSerialized = SerializeResource(resource);
                Utility.CreateNewFile(fileNameFullPath, resourceSerialized);
                ResourcePresentationInSearchUtility.UpdateResource(resource);

                if (resource.ResourcesType == ResourcesType.ThumbUpLocation)
                {
                    fileNameFullPath = string.Format("C:\\git_cjonasl\\Leander\\Solutions\\Nr1\\WebApplication1\\LocationResource\\{0}.txt", resource.ThumbUpLocation);
                    Utility.CreateNewFile(fileNameFullPath, string.Format("{0} {1} {2}", resource.PreviousResource, resource.Id, resource.NextResource));
                }
            }
            catch (Exception e)
            {
                errorMessage = string.Format("ERROR!! An Exception occured in method EditResource! e.Message:\r\n{0}", e.Message);
                return null;
            }

            return GetResource(resource.Id, out errorMessage); //Return the newly updated resource
        }

        public static string GetFileTextForHtmlResource(int id, out string errorMessage)
        {
            Resource resource;
            string fileText;

            resource = GetResource(id, out errorMessage);

            if (errorMessage != null)
                return null;

            if (resource.ResourcesType != ResourcesType.Html)
            {
                errorMessage = string.Format("ERROR!! The resource is of type '{0}' and not of type Html as expected!", resource.ResourcesType.ToString());
                return null;
            }

            fileText = Utility.ReturnFileContents(_basePath + resource.HtmlFile.Replace('/', '\\'));

            return fileText;
        }

        public static void UpdateFileTextAndTextareaDimensionForHtmlResource(WidthHeightTextData widthHeightTextData, out string errorMessage)
        {
            Resource resource;
            string textExceptFirstRow, firstRow, firstRowTemplate;
            int ifw, ifh, tw, th;

            try
            {
                resource = GetResource(widthHeightTextData.Id, out errorMessage);

                if (errorMessage != null)
                    return;

                if (resource.ResourcesType != ResourcesType.Html)
                {
                    errorMessage = string.Format("ERROR!! The resource is of type '{0}' and not of type Html as expected!", resource.ResourcesType.ToString());
                    return;
                }

                textExceptFirstRow = Utility.ReturnTextExceptFirstRow(widthHeightTextData.Text.Replace("\n", "\r\n"), out firstRow);

                if (Utility.CheckFirstRowInHtmlResource(firstRow, false, out firstRowTemplate, out ifw, out ifh, out tw, out th, out errorMessage))
                {
                    Utility.CreateNewFile(_basePath + resource.HtmlFile.Replace('/', '\\'), firstRowTemplate.Replace("#####REPLACE#####", string.Format("{0},{1}", widthHeightTextData.Width, widthHeightTextData.Height)) + textExceptFirstRow);
                }
                else //Error
                {
                    return;
                }
            }
            catch (Exception e)
            {
                errorMessage = string.Format("ERROR!! An Exception occured in method UpdateFileTextForHtmlResource! e.Message:\r\n{0}", e.Message);
                return;
            }
        }

        public static void UpdateHtmlResourceIframeDimension(int id, int width, int height, out string errorMessage)
        {
            Resource resource;
            string textExceptFirstRow, firstRow, fileText, firstRowTemplate;
            int ifw, ifh, tw, th;

            try
            {
                resource = GetResource(id, out errorMessage);

                if (errorMessage != null)
                    return;

                if (resource.ResourcesType != ResourcesType.Html)
                {
                    errorMessage = string.Format("ERROR!! The resource is of type '{0}' and not of type Html as expected!", resource.ResourcesType.ToString());
                    return;
                }

                fileText = Utility.ReturnFileContents(_basePath + resource.HtmlFile.Replace('/', '\\'));
                textExceptFirstRow = Utility.ReturnTextExceptFirstRow(fileText, out firstRow);
                
                if (Utility.CheckFirstRowInHtmlResource(firstRow, true, out firstRowTemplate, out ifw, out ifh, out tw, out th, out errorMessage))
                {
                    Utility.CreateNewFile(_basePath + resource.HtmlFile.Replace('/', '\\'), firstRowTemplate.Replace("#####REPLACE#####", string.Format("{0},{1}", width.ToString(), height.ToString())) + textExceptFirstRow);
                }
                else //Error
                {
                    return;
                }
            }
            catch (Exception e)
            {
                errorMessage = string.Format("ERROR!! An Exception occured in method UpdateFileTextForHtmlResource! e.Message:\r\n{0}", e.Message);
                return;
            }
        }
    }
}