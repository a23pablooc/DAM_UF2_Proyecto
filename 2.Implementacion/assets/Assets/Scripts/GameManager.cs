using System;
using System.Collections.Generic;
using UnitScripts;
using UnitScripts.PlanetScripts;
using UnitScripts.ShipScripts;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Clase GameManager. Controla el flujo del juego.
/// Contiene la lógica del juego y los datos de los jugadores.
/// </summary>
public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject fastShipPrefab;
    [SerializeField] private GameObject normalShipPrefab;
    [SerializeField] private GameObject bomberShipPrefab;
    
    [SerializeField] private ApplicationManager applicationManager;

    private readonly object _lock1 = new();
    private readonly object _lock2 = new();

    public PlayerType Winner { get; private set; } = PlayerType.Neutral;

    public List<PlanetUnit> NeutralPlanets { get; } = new();

    public List<ShipUnit> PlayerShips { get; } = new();

    public Dictionary<ShipType, int> PlayerCreatedShips { get; } = new()
    {
        { ShipType.FastShip, 0 },
        { ShipType.NormalShip, 0 },
        { ShipType.BomberShip, 0 }
    };

    public Dictionary<ShipType, int> PlayerDestroyedShips { get; } = new()
    {
        { ShipType.FastShip, 0 },
        { ShipType.NormalShip, 0 },
        { ShipType.BomberShip, 0 }
    };

    public List<PlanetUnit> PlayerPlanets { get; } = new();

    public Dictionary<ResourceType, Resource> PlayerResources { get; } = new()
    {
        { ResourceType.Credits, new Resource(ResourceType.Credits, 490, 99_999) },
        { ResourceType.Metal, new Resource(ResourceType.Metal, 800, 99_999) },
        { ResourceType.Energy, new Resource(ResourceType.Energy, 800, 99_999) },
        { ResourceType.Population, new Resource(ResourceType.Population, 0, 0) }
    };

    public Dictionary<ResourceType, int> PlayerTotalResources { get; } = new()
    {
        { ResourceType.Credits, 0 },
        { ResourceType.Metal, 0 },
        { ResourceType.Energy, 0 },
        { ResourceType.Population, 0 }
    };

    public Dictionary<ResourceType, int> PlayerUsedResources { get; } = new()
    {
        { ResourceType.Credits, 0 },
        { ResourceType.Metal, 0 },
        { ResourceType.Energy, 0 },
        { ResourceType.Population, 0 }
    };

    public List<ShipUnit> IaShips { get; } = new();

    public Dictionary<ShipType, int> IaCreatedShips { get; } = new()
    {
        { ShipType.FastShip, 0 },
        { ShipType.NormalShip, 0 },
        { ShipType.BomberShip, 0 }
    };

    public Dictionary<ShipType, int> IaDestroyedShips { get; } = new()
    {
        { ShipType.FastShip, 0 },
        { ShipType.NormalShip, 0 },
        { ShipType.BomberShip, 0 }
    };

    public List<PlanetUnit> IaPlanets { get; } = new();

    public Dictionary<ResourceType, Resource> IaResources { get; } = new()
    {
        { ResourceType.Credits, new Resource(ResourceType.Credits, 490, 99_999) },
        { ResourceType.Metal, new Resource(ResourceType.Metal, 800, 99_999) },
        { ResourceType.Energy, new Resource(ResourceType.Energy, 800, 99_999) },
        { ResourceType.Population, new Resource(ResourceType.Population, 0, 0) }
    };

    public Dictionary<ResourceType, int> IaTotalResources { get; } = new()
    {
        { ResourceType.Credits, 0 },
        { ResourceType.Metal, 0 },
        { ResourceType.Energy, 0 },
        { ResourceType.Population, 0 }
    };

    public Dictionary<ResourceType, int> IaUsedResources { get; } = new()
    {
        { ResourceType.Credits, 0 },
        { ResourceType.Metal, 0 },
        { ResourceType.Energy, 0 },
        { ResourceType.Population, 0 }
    };

    public event Action<PlanetUnit> OnPlanetDestroyed;
    public event Action<ShipUnit> OnShipDestroyed;

    public static GameManager Instance { get; private set; }

    private void Awake()
    {
        // Si se carga la escena 0 (pantalla de inicio) o ya existe una instancia de GameManager, se destruye.
        // Se destruye en la escena 0 para que al volver a cargar la escena de juego estean todos
        // los datos en su estado inicial
        if (SceneManager.GetActiveScene().buildIndex == 0 || Instance && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    public void RegisterPlanet(PlanetUnit planet)
    {
        switch (planet.Owner)
        {
            case PlayerType.Player:
                PlayerPlanets.Add(planet);
                PlayerResources[ResourceType.Population].IncreaseMaxAmount(planet.PopulationCapacity);
                PlayerResources[ResourceType.Credits].Add(planet.CreditReward);
                break;
            case PlayerType.IA:
                IaPlanets.Add(planet);
                IaResources[ResourceType.Population].IncreaseMaxAmount(planet.PopulationCapacity);
                IaResources[ResourceType.Credits].Add(planet.CreditReward);
                break;
            case PlayerType.Neutral:
                NeutralPlanets.Add(planet);
                break;
            default:
                throw new Exception(
                    $"Invalid planet owner: {planet.Owner}, should be {PlayerType.Player}, {PlayerType.IA} or {PlayerType.Neutral}");
        }
    }

    public void RegisterShip(ShipUnit ship)
    {
        switch (ship.Owner)
        {
            case PlayerType.Player:
                PlayerShips.Add(ship);
                PlayerCreatedShips[ship.ShipType]++;
                PlayerResources[ResourceType.Population]
                    .Add(ShipCosts.GetCosts(ship.ShipType)[ResourceType.Population]);
                break;
            case PlayerType.IA:
                IaShips.Add(ship);
                IaCreatedShips[ship.ShipType]++;
                IaResources[ResourceType.Population].Add(ShipCosts.GetCosts(ship.ShipType)[ResourceType.Population]);
                break;
            case PlayerType.Neutral:
            default:
                throw new Exception(
                    $"Invalid planet owner: {ship.Owner}, should be {PlayerType.Player} or {PlayerType.IA}");
        }
    }

    public void ShipDestroyed(GameObject ship)
    {
        if (!ship.TryGetComponent<ShipUnit>(out var shipUnit))
            throw new Exception($"Invalid object: {ship.name} is not a ship or has no ShipUnit component");

        var list = shipUnit.Owner switch
        {
            PlayerType.Player => PlayerShips,
            PlayerType.IA => IaShips,
            PlayerType.Neutral => throw new Exception(
                $"Invalid ship owner: {shipUnit.Owner}, should be {PlayerType.Player} or {PlayerType.IA}"),
            _ => throw new Exception(
                $"Invalid ship owner: {shipUnit.Owner}, should be {PlayerType.Player} or {PlayerType.IA}")
        };

        var resources = shipUnit.Owner switch
        {
            PlayerType.Player => PlayerResources,
            PlayerType.IA => IaResources,
            PlayerType.Neutral => throw new Exception(
                $"Invalid ship owner: {shipUnit.Owner}, should be {PlayerType.Player} or {PlayerType.IA}"),
            _ => throw new Exception(
                $"Invalid ship owner: {shipUnit.Owner}, should be {PlayerType.Player} or {PlayerType.IA}")
        };

        resources[ResourceType.Population].Subtract(ShipCosts.GetCosts(shipUnit.ShipType)[ResourceType.Population]);

        OnShipDestroyed?.Invoke(shipUnit);
        list.Remove(shipUnit);
    }

    public void PlanetDestroyed(GameObject planet, PlayerType source)
    {
        if (!planet.TryGetComponent<PlanetUnit>(out var planetUnit))
            throw new Exception($"Invalid object: {planet.name} is not a planet or has no PlanetUnit component");

        ChangePlanetOwner(planetUnit, planetUnit.Owner, source);
        OnPlanetDestroyed?.Invoke(planetUnit);

        if (PlayerPlanets.Count == 0)
        {
            Winner = PlayerType.IA;
        }
        else if (IaPlanets.Count == 0)
        {
            Winner = PlayerType.Player;
        }
        
        if (Winner != PlayerType.Neutral)
        {
            applicationManager.EndGame();
        }
    }

    private void ChangePlanetOwner(PlanetUnit planet, PlayerType oldOwner, PlayerType newOwner)
    {
        if (oldOwner == newOwner)
            throw new Exception($"Invalid planet owner change: {oldOwner} -> {newOwner}, should be different");

        switch (oldOwner)
        {
            case PlayerType.Player:
                PlayerPlanets.Remove(planet);
                PlayerResources[ResourceType.Population].DecreaseMaxAmount(planet.PopulationCapacity);
                break;
            case PlayerType.IA:
                IaPlanets.Remove(planet);
                IaResources[ResourceType.Population].DecreaseMaxAmount(planet.PopulationCapacity);
                break;
            case PlayerType.Neutral:
                NeutralPlanets.Remove(planet);
                break;
            default:
                throw new Exception(
                    $"Invalid planet owner: Should be {PlayerType.Player}, {PlayerType.IA} or {PlayerType.Neutral}");
        }

        if (newOwner is not (PlayerType.Player or PlayerType.IA))
            throw new Exception($"Invalid planet owner: Should be {PlayerType.Player} or {PlayerType.IA}");

        planet.SetOwner(newOwner);
        
        
    }

    public void BuySlot(PlanetUnit planet, SlotType slotType)
    {
        var resources = planet.Owner switch
        {
            PlayerType.Player => PlayerResources,
            PlayerType.IA => IaResources,
            PlayerType.Neutral =>
                throw new Exception($"Invalid owner: {planet.Owner}, should be {PlayerType.Player} or {PlayerType.IA}"),
            _ => throw new Exception($"Invalid owner: {planet.Owner}, should be {PlayerType.Player} or {PlayerType.IA}")
        };

        var costs = SlotCosts.GetCosts(slotType);
        switch (planet.Owner)
        {
            case PlayerType.Player:
                lock (_lock1)
                {
                    foreach (var (resourceType, value) in costs)
                    {
                        if (resources[resourceType].Amount < value)
                            throw new Exception(
                                $"{planet.Owner} has not enough {resourceType} to buy a {slotType} slot");
                    }

                    break;
                }
            case PlayerType.IA:
                lock (_lock2)
                {
                    foreach (var (resourceType, value) in costs)
                    {
                        if (resources[resourceType].Amount < value)
                            throw new Exception(
                                $"{planet.Owner} has not enough {resourceType} to buy a {slotType} slot");
                    }

                    break;
                }
            case PlayerType.Neutral:
            default:
                throw new Exception($"Invalid owner: {planet.Owner}, should be {PlayerType.Player} or {PlayerType.IA}");
        }

        planet.AddSlot(SlotFarm.Create(slotType));

        var usedResources = planet.Owner switch
        {
            PlayerType.Player => PlayerUsedResources,
            PlayerType.IA => IaUsedResources,
            PlayerType.Neutral =>
                throw new Exception($"Invalid owner: {planet.Owner}, should be {PlayerType.Player} or {PlayerType.IA}"),
            _ => throw new Exception($"Invalid owner: {planet.Owner}, should be {PlayerType.Player} or {PlayerType.IA}")
        };

        foreach (var (resourceType, value) in costs)
        {
            resources[resourceType].Subtract(value);
            usedResources[resourceType] += value;
        }
    }

    public void CollectResource(PlayerType owner, Slot slot)
    {
        var resources = owner switch
        {
            PlayerType.Player => PlayerResources,
            PlayerType.IA => IaResources,
            PlayerType.Neutral =>
                throw new Exception($"Invalid owner: {owner}, should be {PlayerType.Player} or {PlayerType.IA}"),
            _ => throw new Exception($"Invalid owner: {owner}, should be {PlayerType.Player} or {PlayerType.IA}")
        };

        var totalResources = owner switch
        {
            PlayerType.Player => PlayerTotalResources,
            PlayerType.IA => IaTotalResources,
            PlayerType.Neutral =>
                throw new Exception($"Invalid owner: {owner}, should be {PlayerType.Player} or {PlayerType.IA}"),
            _ => throw new Exception($"Invalid owner: {owner}, should be {PlayerType.Player} or {PlayerType.IA}")
        };

        var resourceType = SlotTypeExtension.GetResourceType(slot.SlotFarm.SlotType);
        resources[resourceType].Add(slot.SlotFarm.ProductionAmount);
        totalResources[resourceType] += slot.SlotFarm.ProductionAmount;
    }

    public void SpawnShip(ShipType shipType, Vector3 position, PlayerType owner)
    {
        var costs = ShipCosts.GetCosts(shipType);
        switch (owner)
        {
            // No lo hago con un solo lock porque si un jugador intenta spawnear una nave y la IA intenta spawnear
            // otra nave al mismo tiempo, el segundo jugador tendría que esperar a que el primero termine de verificar
            // los recursos, lo cual no es necesario.
            case PlayerType.Player:
                lock (_lock1)
                {
                    foreach (var (key, value) in costs)
                    {
                        if (key != ResourceType.Population && PlayerResources[key].Amount < value)
                            throw new Exception($"{owner} has not enough {key} to spawn {shipType}");
                    }

                    var populationResource = PlayerResources[ResourceType.Population];

                    if (populationResource.MaxAmount > populationResource.Amount &&
                        populationResource.MaxAmount - populationResource.Amount < costs[ResourceType.Population])
                        throw new Exception(
                            $"{owner} has not enough population to spawn {shipType} -> MaxAmount: {PlayerResources[ResourceType.Population].MaxAmount}, Amount: {PlayerResources[ResourceType.Population].Amount}, Cost: {costs[ResourceType.Population]}");

                    var prefab = shipType switch
                    {
                        ShipType.FastShip => fastShipPrefab,
                        ShipType.NormalShip => normalShipPrefab,
                        ShipType.BomberShip => bomberShipPrefab,
                        _ => throw new Exception($"Invalid ship type: {shipType}")
                    };

                    SpawnShip(prefab, shipType, position, owner);

                    break;
                }
            case PlayerType.IA:
                lock (_lock2)
                {
                    foreach (var (key, value) in costs)
                    {
                        if (key != ResourceType.Population && IaResources[key].Amount < value)
                            throw new Exception($"{owner} has not enough {key} to spawn {shipType}");
                    }

                    var populationResource = IaResources[ResourceType.Population];

                    if (populationResource.MaxAmount > populationResource.Amount &&
                        populationResource.MaxAmount - populationResource.Amount < costs[ResourceType.Population])
                        throw new Exception($"{owner} has not enough population to spawn {shipType}");

                    var prefab = shipType switch
                    {
                        ShipType.FastShip => fastShipPrefab,
                        ShipType.NormalShip => normalShipPrefab,
                        ShipType.BomberShip => bomberShipPrefab,
                        _ => throw new Exception($"Invalid ship type: {shipType}")
                    };

                    SpawnShip(prefab, shipType, position, owner);
                    break;
                }
            case PlayerType.Neutral:
            default:
                throw new Exception($"Invalid owner: {owner}, should be {PlayerType.Player} or {PlayerType.IA}");
        }
    }

    private void SpawnShip(GameObject prefab, ShipType shipType, Vector3 position, PlayerType owner)
    {
        var costs = ShipCosts.GetCosts(shipType);
        var ship = Instantiate(prefab, position, Quaternion.identity);
        ship.TryGetComponent<ShipUnit>(out var shipUnit);
        shipUnit.Init(owner);

        switch (owner)
        {
            case PlayerType.Player:
                PlayerResources[ResourceType.Credits].Subtract(costs[ResourceType.Credits]);
                PlayerResources[ResourceType.Metal].Subtract(costs[ResourceType.Metal]);
                PlayerResources[ResourceType.Energy].Subtract(costs[ResourceType.Energy]);
                PlayerUsedResources[ResourceType.Credits] += costs[ResourceType.Credits];
                PlayerUsedResources[ResourceType.Metal] += costs[ResourceType.Metal];
                PlayerUsedResources[ResourceType.Energy] += costs[ResourceType.Energy];
                PlayerUsedResources[ResourceType.Population] += costs[ResourceType.Population];
                break;
            case PlayerType.IA:
                IaResources[ResourceType.Credits].Subtract(costs[ResourceType.Credits]);
                IaResources[ResourceType.Metal].Subtract(costs[ResourceType.Metal]);
                IaResources[ResourceType.Energy].Subtract(costs[ResourceType.Energy]);
                IaUsedResources[ResourceType.Credits] += costs[ResourceType.Credits];
                IaUsedResources[ResourceType.Metal] += costs[ResourceType.Metal];
                IaUsedResources[ResourceType.Energy] += costs[ResourceType.Energy];
                IaUsedResources[ResourceType.Population] += costs[ResourceType.Population];
                break;
            case PlayerType.Neutral:
            default:
                throw new Exception($"Invalid owner: {owner}, should be {PlayerType.Player} or {PlayerType.IA}");
        }
    }
}