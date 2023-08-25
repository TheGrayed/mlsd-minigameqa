using UnityEngine;

namespace Player
{
    public class Rotator : MonoBehaviour
    {
        [SerializeField] private float _speed = 1f;

        public void Rotate(float direction)
        {
            transform.Rotate(0f, 0f, -direction * _speed * Time.deltaTime);
        }
    }
}
