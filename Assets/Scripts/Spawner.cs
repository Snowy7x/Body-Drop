using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using TMPro;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    [SerializeField] private EnemyAI enemy;

    [SerializeField] private float playerRangeToSpawn;

    [SerializeField] private int maxEnemiesSpawned;
    [SerializeField] private float timeBetweenSpawn;

    [SerializeField] private GameObject deathParticles;
    [SerializeField] private TMP_Text lifesText;
    private float _lastSpawn;
    private int aliveEnemies;

    private int lifes = 3;

    private void Start()
    {
        lifesText.text = lifes.ToString();
    }

    private void Update()
    {
        if (aliveEnemies >= maxEnemiesSpawned) return;
        // TODO: Spawning...
        try
        {
            if (Time.time - _lastSpawn >= timeBetweenSpawn && Vector3.Distance(transform.position, GameManager.Instance.GetPlayer().transform.position) <= playerRangeToSpawn) SpawnEnemy();
        }
        catch
        {
            // ignored
        }
    }

    private void SpawnEnemy()
    {
        EnemyAI enemyAI = Instantiate(enemy, transform.position, Quaternion.identity);
        enemyAI.SetSpawner(this);
        aliveEnemies++;
        _lastSpawn = Time.time;
    }

    public void EnemyKilled()
    {
        aliveEnemies = Mathf.Max(0, aliveEnemies - 1);
    }

    public void TakeLife()
    {
        lifes--;
        lifesText.text = lifes.ToString();
        if (lifes <= 0) Die();
    }

    private void Die()
    {
        Instantiate(deathParticles, transform.position, Quaternion.identity);
        GameManager.Instance.Broken();
        Destroy(gameObject);
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, playerRangeToSpawn);
    }
}
