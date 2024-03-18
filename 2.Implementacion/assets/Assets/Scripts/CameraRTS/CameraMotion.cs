using UnityEngine;

namespace CameraRTS
{
    public class CameraMotion : MonoBehaviour
    {
        [SerializeField] private float baseSpeed = 1f;
        [SerializeField] private float fastSpeed = 5f;
        [SerializeField] private float smoothing = 5f;
        [SerializeField] private Vector2 range = new(1000, 1000);

        private Vector3 _targetPosition;
        private Vector3 _input;
        private float _speed;

        private void Awake()
        {
            _targetPosition = transform.position;
        }

        private void Update()
        {
            GetInput();
            Move();
        }

        private void GetInput()
        {
            var x = Input.GetAxis("Horizontal");
            var y = Input.GetAxis("Vertical");

            var t = transform;
            var right = t.right * x;
            var forward = t.forward * y;

            _input = (right + forward).normalized;
            
            _speed = Input.GetKey(KeyCode.LeftShift) ? fastSpeed : baseSpeed;
        }

        private void Move()
        {
            var nextPosition = _targetPosition + _input * _speed;

            if (IsInBounds(nextPosition))
            {
                _targetPosition = nextPosition;
            }
            else
            {
                _targetPosition.x = Mathf.Clamp(_targetPosition.x, -range.x, range.x);
                _targetPosition.z = Mathf.Clamp(_targetPosition.z, -range.y, range.y);
            }

            transform.position = Vector3.Lerp(transform.position, _targetPosition, Time.deltaTime * smoothing);
        }

        private bool IsInBounds(Vector3 position)
        {
            return position.x > -range.x &&
                   position.x < range.x &&
                   position.z > -range.y &&
                   position.z < range.y;
        }
    }
}