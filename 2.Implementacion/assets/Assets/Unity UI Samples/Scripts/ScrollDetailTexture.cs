using UnityEngine;
using UnityEngine.UI;

namespace Unity_UI_Samples.Scripts
{
    [RequireComponent(typeof(Image))]
    public class ScrollDetailTexture : MonoBehaviour
    {
        public bool uniqueMaterial;
        public Vector2 scrollPerSecond = Vector2.zero;

        private Matrix4x4 _mMatrix;
        private Material _mCopy;
        private Material _mOriginal;
        private Image _mSprite;
        private Material _mMat;
        private static readonly int DetailTex = Shader.PropertyToID("_DetailTex");

        private void OnEnable()
        {
            _mSprite = GetComponent<Image>();
            _mOriginal = _mSprite.material;

            if (!uniqueMaterial || _mSprite.material == null) return;
            _mCopy = new Material(_mOriginal)
            {
                name = "Copy of " + _mOriginal.name,
                hideFlags = HideFlags.DontSave
            };
            _mSprite.material = _mCopy;
        }

        private void OnDisable()
        {
            if (_mCopy != null)
            {
                _mSprite.material = _mOriginal;
                if (Application.isEditor)
                    DestroyImmediate(_mCopy);
                else
                    Destroy(_mCopy);
                _mCopy = null;
            }

            _mOriginal = null;
        }

        private void Update()
        {
            var mat = _mCopy != null ? _mCopy : _mOriginal;

            if (mat == null) return;
            var tex = mat.GetTexture(DetailTex);

            if (tex != null)
            {
                mat.SetTextureOffset(DetailTex, scrollPerSecond * Time.time);

                // TODO: It would be better to add support for MaterialBlocks on UIRenderer,
                // because currently only one Update() function's matrix can be active at a time.
                // With material block properties, the batching would be correctly broken up instead,
                // and would work with multiple widgets using this detail shader.
            }
        }
    }
}