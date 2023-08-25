using DefaultNamespace;
using UnityEngine;

public class Box : Hittable
{
    public override void Die()
    {
        GameObject obj = Instantiate(_destroyEffect, transform.position, Quaternion.identity);
        obj.transform.parent = transform.parent;
        
        Destroy(gameObject);
    }
}
