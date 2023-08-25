#pragma warning disable 0649
using Extensions;
using UnityEngine;

namespace Utilities
{
    public class RandomAnimation : MonoBehaviour
    {
        [SerializeField] private RuntimeAnimatorController[] _animations;

        private void Awake()
        {
            GetComponent<Animator>().runtimeAnimatorController = _animations.GetRandom();
        }
    }
}
