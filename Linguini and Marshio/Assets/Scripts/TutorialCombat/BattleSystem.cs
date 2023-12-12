using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public enum BattleState { START, PLAYERTURN, ENEMYTURN, WON, LOST}

public class BattleSystem : MonoBehaviour
{
    public BattleState state;
    public GameObject playerPrefab;
    public GameObject enemyPrefab;

    public Transform playerBattleStation;
    public Transform enemyBattleStation;


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

    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        state = BattleState.START;

        StartCoroutine(SetupBattle());
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

        dialogueBoxText.text = "What is that? A feral " + enemyUnit.unitName + " approaches!";
        yield return new WaitForSeconds(2f);

        state = BattleState.PLAYERTURN;
        PlayerTurn();
        Debug.Log("Start up finished, state is: " + state);

    }

    void EndBattle()
    {
        if (state == BattleState.WON)
        {
            dialogueBoxText.text = "Tonight we are victorious";
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

    #endregion


    #region PlayerStuff
    void PlayerTurn()
    {
        dialogueBoxText.text = "Choose an action: ";
    }

    IEnumerator PlayerAttack()
    {
        Vector3 originalPosition = playerUnit.transform.position;
        Vector3 attackPosition = originalPosition + new Vector3(0.5f, 0, 0); // Adjust the distance

        dialogueBoxText.text = "Lugini launches a barearmed attack! ";
        bool isDead = enemyUnit.TakeDamage(playerUnit.damage);

        yield return StartCoroutine(MoveToAttackPosition(attackPosition));
        yield return StartCoroutine(PerformAttackAnimation());
        yield return StartCoroutine(MoveToOriginalPosition(originalPosition));

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
#endregion

    #region EnemyStuff
    IEnumerator HandleEnemyDeath()
    {
        Vector3 originalPos = enemyUnit.transform.position;
        Vector3 targetPos = originalPos + new Vector3(0, -180, 0);
        yield return StartCoroutine(EnemyDeathAnimation(enemyUnit.transform, targetPos, 2f));
        EndBattle();
    }

    IEnumerator EnemyDeathAnimation(Transform transform, Vector3 target, float speed)
    {
        while (transform.position != target)
        {
            transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);
            yield return null;
        }
    }

    IEnumerator EnemyTurn()
    {
        //update text
        dialogueBoxText.text = enemyUnit.unitName + " is looking menancingly ";
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

        if (isDead)
        {
            state = BattleState.WON;
            EndBattle();
        }
        else
        {
            state = BattleState.PLAYERTURN;
            PlayerTurn();
        }

    }

    IEnumerator PerformEnemyAttackAnimation()
    {
        enemyAttackSprite.enabled = true; // Show the enemy attack sprite
        audioSource.PlayOneShot(enemyAttackingSFX);

        yield return new WaitForSeconds(1f);

        enemyAttackSprite.enabled = false; // Hide the enemy attack sprite
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
    #endregion
}
