using UnityEngine;

namespace FallingCubes
{
    [RequireComponent(typeof(Collider))]
    public class CubeReturnZone : MonoBehaviour
    {
        [SerializeField] private CubePool _pool;

        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent<Cube>(out Cube cube) == false)
                return;

            _pool.Release(cube);
        }
    }
}
