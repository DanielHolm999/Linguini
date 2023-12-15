using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using System.Threading.Tasks;
public class AquaToad : EnemyCombatBehaviour
{
    // Start is called before the first frame update

    // counts from 0 to 3
    // 0 = NOTHING, 1 = two orbs, 2 = 4 orbs, 3 = 5 orbs MAX.
    int stacks = 0;
    int turnCounter;
    public GameObject TsunamiPrefab;
    public GameObject WaterPrefab;

    public GameObject BubblePrefabGood;
    public GameObject BubblePrefab;



    void Start()
    {
        
    }

    public override IEnumerator EnemyMove(BattleSystem system)
    {
        //update turn counter
        turnCounter++;
        yield return stacks switch
        {
            3 => StartCoroutine(TsunamiAttack(system)),
            _ => StartCoroutine(AttackFromTurnCounter(system))
        };
    }

    private IEnumerator AttackFromTurnCounter(BattleSystem system)
    {
        if (turnCounter%2 == 0) 
        {
            yield return StartCoroutine(BubbleAttack(system));
        }
        else
        {
            yield return StartCoroutine(WaterAttack(system));
        }
    }
    private IEnumerator WaterAttack(BattleSystem system) // normal move
    {
        Debug.Log("USING WATER ATTACK");
        system.dialogueBoxText.text = system.enemyUnit.unitName + " prepares a ranged attack!";

        yield return new WaitForSeconds(2f); // Time before shooting

        // Instantiate and set up projectile
        //Remeber to add a random chance to spawn high or low
        GameObject projectileObject = Instantiate(WaterPrefab, system.enemyUnit.transform.position, Quaternion.identity);

        Debug.Log("Projectile instantiated at position: " + system.enemyUnit.transform.position);

        Projectile projectileScript = projectileObject.GetComponent<Projectile>();
        projectileScript.shooter = Projectile.Shooter.Enemy;

        if (projectileScript != null)
        {
            float projectileSpeed = Random.Range(5f, 10f); //Give a bigger speed range

            Debug.Log("Projectilespeed is " + projectileSpeed);

            projectileScript.SetSpeed(projectileSpeed);

            Vector3 directionToPlayer = system.playerUnit.transform.position - system.enemyUnit.transform.position;

            Vector2 horizontalDirection = new Vector2(directionToPlayer.x, 0).normalized;

            projectileScript.direction = horizontalDirection;

            Debug.Log("Projectile direction set towards: " + system.playerUnit.transform.position);

            // Set up a completion source
            TaskCompletionSource<bool> attackCompletion = new TaskCompletionSource<bool>();
            projectileScript.OnHit += () => { Debug.Log("Projectile hit the player"); attackCompletion.TrySetResult(true); };
            projectileScript.OnLifetimeEnd += () => { Debug.Log("Projectile lifetime ended"); attackCompletion.TrySetResult(false); };

            // Wait for either the projectile to hit or its lifetime to end
            yield return new WaitUntil(() => attackCompletion.Task.IsCompleted);
            Debug.Log("Attack completion detected");
        }
        else
        {
            Debug.LogError("Projectile script not found on the instantiated object");
        }



        // Update HUD after attack
        system.playerHUD.SetHP(system.playerUnit.currentHp);
        Debug.Log("HUD updated after attack");
        yield return new WaitForSeconds(2f); // Post-attack pause
    }

    private IEnumerator BubbleAttack(BattleSystem system) // updates stacks
    {
        Debug.Log("USING BUBBLE ATTACK");
        yield return new WaitForSeconds(2f);

    }

    private IEnumerator TsunamiAttack(BattleSystem system) // only usable when full stacks
    {
        Debug.Log("USING TSUNAMI ATTACK");

        //reset stacks
        stacks = 0;
        yield return new WaitForSeconds(2f);
    }
}
