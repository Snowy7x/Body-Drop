using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    private float _damage;

    [SerializeField] private GameObject impact;
    [SerializeField] Rigidbody2D rb;
    
    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Player"))
            GameManager.Instance.GetPlayer().TakeDamage(_damage);
        
        // TODO: Bullet hit impact
        Instantiate(impact, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }

    public void Init(Vector3 dir, float speed, float damage)
    {
        _damage = damage;
        rb.AddForce(dir * speed);
    } 
}
