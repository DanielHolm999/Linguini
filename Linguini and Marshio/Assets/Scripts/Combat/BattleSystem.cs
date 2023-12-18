using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Threading.Tasks;
using System.Resources;
using UnityEngine.SceneManagement;



public enum RealBattleState { START, PLAYERTURN, ATTACKINGPHASE, ENEMYTURN, WON, LOST}

public class BattleArgs // is passed to EnemyCombatBehaviour to update combat state
{

    public RealBattleState State {get; set;}
    public Unit PlayerUnit {get; set;}
    public Unit EnemyUnit {get; set;}
    //other stuff
}
public class BattleSystem : MonoBehaviour
{
    private string sceneToLoadOnWin = "MainWorld";

    public RealBattleState state;
    public GameObject playerPrefab;
    public GameObject enemyPrefab;

    public Transform playerBattleStation;
    public Transform enemyBattleStation;
    public SpriteRenderer deadSprite;
    public AudioClip winningSFX;
    public AudioClip losingSFX;
    public AudioSource BGMAudioSource;



    public Unit playerUnit { get; private set;}
    public Unit enemyUnit {get; private set; }
    [SerializeField]
    EnemyCombatBehaviour enemyCombatBehaviour;

    public BattleHUD playerHUD;
    public BattleHUD enemyHUD;

    public TextMeshProUGUI dialogueBoxText;

    
    public SpriteRenderer attackSprite; //Lugini in attack
    [SerializeField] private SpriteRenderer enemyAttackSprite;

    private AudioSource audioSource;
    public AudioClip playerAttackingSFX;
    public AudioClip enemyAttackingSFX;

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
        state = RealBattleState.START;
        

