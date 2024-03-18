using UnityEngine;

namespace UnitScripts.PlanetScripts.PlanetGeneration
{
    public readonly struct ColorSetting
    {
        public const string BaseColor = "BaseColor";
        public const string LandColor = "LandColor";
        public const string HillColor = "HillColor";
        public const string MountainColor = "MountainColor";

        public readonly Color Color;

        public ColorSetting(Color color)
        {
            Color = color;
        }
    }
}