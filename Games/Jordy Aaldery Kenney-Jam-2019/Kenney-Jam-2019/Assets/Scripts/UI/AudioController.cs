#pragma warning disable 0649
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class AudioController : MonoBehaviour
    {
        public static AudioController instance { get; private set; }

        private void Awake()
        {
            if (!instance)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else if (instance != this)
            {
                Destroy(gameObject);
            }
        }

        [SerializeField] private AudioSource _musicSource;
        [SerializeField] private AudioSource _sfxSource;

        [SerializeField] private Image _muteImage;
        [SerializeField] private Sprite _onSprite;
        [SerializeField] private Sprite _offSprite;
        private bool _mute;

        public void Play(AudioClip clip, float volume = 1f)
        {
            _sfxSource.pitch = Random.Range(0.95f, 1.05f);
            _sfxSource.volume = volume;
            _sfxSource.PlayOneShot(clip);
        }

        public void Toggle()
        {
            if (!_muteImage)
                return;
            
            _mute = !_mute;
            _muteImage.sprite = _mute ? _offSprite : _onSprite;
            _musicSource.volume = _mute ? 0f : 1f;
        }
    }
}
