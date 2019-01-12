using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Leander.Nr1
{
    public class WorkItem
    {
        public int Id { get; set; }
        public List<WorkItem> Dependencies { get; set; }
    }

    public class AAA
    {
        public List<WorkItem> WorkItems { get; set; }
    }


    public static class R7
    {
        public static void Execute()
        {
            WorkItem w1 = new WorkItem();
            w1.Id = 1;
            w1.Dependencies = new List<WorkItem>();

            WorkItem w2 = new WorkItem();
            w2.Id = 2;
            w2.Dependencies = new List<WorkItem>();

            WorkItem w3 = new WorkItem();
            w3.Id = 3;
            w3.Dependencies = new List<WorkItem>();

            w3.Dependencies.Add(w1);

            AAA aaa = new AAA();
            aaa.WorkItems = new List<WorkItem>();

            aaa.WorkItems.Add(w1);
            aaa.WorkItems.Add(w2);
            aaa.WorkItems.Add(w3);

            WorkItem workItemId = w1;

            //Mark's solution:
            aaa.WorkItems.Where(x => x.Dependencies.Any(y => y.Id == workItemId.Id)).ToList().
            ForEach(x => x.Dependencies.RemoveAll(y => y.Id == workItemId.Id));

            /* My original solution
            foreach (var wi in aaa.WorkItems)
            {
                if (wi.Id != workItemId.Id)
                {
                    for (int i = 0; i < wi.Dependencies.Count; i++)
                    {
                        if (wi.Dependencies[i].Id == workItemId.Id)
                        {
                            wi.Dependencies.RemoveAt(i);
                            break;
                        }
                    }
                }
            }*/
        }
    }
}
