namespace UnitScripts.ShipScripts
{
    public enum ShipType
    {
        FastShip,
        NormalShip,
        BomberShip
    }
    
    public static class ShipTypeExtension
    {
        public static ShipType GetShipType(int slotType)
        {
            return slotType switch
            {
                0 => ShipType.FastShip,
                1 => ShipType.NormalShip,
                2 => ShipType.BomberShip,
                _ => throw new System.ArgumentOutOfRangeException()
            };
        }
    }
    
}