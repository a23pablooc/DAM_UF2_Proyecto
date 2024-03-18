using UnityEngine;

namespace UnitScripts.ShipScripts
{
    public class ShipHealthController : HealthController
    {
        [SerializeField] private AudioSource audioSource;
        
        protected override void DestroyAction(PlayerType _)
        {
            audioSource.Play();
            Destroy(unit.gameObject);
        }

        protected override void OnHitEvent()
        {
            if (unit.Owner != PlayerType.IA) return;

            if (GameObject.FindGameObjectWithTag("IAController").TryGetComponent<IAController>(out var iaController))
            {
                iaController.ShipHit((ShipUnit) unit);
            }
        }
    }
}