using System.Collections;
using UnityEngine;

namespace FallingCubes
{
    [RequireComponent(typeof(CubePool))]
    public class CubeSpawner : MonoBehaviour
    {
        [SerializeField] private float _spawnInterval = 0.25f;
        [SerializeField] private Vector3 _spawnCenter = new Vector3(0f, 12f, 0f);
        [SerializeField] private Vector2 _spawnArea = new Vector2(10f, 10f);

        private CubePool _pool;
        private WaitForSeconds _spawnWait;
        private Coroutine _spawningCoroutine;
        private bool _hasStarted;

        private void Awake()
        {
            _pool = GetComponent<CubePool>();
            _spawnWait = new WaitForSeconds(_spawnInterval);
        }

        private void OnEnable()
        {
            if (_hasStarted)
                RunSpawning();
        }

        private void Start()
        {
            _hasStarted = true;
            RunSpawning();
        }

        private void OnDisable()
        {
            if (_spawningCoroutine == null)
                return;

            StopCoroutine(_spawningCoroutine);
            _spawningCoroutine = null;
        }

        private void RunSpawning()
        {
            _spawningCoroutine = StartCoroutine(SpawnContinuously());
        }

        private IEnumerator SpawnContinuously()
        {
            while (enabled)
            {
                Spawn();

                yield return _spawnWait;
            }

            _spawningCoroutine = null;
        }

        private void Spawn()
        {
            float halfWidth = _spawnArea.x / 2f;
            float halfDepth = _spawnArea.y / 2f;
            float xPosition = Random.Range(_spawnCenter.x - halfWidth, _spawnCenter.x + halfWidth);
            float zPosition = Random.Range(_spawnCenter.z - halfDepth, _spawnCenter.z + halfDepth);
            Vector3 position = new Vector3(xPosition, _spawnCenter.y, zPosition);
            _pool.Get(position, Random.rotation);
        }
    }
}
