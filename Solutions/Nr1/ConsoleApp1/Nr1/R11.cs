using System.IO;
using System.Text;

namespace Leander.Nr1
{
    public static class R11
    {
        public static void Execute()
        {
            FileStream fileStream;
            StreamWriter streamWriter;
            string fileNameFullPath;
            int h, i, j, k;

            for (h = 1; h <= 15; h++)
            {
                fileNameFullPath = string.Format("C:\\git_cjonasl\\Leander\\Solutions\\Nr1\\WebApplication1\\Views\\Main\\_Layout{0}.cshtml", h.ToString());
                fileStream = new FileStream(fileNameFullPath, FileMode.Create, FileAccess.Write);
                streamWriter = new StreamWriter(fileStream, Encoding.UTF8);

                streamWriter.WriteLine("");
                streamWriter.WriteLine("@{");
                streamWriter.WriteLine("  Layout = \"~/Views/Shared/_LayoutTopLevel.cshtml\";");
                streamWriter.WriteLine("}");
                streamWriter.WriteLine("");
                streamWriter.WriteLine("<div class='page-container row-fluid'>");
                streamWriter.WriteLine("  <div class='hijumpSidebarMenu page-sidebar nav-collapse collapse'>");
                streamWriter.WriteLine("    <ul class='page-sidebar-menu'>");
                streamWriter.WriteLine("      <li>");
                streamWriter.WriteLine("        <div class='sidebar-toggler'></div>");
                streamWriter.WriteLine("      </li>");
                streamWriter.WriteLine("      <li>");
                streamWriter.WriteLine("        <div class='sidebar-search'>");
                streamWriter.WriteLine("          <div class='input-box'>");
                streamWriter.WriteLine("            <input id='searchTerm' type='text' name='searchTerm' autocomplete='off' autocorrect='off' autocapitalize='off' spellcheck='false' placeholder='Search...' />");
                streamWriter.WriteLine("            <input type='button' class='submit' value='' onclick='window.jonas.searchResources()' />");
                streamWriter.WriteLine("          </div>");
                streamWriter.WriteLine("        </div>");
                streamWriter.WriteLine("      </li>");
                streamWriter.WriteLine("      <li id='liDashboard'>");
                streamWriter.WriteLine(string.Format("        <a href='javascript: window.jonas.newLocation({0}, 0, 0, 0, 1, false)'>", h.ToString()));
                streamWriter.WriteLine("          <i class='fas fa-tachometer-alt'></i>");
                streamWriter.WriteLine("          <span class='title'>Dashboard</span>");
                streamWriter.WriteLine("        </a>");
                streamWriter.WriteLine("      </li>");

                for (i = 1; i <= 10; i++)
                {
                    streamWriter.WriteLine(string.Format("      <li id='liMenu{0}'>", i.ToString()));
                    streamWriter.WriteLine("        <a href='javascript:;'>");
                    streamWriter.WriteLine(string.Format("          <span class='title' data-location='Menu{0}'>Menu{0}</span>", i.ToString(), i.ToString()));
                    streamWriter.WriteLine(string.Format("          <span id='spanMenu{0}' class='arrow'></span>", i.ToString()));
                    streamWriter.WriteLine("        </a>");
                    streamWriter.WriteLine(string.Format("        <ul id='ulMenu{0}' class='sub-menu'>", i.ToString()));

                    for (j = 1; j <= 5; j++)
                    {
                        streamWriter.WriteLine(string.Format("          <li id='liMenu{0}Sub{1}'>", i.ToString(), j.ToString()));
                        streamWriter.WriteLine("            <a href='javascript:;'>");
                        streamWriter.WriteLine(string.Format("              <span class='title' data-location='Menu{0}Sub{1}'>Sub{1}</span>", i.ToString(), j.ToString()));
                        streamWriter.WriteLine(string.Format("              <span id='spanMenu{0}Sub{1}' class='arrow'></span>", i.ToString(), j.ToString()));
                        streamWriter.WriteLine("            </a>");
                        streamWriter.WriteLine(string.Format("            <ul id='ulMenu{0}Sub{1}' class='sub-menu'>", i.ToString(), j.ToString()));

                        for (k = 1; k <= 5; k++)
                        {
                            streamWriter.WriteLine(string.Format("              <li id=liMenu{0}Sub{1}Sub{2}>", i.ToString(), j.ToString(), k.ToString()));
                            streamWriter.WriteLine(string.Format("                <a href='javascript: window.jonas.newLocation({0}, {1}, {2}, {3}, 1, false)'>", h.ToString(), i.ToString(), j.ToString(), k.ToString()));
                            streamWriter.WriteLine(string.Format("                  <span class='title' data-location='Menu{0}Sub{1}Sub{2}'>Sub{2}</span>", i.ToString(), j.ToString(), k.ToString()));
                            streamWriter.WriteLine("                </a>");
                            streamWriter.WriteLine("              </li>");
                        }

                        streamWriter.WriteLine("            </ul>");
                        streamWriter.WriteLine("          </li>");
                    }

                    streamWriter.WriteLine("        </ul>");
                    streamWriter.WriteLine("      </li>");
                }

                streamWriter.WriteLine("    </ul>");
                streamWriter.WriteLine("  </div>");
                streamWriter.WriteLine("  <div class='page-content'>");
                streamWriter.WriteLine("    <div class='container-fluid'>");
                streamWriter.WriteLine("      <div class='clearfix'>");
                streamWriter.WriteLine("        @RenderBody()");
                streamWriter.WriteLine("      </div>");
                streamWriter.WriteLine("    </div>");
                streamWriter.WriteLine("  </div>");
                streamWriter.WriteLine("</div>");
                streamWriter.Flush();
                fileStream.Flush();
                streamWriter.Close();
                fileStream.Close();
            }
        }
    }
}
