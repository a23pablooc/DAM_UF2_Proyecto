using UnityEngine;

namespace UnitScripts.PlanetScripts
{
    /// <summary>
    /// Controllador de la salud de un planeta
    /// </summary>
    public class PlanetHealthController : HealthController
    {
        protected override void DestroyAction(PlayerType source)
        {
            GameManager.Instance.PlanetDestroyed(unit.gameObject, source);
            ResetHealth();
        }

        protected override void OnHitEvent()
        {
            if (unit.Owner != PlayerType.IA) return;

            if (GameObject.FindGameObjectWithTag("IAController").TryGetComponent<IaController>(out _))
            {
                IaController.PlanetHit((PlanetUnit) unit);
            }
        }

        private void ResetHealth()
        {
            Health = maxHealth;
            UpdateHealthBar();
        }
    }
}