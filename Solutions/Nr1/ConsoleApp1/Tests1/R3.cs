using hiJump.Infrastructure.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utility.Utility_classes;

namespace Leander.Nr1
{
    public static class R3
    {
        public static void Execute()
        {
            Truck truck = new Truck(1, 1200, 300);
            Object obj1 = ReflectionHelpers.GetPropertyValue(truck, "Id");
            Console.WriteLine(string.Format("Vehicle Id = {0} and Truck Id = {1}, reflection value = {2}", truck.GetVehicleId(), truck.GetTruckId(), obj1.ToString()));
        }
    }
}
