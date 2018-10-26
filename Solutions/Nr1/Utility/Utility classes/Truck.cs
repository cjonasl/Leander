using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utility.Utility_classes
{
    public class Truck : Vehicle
    {
        public int MaxLoad { get; set; }
        public new int Id { get; set; }

        public Truck(int id, int weight, int maxLoad)
            : base(id, weight)
        {
            this.MaxLoad = maxLoad;
            this.Id = id + 1;
        }

        public int GetTruckId()
        {
            return this.Id;
        }
    }
}
