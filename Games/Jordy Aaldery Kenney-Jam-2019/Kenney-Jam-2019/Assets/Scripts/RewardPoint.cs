using UnityEngine;

public class RewardPoint : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.GetComponent<Player.PlayerController>() != null)
        {
            Debug.Log("ai;reward_game;5");
            gameObject.SetActive(false);
        }
    }
}
