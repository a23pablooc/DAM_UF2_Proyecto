using UnitScripts;
using UnitScripts.ShipScripts;
using Unity.AI.Navigation;
using UnityEngine;

namespace SolarSystem
{
    /// <summary>
    /// Genera el sistema solar, recalcula el navmesh y spawnea las naves iniciales
    /// </summary>
    public class SceneLoader : MonoBehaviour
    {
        [SerializeField] private NavMeshSurface surface;

        private SolarSystemGanerator _solarSystemGenerator;

        private void Awake()
        {
            _solarSystemGenerator = GetComponent<SolarSystemGanerator>();
        }

        private void Start()
        {
            GenerateSolarSystem();
            RecalculateNavMesh();
            SpawnStarterShips();

            //Se destruye porque ya no se necesita
            Destroy(gameObject);
        }

        private void GenerateSolarSystem()
        {
            _solarSystemGenerator.StartGeneration();
        }

        private void RecalculateNavMesh()
        {
            surface.BuildNavMesh();
        }

        private void SpawnStarterShips()
        {
            var playerPlanetPosition = _solarSystemGenerator.PlayerStarterPlanet.transform.position;
            GameManager.Instance.SpawnShip(ShipType.NormalShip, playerPlanetPosition + Vector3.right * 50,
                PlayerType.Player);
            GameManager.Instance.SpawnShip(ShipType.NormalShip, playerPlanetPosition + Vector3.right * 60,
                PlayerType.Player);
            GameManager.Instance.SpawnShip(ShipType.NormalShip,
                playerPlanetPosition + Vector3.right * 55 - Vector3.forward * 5, PlayerType.Player);

            var iaPlanetPosition = _solarSystemGenerator.IaStarterPlanet.transform.position;
            GameManager.Instance.SpawnShip(ShipType.NormalShip, iaPlanetPosition + Vector3.right * 50, PlayerType.IA);
            GameManager.Instance.SpawnShip(ShipType.NormalShip, iaPlanetPosition + Vector3.right * 60, PlayerType.IA);
            GameManager.Instance.SpawnShip(ShipType.NormalShip, iaPlanetPosition + Vector3.right * 50 - Vector3.forward * 5,
                PlayerType.IA);
        }
    }
}