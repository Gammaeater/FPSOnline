using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Target : MonoBehaviour ,ITakeDamage

{
    [SerializeField]
    private float health = 50f;

    public void ITakeDamage(float amount)
    {
        health -= amount;
        if(health <= 0)
        {
            Die();
        }
    }
    void Die()
    {
        Destroy(gameObject);
    }
}
