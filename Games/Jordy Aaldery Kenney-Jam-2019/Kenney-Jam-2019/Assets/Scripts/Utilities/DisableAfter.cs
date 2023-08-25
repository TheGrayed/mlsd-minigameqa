using UnityEngine;

namespace Utilities
{
    public class DisableAfter : MonoBehaviour
    {
        [SerializeField] private float _lifetime = 1f;

        private void Awake()
        {
            Invoke(nameof(Disable), _lifetime);
        }

        private void Disable()
        {
            gameObject.SetActive(false);
        }
    }
}
