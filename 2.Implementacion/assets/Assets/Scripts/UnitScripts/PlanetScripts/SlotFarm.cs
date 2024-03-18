using System;

namespace UnitScripts.PlanetScripts
{
    /// <summary>
    /// Clase que contiene la información de la producción de un slot
    /// </summary>
    public class SlotFarm
    {
        private const float CreditProductionRate = 15;
        private const float MetalProductionRate = 15;
        private const float EnergyProductionRate = 15;

        private const int CreditProductionAmount = 10;
        private const int MetalProductionAmount = 10;
        private const int EnergyProductionAmount = 10;

        public SlotType SlotType { get; }
        public float ProductionRate { get; }
        public int ProductionAmount { get; }

        private SlotFarm(SlotType type, float productionRate, int productionAmount)
        {
            SlotType = type;
            ProductionRate = productionRate;
            ProductionAmount = productionAmount;
        }

        public static SlotFarm Create(SlotType type)
        {
            return type switch
            {
                SlotType.CreditFarm => new SlotFarm(type, CreditProductionRate, CreditProductionAmount),
                SlotType.MetalFarm => new SlotFarm(type, MetalProductionRate, MetalProductionAmount),
                SlotType.EnergyFarm => new SlotFarm(type, EnergyProductionRate, EnergyProductionAmount),
                SlotType.PopulationFarm => new SlotFarm(type, -1f, -1),
                _ => throw new ArgumentOutOfRangeException()
            };
        }
    }
}