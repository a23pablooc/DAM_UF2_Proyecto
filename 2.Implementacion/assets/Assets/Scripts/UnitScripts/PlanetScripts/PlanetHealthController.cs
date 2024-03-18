using UnityEngine;

namespace UnitScripts.PlanetScripts
{
    public class PlanetHealthController : HealthController
    {
        protected override void DestroyAction(PlayerType source)
        {
            GameManager.Instance.PlanetDestroyed(base.unit.gameObject, source);
            ResetHealth();
        }

        protected override void OnHitEvent()
        {
            if (unit.Owner != PlayerType.IA) return;

            if (GameObject.FindGameObjectWithTag("IAController").TryGetComponent<IAController>(out var iaController))
            {
                iaController.PlanetHit((PlanetUnit) unit);
            }
        }

        private void ResetHealth()
        {
            Health = maxHealth;
            UpdateHealthBar();
        }
    }
}