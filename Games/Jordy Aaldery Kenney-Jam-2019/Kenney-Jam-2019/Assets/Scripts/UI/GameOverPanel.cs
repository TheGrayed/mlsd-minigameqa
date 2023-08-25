#pragma warning disable 0649
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace UI
{
    public class GameOverPanel : MonoBehaviour
    {
        [SerializeField] private Text _subtitleText;

        private void Awake()
        {
            transform.GetChild(0).gameObject.SetActive(false);
            _subtitleText.text = $"Level {SceneManager.GetActiveScene().buildIndex}";
        }

        public void Enable()
        {
            Debug.Log("ai;reward_game;-100");
            Debug.Log("ai;starting_menu;1");
            transform.GetChild(0).gameObject.SetActive(true);
        }

        public void Restart()
        {
            Debug.Log("ai;reward_menu;100");
            Debug.Log("ai;starting_level;" + SceneManager.GetActiveScene().buildIndex.ToString());
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        public void Menu()
        {
            Debug.Log("ai;reward_menu;-100");
            Debug.Log("ai;starting_menu;0");
            SceneManager.LoadScene(0);
        }
    }
}
