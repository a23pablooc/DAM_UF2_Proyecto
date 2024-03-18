using UnityEngine;
using Cursor = UnityEngine.Cursor;

namespace CameraRTS
{
    public class CameraRotation : MonoBehaviour
    {
        [SerializeField] private float speed = 4f;
        [SerializeField] private float smoothing = 10f;

        private Vector3 _mousePosition;

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
                _mousePosition = Input.mousePosition;
                Cursor.lockState = CursorLockMode.Locked;
            }

            if (Input.GetMouseButton(2))
            {
                _targetAngle += Input.GetAxisRaw("Mouse X") * speed;
            }

            if (Input.GetMouseButtonUp(2))
            {
                Cursor.lockState = CursorLockMode.None;
                //move the mouse to the position it was before the middle mouse button was pressed
                // Cursor.position = _mousePosition;
            }
        }

        private void Rotate()
        {
            _currentAngle = Mathf.Lerp(_currentAngle, _targetAngle, Time.deltaTime * smoothing);
            transform.rotation = Quaternion.AngleAxis(_currentAngle, Vector3.up);
        }
    }
}