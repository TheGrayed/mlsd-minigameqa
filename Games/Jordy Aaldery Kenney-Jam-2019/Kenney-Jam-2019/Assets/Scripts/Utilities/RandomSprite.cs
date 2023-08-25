#pragma warning disable 0649
using Extensions;
using UnityEngine;

namespace Utilities
{
    public class RandomSprite : MonoBehaviour
    {
        [SerializeField] private Sprite[] _sprites;

        private void Awake()
        {
            GetComponent<SpriteRenderer>().sprite = _sprites.GetRandom();
        }
    }
}
