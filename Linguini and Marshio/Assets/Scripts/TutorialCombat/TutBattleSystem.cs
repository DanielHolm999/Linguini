using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Threading.Tasks;
using UnityEngine.SceneManagement;

public enum BattleState { START, PLAYERTURN, ATTACKINGPHASE, ENEMYTURN, WON, LOST }

public class TutorialBattleSystem : MonoBehaviour
{
    private int tutorialStepCounter;

    public BattleState state;
    public GameObject playerPrefab;
    public GameObject enemyPrefab;

    public Transform playerBattleStation;
    public Transform enemyBattleStation;

    public SceneManager sceneManager;
    Unit playerUnit;
    Unit enemyUnit;

    public BattleHUD playerHUD;
    public BattleHUD enemyHUD;

    public TextMeshProUGUI dialogueBoxText;


    public SpriteRenderer attackSprite; //Lugini in attack
    [SerializeField] private SpriteRenderer enemyAttackSprite;

    private AudioSource audioSource;
    public AudioClip playerAttackingSFX;
    public AudioClip enemyAttackingSFX;
    public AudioClip WinningBattleSFX;

   

    public GameObject projectilePrefab;

    //
    public GameObject animationSprite1;
    public GameObject animationSprite2;
    public GameObject luginiProjectilePrefab;

