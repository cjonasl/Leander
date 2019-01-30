using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication1.Models
{

    /*Data needed for every non-default location to call 5 JavaScript functions in $(document).ready in end of cshtml-file:

     $(document).ready(function () {
        window.jonas.checkIconAndSetTitle('@Model.Icon', '@Model.Title');
        window.jonas.setTabNames('@Model.TabNames[0]', '@Model.TabNames[1]', '@Model.TabNames[2]', '@Model.TabNames[3]', '@Model.TaNamesb[4]', '@Model.TabNames[5]', '@Model.TabNames[6]', '@Model.TabNames[7]', '@Model.TabNames[8]', '@Model.TabNames[9]');
        window.jonas.handlePreviousCurrentNextResource(@Model.PreviousResource, @Model.CurrentResource, @Model.NextResource);
        window.jonas.setTitleTextInBrowser(@Model.Page, @Model.Menu, @Model.Sub1, @Model.Sub2, @Model.Tab, '@Model.CshtmlFile');
        window.jonas.updateCssDisplayForContentDivs("nonDefaultLocation");
    });

    */
    public class DocumentReadyDataForNonDefaultLocation
    {
        public int Page { get; set; }
        public int Menu { get; set; }
        public int Sub1 { get; set; }
        public int Sub2 { get; set; }
        public int Tab { get; set; }
        public string CshtmlFile;
        public string Icon { get; set; }
        public string Title { get; set; }
        public int PreviousResource { get; set; }
        public int CurrentResource { get; set; }
        public int NextResource { get; set; }
        public string[] TabNames { get; set; }

        public DocumentReadyDataForNonDefaultLocation() { }
        public DocumentReadyDataForNonDefaultLocation(Location location, string cshtmlFile)
        {
            this.Page = location.Page;
            this.Menu = location.Menu;
            this.Sub1 = location.Sub1;
            this.Sub2 = location.Sub2;
            this.Tab = location.Tab;
            this.CshtmlFile = string.Format(" (non-default location - cshtml file: {0})", cshtmlFile);
            this.Icon = "fa-anchor";
            this.Title = "Title";

            this.TabNames = new string[10];

            for (int i = 1; i <= 10; i++)
            {
                this.TabNames[i - 1] = string.Format("Tab{0}", i.ToString());
            }
        }
    }
}