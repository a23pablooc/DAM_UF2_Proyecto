using System.Collections.Generic;

namespace UnitScripts.PlanetScripts
{
    public static class SlotCosts
    {
        private static readonly Dictionary<SlotType, Dictionary<ResourceType, int>> Costs = new()
        {
            {
                SlotType.CreditFarm, new Dictionary<ResourceType, int>
                {
                    { ResourceType.Credits, 100 },
                    { ResourceType.Metal, 50 },
                    { ResourceType.Energy, 50 }
                }
            },
            {
                SlotType.MetalFarm, new Dictionary<ResourceType, int>
                {
                    { ResourceType.Credits, 50 },
                    { ResourceType.Metal, 100 },
                    { ResourceType.Energy, 50 }
                }
            },
            {
                SlotType.EnergyFarm, new Dictionary<ResourceType, int>
                {
                    { ResourceType.Credits, 50 },
                    { ResourceType.Metal, 50 },
                    { ResourceType.Energy, 100 }
                }
            },
            {
                SlotType.PopulationFarm, new Dictionary<ResourceType, int>
                {
                    { ResourceType.Credits, 50 },
                    { ResourceType.Metal, 50 },
                    { ResourceType.Energy, 50 }
                }
            }
        };

        public static Dictionary<ResourceType, int> GetCosts(SlotType resourceType)
        {
            return Costs[resourceType];
        }
    }
}