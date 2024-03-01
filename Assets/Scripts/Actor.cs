using UnityEngine;

public abstract class Actor : MonoBehaviour
{
    public float health;
    public float maxHealth;
    protected bool dead;

    public virtual void TakeDamage(float damage)
    {
        if (dead)
        {
            Die();
            return;
        }
        health = Mathf.Max(0, health - damage);
        if (health <= 0) Die();
    }

    protected abstract void Die();

}