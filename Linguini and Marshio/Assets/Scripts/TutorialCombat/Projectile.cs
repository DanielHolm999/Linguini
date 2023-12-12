using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed = 5f;
    public Vector2 direction;
    public float lifetime = 5f; // Lifetime of the projectile
    public int damage = 10;

    public event Action OnHit; // Event triggered on hit
    public event Action OnLifetimeEnd; // Event triggered when lifetime ends

    public enum Shooter { Player, Enemy }
    public Shooter shooter;

    private void Start()
    {
        StartCoroutine(LifetimeCountdown());
    }

    void Update()
    {
        transform.Translate(direction.normalized * speed * Time.deltaTime);
    }

    private IEnumerator LifetimeCountdown()
    {
        yield return new WaitForSeconds(lifetime);
        OnLifetimeEnd?.Invoke();
        Destroy(gameObject);
    }

    public void SetSpeed(float newSpeed)
    {
        speed = newSpeed;
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        if ((shooter == Shooter.Player && collider.gameObject.CompareTag("Enemy")) ||
            (shooter == Shooter.Enemy && collider.gameObject.CompareTag("Player")))
        {
            OnHit?.Invoke();
            Unit playerUnit = collider.gameObject.GetComponent<Unit>();
            if (playerUnit != null)
            {
                playerUnit.TakeDamage(damage);
            }
            Destroy(gameObject);
        }
    }
}
