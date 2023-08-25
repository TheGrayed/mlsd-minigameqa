using Player;
using UI;
using UnityEngine;

public class GameController : MonoBehaviour
{
    private bool _isDone;
    
    private void Awake()
    {
        Time.timeScale = 1f;
    }

    public void LevelComplete()
    {
        if (_isDone)
            return;
        
        _isDone = true;
        FindObjectOfType<LevelCompletePanel>().Enable();
        FindObjectOfType<InputController>().enabled = false;
        Time.timeScale = 0.25f;
    }

    public void GameOver()
    {
        if (_isDone)
            return;

        _isDone = true;
        FindObjectOfType<GameOverPanel>().Enable();
        FindObjectOfType<InputController>().enabled = false;
        Time.timeScale = 0.25f;
    }
}
