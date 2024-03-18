using UnityEngine;

namespace Unity_UI_Samples.Scripts
{
    public class TiltWindow : MonoBehaviour
    {
        public Vector2 range = new Vector2(5f, 3f);

        private Transform _mTrans;
        private Quaternion _mStart;
        private Vector2 _mRot = Vector2.zero;

        private void Start()
        {
            _mTrans = transform;
            _mStart = _mTrans.localRotation;
        }

        private void Update()
        {
            var pos = Input.mousePosition;

            var halfWidth = Screen.width * 0.5f;
            var halfHeight = Screen.height * 0.5f;
            var x = Mathf.Clamp((pos.x - halfWidth) / halfWidth, -1f, 1f);
            var y = Mathf.Clamp((pos.y - halfHeight) / halfHeight, -1f, 1f);
            _mRot = Vector2.Lerp(_mRot, new Vector2(x, y), Time.deltaTime * 5f);

            _mTrans.localRotation = _mStart * Quaternion.Euler(-_mRot.y * range.y, _mRot.x * range.x, 0f);
        }
    }
}