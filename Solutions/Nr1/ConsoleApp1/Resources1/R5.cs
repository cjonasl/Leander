namespace Leander.Nr1
{
    public static class R5
    {
        public static void Execute(string fileNameFullPath1, string fileNameFullPath2)
        {
            string errorMessage;
            string[] fileNamesFullPath;

            fileNamesFullPath = Utility.ReturnRowsInFile(fileNameFullPath1, out errorMessage);

            if (errorMessage != null)
            {
                Utility.Print(errorMessage);
                return;
            }

            if(!Utility.AllFilesExist(fileNamesFullPath, out errorMessage))
            {
                Utility.Print(errorMessage);
                return;
            }

            Utility.PutContentsInFilesInOneFile(fileNamesFullPath, fileNameFullPath2, out errorMessage);

            if (errorMessage != null)
            {
                Utility.Print(errorMessage);
            }
        }
    }
}
