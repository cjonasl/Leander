using System;
using System.Collections.Generic;
using System.Linq;
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
        public int CurrentResourceFolder { get; set; } //1,2,3,... to make folders "ResourceFolder\Resource1", "ResourceFolder\Resource1",...
        public int MaxNumberOfFilesInFolder { get; set; }
        public string CsProj { get; set; } //The csproj where to insert the resource
        public string Href { get; set; } //For C# null, otherwise must be set

        public AdhocTemplate() { }

        public AdhocTemplate(int id, string name, string keyWords, string templateFile, string resourceFolder, int currentResourceFolder, int maxNumberOfFilesInFolder, string csproj, string href)
        {
            this.Id = id;
            this.Name = name;
            this.KeyWords = keyWords;
            this.TemplateFile = templateFile;
            this.ResourceFolder = resourceFolder;
            this.CurrentResourceFolder = currentResourceFolder;
            this.MaxNumberOfFilesInFolder = maxNumberOfFilesInFolder;
            this.CsProj = csproj;
            this.Href = href;
        }
    }

    public static class AdhocTemplateUtility
    {
        private const string _fileNameFullPathToConfigFile = @"C:\git_cjonasl\Leander\Solutions\Nr1\WebApplication1\Text\Page1Menu1Sub1Sub1Tab1.txt";

        public static List<AdhocTemplate> ReturnListWithAdhocTemplates(out string errorMessage)
        {
            List<AdhocTemplate> listWithAdhocTemplates;
            string fileContents;
            int index;
            string[] u, v;
            int i;

            try
            {
                errorMessage = null;
                listWithAdhocTemplates = new List<AdhocTemplate>();
                fileContents = Utility.ReturnFileContents(_fileNameFullPathToConfigFile);
                index = fileContents.IndexOf("*/");
                u = fileContents.Substring(4 + index).Split(new string[] { "\r\n----- New adhoc -----\r\n" }, StringSplitOptions.None);

                for (i = 0; i < u.Length; i++)
                {
                    v = u[i].Split(new string[] { "\r\n" }, StringSplitOptions.None);
                    listWithAdhocTemplates.Add(new AdhocTemplate(int.Parse(v[0]), v[1], v[2], v[3], v[4], int.Parse(v[5]), int.Parse(v[6]), v[7], v[8]));
                }
            }
            catch (Exception e)
            {
                errorMessage = string.Format("ERROR!! An Exception occured in method ReturnListWithAdhocTemplates! e.Message:\r\n{0}", e.Message);
                return null;
            }

            return listWithAdhocTemplates;
        }

        public static IdText[] ReturnArrayWithAdhocTemplates(out string errorMessage)
        {
            List<AdhocTemplate> listWithAdhocTemplates;
            IdText[] idText;

            errorMessage = null;

            try
            {
                listWithAdhocTemplates = ReturnListWithAdhocTemplates(out errorMessage);

                if (errorMessage != null)
                    return null;

                idText = new IdText[listWithAdhocTemplates.Count];

                for (int i = 0; i < listWithAdhocTemplates.Count; i++)
                {
                    idText[i] = new IdText(listWithAdhocTemplates[i].Id, listWithAdhocTemplates[i].Name);
                }
            }
            catch (Exception e)
            {
                errorMessage = string.Format("ERROR!! An Exception occured in method ReturnArrayWithAdhocTemplates! e.Message:\r\n{0}", e.Message);
                return null;
            }

            return idText;
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
    }
}