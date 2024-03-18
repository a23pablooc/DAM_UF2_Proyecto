using UnityEngine;
using UnityEngine.UI;

namespace UnitScripts
{
    public abstract class HealthController : MonoBehaviour
    {
        [SerializeField] private Transform healthBarTransform;
        [SerializeField] private Image healthBar;
        private Camera _camera;
        
        [SerializeField] protected float maxHealth;
        protected float Health;
        
        [SerializeField] protected Unit unit;
        
        private void Awake()
        {
            _camera = Camera.main;
        }

        private void Start()
        {
            Health = maxHealth;
            UpdateHealthBar();
            unit.OnHitReceived += OnHit;
        }

        private void OnDestroy()
        {
            unit.OnHitReceived -= OnHit;
        }

        private void Update()
        {
            healthBarTransform.LookAt(_camera.transform);
        }

        private void OnHit(PlayerType source, float damage)
        {
            Health -= damage;
            
            if (Health <= 0)
            {
                DestroyAction(source);
            }
            
            UpdateHealthBar();

            OnHitEvent();
        }
        
        protected abstract void DestroyAction(PlayerType source);

        protected abstract void OnHitEvent();

        protected void UpdateHealthBar()
        {
            healthBar.fillAmount = Health / maxHealth;
        }
    }
}