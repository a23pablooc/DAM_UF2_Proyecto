namespace UnitScripts.PlanetScripts
{
    public enum SlotType
    {
        CreditFarm,
        MetalFarm,
        EnergyFarm,
        PopulationFarm
        
    }

    public static class SlotTypeExtension
    {
        public static SlotType GetSlotType(int slotType)
        {
            return slotType switch
            {
                0 => SlotType.CreditFarm,
                1 => SlotType.MetalFarm,
                2 => SlotType.EnergyFarm,
                3 => SlotType.PopulationFarm,
                _ => throw new System.ArgumentOutOfRangeException()
            };
        }
        
        public static ResourceType GetResourceType(SlotType slotType)
        {
            return slotType switch
            {
                SlotType.CreditFarm => ResourceType.Credits,
                SlotType.MetalFarm => ResourceType.Metal,
                SlotType.EnergyFarm => ResourceType.Energy,
                SlotType.PopulationFarm => ResourceType.Population,
                _ => throw new System.ArgumentOutOfRangeException()
            };
        }
    }
}