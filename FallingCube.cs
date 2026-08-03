using System.Collections;
using UnityEngine;

namespace FallingCubes
{
    [RequireComponent(typeof(Rigidbody), typeof(Collider), typeof(FallingCubeView))]
    public sealed class FallingCube : MonoBehaviour
    {
        [SerializeField] private float _minimumLifetime = 2f;
        [SerializeField] private float _maximumLifetime = 5f;

        private FallingCubePool _pool;
        private Rigidbody _rigidbody;
        private FallingCubeView _view;
        private Coroutine _returnCoroutine;
        private bool _hasTouchedSurface;
        private float _disappearanceTime;

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
            _view = GetComponent<FallingCubeView>();
        }

        private void OnEnable()
        {
            if (_hasTouchedSurface == false)
                return;

            float remainingLifetime = _disappearanceTime - Time.time;

            if (remainingLifetime <= 0f)
            {
                _pool.Release(this);
                return;
            }

            RunReturnCountdown(remainingLifetime);
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (_hasTouchedSurface)
                return;

            if (collision.gameObject.TryGetComponent<FallingCubeSurface>(out _) == false)
                return;

            _hasTouchedSurface = true;
            _view.ShowContactColor();

            float lifetime = Random.Range(_minimumLifetime, _maximumLifetime);
            _disappearanceTime = Time.time + lifetime;
            RunReturnCountdown(lifetime);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent<FallingCubeReturnZone>(out _) == false)
                return;

            _pool.Release(this);
        }

        private void OnDisable()
        {
            if (_returnCoroutine == null)
                return;

            StopCoroutine(_returnCoroutine);
            _returnCoroutine = null;
        }

        internal void Initialize(FallingCubePool pool)
        {
            _pool = pool;
        }

        internal void Activate(Vector3 position, Quaternion rotation)
        {
            _hasTouchedSurface = false;
            transform.SetPositionAndRotation(position, rotation);
            gameObject.SetActive(true);

            _rigidbody.linearVelocity = Vector3.zero;
            _rigidbody.angularVelocity = Vector3.zero;
            _view.ShowFallingColor();
        }

        private IEnumerator ReturnAfterDelay(float lifetime)
        {
            WaitForSeconds wait = new WaitForSeconds(lifetime);
            yield return wait;

            _returnCoroutine = null;
            _pool.Release(this);
        }

        private void RunReturnCountdown(float lifetime)
        {
            _returnCoroutine = StartCoroutine(ReturnAfterDelay(lifetime));
        }
    }
}
