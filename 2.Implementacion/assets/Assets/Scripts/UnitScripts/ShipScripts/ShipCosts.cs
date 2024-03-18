using System.Collections.Generic;

namespace UnitScripts.ShipScripts
{
    public static class ShipCosts
    {
        private static readonly Dictionary<ShipType, Dictionary<ResourceType, int>> Costs = new()
        {
            {
                ShipType.FastShip, new Dictionary<ResourceType, int>
                {
                    { ResourceType.Credits, 30 },
                    { ResourceType.Metal, 50 },
                    { ResourceType.Energy, 50 },
                    { ResourceType.Population, 3 }
                }
            },
            {
                ShipType.NormalShip, new Dictionary<ResourceType, int>
                {
                    { ResourceType.Credits, 30 },
                    { ResourceType.Metal, 100 },
                    { ResourceType.Energy, 100 },
                    { ResourceType.Population, 5 }
                }
            },
            {
                ShipType.BomberShip, new Dictionary<ResourceType, int>
                {
                    { ResourceType.Credits, 30 },
                    { ResourceType.Metal, 150 },
                    { ResourceType.Energy, 150 },
                    { ResourceType.Population, 10 }
                }
            }
        };

        public static Dictionary<ResourceType, int> GetCosts(ShipType shipType)
        {
            return Costs[shipType];
        }
    }
}