using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication1.Models
{

    /*Data needed for every non-default location to call 3 JavaScript functions in $(document).ready in end of cshtml-file:

     $(document).ready(function () {
        window.jonas.checkIconAndSetTitle('@Model.Icon', '@Model.Title');
        window.jonas.setTabNames('@Model.Tab[0]', '@Model.Tab[1]', '@Model.Tab[2]', '@Model.Tab[3]', '@Model.Tab[4]', '@Model.Tab[5]', '@Model.Tab[6]', '@Model.Tab[7]', '@Model.Tab[8]', '@Model.Tab[9]');
        window.jonas.handlePreviousCurrentNextResource(@Model.PreviousResource, @Model.CurrentResource, @Model.NextResource);
    });

    */
    public class DocumentReadyDataForNonDefaultLocation
    {
        public string Icon { get; set; }
        public string Title { get; set; }
        public int PreviousResource { get; set; }
        public int CurrentResource { get; set; }
        public int NextResource { get; set; }
        public string[] Tab { get; set; }

        public DocumentReadyDataForNonDefaultLocation()
        {
            this.Icon = "fa-anchor";
            this.Title = "Title";

            this.Tab = new string[10];

            for (int i = 1; i <= 10; i++)
            {
                this.Tab[i - 1] = string.Format("Tab{0}", i.ToString());
            }
        }
    }
}