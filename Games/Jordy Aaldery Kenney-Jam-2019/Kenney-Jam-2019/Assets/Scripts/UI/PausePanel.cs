#pragma warning disable 0649
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace UI
{
    public class PausePanel : MonoBehaviour
    {
        [SerializeField] private Text _subtitleText;

        private bool _enabled;

        private void Awake()
        {
            transform.GetChild(0).gameObject.SetActive(false);
            _subtitleText.text = $"Level {SceneManager.GetActiveScene().buildIndex}";
        }

        public void Toggle()
        {
            _enabled = !_enabled;
            transform.GetChild(0).gameObject.SetActive(_enabled);
            Time.timeScale = _enabled ? 0f : 1f;
        }

        public void Continue()
        {
            transform.GetChild(0).gameObject.SetActive(false);
            Time.timeScale = 1f;
        }

        public void Restart()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        public void Menu()
        {
            SceneManager.LoadScene(0);
        }
    }
}
