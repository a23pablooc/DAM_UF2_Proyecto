using UnitScripts.PlanetScripts;
using UnitScripts.ShipScripts;
using UnityEngine;

public class IAController : MonoBehaviour
{
    private float _lastInteraction;
    private float _interactionInterval = 180f;

    private void Update()
    {
        if (Time.time - _lastInteraction <= _interactionInterval) return;

        _lastInteraction = Time.time;
        var planets = GameManager.Instance.PlayerPlanets;

        foreach (var planet in planets)
        {
            if (planet.Owner != PlayerType.IA) continue;

            var iaShips = GameManager.Instance.IaShips;

            foreach (var ship in iaShips)
            {
                ship.gameObject.GetComponent<ShipOrderController>().SetDestination(planet.transform.position);
            }
        }
    }

    public void PlanetHit(PlanetUnit planetUnit)
    {
        var iaShips = GameManager.Instance.IaShips;

        foreach (var ship in iaShips)
        {
            ship.gameObject.GetComponent<ShipOrderController>().SetDestination(planetUnit.transform.position);
        }
    }

    public void ShipHit(ShipUnit shipUnit)
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
            if (ship == shipUnit.gameObject) continue;

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