        StartCoroutine(SetupBattle());
    }

    void Update()
    {
        // Toggle crouch on press and release of 'C' key
        if (Input.GetKeyDown(KeyCode.C) && state == RealBattleState.ENEMYTURN)
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
        GameObject playerGO = Instantiate(playerPrefab, playerBattleStation);
        playerUnit = playerGO.GetComponent<Unit>();

        GameObject enemyGO = Instantiate(enemyPrefab, enemyBattleStation);
        enemyUnit = enemyGO.GetComponent<Unit>();
        enemyCombatBehaviour = enemyGO.GetComponent<EnemyCombatBehaviour>();

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

        playerHUD.SetHud(playerUnit);
        enemyHUD.SetHud(enemyUnit);

        dialogueBoxText.text = "The cold is excrutiating, a chilling wind arrives...";
        yield return new WaitForSeconds(3f);

        dialogueBoxText.text = "A " + enemyUnit.unitName + " Is approaching!";
        yield return new WaitForSeconds(3f);

        state = RealBattleState.PLAYERTURN;
        PlayerTurn();
    }

    IEnumerator EndBattle()
    {
        if (state == RealBattleState.WON)
        {
            Debug.Log("in endbattle");
            BGMAudioSource.Stop();
            audioSource.PlayOneShot(winningSFX);
            //Remember sounds
            dialogueBoxText.text = "Tonight we are victorious";
            yield return new WaitForSeconds(2.5f);
            SceneManager.LoadScene(sceneToLoadOnWin);

        }
        else if (state == RealBattleState.LOST)
        {
            //Remember sounds
            BGMAudioSource.Stop();
            audioSource.PlayOneShot(losingSFX);
            PlayerDeathAnimation();
            dialogueBoxText.text = enemyUnit.unitName + " devoured Lugini, next up: Princess is going down";
            yield return new WaitForSeconds(2f);
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

    #endregion


    #region PlayerStuff
    void PlayerTurn()
    {
        dialogueBoxText.text = "Choose an action: ";
    }

    IEnumerator PlayerAttack()
    {
        state = RealBattleState.ATTACKINGPHASE;

        Vector3 originalPosition = playerUnit.transform.position;
        Vector3 attackPosition = originalPosition + new Vector3(0.5f, 0, 0); // Adjust the distance

        dialogueBoxText.text = "Lugini launches a barearmed attack! ";

        yield return StartCoroutine(MoveToAttackPosition(attackPosition));
        yield return StartCoroutine(PerformAttackAnimation());
        yield return StartCoroutine(MoveToOriginalPosition(originalPosition));

        //Insert a random block chance method so enemyUnit takes less damage

        bool isDead = enemyUnit.TakeDamage(playerUnit.damage);
        state = RealBattleState.ENEMYTURN;

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
            Debug.Log("in is dead");
            state = RealBattleState.WON;
            //sounds
            StartCoroutine(HandleEnemyDeath());
        }
        else
        {
            state = RealBattleState.ENEMYTURN;
            //sounds
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

        //check if dead and end turn
        bool isDead = enemyUnit.currentHp <= 0 ? true : false;
        UpdateHUDAndCheckBattleState(isDead);
    }
#endregion

    #region EnemyStuff
    IEnumerator HandleEnemyDeath()
    {
        Debug.Log("In handle death");
        Vector3 originalPos = enemyUnit.transform.position;
        Vector3 targetPos = originalPos + new Vector3(0, -180, 0);
        StartCoroutine(EnemyDeathAnimation(enemyUnit.transform, targetPos, 10f));
        yield return new WaitForSeconds(1f);
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

    IEnumerator EnemyProjectileAttack()
    {
        // Update text
        dialogueBoxText.text = enemyUnit.unitName + " prepares a ranged attack!";

        yield return new WaitForSeconds(2.5f); // Time before shooting

        // Instantiate and set up projectile
        //Remeber to add a random chance to spawn high or low
        GameObject projectileObject = Instantiate(projectilePrefab, enemyUnit.transform.position, Quaternion.identity);

        Debug.Log("Projectile instantiated at position: " + enemyUnit.transform.position);

        Projectile projectileScript = projectileObject.GetComponent<Projectile>();
        projectileScript.shooter = Projectile.Shooter.Enemy;


        if (projectileScript != null)
        {
            float projectileSpeed = Random.Range(5f, 10f); //Give a bigger speed range

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
        bool isDead = playerUnit.TakeDamage(enemyUnit.damage);

        yield return StartCoroutine(MoveTowards(enemyUnit.transform, attackPosition, 0.8f));
        yield return StartCoroutine(PerformEnemyAttackAnimation());
        yield return StartCoroutine(MoveTowards(enemyUnit.transform, originalPosition, 0.8f));

        playerHUD.SetHP(playerUnit.currentHp);

        yield return new WaitForSeconds(2f);
    }

    IEnumerator EnemyTurn()
    {
        //Logic to choose attack

        //yield return StartCoroutine(EnemyProjectileAttack());
        if (enemyCombatBehaviour != null)
        {
            yield return StartCoroutine(enemyCombatBehaviour.EnemyMove(this));
        }
        else 
        {
            yield return StartCoroutine(EnemyProjectileAttack());
        }

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
            state = RealBattleState.LOST;
            StartCoroutine(EndBattle());
        }
        else
        {
            state = RealBattleState.PLAYERTURN;
            PlayerTurn();
        }
    }
    private void Crouch(bool isCrouching)
    {
        // Activate/deactivate the player and crouched sprites based on crouch state
        playerUnit.gameObject.SetActive(!isCrouching);
        luginiCrouchedSprite.SetActive(isCrouching);
    }

    void PlayerDeathAnimation()
    {
        playerUnit.gameObject.SetActive(false);
        deadSprite.gameObject.SetActive(true);
    }


    #endregion


    #region buttons
    public void OnAttackButton()
    {
        Debug.Log("On attack clicked in state: " + state);
        if (state != RealBattleState.PLAYERTURN)
            return;

        StartCoroutine(PlayerAttack());
    }

    public void OnSkillButton()
    {
        Debug.Log("On skill clicked in state: " + state);
        
            if (state != RealBattleState.PLAYERTURN)
            {
                return;
            }
        PerformSpecialAttack();
        
    }
    #endregion
}
