using UnitScripts;
using UnityEngine;

namespace Bullet
{
    public class BulletController : MonoBehaviour
    {
        private PlayerType _unitSource;
        private float _damage;

        private float _bulletSpeed;

        public void Init(GameObject unitSource, float damage, float bulletLifeTime, float bulletSpeed)
        {
            _unitSource = unitSource.GetComponent<Unit>().Owner;
            _damage = damage;
            _bulletSpeed = bulletSpeed;

            Destroy(gameObject, bulletLifeTime);
        }

        private void Update()
        {
            transform.Translate(Vector3.forward * (_bulletSpeed * Time.deltaTime));
        }

        private void OnCollisionEnter(Collision other)
        {
            Destroy(gameObject);

            if (!other.gameObject.TryGetComponent<IShooteable>(out var shooteable)) return;

            if (!other.gameObject.TryGetComponent<Unit>(out var otherUnit)) return;

            if (otherUnit.Owner == _unitSource) return;

            shooteable.OnHit(_unitSource, _damage);
        }
    }
}