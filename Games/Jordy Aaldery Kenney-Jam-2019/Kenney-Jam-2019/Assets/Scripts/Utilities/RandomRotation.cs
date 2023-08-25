using UnityEngine;

namespace Utilities
{
    public class RandomRotation : MonoBehaviour
    {
        [SerializeField] private float _min = 0f;
        [SerializeField] private float _max = 360f;

        private void Awake()
        {
            transform.Rotate(0f, 0f, Random.Range(_min, _max));
        }
    }
}
