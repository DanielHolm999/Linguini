using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Threading.Tasks;
using System.Resources;
using UnityEngine.SceneManagement;

public class CryoshellBattle : MonoBehaviour
{
    private string sceneToLoadOnWin = "MainWorld";
    public BattleState state;
    public GameObject playerPrefab;
    public GameObject enemyPrefab;

    public Transform playerBattleStation;
    public Transform enemyBattleStation;


    Unit playerUnit;
    CryoshellUnit enemyUnit;

    public BattleHUD playerHUD;
    public BattleHUD enemyHUD;

    public TextMeshProUGUI dialogueBoxText;

    public SpriteRenderer attackSprite; //Lugini in attack
    [SerializeField] private SpriteRenderer enemyAttackSprite;

    public SpriteRenderer deadSprite;

    public AudioSource BGMAudioSource;

    private AudioSource audioSource;
    public AudioClip playerAttackingSFX;
    public AudioClip enemyAttackingSFX;
    public AudioClip winningSFX;
    public AudioClip losingSFX;

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
        enemyUnit = enemyGO.GetComponent<CryoshellUnit>();

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

        dialogueBoxText.text = "What an excrutiating cold... A chilling" + enemyUnit.unitName + " approaches!";
        yield return new WaitForSeconds(3f);

        dialogueBoxText.text = "It looks dangerous, you better be careful!";
        yield return new WaitForSeconds(3f);

