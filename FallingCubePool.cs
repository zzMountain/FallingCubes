using System.Collections.Generic;
using UnityEngine;

namespace FallingCubes
{
    public sealed class FallingCubePool : MonoBehaviour
    {
        [SerializeField] private FallingCube _cubeTemplate;
        [SerializeField] private int _initialCapacity = 32;

        private Stack<FallingCube> _availableCubes;

        private void Awake()
        {
            _availableCubes = new Stack<FallingCube>(_initialCapacity);

            for (int i = 0; i < _initialCapacity; i++)
                _availableCubes.Push(CreateCube());
        }

        internal FallingCube Get()
        {
            return _availableCubes.Count > 0 ? _availableCubes.Pop() : CreateCube();
        }

        internal void Release(FallingCube cube)
        {
            cube.gameObject.SetActive(false);
            _availableCubes.Push(cube);
        }

        private FallingCube CreateCube()
        {
            FallingCube cube = Instantiate(_cubeTemplate, transform);
            cube.gameObject.SetActive(false);
            cube.name = nameof(FallingCube);
            cube.Initialize(this);
            return cube;
        }
    }
}
