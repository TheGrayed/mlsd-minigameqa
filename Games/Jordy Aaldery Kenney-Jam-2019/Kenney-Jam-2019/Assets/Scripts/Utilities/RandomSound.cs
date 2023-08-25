#pragma warning disable 0649
using Extensions;
using UI;
using UnityEngine;

namespace Utilities
{
    public class RandomSound : MonoBehaviour
    {
        [SerializeField] private AudioClip[] _sounds;

        public void Play()
        {
            AudioController.instance.Play(_sounds.GetRandom());
        }
    }
}
