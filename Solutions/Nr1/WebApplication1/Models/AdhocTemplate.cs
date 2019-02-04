using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using Leander.Nr1;

namespace WebApplication1.Models
{
    public class AdhocTemplate
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string KeyWords { get; set; } //Key word ids comma separated
        public string TemplateFile { get; set; } //Full path
        public string ResourceFolder { get; set; }
        public int CurrentResourceFolderIndex { get; set; } //1,2,3,... to make folders "ResourceFolder\Resource1", "ResourceFolder\Resource1",...)
        public int MaxNumberOfFilesInFolder { get; set; }
        public string CsProjFileFullPath { get; set; } //The csproj where to insert the resource
        public string Href { get; set; } //For C# null, otherwise must be set

        public AdhocTemplate() { }

        public AdhocTemplate(int id, string name, string keyWords, string templateFile, string resourceFolder, int currentResourceFolderIndex, int maxNumberOfFilesInFolder, string csProjFileFullPath, string href)
        {
            this.Id = id;
            this.Name = name;
            this.KeyWords = keyWords;
            this.TemplateFile = templateFile;
            this.ResourceFolder = resourceFolder;
            this.CurrentResourceFolderIndex = currentResourceFolderIndex;
            this.MaxNumberOfFilesInFolder = maxNumberOfFilesInFolder;
            this.CsProjFileFullPath = csProjFileFullPath;
            this.Href = href;
        }
    }

    public static class AdhocTemplateUtility
    {
        private const string _fileNameFullPathToConfigFile = @"C:\git_cjonasl\Leander\Solutions\Nr1\WebApplication1\Text\Page1Menu1Sub1Sub1Tab1.txt";

        private static AdhocTemplate DeserializeAdhocTemplate(string adhocTemplate)
        {
            string[] v = adhocTemplate.Split(new string[] { "\r\n" }, StringSplitOptions.None);
            return new AdhocTemplate(int.Parse(v[0]), v[1], v[2], v[3], v[4], int.Parse(v[5]), int.Parse(v[6]), v[7], v[8]);
        }

        private static string SerializeAdhocTemplate(AdhocTemplate adhocTemplate)
        {
            return string.Format("{0}\r\n{1}\r\n{2}\r\n{3}\r\n{4}\r\n{5}\r\n{6}\r\n{7}\r\n{8}", adhocTemplate.Id.ToString(), adhocTemplate.Name, adhocTemplate.KeyWords, adhocTemplate.TemplateFile, adhocTemplate.ResourceFolder, adhocTemplate.CurrentResourceFolderIndex.ToString(), adhocTemplate.MaxNumberOfFilesInFolder.ToString(), adhocTemplate.CsProjFileFullPath, adhocTemplate.Href);
        }

        public static List<AdhocTemplate> ReturnListWithAdhocTemplates(out string errorMessage)
        {
            List<AdhocTemplate> listWithAdhocTemplates;
            string fileContents;
            int index;
            string[] v;
            int i;

            try
            {
                errorMessage = null;
                listWithAdhocTemplates = new List<AdhocTemplate>();
                fileContents = Utility.ReturnFileContents(_fileNameFullPathToConfigFile);
                index = fileContents.IndexOf("*/");
                v = fileContents.Substring(4 + index).Split(new string[] { "\r\n----- New adhoc -----\r\n" }, StringSplitOptions.None);

                for (i = 0; i < v.Length; i++)
                {
                    listWithAdhocTemplates.Add(DeserializeAdhocTemplate(v[i]));
                }
            }
            catch (Exception e)
            {
                errorMessage = string.Format("ERROR!! An Exception occured in method ReturnListWithAdhocTemplates! e.Message:\r\n{0}", e.Message);
                return null;
            }

            return listWithAdhocTemplates;
        }

        private static void SaveListWithAdhocTemplates(List<AdhocTemplate> listWithAdhocTemplates)
        {
            StringBuilder sb = new StringBuilder();

            for(int i = 0; i < listWithAdhocTemplates.Count; i++)
            {
                if (i == 0)
                    sb.Append(SerializeAdhocTemplate(listWithAdhocTemplates[0]));
                else
                    sb.Append(string.Format("\r\n----- New adhoc -----\r\n{0}", SerializeAdhocTemplate(listWithAdhocTemplates[i])));
            }

            Utility.CreateNewFile(_fileNameFullPathToConfigFile, sb.ToString());
        }

        public static IdText[] ReturnArrayWithIdText(out string errorMessage)
        {
            List<AdhocTemplate> listWithAdhocTemplates;
            IdText[] arrayWithIdText;

            errorMessage = null;

            try
            {
                listWithAdhocTemplates = ReturnListWithAdhocTemplates(out errorMessage);

                if (errorMessage != null)
                    return null;

                arrayWithIdText = new IdText[listWithAdhocTemplates.Count];

                for (int i = 0; i < listWithAdhocTemplates.Count; i++)
                {
                    arrayWithIdText[i] = new IdText(listWithAdhocTemplates[i].Id, listWithAdhocTemplates[i].Name);
                }
            }
            catch (Exception e)
            {
                errorMessage = string.Format("ERROR!! An Exception occured in method ReturnArrayWithIdText! e.Message:\r\n{0}", e.Message);
                return null;
            }

            return arrayWithIdText;
        }

        public static KeyWordsText ReturnKeyWordTextForAdhocTemplate(int id, out string errorMessage)
        {
            List<AdhocTemplate> listWithAdhocTemplates;
            KeyWordsText keyWordText;

            errorMessage = null;

            try
            {
                listWithAdhocTemplates = ReturnListWithAdhocTemplates(out errorMessage);

                if (errorMessage != null)
                    return null;

                keyWordText = new KeyWordsText(listWithAdhocTemplates[id - 1].KeyWords, Utility.ReturnFileContents(listWithAdhocTemplates[id - 1].TemplateFile).Substring(12)); //Need to take substring(12) since the dimension of the textarea is placed first in the file (fixed length 12 positions)
            }
            catch (Exception e)
            {
                errorMessage = string.Format("ERROR!! An Exception occured in method ReturnKeyWordTextForAdhocTemplate! e.Message:\r\n{0}", e.Message);
                return null;
            }

            return keyWordText;
        }

        public static int AddAdhocResource(TemplateData templateData, out string errorMessage)
        {
            List<AdhocTemplate> listWithAdhocTemplates;
            Resource resource, newResource;

            try
            {
                errorMessage = null;
                listWithAdhocTemplates = ReturnListWithAdhocTemplates(out errorMessage);

                if (errorMessage != null)
                    return -1;

                int resourceId = ResourceUtility.ReturnNextResourceId();
                string filePrefix = "R";
                string fileExtension = templateData.Id == 1 ? ".cs" : ".html";
                string fileNameFullPathCjProjFile = listWithAdhocTemplates[templateData.Id - 1].CsProjFileFullPath;
                string folderPrefix = "Nr";
                int currentResourceFolderIndex = listWithAdhocTemplates[templateData.Id - 1].CurrentResourceFolderIndex;
                int maxNumberOfFilesInAFolder = listWithAdhocTemplates[templateData.Id - 1].MaxNumberOfFilesInFolder;
                string resourceFolder = listWithAdhocTemplates[templateData.Id - 1].ResourceFolder;
                string textForNonIncludeFileToCreate;

                if (templateData.Id == 1)
                {
                    textForNonIncludeFileToCreate = templateData.CodeText.Replace("\n", "\r\n").Replace("#####", resourceId.ToString());
                }
                else
                {
                    textForNonIncludeFileToCreate = templateData.CodeText.Replace("\n", "\r\n");
                }

                int nextResourceFolderIndex;

                Utility.InsertNoneIncludeFileInCsProj(
                    resourceId,
                    filePrefix,
                    fileExtension,
                    fileNameFullPathCjProjFile,
                    folderPrefix,
                    currentResourceFolderIndex,
                    maxNumberOfFilesInAFolder,
                    resourceFolder,
                    textForNonIncludeFileToCreate,
                    out nextResourceFolderIndex,
                    out errorMessage
                    );

                if (errorMessage != null)
                    return -1;

                if (nextResourceFolderIndex != currentResourceFolderIndex)
                {
                    listWithAdhocTemplates[templateData.Id - 1].CurrentResourceFolderIndex = nextResourceFolderIndex;
                    SaveListWithAdhocTemplates(listWithAdhocTemplates);
                }

                int id = 0;
                ResourcesType resourcesType = templateData.Id == 1 ? ResourcesType.Self : ResourcesType.Html;
                string created = null;
                string title = templateData.Title;
                string keyWords = templateData.KeyWords;
                string note = templateData.Note;
                int previousResource = 0;
                int nextResource = 0;
                string thumbUpLocation = null;
                string htmlFile = templateData.Id == 1 ? null : string.Format("{0}\\Nr{1}\\R{2}.html", listWithAdhocTemplates[templateData.Id - 1].Href, currentResourceFolderIndex.ToString(), resourceId.ToString());
                string files = templateData.Id == 1 ? string.Format("{0}\\Nr{1}\\R{2}.cs", listWithAdhocTemplates[templateData.Id - 1].ResourceFolder, currentResourceFolderIndex.ToString(), resourceId.ToString()) : null;
                string links = null;

                resource = new Resource(
                    id,
                    resourcesType,
                    created,
                    title,
                    keyWords,
                    note,
                    previousResource,
                    nextResource,
                    thumbUpLocation,
                    htmlFile,
                    files,
                    links);

                newResource = ResourceUtility.AddResource(resource, out errorMessage);

                if (errorMessage != null)
                    return -1;
            }
            catch (Exception e)
            {
                errorMessage = string.Format("ERROR!! An Exception occured in method AddAdhocResource! e.Message:\r\n{0}", e.Message);
                return -1;
            }

            return newResource.Id;
        }
    }
}