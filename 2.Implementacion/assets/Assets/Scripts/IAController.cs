using System.Linq;
using UnitScripts;
using UnitScripts.PlanetScripts;
using UnitScripts.ShipScripts;
using UnityEngine;

/// <summary>
/// Controla el comportamiento de la IA
/// Solo puede controlar las naves iniciales para atacar algun planeta aleatorio (que no sea de la IA) y defender sus planetas
/// </summary>
public class IaController : MonoBehaviour
{
    private float _lastInteraction;
    private const float InteractionInterval = 180f;

    private void Update()
    {
        if (Time.time - _lastInteraction <= InteractionInterval) return;

        _lastInteraction = Time.time;
        var planets = GameManager.Instance.PlayerPlanets;

        var targetPlanet = planets.First(unit => unit.Owner != PlayerType.IA);

        if (!targetPlanet) return;
        
        var iaShips = GameManager.Instance.IaShips;
        foreach (var ship in iaShips)
        {
            ship.gameObject.TryGetComponent<ShipOrderController>(out var shipOrderController);
            shipOrderController.SetDestination(targetPlanet.transform.position);
        }
    }

    public static void PlanetHit(PlanetUnit planetUnit)
    {
        var iaShips = GameManager.Instance.IaShips;

        foreach (var ship in iaShips)
        {
            ship.gameObject.GetComponent<ShipOrderController>().SetDestination(planetUnit.transform.position);
        }
    }

    public static void ShipHit(ShipUnit shipUnit)
    {
        var ships = GameManager.Instance.IaShips;
        var closestShips = new ShipUnit[Mathf.Min(ships.Count - 1, 3)];
        var closestDistances = new float[closestShips.Length];
        for (var i = 0; i < closestDistances.Length; i++)
        {
            closestDistances[i] = float.MaxValue;
        }

        foreach (var ship in ships)
        {
            if (ship.gameObject == shipUnit.gameObject) continue;

            var distance = Vector3.Distance(shipUnit.transform.position, ship.transform.position);
            for (var i = 0; i < 3; i++)
            {
                if (distance >= closestDistances[i]) continue;
                for (var j = 2; j > i; j--)
                {
                    closestDistances[j] = closestDistances[j - 1];
                    closestShips[j] = closestShips[j - 1];
                }

                closestDistances[i] = distance;
                closestShips[i] = ship;
                break;
            }
        }

        foreach (var ship in closestShips)
        {
            ship.gameObject.GetComponent<ShipOrderController>().SetDestination(shipUnit.transform.position);
        }
    }
}