        state = BattleState.PLAYERTURN;
        PlayerTurn();
        Debug.Log("Start up finished, state is: " + state);

    }

    IEnumerator EndBattle()
    {
        if (state == BattleState.WON)
        {
            StatsController.SkillPoints += 1;
            BGMAudioSource.Stop();
            audioSource.PlayOneShot(winningSFX);
            dialogueBoxText.text = "Yahoo!, Lugini defeated the " + enemyUnit.unitName;
            yield return new WaitForSeconds(4f);
            dialogueBoxText.text = "Lugini feels his strength growing";
            yield return new WaitForSeconds(4f);
            dialogueBoxText.text = "Lugini realizes he still has a lot of training to do";
            yield return new WaitForSeconds(4f);
            dialogueBoxText.text = "This world is not like the one Lugini knew";
            yield return new WaitForSeconds(4f);
            dialogueBoxText.text = "he must become stronger";
            SceneManager.LoadScene(sceneToLoadOnWin);
        }
        else if (state == BattleState.LOST)
        {
            BGMAudioSource.Stop();
            audioSource.PlayOneShot(losingSFX);
            PlayerDeathAnimation();
            yield return new WaitForSeconds(2f);
            dialogueBoxText.text = enemyUnit.unitName + " was too formiddable for Lugini...";
            yield return new WaitForSeconds(4f);
            dialogueBoxText.text = enemyUnit.unitName + "'s shield glowing with each block";
            yield return new WaitForSeconds(4f);
            dialogueBoxText.text = "seemed to make him stronger...";
            yield return new WaitForSeconds(4f);
            dialogueBoxText.text = enemyUnit.unitName + " also melee attacked more often..";
            yield return new WaitForSeconds(4f);
            dialogueBoxText.text = "in proportion to how many projectiles he blocked..";
            yield return new WaitForSeconds(4f);
            dialogueBoxText.text = "Perhaps there is an element of risk/reward";
            yield return new WaitForSeconds(4f);
            dialogueBoxText.text = "Lets try again";
            yield return new WaitForSeconds(4f);
            Scene currentScene = SceneManager.GetActiveScene();
            SceneManager.LoadScene(currentScene.name);
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

    IEnumerator EnemyDeathAnimation(Transform transform, Vector3 target, float speed)
    {
        float duration = 3f;
        float elapsedTime = 0;
        SpriteRenderer enemySprite = transform.GetComponent<SpriteRenderer>();
        Color originalColor = enemySprite.color;
        Color transparentColor = new Color(originalColor.r, originalColor.g, originalColor.b, 0);
        while (elapsedTime < duration)
        {
            // Move the enemy towards the target
            transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);

            // Fade out the enemy gradually
            enemySprite.color = Color.Lerp(originalColor, transparentColor, elapsedTime / duration);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Make sure the enemy is fully transparent and has reached the target position
        transform.position = target;
        enemySprite.color = transparentColor;

        // Optional: Destroy the enemy GameObject or disable it
        // Destroy(enemyTransform.gameObject);
    }
    void PlayerDeathAnimation()
    {
        playerUnit.gameObject.SetActive(false);
        deadSprite.gameObject.SetActive(true);
    }

    #endregion


    #region PlayerStuff
    void PlayerTurn()
    {
        Crouch(false);
        dialogueBoxText.text = "Choose an action: ";
        
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
        StartCoroutine(SpecialAttackRoutine());
    }

    private IEnumerator SpecialAttackRoutine()
    {
        // Hide the normal sprite and show the first animation sprite
        playerUnit.gameObject.SetActive(false);
        animationSprite1.SetActive(true);

        yield return new WaitForSeconds(1f); // Wait for 1 second

        // Switch to the next animation sprite
        animationSprite1.SetActive(false);
        animationSprite2.SetActive(true);

        yield return new WaitForSeconds(1f); // Wait for another second

        enemyUnit.TryActivateShield();

        // Instantiate and fire the projectile
        GameObject projectileObject = Instantiate(luginiProjectilePrefab, playerUnit.transform.position, Quaternion.identity);
        Projectile projectileScript = projectileObject.GetComponent<Projectile>();

        // Set the shooter as Player
        projectileScript.shooter = Projectile.Shooter.Player;

        // Randomize projectile speed
        float projectileSpeed = Random.Range(15f, 23f);
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
        state = BattleState.WON;
        StartCoroutine(EndBattle());
    }

    IEnumerator EnemyProjectileAttack(int verticalOffsetType)
    {
        Vector3 spawnPosition = enemyUnit.transform.position;
        if (verticalOffsetType == 1) //high
        {
            Debug.Log("High ball");
            float verticalOffset = 1.5f; //modify
            spawnPosition += new Vector3(0, verticalOffset, 0);
        }
        else
        {
            spawnPosition += new Vector3(0, -0.5f, 0);
        }
        // Update text
        dialogueBoxText.text = enemyUnit.unitName + " prepares a ranged attack!";
        Debug.Log("Preparing ranged attack");
        yield return new WaitForSeconds(2.5f); // Time before shooting

        // Instantiate and set up projectile
        GameObject projectileObject = Instantiate(projectilePrefab, spawnPosition, Quaternion.identity);
        Debug.Log("Projectile instantiated at position: " + enemyUnit.transform.position);

        Projectile projectileScript = projectileObject.GetComponent<Projectile>();
        projectileScript.shooter = Projectile.Shooter.Enemy;


        if (projectileScript != null)
        {
            float projectileSpeed = Random.Range(13f, 18f);
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
    }

    IEnumerator EnemyNormalAttack()
    {
        dialogueBoxText.text = enemyUnit.unitName + " is looking menacingly ";
        yield return new WaitForSeconds(3f);

        Vector3 originalPosition = enemyUnit.transform.position;
        Vector3 attackPosition = originalPosition + new Vector3(-0.5f, 0, 0);

        dialogueBoxText.text = enemyUnit.unitName + " launches an attack!";

        yield return StartCoroutine(MoveTowards(enemyUnit.transform, attackPosition, 0.8f));
        yield return StartCoroutine(PerformEnemyAttackAnimation());
        yield return StartCoroutine(MoveTowards(enemyUnit.transform, originalPosition, 0.8f));

        bool isDead = playerUnit.TakeDamage(enemyUnit.damage);
        playerHUD.SetHP(playerUnit.currentHp);

        yield return new WaitForSeconds(2f);
    }

    IEnumerator EnemyTurn()
    {
        var projectileDamage = projectilePrefab.GetComponent<Projectile>().damage;
        var enemyDamage = enemyUnit.damage;
        if (projectileDamage > enemyDamage)
        {
            yield return StartCoroutine(EnemyProjectileAttack(Random.Range(0,2)));
        }
        else if(projectileDamage < enemyDamage)
        {
            if (Random.Range(0, 100) < 75)
            {
                yield return EnemyNormalAttack();

            }
            else
            {
                yield return StartCoroutine(EnemyProjectileAttack(Random.Range(0, 2)));
            }
        }
        CheckPlayerState();
    }

    IEnumerator PerformEnemyAttackAnimation()
    {
        enemyUnit.gameObject.SetActive(false);
        enemyAttackSprite.gameObject.SetActive(true);
        audioSource.PlayOneShot(enemyAttackingSFX);

        yield return new WaitForSeconds(1f);

        enemyUnit.gameObject.SetActive(true);
        enemyAttackSprite.gameObject.SetActive(false);
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

