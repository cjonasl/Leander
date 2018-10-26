using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utility.Utility_classes
{
    public class Vehicle
    {
        public int Id { get; set; }
        public int Weight { get; set; }

        public Vehicle(int id, int weight)
        {
            this.Id = id;
            this.Weight = weight;
        }

        public int GetVehicleId()
        {
            return Id;
        }
    }
}
