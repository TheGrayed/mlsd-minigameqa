using UnityEngine;

namespace Utilities
{
    public class DestroyAfter : MonoBehaviour
    {
        [SerializeField] private float _lifetime = 1f;

        private void Awake()
        {
            Invoke(nameof(Destroy), _lifetime);
        }

        private void Destroy()
        {
            Destroy(gameObject);
        }
    }
}
