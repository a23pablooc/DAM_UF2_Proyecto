using System.Collections.Generic;
using UnitScripts;
using UnitScripts.PlanetScripts;
using UnityEngine;
using Random = UnityEngine.Random;

namespace SolarSystem
{
    /// <summary>
    /// Genera el sistema solar
    /// </summary>
    public class SolarSystemGanerator : MonoBehaviour
    {
        [SerializeField] private GameObject starPrefab;
        [SerializeField] private GameObject planetPrefab;
        private const int NumPlanets = 15;
        private const float MinDistanceFromStar = 200f;
        private const float MaxDistanceFromStar = 600f;
        private const float MinDistanceBetweenPlanets = 200f;
        private const float MinStarScale = 80f;
        private const float MaxStarScale = 100f;

        private GameObject _solarSystem;
        private GameObject _star;

        public GameObject PlayerStarterPlanet { get; private set; }
        public GameObject IaStarterPlanet { get; private set; }

        private readonly List<PlanetUnit> _planets = new();

        public void StartGeneration()
        {
            SolarSystemGameObject();
            GenerateStar();
            GeneratePlanets();
        }

        private void SolarSystemGameObject()
        {
            _solarSystem = new GameObject("Solar System")
            {
                transform =
                {
                    position = Vector3.zero
                }
            };
        }

        private void GenerateStar()
        {
            _star = Instantiate(starPrefab, Vector3.zero, Quaternion.identity);
            _star.transform.SetParent(_solarSystem.transform);
            var starScale = Random.Range(MinStarScale, MaxStarScale);
            _star.transform.localScale = Vector3.one * starScale;
        }

        private void GeneratePlanets()
        {
            InitialPlanets();

            for (var i = 0; i < NumPlanets - 2; i++)
            {
                var planet = Instantiate(planetPrefab, Vector3.zero, Quaternion.identity);
                planet.transform.SetParent(_solarSystem.transform);

                do
                {
                    planet.transform.position = Vector3.right * Random.Range(MinDistanceFromStar, MaxDistanceFromStar);
                    planet.transform.RotateAround(_star.transform.position, Vector3.up, Random.Range(0, 360));
                } while (_planets.Exists(p =>
                             Vector3.Distance(p.transform.position, planet.transform.position) <
                             MinDistanceBetweenPlanets));

                _planets.Add(planet.GetComponent<PlanetUnit>());
            }
        }

        private void InitialPlanets()
        {
            PlayerStarterPlanet = Instantiate(planetPrefab, Vector3.left * 500f, Quaternion.identity);
            PlayerStarterPlanet.name += " Player Starter Planet";
            PlayerStarterPlanet.transform.SetParent(_solarSystem.transform);
            var planetUnit = PlayerStarterPlanet.GetComponent<PlanetUnit>();
            planetUnit.Init(PlayerType.Player);
            _planets.Add(planetUnit);

            IaStarterPlanet = Instantiate(planetPrefab, Vector3.right * 500f, Quaternion.identity);
            IaStarterPlanet.name += " IA Starter Planet";
            IaStarterPlanet.transform.SetParent(_solarSystem.transform);
            planetUnit = IaStarterPlanet.GetComponent<PlanetUnit>();
            planetUnit.Init(PlayerType.IA);
            _planets.Add(planetUnit);
        }
    }
}