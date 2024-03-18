using UnityEngine;

namespace UnitScripts.PlanetScripts
{
    public class PlanetMovement : MonoBehaviour
    {
        [SerializeField] private float minRotationSpeed = 5f;
        [SerializeField] private float maxRotationSpeed = 10f;

        private float _rotationSpeed;

        private void Start()
        {
            _rotationSpeed = Random.Range(minRotationSpeed, maxRotationSpeed) * (1 - Random.Range(0, 2) * 2); //velocidad * (1 o -1) para rotar en un sentido u otro
        
        }

        private void Update()
        {
            Rotate();
        }

        private void Rotate()
        {
            transform.Rotate(Vector3.up, _rotationSpeed * Time.deltaTime);
        }
    }
}