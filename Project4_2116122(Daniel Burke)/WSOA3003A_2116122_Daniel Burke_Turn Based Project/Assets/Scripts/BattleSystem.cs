using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public enum BattleState {  START, PLAYERTURN, ENEMYTURN, WON, LOST}

public class BattleSystem : MonoBehaviour
{
    public GameObject battleStage, miniEnemy,  miniPlayer, gameOverPanel;

    public Text gameOver;

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
    
    string pickAttack, pickCrit, pickMiss, pickHeal;
    
    public BattleState state;

    public Button playerHealButton, playerAttackButton, fleeButton, skillButton;

    void Start()
    {
        
        state = BattleState.START;
        StartCoroutine( SetupBattle());

        
    }

    IEnumerator SetupBattle()
    {
        playerAttackButton.interactable = false;
        playerHealButton.interactable = false;
        fleeButton.interactable = false;
        skillButton.interactable = false;
        SUN.intensity = 0.2f;
        playerGO = Instantiate(playerPrefab, playerBattleStation);
       playerUnit = playerGO.GetComponent<Unit>();
        healText.text = "Healing Potions: " + playerUnit.currHealCharge + "/" + playerUnit.healChargeMax;
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
        playerAttackButton.interactable = false;
        playerHealButton.interactable = false;
        fleeButton.interactable = false;
        skillButton.interactable = false;

        
        float randCrit = Random.Range(0, 100);
        if(randCrit > 90)            
        {
            CritString();
            Debug.Log(randCrit);
            dialogueText.text = pickCrit;
           
            bool isDead = enemyUnit.TakeDamage(playerUnit.damage * 2);

            
            enemyHUD.SetHP(enemyUnit.currHP);

            yield return new WaitForSeconds(1f);
            if (isDead)
            {
                enemyGo.transform.Rotate(Vector3.forward * -90f);
                yield return new WaitForSeconds(1f);

                //enemyGo.SetActive(false);
                playerUnit.currHealCharge = playerUnit.currHealCharge + 1;
                state = BattleState.WON;
                EndBattle();

            }
            else
            {
                state = BattleState.ENEMYTURN;
                StartCoroutine(EnemyTurn());
            }
        }
       
       else if (randCrit < 3.6f)

        {
            MissString();
            dialogueText.text = pickMiss;
            bool isDead = enemyUnit.TakeDamage(0);


            enemyHUD.SetHP(enemyUnit.currHP);

            yield return new WaitForSeconds(1f);
            if (isDead)
            {
                enemyGo.transform.Rotate(Vector3.forward * -90f);
                yield return new WaitForSeconds(1f);

                //enemyGo.SetActive(false);
                playerUnit.currHealCharge = playerUnit.currHealCharge + 1;
                state = BattleState.WON;
                EndBattle();

            }
            else
            {
                state = BattleState.ENEMYTURN;
                StartCoroutine(EnemyTurn());
            }
        }
        else
        {
            AttackString();
            dialogueText.text = pickAttack;
            bool isDead = enemyUnit.TakeDamage(playerUnit.damage);


            enemyHUD.SetHP(enemyUnit.currHP);

            yield return new WaitForSeconds(1f);
            if (isDead)
            {
                enemyGo.transform.Rotate(Vector3.forward * -90f);
                yield return new WaitForSeconds(1f);
                
                //enemyGo.SetActive(false);
                playerUnit.currHealCharge = playerUnit.currHealCharge + 1;
                state = BattleState.WON;
                EndBattle();
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
        playerAttackButton.interactable = true;
        playerHealButton.interactable = true;
        fleeButton.interactable = true;
        skillButton.interactable = true;

        dialogueText.text = "What will you do?";

        if(playerUnit.currHealCharge == 0)
        playerHealButton.interactable = false;

    }

    public void OnAttackButton()
    {
        if (state != BattleState.PLAYERTURN)
            return;

        StartCoroutine(PlayerAttack());
    }

    IEnumerator PlayerHeal()
    {
        HealString();
        if (playerUnit.currHealCharge > 0 && playerUnit.currHP < playerUnit.maxHP)
        {
            playerAttackButton.interactable = false;
            playerHealButton.interactable = false;
            fleeButton.interactable = false;
            skillButton.interactable = false;
            playerUnit.Heal();
            
            playerHUD.SetHP(playerUnit.currHP);
            dialogueText.text = pickHeal;
            yield return new WaitForSeconds(2f);

            state = BattleState.ENEMYTURN;
            StartCoroutine(EnemyTurn());

            playerHUD.SetHUD(playerUnit);
            healText.text = "Healing Potions: " + playerUnit.currHealCharge + "/" + playerUnit.healChargeMax;
        }else if(playerUnit.currHealCharge > 0 && playerUnit.currHP == playerUnit.maxHP)
        {
            dialogueText.text = "You're already at full health";
            yield return new WaitForSeconds(2f);
            dialogueText.text = "What will you do?";
        }
        else
        {
            dialogueText.text = "No more potions left!";
            yield return new WaitForSeconds(2f);
            dialogueText.text = "What will you do?";
            playerAttackButton.interactable = true;
            playerHealButton.interactable = false;
            fleeButton.interactable = true;
            skillButton.interactable = true;
        }
        
    }
    public void OnHealButton()
    {
        if (state != BattleState.PLAYERTURN)
            return;
       
            StartCoroutine(PlayerHeal());       
        
    }

    public void OnFleeButton()
    {

        StartCoroutine(Flee());
    }

    public void OnSkillButton()
    {
        
        
        StartCoroutine(PlayerSkill());
    }
    IEnumerator PlayerSkill()
    {
        playerAttackButton.interactable = false;
        playerHealButton.interactable = false;
        fleeButton.interactable = false;
        skillButton.interactable = false;

        dialogueText.text = "Finally, a chance to use your pocket sand!";

        float randTurnMiss = Random.Range(0, 100);

        yield return new WaitForSeconds(1.5f);

        if(randTurnMiss >= 25)
        {

            bool isDead = enemyUnit.TakeDamage(2);


            enemyHUD.SetHP(enemyUnit.currHP);

            yield return new WaitForSeconds(1f);
            if (isDead)
            {
                yield return new WaitForSeconds(1f);
                enemyGo.transform.Rotate(Vector3.forward * -90f);
                //enemyGo.SetActive(false);
                playerUnit.currHealCharge = playerUnit.currHealCharge + 1;
                state = BattleState.WON;
                EndBattle();

            }
            else
            {
                state = BattleState.PLAYERTURN;
                PlayerTurn();
            }
        }
        else if(randTurnMiss < 2)
        {
            bool isDead = playerUnit.TakeDamage(2);


            playerHUD.SetHP(playerUnit.currHP);

            
            if (isDead)
            {
                playerGO.transform.Rotate(Vector3.forward * 90f);
                yield return new WaitForSeconds(1f);
                state = BattleState.LOST;
                EndBattle();


            }
            else
            {
                state = BattleState.ENEMYTURN;
                StartCoroutine(EnemyTurn());
            }
        }
        else if(randTurnMiss > 2 && randTurnMiss < 25)
        {
            dialogueText.text = "The hastily cast pocket sand falls to the floor";
            yield return new WaitForSeconds(1.5f);
            state = BattleState.ENEMYTURN;
            StartCoroutine(EnemyTurn());
        }

        
    }

    IEnumerator Flee()
    {
        playerAttackButton.interactable = false;
        playerHealButton.interactable = false;
        fleeButton.interactable = false;
        skillButton.interactable = false;
        dialogueText.text = "The " + enemyUnit.unitName + " slashes at you as you flee";
        playerUnit.TakeDamage(playerUnit.currHP / 2);
        playerHUD.SetHP(playerUnit.currHP);
        yield return new WaitForSeconds(2f);

        state = BattleState.LOST;
    }
    IEnumerator EnemyTurn()
    {
        
        
            int critDamage = (enemyUnit.damage * 2);
            float randCrit = Random.Range(0, 100);
            if (randCrit > 90)
            {
                CritString();
                Debug.Log(randCrit);
                dialogueText.text = "The " + enemyUnit.unitName + "Strikes a critical hit!";

                bool isDead = playerUnit.TakeDamage(critDamage);


                playerHUD.SetHP(playerUnit.currHP);

                yield return new WaitForSeconds(1f);
                if (isDead)
                {
                    playerGO.transform.Rotate(Vector3.forward * 90f);
                    yield return new WaitForSeconds(1f);
                    state = BattleState.LOST;
                    EndBattle();


                }
                else
                {
                    state = BattleState.PLAYERTURN;
                    PlayerTurn();
                }
            }

            else if (randCrit < 3.6f)

            {
                dialogueText.text = "The " + enemyUnit.unitName + "'s attack narrowly misses!"; ;
                bool isDead = playerUnit.TakeDamage(0);


                playerHUD.SetHP(playerUnit.currHP);


                if (isDead)
                {

                    playerGO.transform.Rotate(Vector3.forward * 90f);
                    yield return new WaitForSeconds(1f);
                    state = BattleState.LOST;
                    EndBattle();
                    //enemyGo.SetActive(false);


                }
                else
                {
                    state = BattleState.PLAYERTURN;
                    PlayerTurn();
                }
            }
            else
            {
                AttackString();
                dialogueText.text = "The " + enemyUnit.unitName + " lands a hit on you!";
                bool isDead = playerUnit.TakeDamage(enemyUnit.damage);


                playerHUD.SetHP(playerUnit.currHP);

                yield return new WaitForSeconds(1f);
                if (isDead)
                {
                    playerGO.transform.Rotate(Vector3.forward * 90f);
                    yield return new WaitForSeconds(1f);
                    state = BattleState.LOST;
                    EndBattle();
                    //enemyGo.SetActive(false);


                }
                else
                {
                    state = BattleState.PLAYERTURN;
                    PlayerTurn();
                }
            }
        
        
        //Attack
       

        //Heal
        
            
            

        
    }

    void EndBattle()
    {
        
        SUN.intensity = 1f;
        if (state == BattleState.WON)
        {
            gameOverPanel.SetActive(true);
            gameOver.text = "Well done for killing that one measly, fearsome, " + enemyUnit.unitName + "!";
            dialogueText.text = "Another foe vanquished!";
            battleStage.SetActive(false);
            
            miniEnemy.transform.Rotate(Vector3.forward * -90f);
        }
        else if(state == BattleState.LOST)
        {
            gameOverPanel.SetActive(true);
            gameOver.text = "Nice, a " + enemyUnit.unitName + " that weak can kill you? Try again.";
            dialogueText.text = "Another foolish adventurer claimed by the dungeon!";
            battleStage.SetActive(false);
            
            miniPlayer.transform.Rotate(Vector3.forward * 90f);
        }
    }

    void AttackString()
    {
        string[] attacks = new string[] { "You stab the " + enemyUnit.unitName + " in the eye!", "You swing your axe at the " + enemyUnit.unitName + "!", "You dismember the " + enemyUnit.unitName + "!", "En garde you " + enemyUnit.unitName + " swine!", "The " + enemyUnit.unitName + " is hurt by your scathing words!", "THWACK!" };
            System.Random random = new System.Random();
            int useAttack = random.Next(attacks.Length);
            pickAttack = attacks[useAttack];
            
        
    }
    void CritString()
    {
        string[] crits = new string[] { "Your attacks inflict more than just flesh wounds on the " + enemyUnit.unitName + "!", "You somehow remove the " + enemyUnit.unitName + "'s brain from his head without damaging the skull!", "A chandelier falls through the ceiling and lands on the " + enemyUnit.unitName + "!", "Ура капиталистическая мразь!", "You made the " + enemyUnit.unitName + " realize why nobody loves him!", "You finally get to use your Scroll of Untold Power!"};
        System.Random random = new System.Random();
        int useCrit = random.Next(crits.Length);
        pickCrit = crits[useCrit];


    }
    void MissString()
    {
        string[] miss = new string[] { "A glancing blow on the " + enemyUnit.unitName + "!", "Your blow skitters across the faceplate of the  " + enemyUnit.unitName + "!", "A chandelier falls through the ceiling and narrowly misses the " + enemyUnit.unitName + "!", "A swing and a miss!", "You can't win this through rock, paper, scissors", "The " + enemyUnit.unitName + " evaded your attacklike you evade taxes!" };
        System.Random random = new System.Random();
        int useMiss = random.Next(miss.Length);
        pickMiss = miss[useMiss];


    }
    void HealString()
    {
        string[] heal = new string[] { "That's the stuff!", "You shrug off the damaged caused by the" + enemyUnit.unitName + "!", "You slurp up that good good !", "The " + enemyUnit.unitName + "Shouts no fair as you heal!", "Roll 3d8 after a long rest!", "Was it the red or the blue potion? Why not both!" };
        System.Random random = new System.Random();
        int useHeal = random.Next(heal.Length);
        pickHeal = heal[useHeal];


    }
}
