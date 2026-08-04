using System;
using System.Collections;
using UnityEngine;

namespace FallingCubes
{
    [RequireComponent(typeof(Rigidbody), typeof(Renderer), typeof(Collider))]
    public class Cube : MonoBehaviour
    {
        private static readonly int s_baseColorID = Shader.PropertyToID("_BaseColor");
        private static readonly int s_colorID = Shader.PropertyToID("_Color");

        [SerializeField] private Color _fallingColor = new Color(0.12f, 0.55f, 1f, 1f);
        [SerializeField] private Color _contactColor = new Color(1f, 0.27f, 0.08f, 1f);
        [SerializeField] private float _minimumLifetime = 2f;
        [SerializeField] private float _maximumLifetime = 5f;

        private Rigidbody _rigidbody;
        private Renderer _renderer;
        private MaterialPropertyBlock _propertyBlock;
        private Coroutine _lifetimeCoroutine;
        private bool _hasTouchedPlatform;
        private float _remainingLifetime;
        private float _expirationTime;

        public event Action<Cube> LifetimeExpired;

        private void Awake()
        {
            CacheComponents();
        }

        private void OnEnable()
        {
            if (_hasTouchedPlatform == false)
                return;

            if (_remainingLifetime <= 0f)
            {
                OnLifetimeExpired();
                return;
            }

            RunLifetimeCountdown(_remainingLifetime);
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (_hasTouchedPlatform)
                return;

            if (collision.gameObject.TryGetComponent<Platform>(out _) == false)
                return;

            _hasTouchedPlatform = true;
            SetColor(_contactColor);

            float lifetime = UnityEngine.Random.Range(_minimumLifetime, _maximumLifetime);
            RunLifetimeCountdown(lifetime);
        }

        private void OnDisable()
        {
            if (_lifetimeCoroutine == null)
                return;

            _remainingLifetime = Mathf.Max(0f, _expirationTime - Time.time);
            StopCoroutine(_lifetimeCoroutine);
            _lifetimeCoroutine = null;
        }

        internal void PrepareForSpawn(Vector3 position, Quaternion rotation)
        {
            if (_rigidbody == null)
                CacheComponents();

            _hasTouchedPlatform = false;
            _remainingLifetime = 0f;
            _expirationTime = 0f;
            transform.SetPositionAndRotation(position, rotation);

            _rigidbody.linearVelocity = Vector3.zero;
            _rigidbody.angularVelocity = Vector3.zero;
            SetColor(_fallingColor);
        }

        private IEnumerator ExpireAfterDelay(float lifetime)
        {
            WaitForSeconds wait = new WaitForSeconds(lifetime);
            yield return wait;

            _remainingLifetime = 0f;
            _expirationTime = 0f;
            _lifetimeCoroutine = null;
            OnLifetimeExpired();
        }

        private void RunLifetimeCountdown(float lifetime)
        {
            _remainingLifetime = lifetime;
            _expirationTime = Time.time + lifetime;
            _lifetimeCoroutine = StartCoroutine(ExpireAfterDelay(lifetime));
        }

        private void CacheComponents()
        {
            _rigidbody = GetComponent<Rigidbody>();
            _renderer = GetComponent<Renderer>();
            _propertyBlock = new MaterialPropertyBlock();
        }

        private void SetColor(Color color)
        {
            _renderer.GetPropertyBlock(_propertyBlock);
            _propertyBlock.SetColor(s_baseColorID, color);
            _propertyBlock.SetColor(s_colorID, color);
            _renderer.SetPropertyBlock(_propertyBlock);
        }

        private void OnLifetimeExpired()
        {
            LifetimeExpired?.Invoke(this);
        }
    }
}
