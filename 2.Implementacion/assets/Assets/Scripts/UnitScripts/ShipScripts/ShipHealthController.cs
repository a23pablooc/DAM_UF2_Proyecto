using UnityEngine;

namespace UnitScripts.ShipScripts
{
    /// <summary>
    /// Controlador de la salud de una nave
    /// </summary>
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

            if (GameObject.FindGameObjectWithTag("IAController").TryGetComponent<IaController>(out _))
            {
                IaController.ShipHit((ShipUnit) unit);
            }
        }
    }
}