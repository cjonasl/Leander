using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication1.Models
{
    public class Resource
    {
        public int? Id { get; set; }
        public string Created { get; set; } //In format: yyyy-MM-dd HH:mm:ss
        public string Title { get; set; }
        public string Note { get; set; }
    }

    public static class ResourceUtility
    {
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
    }
}