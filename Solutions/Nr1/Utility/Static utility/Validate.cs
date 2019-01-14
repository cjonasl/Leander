using System;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Leander.Nr1
{
    public partial class Utility
    {
        public static bool IconExists(string icon)
        {
            bool returnValue;
            string errorMessage;

            ArrayList v = ReturnRowsInFileInArrayList("C:\\git_cjonasl\\Leander\\Design Leander\\Font Awesome free icons__Without_fasfa.txt", out errorMessage);

            if (v.IndexOf(icon) >= 0) 
                returnValue = true;
            else  
                returnValue = false;

            return returnValue;
        }
    }
}
