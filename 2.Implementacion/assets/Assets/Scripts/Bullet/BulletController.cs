using UnitScripts;
using UnityEngine;

namespace Bullet
{
    /// <summary>
    /// Controla el comportamiento de la bala. Movimiento y colisiones.
    /// </summary>
    public class BulletController : MonoBehaviour
    {
        private PlayerType _unitSource;
        private float _damage;
        private float _bulletSpeed;

        /// <summary>
        /// Inicializa la bala con los valores necesarios.
        /// </summary>
        /// <param name="unitSource">El objeto que dispara la bala</param>
        /// <param name="damage">Daño que causará la bala</param>
        /// <param name="bulletLifeTime">Tiempo de vida de la bala en segundos</param>
        /// <param name="bulletSpeed">Velocidad de la bala</param>
        public void Init(GameObject unitSource, float damage, float bulletLifeTime, float bulletSpeed)
        {
            _unitSource = unitSource.GetComponent<Unit>().Owner;
            _damage = damage;
            _bulletSpeed = bulletSpeed;

            // Destruye la bala después de un tiempo
            Destroy(gameObject, bulletLifeTime);
        }

        /// <summary>
        /// Actualiza la posición de la bala.
        /// </summary>
        private void Update()
        {
            // Mueve la bala hacia adelante
            transform.Translate(Vector3.forward * (_bulletSpeed * Time.deltaTime));
        }

        /// <summary>
        /// Maneja la colisión de la bala con otros objetos.
        /// </summary>
        /// <param name="other">El objeto con el que la bala choca</param>
        private void OnCollisionEnter(Collision other)
        {
            // Destruye la bala al chocar con otro objeto
            Destroy(gameObject);

            // Si el otro objeto no es un IShooteable, no hacer nada
            if (!other.gameObject.TryGetComponent<IShooteable>(out var shooteable)) return;

            // Si el otro objeto no es un Unit, no hacer nada
            if (!other.gameObject.TryGetComponent<Unit>(out var otherUnit)) return;

            // Si el otro objeto es del mismo jugador que el que disparó la bala, no hacer nada
            if (otherUnit.Owner == _unitSource) return;

            // Aplica el daño al objeto con el que chocó la bala
            shooteable.OnHit(_unitSource, _damage);
        }
    }
}