using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace UI
{
    public class MainMenuPanel : MonoBehaviour
    {
        private void Awake()
        {
            var pos = Input.mousePosition;
            Debug.Log("ai;starting_menu;0");
        }

        public void StartGame()
        {
            Debug.Log("ai;starting_level;1");
            Debug.Log("ai;reward_menu;100");
            SceneManager.LoadScene(1);
        }

        public void ContinueGame()
        {
            int bestLevel = PlayerPrefs.GetInt("BestLevel", 1);
            Debug.Log("ai;starting_level;" + bestLevel.ToString());
            Debug.Log("ai;reward_menu;100");
            SceneManager.LoadScene(bestLevel);
        }

        public void Quit()
        {
            Debug.Log("ai;quitting;0");
            Debug.Log("ai;reward_menu;-1000");
            Application.Quit();
        }
    }
}
