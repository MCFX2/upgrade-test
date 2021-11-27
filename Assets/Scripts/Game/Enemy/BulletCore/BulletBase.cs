using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Rigidbody))]
public class BulletBase : MonoBehaviour
{
    [SerializeField] string playerTag = "Player";
    //if false - bullet destroyts itself when damaging a player
    [SerializeField] bool playerPassThru = true;
    //whether to destroy the bullet as soon as it's offscreen
    //strongly recommended that you don't create bullets which rely
    //on offscreen behavior but it can be useful
    [SerializeField] bool destroyOffscreen = true;
    public int damage = 1;

    //lifetime of bullet, can be overridden by a behavior.
    //this is simply meant to catch bullets that never get a chance to despawn properly,
    //avoiding cases where bullets remain stuck offscreen indefinitely.
    public float lifetime = 180f;

    private void DestroyThis()
    {
        Destroy(this.gameObject);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(playerTag))
        {
            other.gameObject.GetComponent<PlayerHealth>().Damage(damage);
            if(!playerPassThru)
            {
                DestroyThis();
            }
        }
    }

    void Update()
    {
        lifetime -= Time.deltaTime;
        if(lifetime <= 0)
        {
            DestroyThis();
        }
    }

    void OnBecameInvisible()
    {
        if (destroyOffscreen)
        {
            DestroyThis();
        }
    }
}