    public GameObject luginiCrouchedSprite;

    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        state = BattleState.START;
        tutorialStepCounter = 0;
        StartCoroutine(SetupBattle());
    }

    private void Update()
    {
        // Toggle crouch on press and release of 'C' key
        if (Input.GetKeyDown(KeyCode.C) && state == BattleState.ENEMYTURN)
        {
            Crouch(true);
        }
        else if (Input.GetKeyUp(KeyCode.C))
        {
            Crouch(false);
        }
    }

    IEnumerator SetupBattle()
    {
        Debug.Log("Starting up");
        GameObject playerGO = Instantiate(playerPrefab, playerBattleStation);
        playerUnit = playerGO.GetComponent<Unit>();

        GameObject enemyGO = Instantiate(enemyPrefab, enemyBattleStation);
        enemyUnit = enemyGO.GetComponent<Unit>();

        playerHUD.SetHud(playerUnit);
        enemyHUD.SetHud(enemyUnit);


        CrouchedStateScript crouchedController = FindObjectOfType<CrouchedStateScript>();
        if (crouchedController != null)
        {
            crouchedController.playerUnit = playerUnit;
            crouchedController.gameObject.SetActive(false);
        }
        else
        {
            Debug.LogError("CrouchedSpriteController not found in the scene.");
        }

        dialogueBoxText.text = "What is that? A feral " + enemyUnit.unitName + " approaches!";
        yield return new WaitForSeconds(3f);

        dialogueBoxText.text = "It looks dangerous, you better be careful!";
        yield return new WaitForSeconds(3f);

        dialogueBoxText.text = "Try choosing attack and get its HP down!";
        yield return new WaitForSeconds(3f);

        state = BattleState.PLAYERTURN;
        PlayerTurn();
        Debug.Log("Start up finished, state is: " + state);

    }

    IEnumerator EndBattle()
    {
        if (state == BattleState.WON)
        {
            dialogueBoxText.text = "Congratulations, you completed the tutorial.";
            audioSource.PlayOneShot(WinningBattleSFX);
            yield return new WaitForSeconds(4);
            SceneManager.LoadScene("StartMenu");
        }
        else if (state == BattleState.LOST)
        {
            dialogueBoxText.text = enemyUnit.unitName + " devoured Lugini, next up: Princess is going down";
        }
    }

    #region animationstuff
    IEnumerator MoveTowards(Transform transform, Vector3 target, float speed)
    {
        while (transform.position != target)
        {
            transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);
            yield return null;
        }
    }


    IEnumerator MoveToAttackPosition(Vector3 attackPosition)
    {
        yield return StartCoroutine(MoveTowards(playerUnit.transform, attackPosition, 0.8f));
    }

    IEnumerator MoveToOriginalPosition(Vector3 originalPosition)
    {
        yield return StartCoroutine(MoveTowards(playerUnit.transform, originalPosition, 0.8f));
    }

    void SwitchToAttackSprite()
    {
        playerUnit.gameObject.SetActive(false);
        attackSprite.gameObject.SetActive(true);
    }

    void SwitchBackToOriginalSprite()
    {
        playerUnit.gameObject.SetActive(true);
        attackSprite.gameObject.SetActive(false);
    }

    private void Crouch(bool isCrouching)
    {
        // Activate/deactivate the player and crouched sprites based on crouch state
        playerUnit.gameObject.SetActive(!isCrouching);
        luginiCrouchedSprite.SetActive(isCrouching);
    }

    #endregion


    #region PlayerStuff
    void PlayerTurn()
    {
        Crouch(false);
        Debug.Log(tutorialStepCounter);
        //tutorialStepCounter += 1;
        if (tutorialStepCounter >= 2)
        {
            dialogueBoxText.text = "Time to end this fight! Choose skill ";
        }
        else
        {
            dialogueBoxText.text = "Choose attack: ";
        }
    }

    IEnumerator PlayerAttack()
    {
        state = BattleState.ATTACKINGPHASE;

        Vector3 originalPosition = playerUnit.transform.position;
        Vector3 attackPosition = originalPosition + new Vector3(0.5f, 0, 0); // Adjust the distance

        dialogueBoxText.text = "Lugini launches a barearmed attack! ";

        yield return StartCoroutine(MoveToAttackPosition(attackPosition));
        yield return StartCoroutine(PerformAttackAnimation());
        yield return StartCoroutine(MoveToOriginalPosition(originalPosition));

        bool isDead = enemyUnit.TakeDamage(playerUnit.damage);
        state = BattleState.ENEMYTURN;

        UpdateHUDAndCheckBattleState(isDead);
    }



    IEnumerator PerformAttackAnimation()
    {
        yield return new WaitForSeconds(0.5f);

        SwitchToAttackSprite();
        audioSource.PlayOneShot(playerAttackingSFX);

        yield return new WaitForSeconds(1f);

        SwitchBackToOriginalSprite();
    }



    void UpdateHUDAndCheckBattleState(bool isDead)
    {
        enemyHUD.SetHP(enemyUnit.currentHp);
        dialogueBoxText.text = "The attack was successful! ";

        if (isDead)
        {
            state = BattleState.WON;
            StartCoroutine(HandleEnemyDeath());
        }
        else
        {
            state = BattleState.ENEMYTURN;
            StartCoroutine(EnemyTurn());
        }
    }

    public void PerformSpecialAttack()
    {
        if (state != BattleState.PLAYERTURN)
        {
            return;
        }
        StartCoroutine(SpecialAttackRoutine());
    }

    private IEnumerator SpecialAttackRoutine()
    {
        state = BattleState.ATTACKINGPHASE;
        // Hide the normal sprite and show the first animation sprite
        playerUnit.gameObject.SetActive(false);
        animationSprite1.SetActive(true);

        yield return new WaitForSeconds(1f); // Wait for 1 second

        // Switch to the next animation sprite
        animationSprite1.SetActive(false);
        animationSprite2.SetActive(true);

        yield return new WaitForSeconds(1f); // Wait for another second

        // Instantiate and fire the projectile
        GameObject projectileObject = Instantiate(luginiProjectilePrefab, playerUnit.transform.position, Quaternion.identity);
        Projectile projectileScript = projectileObject.GetComponent<Projectile>();

        // Set the shooter as Player
        projectileScript.shooter = Projectile.Shooter.Player;

        // Randomize projectile speed
        float projectileSpeed = Random.Range(8f, 13f);
        Debug.Log("Projectile speed is " + projectileSpeed);
        projectileScript.SetSpeed(projectileSpeed);

        // Set the projectile's direction towards the enemy
        Vector3 directionToEnemy = enemyUnit.transform.position - playerUnit.transform.position;
        Vector2 horizontalDirection = new Vector2(directionToEnemy.x, 0).normalized;
        projectileScript.direction = horizontalDirection;
        Debug.Log("Projectile direction set towards: " + enemyUnit.transform.position);
        TaskCompletionSource<bool> attackCompletion = new TaskCompletionSource<bool>();
        projectileScript.OnHit += () => { Debug.Log("Projectile hit the player"); attackCompletion.TrySetResult(true); };
        projectileScript.OnLifetimeEnd += () => { Debug.Log("Projectile lifetime ended"); attackCompletion.TrySetResult(false); };

        // Wait for either the projectile to hit or its lifetime to end
        yield return new WaitUntil(() => attackCompletion.Task.IsCompleted);
        Debug.Log("Attack completion detected");
        // Return to the normal sprite
        yield return new WaitForSeconds(1f); // Wait a moment before switching back
        animationSprite2.SetActive(false);
        playerUnit.gameObject.SetActive(true);

        bool isDead = enemyUnit.currentHp <= 0 ? true : false;
        UpdateHUDAndCheckBattleState(isDead);

    }
    #endregion

    #region EnemyStuff
    IEnumerator HandleEnemyDeath()
    {
        Vector3 originalPos = enemyUnit.transform.position;
        Vector3 targetPos = originalPos + new Vector3(0, -10, 0);
        yield return StartCoroutine(EnemyDeathAnimation(enemyUnit.transform, targetPos, 20f));
        StartCoroutine(EndBattle());
    }

    IEnumerator EnemyDeathAnimation(Transform transform, Vector3 target, float speed)
    {
        while (transform.position != target)
        {
            transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);
            yield return null;
        }
    }

    IEnumerator EnemyProjectileAttack(int verticalOffsetType)
    {
        Vector3 spawnPosition = enemyUnit.transform.position;
        if (verticalOffsetType == 1) //high
        {
            float verticalOffset = 2.0f; //modify
            spawnPosition += new Vector3(0, verticalOffset, 0);
        }
        //Tutorial specific:
        var playerHp = playerUnit.currentHp;
        // Update text
        dialogueBoxText.text = enemyUnit.unitName + " prepares a ranged attack!";
        Debug.Log("Preparing ranged attack");
        yield return new WaitForSeconds(2.5f); // Time before shooting

        audioSource.PlayOneShot(enemyAttackingSFX);
        // Instantiate and set up projectile
        GameObject projectileObject = Instantiate(projectilePrefab, spawnPosition, Quaternion.identity);
        Debug.Log("Projectile instantiated at position: " + enemyUnit.transform.position);

        Projectile projectileScript = projectileObject.GetComponent<Projectile>();
        projectileScript.shooter = Projectile.Shooter.Enemy;


        if (projectileScript != null)
        {
            float projectileSpeed = Random.Range(8f, 13f);
            Debug.Log("Projectilespeed is " + projectileSpeed);
            projectileScript.SetSpeed(projectileSpeed);
            Vector3 directionToPlayer = playerUnit.transform.position - enemyUnit.transform.position;
            Vector2 horizontalDirection = new Vector2(directionToPlayer.x, 0).normalized;
            projectileScript.direction = horizontalDirection;
            Debug.Log("Projectile direction set towards: " + playerUnit.transform.position);

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
        playerHUD.SetHP(playerUnit.currentHp);
        Debug.Log("HUD updated after attack");
        yield return new WaitForSeconds(2f); // Post-attack pause

        //tutorial specific:
        if (playerUnit.currentHp < playerHp)
        {
            if (tutorialStepCounter > 0)
            {
                tutorialStepCounter -= 1;
            }

            dialogueBoxText.text = "You took a big hit!";
            yield return new WaitForSeconds(2f);
            dialogueBoxText.text = "No worries, in the tutorial your health gets reset!";
            yield return new WaitForSeconds(3f);
            dialogueBoxText.text = "Keep practicing your jumps and ducks!";
            yield return new WaitForSeconds(4f);
            playerUnit.currentHp = playerUnit.maxHp;
            playerHUD.SetHP(playerUnit.maxHp);
        }
        else
        {
            
            dialogueBoxText.text = "Good job! You successfully dodged the attack!";
            yield return new WaitForSeconds(4f);
            if(tutorialStepCounter < 2)
            {
                dialogueBoxText.text = "All monsters shoot at different speeds!";
                yield return new WaitForSeconds(3f);
                dialogueBoxText.text = "Some monsters even shoot at different speeds each time!";
                yield return new WaitForSeconds(3f);
                dialogueBoxText.text = "They can also shoot high or low";
                yield return new WaitForSeconds(3f);
                dialogueBoxText.text = "So always be ready when a monster is preparing an attack!";
                yield return new WaitForSeconds(3f);
                dialogueBoxText.text = "Now lets practice ducking under a high shot!";
                yield return new WaitForSeconds(3f);

            }
            tutorialStepCounter += 1;
        }
    }

    IEnumerator EnemyNormalAttack()
    {
        dialogueBoxText.text = enemyUnit.unitName + " is looking menacingly ";
        yield return new WaitForSeconds(3f);

        Vector3 originalPosition = enemyUnit.transform.position;
        Vector3 attackPosition = originalPosition + new Vector3(-0.5f, 0, 0);

        dialogueBoxText.text = enemyUnit.unitName + " launches an attack!";
        bool isDead = playerUnit.TakeDamage(enemyUnit.damage);

        yield return StartCoroutine(MoveTowards(enemyUnit.transform, attackPosition, 0.8f));
        yield return StartCoroutine(PerformEnemyAttackAnimation());
        yield return StartCoroutine(MoveTowards(enemyUnit.transform, originalPosition, 0.8f));

        playerHUD.SetHP(playerUnit.currentHp);

        yield return new WaitForSeconds(2f);
    }

    IEnumerator EnemyTurn()
    {
        dialogueBoxText.text = "Ratty took some damage! Good job";
        yield return new WaitForSeconds(3f);

        dialogueBoxText.text = "Ratty is about to attack, pay close attention!";
        yield return new WaitForSeconds(3f);
        dialogueBoxText.text = "When it's attack is close to you, hit SPACEBAR to jump over it!";
        yield return new WaitForSeconds(5f);
        yield return StartCoroutine(EnemyProjectileAttack(0));

        dialogueBoxText.text = "Ratty is about to attack, pay close attention!";
        yield return new WaitForSeconds(3f);
        dialogueBoxText.text = "When it's attack is close to you, hold down 'C' to duck under it!";
        yield return new WaitForSeconds(5f);
        yield return StartCoroutine(EnemyProjectileAttack(1));

        CheckPlayerState();
    }

    IEnumerator PerformEnemyAttackAnimation()
    {
        enemyAttackSprite.enabled = true; // Show the enemy attack sprite
        audioSource.PlayOneShot(enemyAttackingSFX);

        yield return new WaitForSeconds(1f);

        enemyAttackSprite.enabled = false; // Hide the enemy attack sprite
    }

    void CheckPlayerState()
    {
        bool isDead = playerUnit.currentHp <= 0;
        if (isDead)
        {
            state = BattleState.LOST;
            StartCoroutine(EndBattle());
        }
        else
        {
            state = BattleState.PLAYERTURN;
            PlayerTurn();
        }
    }

    #endregion


    #region buttons
    public void OnAttackButton()
    {
        Debug.Log("On attack clicked in state: " + state);
        if (state != BattleState.PLAYERTURN)
            return;

        StartCoroutine(PlayerAttack());
    }

    public void OnSkillButton()
    {
        Debug.Log("On skill clicked in state: " + state);

        if (state != BattleState.PLAYERTURN)
        {
            return;
        }
        PerformSpecialAttack();

    }
    #endregion
}
