using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using JetBrains.Annotations;
using TarodevController;
using UnityEngine;

enum EnemyType
{
    Explode,
    Shooter
}

enum State
{
    Idle,
    Patrolling,
    Following,
    Attacking
} 

public class EnemyAI : Actor
{
    [SerializeField] private EnemyType type;
    [SerializeField] private State _state;

    [SerializeField] private GameObject explosion;
    [SerializeField] private Projectile projectile;
    [SerializeField] private Transform shootingPointRight;
    [SerializeField] private Transform shootingPointLeft;
    
    [SerializeField] [Range(1f, 20f)] private float speed;
    
    [SerializeField] [Range(0.1f, 10f)] private float maxGround;
    [SerializeField] [Range(0.5f, 10f)] private float noticeRange;
    [SerializeField] [Range(0.5f, 20f)] private float followRange;
    [SerializeField] [Range(0.5f, 8f)] private float attackRange;
    [SerializeField] private bool stopWhenAttacking;
    [SerializeField] [Range(0.5f, 8f)] float explosionRange;
    [SerializeField] [Range(0.1f, 5)] float timeBetweenShots;
    [SerializeField] float shootingForce;
    [SerializeField] private float damage;

    private SpriteRenderer _renderer;
    private Rigidbody2D rb;
    private Transform target;
    
    Vector3 botLeft;
    Vector3 botRight;

    private float _lastShot = 0;

    private bool _started = false;
    private Spawner mySpawner;

    private AudioSource _audio;

    public void SetSpawner(Spawner spawner)
    {
        mySpawner = spawner;
    }
    
    private void Awake()
    {
        _renderer = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        _audio = GetComponent<AudioSource>();
    }

    private void Start()
    {
        transform.localScale = Vector3.zero;
        transform.DOScale(1, 0.5f).onComplete += () =>
        {
            _started = true;
        };
        UpdateBottomLines();
    }

    private void UpdateBottomLines()
    {
        var sprite = _renderer.sprite;
        botLeft = _renderer.transform.TransformPoint(sprite.bounds.min);
        botLeft.y += 0.1f;
        botRight = _renderer.transform.TransformPoint(new Vector3(sprite.bounds.max.x, sprite.bounds.min.y, 0));
        botRight.y += 0.1f;
    }

    private void Update()
    {
        if (!_started) return;
        if (!target) UpdateTarget();
        float distance = Vector2.Distance(transform.position, target.position);
        UpdateBottomLines();
        UpdateState(distance);
    }

    private void UpdateState(float distance)
    {
        if (distance > followRange)
        {
            Idle();
            // Petrol - Idle - Stop Following
        }else if (distance <= followRange)
        {
            if (_state == State.Idle || _state == State.Patrolling)
                if (distance > noticeRange) return;
            Follow(distance);
        }
        else
        {
            Idle();
        }
    }

    private void Idle()
    {
        _state = State.Idle;
    }

    void Follow(float distance)
    {
        // Follow:
        if (distance <= attackRange)
        {
            _state = State.Attacking;
            // Attack
            if (!stopWhenAttacking)
                Chase();
            Attack();
        }
        else
        {
            _state = State.Following;
            Chase();
        }
    }

    void Chase()
    {
        //TODO: Chase
        Vector2 dir = (target.transform.position - transform.position).normalized;
        if (CaneMove(dir))
        {
            rb.velocity = new Vector2(dir.x * speed, rb.velocity.y);
        }
        else
        {
            rb.velocity =new Vector2(0, rb.velocity.y);
        }
    }

    void Attack()
    {
        //TODO: Attack
        switch (type)
        {
            case EnemyType.Explode:
                Explode();
                break;
            case EnemyType.Shooter:
                Shoot();
                break;
        }
    }

    private void Shoot()
    {
        if (Time.time - _lastShot < timeBetweenShots)return;
        _lastShot = Time.time;
        Vector3 pos = transform.position;
        Vector3 dir = (pos - target.position).normalized;
        var point = dir.x > 0 ? shootingPointRight :
        shootingPointLeft;
        Projectile proj = Instantiate(projectile, point.position, point.rotation);
        proj.Init(-dir, shootingForce, damage);
    }

    void UpdateTarget()
    {
        target = GameManager.Instance.GetPlayer()?.transform;
    }

    bool CaneMove(Vector2 dir)
    {
        if (dir.x == 0) return false;
        
        Vector3 pos1 = dir.x < 0 ? botLeft : botRight;
        Vector3 pos2 = pos1;
        pos2.y -= maxGround;

        RaycastHit2D[] hits = new RaycastHit2D[5];
        int hitsCount = Physics2D.LinecastNonAlloc(pos1, pos2, hits);

        if (hitsCount >= 1) return true;
        return false;
    }
    
    void Explode(bool isDeath = false)
    {
        // Spawning the explosion particles:
        Collider2D[] results = new Collider2D[30];
        int hits = Physics2D.OverlapCircleNonAlloc(transform.position, explosionRange, results);
        
        for (int i = 0; i < hits; i++)
        {
            PlayerController controller = results[i].GetComponent<PlayerController>();
            if (controller)
            {
                if (!controller.IsDashing()) results[i].GetComponent<Actor>().TakeDamage(damage);
                else if(!isDeath) GameManager.Instance.GetPlayer().GainWeight(0.1f);
            }
        }
        SoundManager.Instance.Play("explo", _audio);
        Instantiate(explosion, transform.position, Quaternion.identity);
        mySpawner?.EnemyKilled();
        Destroy(gameObject);
    }
    
    private void OnDrawGizmos()
    {
        var position = transform.position;
        
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(position, noticeRange);
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(position, followRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(position, attackRange);
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(position, explosionRange);
        
        Gizmos.color = Color.red;
        Vector3 downR = botRight;
        downR.y -= maxGround;
        Gizmos.DrawLine(botRight + new Vector3(0, 0.1f, 0), downR);
        
        Vector3 downL = botLeft;
        downL.y -= maxGround;
        Gizmos.DrawLine(botLeft + new Vector3(0, 0.1f, 0), downL);
    }
    
    private void OnTriggerEnter2D(Collider2D col)
    {       
        if (col.CompareTag("Player"))
        {
            PlayerController controller = col.GetComponent<PlayerController>();
            if (controller)
            {
                if (!controller.IsDashing()) col.GetComponent<Actor>().TakeDamage(damage);
                else
                {
                    Die();
                }
            }
        }
    }

    protected override void Die()
    {
        if (dead) return;
        GameManager.Instance.GetPlayer().GainWeight(0.1f);
        Explode(true);
    }
}
