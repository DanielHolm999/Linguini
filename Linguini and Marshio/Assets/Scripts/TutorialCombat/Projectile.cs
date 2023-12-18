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
        if (shooter == Shooter.Player && collider.gameObject.CompareTag("CryoShellShield"))
        {
            OnHit?.Invoke();
            Destroy(gameObject);
        }
        if (shooter == Shooter.Player && collider.gameObject.CompareTag("Cryoshell"))
        {
            OnHit?.Invoke();
            CryoshellUnit unit = collider.gameObject.GetComponent<CryoshellUnit>();
            unit.TakeDamage(damage);
            Destroy(gameObject);
        }
        //if enemy is hit
        if (shooter == Shooter.Player && collider.gameObject.CompareTag("Enemy"))
        {
            Debug.Log("is enemy");
            OnHit?.Invoke();
            Unit unit = collider.gameObject.GetComponent<Unit>();
            unit.TakeDamage(damage);
            Destroy(gameObject);
        }
        if (shooter == Shooter.Enemy && collider.gameObject.CompareTag("Player"))
        {
            CrouchedStateScript crouchedScript = collider.gameObject.GetComponent<CrouchedStateScript>();
            if (crouchedScript != null)
            {
                Debug.Log("in crouched");
                crouchedScript.TakeDamage(damage);
            }
            else
            {
                // Fallback to Unit script if CrouchedStateScript is not found
                Unit unit = collider.gameObject.GetComponent<Unit>();
                if (unit != null)
                {
                    Debug.Log("in not crouched");
                    unit.TakeDamage(damage);
                }
                else
                {
                    Debug.LogError("Appropriate damage handling script not found on the collided player object.");
                }
            }

            OnHit?.Invoke();
            Destroy(gameObject);
        }
    }
    /*
    void HandleDamage(GameObject target)
    {
        Unit unit = target.GetComponent<Unit>();
        unit.TakeDamage(damage);
    }
    */
}
