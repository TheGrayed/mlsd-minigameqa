#pragma warning disable 0649
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace UI
{
    public class LevelCompletePanel : MonoBehaviour
    {
        [SerializeField] private Text _subtitleText;
        [SerializeField] private GameObject _continueButton;

        private void Awake()
        {
            transform.GetChild(0).gameObject.SetActive(false);
            _subtitleText.text = $"Level {SceneManager.GetActiveScene().buildIndex}";
        }

        public void Enable()
        {
            Debug.Log("ai;reward_game;100");
            Debug.Log("ai;starting_menu;2");
            transform.GetChild(0).gameObject.SetActive(true);
        }

        public void Continue()
        {
            int currentIndex = SceneManager.GetActiveScene().buildIndex;
            PlayerPrefs.SetInt("BestLevel", currentIndex);
            Debug.Log("ai;reward_menu;100");
            Debug.Log("ai;starting_level;" + (currentIndex + 1).ToString());
            SceneManager.LoadScene(currentIndex + 1);
        }

        public void Menu()
        {
            Debug.Log("ai;reward_menu;-100");
            Debug.Log("ai;starting_menu;0");
            SceneManager.LoadScene(0);
        }
    }
}
