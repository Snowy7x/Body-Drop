using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TarodevController;
using UnityEngine;

public class Player : Actor
{
    [SerializeField] private GameObject drop;
    [SerializeField] private PlayerController controller;
    [SerializeField] private float timeForEachDrop;
    [SerializeField] private float weightForEachDrop = 0.1f;

    [SerializeField] private GameObject dieParticles;

    [SerializeField] private ParticleSystem weightPs;
    
    private float _lastScale = 0;
    private float _timer;

    private void Start()
    {
        if (!controller)
            controller = GetComponent<PlayerController>();
        _lastScale = transform.localScale.x;
    }

    private void Update()
    {
        if (transform.position.y < -46)
        {
            Die();
        }
        /*if (controller.IsGrounded() && Mathf.Abs(controller.Velocity.x) >= 0.05f)
        {
            _timer += Time.deltaTime;
        }

        if (_timer >= timeForEachDrop)
        {
            _timer = 0;
            LoseWeight();
        }*/
    }

    public void LoseWeight()
    {
        if (_lastScale <= 0.2f)
        {
            Die();
        }
        Instantiate(drop, transform.position, Quaternion.identity);
        _lastScale -= weightForEachDrop;
        _lastScale = Mathf.Abs(_lastScale);
        ChangeScale();
    }

    public void GainWeight(float weight)
    {
        ParticleSystem ps = Instantiate(weightPs, transform.position, Quaternion.identity);
        ps.Play();
        _lastScale += weight;
        ChangeScale();
    }

    void ChangeScale()
    {
        transform.DOScale(_lastScale * 1.5f, 0.3f).onComplete += () =>
        {
            transform.DOScale(_lastScale, 0.2f);
        };
        transform.localScale = new Vector3(_lastScale, _lastScale, _lastScale);
    }

    protected override void Die()
    {
        Instantiate(dieParticles, transform.position, Quaternion.identity);
        Destroy(gameObject);
        GameManager.Instance.Restart();
    }
}
