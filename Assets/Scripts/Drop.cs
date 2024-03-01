using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using Random = UnityEngine.Random;

public class Drop : MonoBehaviour
{
    public bool isBad;
    [Range(0.1f, 3f)] [SerializeField] private float minScale;
    [Range(0.1f, 3f)] [SerializeField] private float maxScale;
    
    [Range(1f, 10f)] [SerializeField] float explodeAfter = 3f;
    [Range(1f, 10f)] [SerializeField] float affectingRadius = 1f;
    [SerializeField] private GameObject explosion;
    private AudioSource _audio;
    

    private void Awake()
    {
        float scale = Random.Range(minScale, maxScale);
        transform.DOScale(scale * 1.5f, 0.3f).onComplete += () =>
        {
            transform.DOScale(scale, 0.2f);
        };
    }

    private IEnumerator Start()
    {
        yield return new WaitForSeconds(explodeAfter);
        Explode();
    }


    void Explode()
    {
        // Spawning the explosion particles:
        Instantiate(explosion, transform.position, Quaternion.identity);
        Destroy(gameObject);
        Collider2D[] results = new Collider2D[30];
        int hits = Physics2D.OverlapCircleNonAlloc(transform.position, affectingRadius, results);
        
        for (int i = 0; i < hits; i++)
        {
            Actor actor = results[i].GetComponent<Actor>();
            if (actor)
            {
                actor.TakeDamage(1000f);
                GameManager.Instance.GetPlayer().GainWeight(0.1f);
            }
            
            results[i].GetComponent<Spawner>()?.TakeLife();
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, affectingRadius);
    }
}
