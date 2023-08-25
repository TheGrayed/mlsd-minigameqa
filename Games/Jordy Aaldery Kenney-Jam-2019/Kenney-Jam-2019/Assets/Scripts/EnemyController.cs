#pragma warning disable 0649
using DefaultNamespace;
using Extensions;
using UnityEngine;

public class EnemyController : Hittable
{
    [SerializeField] private float _cooldown = 5f;
    [SerializeField] private float _viewDistance = 5f;
    
    [SerializeField] private LayerMask _viewMask = ~(1 << 9);
    [SerializeField] private GameObject _bulletPrefab;

    private Transform _player;
    private bool _canShoot;

    private void Awake()
    {
        _player = GameObject.FindWithTag("Player").transform;
        Invoke(nameof(EnableShoot), _cooldown);
    }
    
    private void Update()
    {
        Aim();
    }

    private void Aim()
    {
        if (!_canShoot)
            return;
        
        Vector2 pos = transform.position;
        Vector2 direction = (_player.position.AsVector2() - pos).normalized;
        RaycastHit2D hit = Physics2D.Raycast(pos, direction, _viewDistance, _viewMask);

        if (hit && hit.collider.CompareTag("Player"))
        {
            Shoot(direction);
        }
    }

    private void Shoot(Vector3 direction)
    {
        _canShoot = false;
        Invoke(nameof(EnableShoot), _cooldown);

        GameObject bullet = Instantiate(_bulletPrefab, transform.position, Quaternion.identity);
        Projectile projectile = bullet.GetComponent<Projectile>();
        
        projectile.Parent = gameObject;
        projectile.Initialize(direction);
    }

    private void EnableShoot()
    {
        _canShoot = true;
    }

    public override void Die()
    {
        GameObject obj = Instantiate(_destroyEffect, transform.position, Quaternion.identity);
        obj.transform.parent = transform.parent;
        
        Destroy(gameObject);
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            other.gameObject.GetComponent<Hittable>().Die();
            Die();
        }
    }
}
