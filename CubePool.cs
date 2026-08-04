using System.Collections.Generic;
using UnityEngine;

namespace FallingCubes
{
    public class CubePool : MonoBehaviour
    {
        [SerializeField] private Cube _cubeTemplate;
        [SerializeField] private int _initialCapacity = 32;

        private Stack<Cube> _availableCubes;
        private HashSet<Cube> _availableCubeSet;
        private HashSet<Cube> _ownedCubes;

        private void Awake()
        {
            _availableCubes = new Stack<Cube>(_initialCapacity);
            _availableCubeSet = new HashSet<Cube>();
            _ownedCubes = new HashSet<Cube>();

            for (int i = 0; i < _initialCapacity; i++)
            {
                Cube cube = CreateCube();
                _availableCubes.Push(cube);
                _availableCubeSet.Add(cube);
            }
        }

        internal Cube Get(Vector3 position, Quaternion rotation)
        {
            Cube cube = _availableCubes.Count > 0 ? _availableCubes.Pop() : CreateCube();
            _availableCubeSet.Remove(cube);
            cube.PrepareForSpawn(position, rotation);
            cube.gameObject.SetActive(true);
            return cube;
        }

        internal void Release(Cube cube)
        {
            if (_ownedCubes.Contains(cube) == false || _availableCubeSet.Add(cube) == false)
                return;

            cube.gameObject.SetActive(false);
            _availableCubes.Push(cube);
        }

        private Cube CreateCube()
        {
            Cube cube = Instantiate(_cubeTemplate, transform);
            cube.gameObject.SetActive(false);
            cube.name = nameof(Cube);
            cube.LifetimeExpired += Release;
            _ownedCubes.Add(cube);
            return cube;
        }
    }
}
