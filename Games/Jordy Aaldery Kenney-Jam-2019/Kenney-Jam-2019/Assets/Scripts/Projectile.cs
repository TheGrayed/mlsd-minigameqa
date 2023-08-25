#pragma warning disable 0649
using DefaultNamespace;
using Player;
using UI;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private float _speed = 1f;
    [SerializeField] private float _damage = 10f;
    [SerializeField] private float _lifetime = 5f;

    [SerializeField] private GameObject _destroyEffect;
    [SerializeField] private AudioClip _destroyClip;
    
    private GameObject _parent;
    public GameObject Parent
    {
        private get => _parent;
        set
        {
            Debug.Assert(!_parent, "A parent has already been set.");
            _parent = value;
        }
    }

    private bool _initialized;

    private void Awake()
    {
        transform.parent = FindObjectOfType<Rotator>().transform;
        
        Destroy(gameObject, _lifetime);
    }

    public void Initialize(Vector2 direction)
    {
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.Rotate(0f, 0f, angle);
        _initialized = true;
    }

    private void FixedUpdate()
    {
        if (!_initialized)
            return;

        Transform t = transform;
        t.position += _speed * Time.fixedDeltaTime * t.right;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject == Parent)
            return;

        Hittable hit = other.GetComponent<Hittable>();
        if (hit)
        {
            hit.Health -= _damage;
        }
        
        GameObject obj = Instantiate(_destroyEffect, transform.position, Quaternion.identity);
        obj.transform.parent = transform.parent;
        
        AudioController.instance.Play(_destroyClip, 0.5f);
        
        Destroy(gameObject);
    }
}
