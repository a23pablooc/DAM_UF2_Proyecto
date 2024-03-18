using UnityEngine;
using Cursor = UnityEngine.Cursor;

namespace CameraRTS
{
    /// <summary>
    /// Controla la rotación de la cámara.
    /// </summary>
    public class CameraRotation : MonoBehaviour
    {
        [SerializeField] private float speed = 4f;
        [SerializeField] private float smoothing = 10f;


        private float _targetAngle;
        private float _currentAngle;

        private void Awake()
        {
            _targetAngle = transform.eulerAngles.y;
            _currentAngle = _targetAngle;
        }

        private void Update()
        {
            GetInput();
            Rotate();
        }

        private void GetInput()
        {
            if (Input.GetMouseButtonDown(2))
            {
                Cursor.lockState = CursorLockMode.Locked;
            }

            if (Input.GetMouseButton(2))
            {
                _targetAngle += Input.GetAxisRaw("Mouse X") * speed;
            }

            if (Input.GetMouseButtonUp(2))
            {
                Cursor.lockState = CursorLockMode.None;
            }
        }

        private void Rotate()
        {
            _currentAngle = Mathf.Lerp(_currentAngle, _targetAngle, Time.deltaTime * smoothing);
            transform.rotation = Quaternion.AngleAxis(_currentAngle, Vector3.up);
        }
    }
}