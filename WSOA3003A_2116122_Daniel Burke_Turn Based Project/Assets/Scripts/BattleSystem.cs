using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public enum BattleState {  START, PLAYERTURN, ENEMYTURN, WON, LOST}

public class BattleSystem : MonoBehaviour
{
    public GameObject playerPrefab, enemyPrefab, deadPrefab;

    public Transform playerBattleStation, enemyBattleStation;

    public Text dialogueText;

    public BattleHUD playerHUD;

    public BattleHUD enemyHUD;
    public UnityEngine.Experimental.Rendering.Universal.Light2D SUN;

    Unit playerUnit, enemyUnit;
    public Text healText;

    GameObject enemyGo;
    
    GameObject playerGO;
    
    
    string pickAttack;
    
    public BattleState state;


    void Start()
    {
        
        state = BattleState.START;
        StartCoroutine( SetupBattle());

        SUN = GetComponent<UnityEngine.Experimental.Rendering.Universal.Light2D>();
    }

    IEnumerator SetupBattle()
    {

        SUN.intensity = 0.2f;
        playerGO = Instantiate(playerPrefab, playerBattleStation);
       playerUnit = playerGO.GetComponent<Unit>();
        healText.text = playerUnit.currHealCharge + "/" + playerUnit.healChargeMax;
        enemyGo = Instantiate(enemyPrefab, enemyBattleStation);
        enemyUnit = enemyGo.GetComponent<Unit>();

        dialogueText.text = "A " + enemyUnit.unitName + " appears!";

        playerHUD.SetHUD(playerUnit);

        enemyHUD.SetHUD(enemyUnit);
        yield return new WaitForSeconds(2f);

        state = BattleState.PLAYERTURN;
        PlayerTurn();
    }

    IEnumerator PlayerAttack()
    {
        AttackString();
        int critDamage = playerUnit.damage * 2;
        float randCrit = Random.Range(0, 100);
        if(randCrit > 90)
            
        {
            Debug.Log(randCrit);
            dialogueText.text = "A decisive blow on " + enemyUnit.unitName + "!";

            bool isDead = enemyUnit.TakeDamage(critDamage);

            
            enemyHUD.SetHP(enemyUnit.currHP);

            yield return new WaitForSeconds(1f);
            if (isDead)
            {
                state = BattleState.WON;
                EndBattle();
                enemyGo.transform.Rotate(Vector3.forward * -90f);
                //enemyGo.SetActive(false);
                playerUnit.currHealCharge = playerUnit.currHealCharge + 1;

            }
            else
            {
                state = BattleState.ENEMYTURN;
                StartCoroutine(EnemyTurn());
            }
        }
        else
        {
            dialogueText.text = pickAttack;
            bool isDead = enemyUnit.TakeDamage(playerUnit.damage);

            
            enemyHUD.SetHP(enemyUnit.currHP);

            yield return new WaitForSeconds(1f);
            if (isDead)
            {
                state = BattleState.WON;
                EndBattle();
                enemyGo.transform.Rotate(Vector3.forward * -90f);
                //enemyGo.SetActive(false);
                playerUnit.currHealCharge = playerUnit.currHealCharge + 1;

            }
            else
            {
                state = BattleState.ENEMYTURN;
                StartCoroutine(EnemyTurn());
            }
        }

    }

    void PlayerTurn()
    {

        dialogueText.text = "What will you do?";
        
    }

    public void OnAttackButton()
    {
        if (state != BattleState.PLAYERTURN)
            return;

        StartCoroutine(PlayerAttack());
    }

    IEnumerator PlayerHeal()
    {
        if(playerUnit.currHealCharge > 0)
        {
            playerUnit.Heal();

            playerHUD.SetHP(playerUnit.currHP);
            dialogueText.text = "You slurp the good stuff and feel great!";
            yield return new WaitForSeconds(1f);

            state = BattleState.ENEMYTURN;
            StartCoroutine(EnemyTurn());

            playerHUD.SetHUD(playerUnit);
            healText.text = playerUnit.currHealCharge + "/" + playerUnit.healChargeMax;
        }
        else
        {
            dialogueText.text = "No more potions left!";
            yield return new WaitForSeconds(1f);
            dialogueText.text = "What will you do?";
        }
        
    }
    public void OnHealButton()
    {
        if (state != BattleState.PLAYERTURN)
            return;
       
            StartCoroutine(PlayerHeal());
        
        
    }

    IEnumerator EnemyTurn()
    {
        
        dialogueText.text = "The "  + enemyUnit.unitName + " attacks!";
        yield return new WaitForSeconds(1f);

        bool isDead = playerUnit.TakeDamage(enemyUnit.damage);

        playerHUD.SetHP(playerUnit.currHP);

        yield return new WaitForSeconds(1f);

        if (isDead)
        {
            state = BattleState.LOST;
            EndBattle();
            playerGO.transform.Rotate(Vector3.forward * 90f);
            //playerGO.SetActive(false);
            
        }
        else
        {
            state = BattleState.PLAYERTURN;
            PlayerTurn();
        }
    }

    void EndBattle()
    {
        if (state == BattleState.WON)
        {
            dialogueText.text = "Another foe vanquished!";
        }else if(state == BattleState.LOST)
        {
            dialogueText.text = "Another foolish adventurer claimed by the dungeon!";
        }
    }

    void AttackString()
    {
        string[] attacks = new string[] { "You stab the " + enemyUnit.unitName + " in the eye!", "You swing your axe at the " + enemyUnit.unitName + "!", "You dismember the " + enemyUnit.unitName + "!", "En garde you" + enemyUnit.unitName + " swine!", "The" + enemyUnit.unitName + " is hurt by your scathing words!", "THWACK!" };
            System.Random random = new System.Random();
            int useAttack = random.Next(attacks.Length);
            pickAttack = attacks[useAttack];
            
        
    }
}
