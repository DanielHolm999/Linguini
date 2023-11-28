using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public enum BattleState { START, PLAYERTURN, ENEMYTURN, WON, LOST}
public class BattleSystem : MonoBehaviour
{
    // Start is called before the first frame update

    public GameObject playerPrefab;
    public GameObject enemyPrefab;
    public BattleState state;
    public TextMeshProUGUI dialogueText;
    public GameObject dialoguePanel;
    public GameObject BattleHUD;

    public Transform playerBattleStation;
    public Transform enemyBattleStation;

    PlayerUnit playerUnit;
    EnemyUnit enemyUnit;
    void Start()
    {
        state = BattleState.START;
        BattleHUD.SetActive(false);
        StartCoroutine(SetupBattle());
        
    }

    IEnumerator SetupBattle() 
    {
        GameObject PlayerGO = Instantiate(playerPrefab, playerBattleStation);
        playerUnit = PlayerGO.GetComponent<PlayerUnit>();

        GameObject EnemyGO = Instantiate(enemyPrefab, enemyBattleStation);
        enemyUnit = EnemyGO.GetComponent<EnemyUnit>(); 

        dialogueText.text = playerUnit.unitName + " is facing " + enemyUnit.unitName + " in combat";

        yield return new WaitForSeconds(2f);

        dialoguePanel.SetActive(false);
        state = BattleState.PLAYERTURN;
        playerTurn();

    }

    IEnumerator EndBattle()
    {
        if (state == BattleState.WON)
        {
            dialogueText.text = playerUnit.unitName + " won!";

        }
        else if (state == BattleState.LOST)
        {
            dialogueText.text = playerUnit.unitName + " lost!";

        }
        dialoguePanel.SetActive(true);
        yield return new WaitForSeconds(2f);
        //end scene
    }

    void playerTurn()
    {
        if (state != BattleState.PLAYERTURN)
            return;
        
        BattleHUD.SetActive(true);
    }

    IEnumerator EnemyTurn()
    {
        yield return new WaitForSeconds(1f);
        playerUnit.canBlock = true;
        // RUN ENEMY ATTACK COROUTINE WITH REFERENCE TO PLAYER UNIT
        yield return enemyUnit.AttackPlayer(playerUnit, dialogueText, dialoguePanel);
        // AFTERWARDS CHECK IF PLAYER CURRENT HP IS 0

        Debug.Log("checking isDead");
        playerUnit.canBlock = false;
        bool isDead = playerUnit.isDead();

        if (isDead)
        {
            state = BattleState.LOST;
            StartCoroutine(EndBattle());
        }
        else
        {
            state = BattleState.PLAYERTURN;
            Debug.Log("Now player turn from enemy turn");
            playerTurn();
        }
        yield return new WaitForSeconds(1f);
        dialoguePanel.SetActive(false);
        

    }

    IEnumerator playerAttack()
    {
        //damage
        bool isDead = enemyUnit.takeDamage(playerUnit.damage);
        yield return new WaitForSeconds(2f);
        // check if dead
        // change state based of if dead
        if (isDead)
        {
            state = BattleState.WON;
            StartCoroutine(EndBattle());
        }
        else
        {
            state = BattleState.ENEMYTURN;
            StartCoroutine(EnemyTurn());
        }

    }

    public void onAttackButton()
    {
        if (state != BattleState.PLAYERTURN)
            return;
        
        BattleHUD.SetActive(false);
        StartCoroutine(playerAttack());
    }
}
