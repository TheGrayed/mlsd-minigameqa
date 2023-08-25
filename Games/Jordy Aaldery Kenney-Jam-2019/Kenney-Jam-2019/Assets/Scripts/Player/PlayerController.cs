#pragma warning disable 0649
using System.Collections;
using DefaultNamespace;
using UI;
using UnityEngine;

namespace Player
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class PlayerController : Hittable
    {
        [SerializeField] private float _jumpForce = 10f;
        [SerializeField] private float _jumpOffset = 0.5f;
        [SerializeField] private LayerMask _jumpMask = ~(1 << 8);

        [SerializeField] private float _cooldown = 5f;
        [SerializeField] private GameObject _bulletPrefab;

        [SerializeField] private int _ammo;
        private int Ammo
        {
            get => _ammo;
            set
            {
                _ammo = value;
                _ammoPanel.SetAmmo(_ammo);
            }
        }
        
        private bool _canShoot = true;
        private Rigidbody2D _rb;
        private AmmoPanel _ammoPanel;

        private void Awake()
        {
            if (Input.AIPosessed)
                _ammo = 10000;
            _rb = GetComponent<Rigidbody2D>();
            _ammoPanel = FindObjectOfType<AmmoPanel>();
            
            _ammoPanel.SetAmmo(_ammo);
            _ammoPanel.SetCooldown(1f);
        }

        public void Jump()
        {
            Vector2 pos = transform.position;
            Vector2 direction = Vector2.down;
            RaycastHit2D hit = Physics2D.Raycast(pos, direction, _jumpOffset, _jumpMask);

            if (hit)
            {
                _rb.AddForce(new Vector2(0f, _jumpForce));
            }
        }

        public void Shoot(Vector2 direction)
        {
            if (!_canShoot || _ammo < 1)
                return;

            _canShoot = false;
            Ammo--;
            StartCoroutine(EnableShoot());

            GameObject bullet = Instantiate(_bulletPrefab, transform.position, Quaternion.identity);
            Projectile projectile = bullet.GetComponent<Projectile>();
            
            projectile.Parent = gameObject;
            projectile.Initialize(direction);

            var enemies = FindObjectsOfType<EnemyController>();
            var min_error = float.MaxValue;
            for( int i=0; i<enemies.Length; ++i)
            {
                var dist = enemies[i].transform.position - transform.position;
                var deg = Vector2.Angle(dist, direction);
                var err = Mathf.Abs(Mathf.Tan(deg * Mathf.Deg2Rad) * dist.magnitude);
                min_error = Mathf.Min(err, min_error);
            }
            if (min_error < 1)
                Debug.Log("ai;reward_game;1");

        }

        private IEnumerator EnableShoot()
        {
            float currentCooldown = _cooldown;

            while (currentCooldown > 0f)
            {
                currentCooldown -= Time.deltaTime;
                _ammoPanel.SetCooldown((_cooldown - currentCooldown) / _cooldown);
                
                yield return null;
            }
            
            _canShoot = true;
        }

        public override void Die()
        {
            GameObject obj = Instantiate(_destroyEffect, transform.position, Quaternion.identity);
            obj.transform.parent = transform.parent;
            
            GetComponent<SpriteRenderer>().enabled = false;
            FindObjectOfType<GameController>().GameOver();
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Exit"))
            {
                FindObjectOfType<GameController>().LevelComplete();
            }
        }

        private void OnCollisionEnter2D(Collision2D other)
        {
            GameObject otherObj = other.gameObject;
            if (otherObj.CompareTag("Enemy"))
            {
                otherObj.GetComponent<Hittable>().Die();
                Die();
            }
        }
    }
}
