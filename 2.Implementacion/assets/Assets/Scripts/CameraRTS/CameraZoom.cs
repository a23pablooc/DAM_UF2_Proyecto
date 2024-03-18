using UnityEngine;

namespace CameraRTS
{
    public class CameraZoom : MonoBehaviour
    {
        [SerializeField] private float speed = 500f;
        [SerializeField] private float smoothing = 3f;
        [SerializeField] private Vector2 range = new(0f, 380f);
        [SerializeField] private Transform cameraHolder;
    
        private Vector3 CameraDirection => transform.InverseTransformDirection(cameraHolder.forward);
    
        private Vector3 _targetPosition;
        private float _input;
    
        private void Awake()
        {
            _targetPosition = cameraHolder.localPosition;
        }

        private void Update()
        {
            GetInput();
            Zoom();
        }
    
        private void GetInput()
        {
            _input = Input.GetAxisRaw("Mouse ScrollWheel");
        }
    
        private void Zoom()
        {
            var nextTargetPosition = _targetPosition + CameraDirection * (_input * speed);
        
            if (IsInBounds(nextTargetPosition))
                _targetPosition = nextTargetPosition;
        
            cameraHolder.localPosition = Vector3.Lerp(cameraHolder.localPosition, _targetPosition, Time.deltaTime * smoothing);
        }
    
        private bool IsInBounds(Vector3 position)
        {
            return position.y > range.x && position.y < range.y;
        }
    }
}