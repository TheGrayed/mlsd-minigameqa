#pragma warning disable 0649
using UnityEngine;

namespace DefaultNamespace
{
    public abstract class Hittable : MonoBehaviour
    {
        [SerializeField] protected GameObject _destroyEffect;
        
        [SerializeField] private float _health = 100f;
        public float Health
        {
            get => _health;
            set
            {
                _health = value;
                CheckDeath();
            }
        }
    
        private void CheckDeath()
        {
            if (_health <= 0f)
            {
                Die();
            }
        }

        public abstract void Die();
    }
}