using UnityEngine;

namespace FallingCubes
{
    [RequireComponent(typeof(Renderer))]
    public sealed class FallingCubeView : MonoBehaviour
    {
        private static readonly int s_baseColorID = Shader.PropertyToID("_BaseColor");
        private static readonly int s_colorID = Shader.PropertyToID("_Color");

        [SerializeField] private Color _fallingColor = new Color(0.12f, 0.55f, 1f, 1f);
        [SerializeField] private Color _contactColor = new Color(1f, 0.27f, 0.08f, 1f);

        private Renderer _renderer;
        private MaterialPropertyBlock _propertyBlock;

        private void Awake()
        {
            _renderer = GetComponent<Renderer>();
            _propertyBlock = new MaterialPropertyBlock();
        }

        internal void ShowFallingColor()
        {
            SetColor(_fallingColor);
        }

        internal void ShowContactColor()
        {
            SetColor(_contactColor);
        }

        private void SetColor(Color color)
        {
            _renderer.GetPropertyBlock(_propertyBlock);
            _propertyBlock.SetColor(s_baseColorID, color);
            _propertyBlock.SetColor(s_colorID, color);
            _renderer.SetPropertyBlock(_propertyBlock);
        }
    }
}
