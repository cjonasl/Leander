using System;
using System.Collections.Generic;


//https://code-maze.com/factory-method/
namespace Test2
{
    public enum Actions
    {
        Cooling,
        Warming
    }

    public interface IAirConditioner
    {
        void Operate();
    }

    public class Cooling : IAirConditioner
    {
        private readonly double _temperature;

        public Cooling(double temperature)
        {
            _temperature = temperature;
        }

        public void Operate()
        {
            Console.WriteLine($"Cooling the room to the required temperature of {_temperature} degrees");
        }
    }

    public class Warming : IAirConditioner
    {
        private readonly double _temperature;

        public Warming(double temperature)
        {
            _temperature = temperature;
        }

        public void Operate()
        {
            Console.WriteLine($"Warming the room to the required temperature of {_temperature} degrees.");
        }
    }

    public abstract class AirConditionerFactory
    {
        public abstract IAirConditioner Create(double temperature);
    }

    public class CoolingFactory : AirConditionerFactory
    {
        public override IAirConditioner Create(double temperature) => new Cooling(temperature);
    }

    public class WarmingFactory : AirConditionerFactory
    {
        public override IAirConditioner Create(double temperature) => new Warming(temperature);
    }

    public class AirConditioner
    {
        private readonly Dictionary<Actions, AirConditionerFactory> _factories;

        public AirConditioner()
        {
            _factories = new Dictionary<Actions, AirConditionerFactory>
        {
            { Actions.Cooling, new CoolingFactory() },
            { Actions.Warming, new WarmingFactory() }
        };
        }

        public IAirConditioner ExecuteCreation(Actions action, double temperature) => _factories[action].Create(temperature);
    }

    public class Program
    {
        public static void Run()
        {
            var factory = new AirConditioner().ExecuteCreation(Actions.Warming, 22.5);
            factory.Operate();
        }
    }
}
