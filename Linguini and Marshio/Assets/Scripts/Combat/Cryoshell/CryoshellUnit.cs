using UnityEngine;
using System.Collections;

public class CryoshellUnit : MonoBehaviour
{
    public GameObject shield;
    private SpriteRenderer shieldRenderer; 

    public string unitName;
    public int unitLevel;

    public int damage;

    public int maxHp;
    public int currentHp;


    public AudioClip takeDamageSFX;
    public AudioClip dyingSFX;

    AudioSource audioSource;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        shield = GameObject.FindWithTag("CryoShellShield");
        shield.SetActive(false);
        shieldRenderer = shield.GetComponent<SpriteRenderer>();
    }

    public bool TakeDamage(int damage)
    {
        currentHp -= damage;
        Debug.Log($"{unitName} now has {currentHp} HP.");
        if (currentHp <= 0)
        {
            audioSource.PlayOneShot(dyingSFX);
            return true;
        }
        else
        {
            audioSource.PlayOneShot(takeDamageSFX);
            return false;
        }
    }

    public void TryActivateShield()
    {
        if (Random.Range(0, 100) < 75) // 75% chance
        {
            UpdateShieldColor();
            shield.SetActive(true);
            if (damage < 25)
            {
                damage += 5;
            }

            StartCoroutine(DeactivateShieldAfterDelay(4f)); 
        }
    }

    private void UpdateShieldColor()
    {
        if (shieldRenderer != null)
        {
            if (damage <= 5)
            {
                shieldRenderer.color = Color.white; 
            }
            else if (damage > 6 && damage <= 10)
            {
                shieldRenderer.color = Color.yellow; 
            }
            else
            {
                shieldRenderer.color = Color.red; 
            }
        }
    }

    private IEnumerator DeactivateShieldAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        shield.SetActive(false);
    }
}

