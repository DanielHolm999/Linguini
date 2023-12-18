using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;


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
    public GameObject BubblePrefabBad;
    private AudioSource audioSource;
    public AudioClip bubbleSoundGood;
    public AudioClip bubbleSoundBad;

    public AudioClip waterSound;
    public AudioClip tsunamiSound; 
    private SpriteRenderer FirstStack;
    private SpriteRenderer SecondStack;

    private SpriteRenderer ThirdStack;


    enum BubbleType {Good, Bad}

    void Start()
    {
        audioSource = GetComponent<AudioSource>();

        // get stack sprites and hide them - they are children components of the aquatoad object
        var stackSprites = GetComponentsInChildren<SpriteRenderer>();
        FirstStack = stackSprites[1];
        SecondStack = stackSprites[2];
        ThirdStack = stackSprites[3];
        FirstStack.enabled = false;
        SecondStack.enabled = false;
        ThirdStack.enabled = false;
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
            yield return StartCoroutine(WaterAttack(system));
        }
        else
        {
            yield return StartCoroutine(BubbleAttack(system));
        }
    }
    private IEnumerator WaterAttack(BattleSystem system) // normal move
    {
        Debug.Log("USING WATER ATTACK");
        system.dialogueBoxText.text = system.enemyUnit.unitName + " prepares a ranged attack!";

        yield return new WaitForSeconds(1f); // Time before shooting

        // Instantiate and set up projectile
        float projectileHeight = (Random.Range(0, 2)*1.1f)-0.7f;
        var projectilePosition = system.enemyUnit.transform.position;
        Vector2 projectileStartVector = new(system.enemyUnit.transform.position.x, projectileHeight);

        audioSource.PlayOneShot(waterSound);
        yield return new WaitForSeconds(0.8f);
        GameObject projectileObject = Instantiate(WaterPrefab, projectileStartVector, Quaternion.identity);


        Debug.Log("Projectile instantiated at position: " + system.enemyUnit.transform.position);

        Projectile projectileScript = projectileObject.GetComponent<Projectile>();
        projectileScript.shooter = Projectile.Shooter.Enemy;

        if (projectileScript != null)
        {
            float projectileSpeed = UnityEngine.Random.Range(5f, 10f); //Give a bigger speed range

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

        // get bubble list
        var bubbleList = CreateBubbleAmount(system.enemyUnit);
        // itterate over bubblie list
        //      shoot bubble with same speed
        //      subscribe to onhit event, if good -> remove stack, if bad -> add stack
        foreach(BubbleType bubble in bubbleList)
        {
            Debug.Log("Shooting bubble");
            ShootBubble(bubble, system);
            yield return new WaitForSeconds(1f);
        }
        // wait untill done

    }

    private IEnumerator TsunamiAttack(BattleSystem system) // only usable when full stacks
    {
        Debug.Log("USING TSUNAMI ATTACK");

        //reset stacks
        stacks = 0;
        UpdateStackRender();
        yield return new WaitForSeconds(2f);
        var projectilePosition = system.enemyUnit.transform.position;
        Vector2 projectileStartVector = new(system.enemyUnit.transform.position.x, 1);

        // Instantiate and set up projectile
        GameObject projectileObject = Instantiate(TsunamiPrefab, projectileStartVector, Quaternion.identity);
        audioSource.PlayOneShot(tsunamiSound);


        Debug.Log("Projectile instantiated at position: " + system.enemyUnit.transform.position);

        Projectile projectileScript = projectileObject.GetComponent<Projectile>();
        projectileScript.shooter = Projectile.Shooter.Enemy;

        if (projectileScript != null)
        {
            float projectileSpeed = 6f;

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

    private BubbleType[] CreateBubbleAmount(Unit enemy)
    {
        // CREATE BUBBLE ARRAY RANDOMLY BASED FROM HP
        // 100% -> 3 bubbles
        // 75% -> 4 bubbles
        // 50% -> 5 bubbles
        // 25% -> 6 bubbles
        int bubbleAmount = 3;
        float percentOfHp = ((float)enemy.currentHp/(float)enemy.maxHp)*100;
        Debug.Log("current enemy hp is: " + enemy.currentHp);
        Debug.Log("max enemy hp is: " + enemy.maxHp);
        Debug.Log("Percent of hp is: " + percentOfHp.ToString());
        if (percentOfHp > 75)
            bubbleAmount = 3;
        else if (percentOfHp > 50)
            bubbleAmount = 4;
        else if (percentOfHp > 25)
            bubbleAmount = 5;
        else
            bubbleAmount = 6;

        BubbleType[] bubbleArr = new BubbleType[bubbleAmount];
        for(int i = 0; i < bubbleAmount; i++)
        {
            bubbleArr[i] = BubbleType.Bad;
        }
        int randomIndex = Random.Range(0, bubbleAmount);
        bubbleArr[randomIndex] = BubbleType.Good;
        return bubbleArr;        
    }

    private void ShootBubble(BubbleType bubble, BattleSystem system)
    {
        //height is either high or low
        float projectileHeight = (Random.Range(0, 2)*0.8f)-0.4f;
        //Debug.Log("PROJECTILE HEIGHT: " + projectileHeight.ToString());

        var projectilePosition = system.enemyUnit.transform.position;
        Vector2 projectileStartPosition = new(system.enemyUnit.transform.position.x, projectileHeight);
        Debug.Log("CURRENT HEIGHT OF PROJETILE: " + projectileStartPosition.y);

        GameObject projectileObject;
        if(bubble == BubbleType.Good)
        {
            projectileObject = Instantiate(BubblePrefabGood, projectileStartPosition, Quaternion.identity);
            audioSource.PlayOneShot(bubbleSoundGood);

        }
        else 
        {
            projectileObject = Instantiate(BubblePrefabBad, projectileStartPosition, Quaternion.identity);
            audioSource.PlayOneShot(bubbleSoundBad);

        }


        Debug.Log("Projectile instantiated at position: " + system.enemyUnit.transform.position);

        Projectile projectileScript = projectileObject.GetComponent<Projectile>();
        projectileScript.shooter = Projectile.Shooter.Enemy;

        if (projectileScript != null)
        {
            float projectileSpeed = 6;

            Debug.Log("Projectilespeed is " + projectileSpeed);

            projectileScript.SetSpeed(projectileSpeed);

            Vector3 directionToPlayer = system.playerUnit.transform.position - system.enemyUnit.transform.position;



            Vector2 horizontalDirection = new Vector2(directionToPlayer.x, 0).normalized;

            projectileScript.direction = horizontalDirection;

            Debug.Log("Projectile direction set towards: " + system.playerUnit.transform.position);

            // Set up a completion source
            TaskCompletionSource<bool> attackCompletion = new TaskCompletionSource<bool>();
            projectileScript.OnHit += () => 
            { 
                Debug.Log("Projectile hit the player"); 
                StackHandler(bubble);
                Debug.Log("Stacks is: " + stacks.ToString());
                attackCompletion.TrySetResult(true); 
            };
            projectileScript.OnLifetimeEnd += () => { Debug.Log("Projectile lifetime ended"); attackCompletion.TrySetResult(false); };
        }
        else
        {
            Debug.LogError("Projectile script not found on the instantiated object");
        }

        // Update HUD after attack
        system.playerHUD.SetHP(system.playerUnit.currentHp);
        Debug.Log("HUD updated after attack");
    }
    
    private void StackHandler(BubbleType bubble)
    {
        if (bubble == BubbleType.Good)
        {
            if(stacks >= 0)
                --stacks;
        }
        else
        {
            if(stacks < 3)
                ++stacks;
        }
        UpdateStackRender();
    }

    private void UpdateStackRender()
    {
        if(stacks == 1)
        {
            FirstStack.enabled = true;
            SecondStack.enabled = false;
            ThirdStack.enabled = false;
        }
        else if(stacks == 2)
        {
            FirstStack.enabled = false;
            SecondStack.enabled = true;
            ThirdStack.enabled = false;
        }
        else if(stacks == 3)
        {
            FirstStack.enabled = false;
            SecondStack.enabled = false;
            ThirdStack.enabled = true;
        }
        else 
        {
            FirstStack.enabled = false;
            SecondStack.enabled = false;
            ThirdStack.enabled = false;
        }
    }
}
