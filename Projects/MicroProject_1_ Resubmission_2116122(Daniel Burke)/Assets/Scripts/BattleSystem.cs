using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleSystem : MonoBehaviour
{
    public enum BattleState { START, PLAYERTURN, ENEMYTURN, WON, LOST, FLED }

    public BattleState state;

    public PlayerUnit player;

    public EnemyUnit enemy;

    public BattleHUD enemyHUD, playerHUD;

    private Vector3 PlayerstartPos, enemyStartPos;

    public Transform playerGOTrans, enemyGOTrans, bulletSPwnPnt;

    //Ints
   public  int  playerCooldown01, playerCooldown02, playerCooldown03;
    //Floats
    public float power = 0.7f;
    public float duration = 1.0f;
    public float slowDownAmount;
    private float initialDuration;
    
    //Strings
    string pickAttack, pickCrit, pickMiss, pickHeal, pickEnemyAttack, pickEnemyCrit, pickEnemyMiss;
    //GameObjects
    public GameObject enemyGO, playerGO, gameOverPanel;

    public GameObject stunParticles, ShieldParticles, healParticles, burnParticles, charmParticles, glockGO, bulletGO;

    //Bools
    public bool shouldPlayerShake = false, shouldEnemyShake = false;
    public bool bossHasHealed = false;
    

    //wizard bools
    bool isBurning = false;
    bool isCharmed = false;
    bool isShielded = false;

    public bool coolDownSkill01, coolDownSkill02, coolDownSkill03;
    public bool enemyCoolDown01;
    public bool isPlayerStun, isEnemyStun;
    //Buttons
    public Button attackButton, healButton, skill01Button, skill02Button, skill03Button, fleeButton;

    //Text
    public Text skillText01, skillText02, skillText03, gameOverText;
    public Text skill01_Num_Text, skill02_Num_Text, skill03_Num_Text, heal_Num_Text, dialogueText;




    //skill02_Num_Text.text = unit.curSkill02 + "/" + unit.maxSkill02;
    //skill03_Num_Text.text = unit.curSkill03 + "/" + unit.maxSkill03;
    private void Start()
    {
        StartCoroutine(SetupBattle());
        

        if (player.name == "Wizard")
        {
            skillText01.text = "The book of many spells";
            skillText02.text = "Fairy dust";
            skillText03.text = "Magical shield";
        }
        else if (player.name == "Rogue")
        {
            skillText01.text = "Throwing knives";
            skillText02.text = "Artful dodger";
            skillText03.text = "Hiding in the shadows";
        }
        else if (player.name == "Knight")
        {
            skillText01.text = "Battlecry";
            skillText02.text = "Shield bash";
            skillText03.text = "Bulwark";
        }
        else if (player.name == "Cultist")
        {
            skillText01.text = "Abysall envoy";
            skillText02.text = "Flagellation";
            skillText03.text = "Unholy Prayers";
        }
        state = BattleState.START;
    }

    private void Update()
    {
        ShakeCamera();

        //Cooldown management

        while (playerCooldown01 > 0 && playerCooldown01 <= 3)
        {
            skill01Button.interactable = false;
        }
        while (playerCooldown02 > 0 && playerCooldown02 <= 3)
        {
            skill02Button.interactable = false;
        }
        while (playerCooldown03 > 0 && playerCooldown03 <= 3)
        {
            skill03Button.interactable = false;
        }
    }

    IEnumerator SetupBattle()
    {
        attackButton.interactable = false;
        healButton.interactable = false;
        skill01Button.interactable = false;
        skill02Button.interactable = false;
        skill03Button.interactable = false;
        fleeButton.interactable = false;

        playerHUD.SetPlayerHUD(player);
        enemyHUD.SetHUD(enemy);

        dialogueText.text = "The " + enemy.name + " has finally chosen to face you!";

        playerGOTrans = playerGO.transform;
        PlayerstartPos = playerGOTrans.localPosition;
        enemyGOTrans = enemyGO.transform;
        enemyStartPos = enemyGOTrans.localPosition;

        skill01_Num_Text.text = player.curSkill01 + "/" + player.maxSkill01;
        skill02_Num_Text.text = player.curSkill02 + "/" + player.maxSkill02;
        skill03_Num_Text.text = player.curSkill03 + "/" + player.maxSkill03;
        heal_Num_Text.text = player.currPotion + "/" + player.maxPotion;
        yield return new WaitForSeconds(2f);

        state = BattleState.PLAYERTURN;
        PlayerTurn();
    }

    //Player turn & Button methods
    void PlayerTurn()
    {
        if (isPlayerStun)
        {
            state = BattleState.ENEMYTURN;
            EnemyTurn();
            Destroy(stunParticles);
            isPlayerStun = false;
        }
        if (isShielded)
        {
            Destroy(ShieldParticles);
            isShielded = false;
        }



        attackButton.interactable = true;
        healButton.interactable = true;
        skill01Button.interactable = true;
        skill02Button.interactable = true;
        skill03Button.interactable = true;

        dialogueText.text = "What will you do?";

        if (player.currPotion <= 0)
            healButton.interactable = false;
        else if (player.currHP == player.maxHP)
            healButton.interactable = false;
        else
            healButton.interactable = true;
        if (player.curSkill01 <= 0)
            skill01Button.interactable = false;

        if (player.curSkill02 <= 0)
            skill02Button.interactable = false;

        if (player.curSkill03 <= 0)
            skill03Button.interactable = false;


        //cooldowns
        if (coolDownSkill01)
        {
            playerCooldown01++;
        }
        if (coolDownSkill02)
        {
            playerCooldown02++;
        }
        if (coolDownSkill03)
        {
            playerCooldown03++;
        }

        if (playerCooldown01 >= 4)
        {
            coolDownSkill01 = false;
            playerCooldown01 = 0;
        }
        if (playerCooldown02 >= 4)
        {
            coolDownSkill02 = false;
            playerCooldown02 = 0;
        }
        if (playerCooldown03 >= 4)
        {
            coolDownSkill03 = false;
            playerCooldown03 = 0;
        }
    }

    public void OnAttackButton()
    {
        if (state != BattleState.PLAYERTURN)
            return;

        attackButton.interactable = false;
        healButton.interactable = false;
        skill01Button.interactable = false;
        skill02Button.interactable = false;
        skill03Button.interactable = false;
        fleeButton.interactable = false;
        if (player.name == "Wizard")
        {
            StartCoroutine(WizardAttack());
        }
        else if (player.name == "Rogue")
        {
            StartCoroutine(RogueAttack());
        }
        else if (player.name == "Knight")
        {
            StartCoroutine(KnightAttack());
        }
        else if (player.name == "Cultist")
        {
            StartCoroutine(CultAttack());
        }
    }

    public void OnHealButton()
    {
        if (state != BattleState.PLAYERTURN)
            return;

        if (player.currPotion > 0)
        {
            attackButton.interactable = false;
            healButton.interactable = false;
            skill01Button.interactable = false;
            skill02Button.interactable = false;
            skill03Button.interactable = false;
            fleeButton.interactable = false;
            StartCoroutine(Heal());
        }
        else if (player.currPotion <= 0)
        {
            healButton.interactable = false;
            dialogueText.text = "You're all out of potions, better do something else!";
        }


    }

    public void OnSkillButton01()
    {
        if (state != BattleState.PLAYERTURN)
            return;


        if (player.name == "Wizard")
        {
            StartCoroutine(WizardSkill01());
        }
        else if (player.name == "Rogue")
        {
            StartCoroutine(RogueSkill01());
        }
        else if (player.name == "Knight")
        {
            StartCoroutine(KnightSkill01());
        }
        else if (player.name == "Cultist")
        {
            StartCoroutine(CultSkill01());
        }

        attackButton.interactable = false;
        healButton.interactable = false;
        skill01Button.interactable = false;
        skill02Button.interactable = false;
        skill03Button.interactable = false;
        fleeButton.interactable = false;

    }

    public void OnSkillButton02()
    {
        if (state != BattleState.PLAYERTURN)
            return;
        if (player.name == "Wizard")
        {
            StartCoroutine(WizardSkill02());
        }
        else if (player.name == "Rogue")
        {
            StartCoroutine(RogueSkill02());
        }
        else if (player.name == "Knight")
        {
            StartCoroutine(KnightSkill02());
        }
        else if (player.name == "Cultist")
        {
            StartCoroutine(CultSkill02());
        }

        attackButton.interactable = false;
        healButton.interactable = false;
        skill01Button.interactable = false;
        skill02Button.interactable = false;
        skill03Button.interactable = false;
        fleeButton.interactable = false;

    }


    public void OnSkillButton03()
    {
        if (state != BattleState.PLAYERTURN)
            return;
        if (player.name == "Wizard")
        {
            StartCoroutine(WizardSkill03());
        }
        else if (player.name == "Rogue")
        {
            StartCoroutine(RogueSkill03());
        }
        else if (player.name == "Knight")
        {
            StartCoroutine(KnightSkill03());
        }
        else if (player.name == "Cultist")
        {
            StartCoroutine(CultSkill03());
        }

        attackButton.interactable = false;
        healButton.interactable = false;
        skill01Button.interactable = false;
        skill02Button.interactable = false;
        skill03Button.interactable = false;
        fleeButton.interactable = false;
    }

    public void OnFleeButton()
    {
        if (state != BattleState.PLAYERTURN)
            return;
        StartCoroutine(Flee());
        attackButton.interactable = false;
        healButton.interactable = false;
        skill01Button.interactable = false;
        skill02Button.interactable = false;
        skill03Button.interactable = false;
        fleeButton.interactable = false;

        state = BattleState.FLED;
        EndBattle();
    }


    IEnumerator Heal()
    {
        yield return new WaitForSeconds(2);
        skill01_Num_Text.text = player.currPotion + "/" + player.maxPotion;
        HealString();
        if (player.currPotion > 0 && player.currHP < player.maxHP)
        {
            attackButton.interactable = false;
            healButton.interactable = false;
            skill01Button.interactable = false;
            skill02Button.interactable = false;
            skill03Button.interactable = false;
            fleeButton.interactable = false;
            player.Heal();
            GameObject healGO = Instantiate(healParticles, playerGO.transform);
            Destroy(healGO, 1.1f);
            playerHUD.SetHP(player.currHP);
            dialogueText.text = pickHeal;
            yield return new WaitForSeconds(2f);

            state = BattleState.ENEMYTURN;
            EnemyTurn();

            playerHUD.SetPlayerHUD(player);
            heal_Num_Text.text = "" + player.currPotion;
          fleeButton.interactable = false;
        }
    }

    IEnumerator Flee()
    {
        yield return new WaitForSeconds(2);
        if (isEnemyStun)
        {
            attackButton.interactable = false;
            healButton.interactable = false;
            skill01Button.interactable = false;
            skill02Button.interactable = false;
            skill03Button.interactable = false;
            fleeButton.interactable = false;
            dialogueText.text = "You run from the " + enemy.name + " while you have the chance!";
        }
        else
        {
            int fleeDmg = player.currHP / 3;
            attackButton.interactable = false;
            healButton.interactable = false;
            skill01Button.interactable = false;
            skill02Button.interactable = false;
            skill03Button.interactable = false;
            fleeButton.interactable = false;
            dialogueText.text = "The " + enemy.name + " slashes at you as you flee and inflicts " + fleeDmg + " points of damage!";
            player.TakeDamage(fleeDmg);
            playerHUD.SetHP(player.currHP);
        }

        yield return new WaitForSeconds(2f);
        state = BattleState.FLED;
        EndBattle();
    }
    //Wizar enums

    IEnumerator WizardAttack()
    {
        float randCrit = Random.Range(0, 100);

        if (randCrit > 90)
        {
            int critDamage = Mathf.RoundToInt(player.damage*2);
            isBurning = true;
            CritString();
            Debug.Log(randCrit);
            dialogueText.text = pickCrit + "(" + critDamage + " points of damage)";
            yield return new WaitForSeconds(1);

            bool isDead = enemy.TakeDamage(critDamage);
            
            shouldEnemyShake = true;
            slowDownAmount = 3;
            power = 0.45f;
            yield return new WaitForSeconds(1);

            enemyHUD.SetHP(enemy.currHP);

            yield return new WaitForSeconds(1f);
            if (isDead)
            {
                enemyGO.transform.Rotate(Vector3.forward * -90f);
                yield return new WaitForSeconds(1f);

                //enemyGO.SetActive(false);
                
                state = BattleState.WON;
                EndBattle();

            }
            else
            {
                state = BattleState.ENEMYTURN;
                EnemyTurn();
            }
        }

        else if (randCrit < 3.6f)

        {
            MissString();
            dialogueText.text = pickMiss;
            yield return new WaitForSeconds(1f);
            bool isDead = enemy.TakeDamage(0);
            dialogueText.text = "You deal 0 points of damage!";
            yield return new WaitForSeconds(1);

            enemyHUD.SetHP(enemy.currHP);

            yield return new WaitForSeconds(1f);
            if (isDead)
            {
                enemyGO.transform.Rotate(Vector3.forward * -90f);
                yield return new WaitForSeconds(1f);

                //enemyGO.SetActive(false);
                
                state = BattleState.WON;
                EndBattle();

            }
            else
            {
                state = BattleState.ENEMYTURN;
                EnemyTurn();
            }
        }
        else
        {

            int dmg = player.damage;
            AttackString();
            dialogueText.text = pickAttack + "(" + dmg + " points of damage)";
            yield return new WaitForSeconds(1);
            bool isDead = enemy.TakeDamage(dmg);

            
            shouldEnemyShake = true;
            slowDownAmount = 3;
            power = 0.25f;
            yield return new WaitForSeconds(1);
            enemyHUD.SetHP(enemy.currHP);

            yield return new WaitForSeconds(1f);
            if (isDead)
            {
                enemyGO.transform.Rotate(Vector3.forward * -90f);
                yield return new WaitForSeconds(1f);

                //enemyGO.SetActive(false);
                
                state = BattleState.WON;
                EndBattle();
            }
            else
            {
                state = BattleState.ENEMYTURN;
                EnemyTurn();
            }
        }
    }

    IEnumerator WizardSkill01()
    {
        coolDownSkill01 = true;
        skill01_Num_Text.text = player.curSkill01 + "/" + player.maxSkill01;
        float randCrit = Random.Range(0, 100);

        //Damage self
        if (randCrit >= 0 && randCrit <= 5)
        {
            int dmg = 5;
            bool isDead = player.TakeDamage(dmg);
            dialogueText.text = "A puff of smoke and sparks singes your robes and burns your enemies nostril hairs" + "(" + dmg + " points of damage)";
            player.TakeDamage(dmg);
            playerHUD.SetHP(player.currHP);
            
            yield return new WaitForSeconds(1f);
            if (isDead)
            {
                playerGO.transform.Rotate(Vector3.forward * 90f);
                yield return new WaitForSeconds(1f);

                //enemyGO.SetActive(false);
               
                state = BattleState.LOST;
                EndBattle();
            }
            else
            {
                state = BattleState.ENEMYTURN;
                EnemyTurn();
            }
        }

        if (randCrit > 5 && randCrit <= 15)
        {
            
            int dmg = 10;
            bool isDead = enemy.TakeDamage(dmg);
            dialogueText.text = "Small storm clouds form and zap the " + enemy.name + "!" + "(" + dmg + " points of damage)"; 
            enemy.TakeDamage(dmg);
           enemyHUD.SetHP(enemy.currHP);

            yield return new WaitForSeconds(1f);
            if (isDead)
            {
                enemyGO.transform.Rotate(Vector3.forward * -90f);
                yield return new WaitForSeconds(1f);

                //enemyGO.SetActive(false);

                state = BattleState.WON;
                EndBattle();
            }
            else
            {
                state = BattleState.ENEMYTURN;
                EnemyTurn();
            }
        }
        if (randCrit > 15 && randCrit <= 35)
        {
            
            int dmg = 15;
            bool isDead = enemy.TakeDamage(dmg);
            dialogueText.text = "The " + enemy.name + " is blasted by a wave of sound!" + " (" + dmg + " points of damage)";
            enemy.TakeDamage(dmg);
            enemyHUD.SetHP(enemy.currHP);
            isEnemyStun = true;
            Instantiate(stunParticles, enemyGOTrans);
            yield return new WaitForSeconds(1f);
            if (isDead)
            {
                enemyGO.transform.Rotate(Vector3.forward * -90f);
                yield return new WaitForSeconds(1f);

                //enemyGO.SetActive(false);

                state = BattleState.WON;
                EndBattle();
            }
            else
            {
                state = BattleState.ENEMYTURN;
                EnemyTurn();
            }
        }
        if (randCrit > 35 && randCrit <= 55)
        {
           
            int dmg = 20;
            bool isDead = enemy.TakeDamage(dmg);
            dialogueText.text = "A burst of holy light emits from the spellbook and strikes the " + enemy.name + "!" + "(" + dmg + " points of damage)";
            enemy.TakeDamage(dmg);
            enemyHUD.SetHP(enemy.currHP);
            isEnemyStun = true;
            Instantiate(stunParticles, enemyGOTrans);
            yield return new WaitForSeconds(1f);
            if (isDead)
            {
                enemyGO.transform.Rotate(Vector3.forward * -90f);
                yield return new WaitForSeconds(1f);

                //enemyGO.SetActive(false);

                state = BattleState.WON;
                EndBattle();
            }
            else
            {
                state = BattleState.ENEMYTURN;
                EnemyTurn();
            }

        }
        if (randCrit > 55 && randCrit <= 75)
        {
            dialogueText.text = "The " + enemy.name + "is stunned by a thunderous clap and blinding light!" ;
            isEnemyStun = true;
            Instantiate(stunParticles, enemyGOTrans);
            yield return new WaitForSeconds(2f);
            state = BattleState.ENEMYTURN;
            EnemyTurn();
            
        }
        if (randCrit > 75 && randCrit <= 100)
        {
            
            int dmg = 40;
            bool isDead = enemy.TakeDamage(dmg);
            dialogueText.text = "The spellbook gives you a glock." + "(" + dmg + " points of damage)";
            
            StartCoroutine(Glock());
            
            isEnemyStun = true;
            Instantiate(stunParticles, enemyGOTrans);
            yield return new WaitForSeconds(2f);
            shouldEnemyShake = true;
            slowDownAmount = 3;
            power = 0.25f;
            enemyHUD.SetHP(enemy.currHP);
            enemy.TakeDamage(dmg);
            if (isDead)
            {
                enemyGO.transform.Rotate(Vector3.forward * -90f);
                yield return new WaitForSeconds(1f);

                //enemyGO.SetActive(false);

                state = BattleState.WON;
                EndBattle();
            }
            else
            {
                state = BattleState.ENEMYTURN;
                EnemyTurn();
            }
        }
        
        
    }


    IEnumerator Glock()
    {
        yield return new WaitForSeconds(0.5f);
        Instantiate(bulletGO, bulletSPwnPnt);
        
        Destroy(glockGO, 1.5f);
    }
    IEnumerator WizardSkill02()
    {
        coolDownSkill02 = true;
        dialogueText.text = "A pinch of white fairy dust will charm your enemies and leave you invigorated!";
        player.currHP += 10;
        playerHUD.SetHP(player.currHP);
        isCharmed = true;
        Instantiate(charmParticles, enemyGOTrans);
        skill01_Num_Text.text = player.curSkill02 + "/" + player.maxSkill02;
        yield return new WaitForSeconds(2);
        state = BattleState.ENEMYTURN;
        EnemyTurn();
        
    }

    IEnumerator WizardSkill03()
    {
        skill01_Num_Text.text = player.curSkill03 + "/" + player.maxSkill03;
        coolDownSkill03 = true;
        isShielded = true;
        dialogueText.text = "You conjure up a magical shield";
        yield return new WaitForSeconds(2);
        

        state = BattleState.ENEMYTURN;
        EnemyTurn();
    }

    //Rogue Enums
    IEnumerator RogueAttack()
    {
        yield return new WaitForSeconds(2);
    }

    IEnumerator RogueSkill01()
    {
        coolDownSkill01 = true;
        yield return new WaitForSeconds(2);
        skill01_Num_Text.text = player.curSkill01 + "/" + player.maxSkill01;
    }

    IEnumerator RogueSkill02()
    {
        coolDownSkill02 = true;
        yield return new WaitForSeconds(2);
        skill01_Num_Text.text = player.curSkill02 + "/" + player.maxSkill02;
    }

    IEnumerator RogueSkill03()
    {
        coolDownSkill03 = true;
        yield return new WaitForSeconds(2);
        skill01_Num_Text.text = player.curSkill03 + "/" + player.maxSkill03;
    }

    //Knight Enums
    IEnumerator KnightAttack()
    {
        yield return new WaitForSeconds(2);
    }

    IEnumerator KnightSkill01()
    {
        coolDownSkill01 = true;
        yield return new WaitForSeconds(2);
        skill01_Num_Text.text = player.curSkill01 + "/" + player.maxSkill01;
    }

    IEnumerator KnightSkill02()
    {
        coolDownSkill02 = true;
        yield return new WaitForSeconds(2);
        skill01_Num_Text.text = player.curSkill02 + "/" + player.maxSkill02;
    }

    IEnumerator KnightSkill03()
    {
        coolDownSkill03 = true;
        yield return new WaitForSeconds(2);
        skill01_Num_Text.text = player.curSkill03 + "/" + player.maxSkill03;
    }
    //Cultist Enums
    IEnumerator CultAttack()
    {
        yield return new WaitForSeconds(2);
    }

    IEnumerator CultSkill01()
    {
        coolDownSkill01 = true;
        yield return new WaitForSeconds(2);
        skill01_Num_Text.text = player.curSkill01 + "/" + player.maxSkill01;
    }

    IEnumerator CultSkill02()
    {
        coolDownSkill02 = true;
        yield return new WaitForSeconds(2);
        skill01_Num_Text.text = player.curSkill02 + "/" + player.maxSkill02;
    }

    IEnumerator CultSkill03()
    {
        coolDownSkill03 = true;
        yield return new WaitForSeconds(2);
        skill01_Num_Text.text = player.curSkill03 + "/" + player.maxSkill03;
    }


    public void ShakeCamera()
    {
        if (shouldEnemyShake)
        {
            if (duration > 0)
            {
                enemyGOTrans.localPosition = enemyStartPos + Random.insideUnitSphere * power;
                duration -= Time.deltaTime * slowDownAmount;
            }
            else
            {
                shouldEnemyShake = false;
                duration = initialDuration;
                enemyGOTrans.localPosition = enemyStartPos;
            }
        }
        if (shouldPlayerShake)
        {
            if (duration > 0)
            {
                playerGOTrans.localPosition = PlayerstartPos + Random.insideUnitSphere * power;
                duration -= Time.deltaTime * slowDownAmount;
            }
            else
            {
                shouldPlayerShake = false;
                duration = initialDuration;
                playerGOTrans.localPosition = PlayerstartPos;
            }
        }
    }

    //Enemy Enums
    public void EnemyTurn()
    {
        if (isEnemyStun)
        {
            state = BattleState.PLAYERTURN;
            PlayerTurn();
            Destroy(stunParticles);
            isEnemyStun = false;
        }
        if (isBurning)
        {
            StartCoroutine(isBurningTimer());
        }
        else
        {
            StartCoroutine(EnemyBaseAttack());
        }
        

    }

    IEnumerator isBurningTimer()
    {
        int burnDamage = 2;
        bool isDead = enemy.TakeDamage(burnDamage);
        shouldEnemyShake = true;
        slowDownAmount = 3;
        power = 0.1f;
        enemy.TakeDamage(burnDamage);
        enemyHUD.SetHP(enemy.currHP);
        yield return new WaitForSeconds(1);
        shouldEnemyShake = true;
        slowDownAmount = 3;
        power = 0.1f;
        enemy.TakeDamage(burnDamage);
        enemyHUD.SetHP(enemy.currHP);
        yield return new WaitForSeconds(1);
        shouldEnemyShake = true;
        slowDownAmount = 3;
        power = 0.1f;
        enemy.TakeDamage(burnDamage);
        enemyHUD.SetHP(enemy.currHP);
        yield return new WaitForSeconds(1);
        shouldEnemyShake = true;
        slowDownAmount = 3;
        power = 0.1f;
        enemy.TakeDamage(burnDamage);
        enemyHUD.SetHP(enemy.currHP);
        yield return new WaitForSeconds(1);
        shouldEnemyShake = true;
        slowDownAmount = 3;
        power = 0.1f;
        enemy.TakeDamage(burnDamage);
        enemyHUD.SetHP(enemy.currHP);
        yield return new WaitForSeconds(1);
        isBurning = false;
        Destroy(burnParticles);
        StartCoroutine(EnemyBaseAttack());
        if (isDead)
        {
            enemyGO.transform.Rotate(Vector3.forward * -90f);
            yield return new WaitForSeconds(1f);

            //enemyGO.SetActive(false);
            
            state = BattleState.WON;
            EndBattle();
        }
        
        
    }

    IEnumerator EnemyBaseAttack()
    {
        float randCrit;
        if (isCharmed)
        {
            randCrit = Random.Range(0, 89);
        }
            
        else {
            randCrit = Random.Range(0, 100);
        }
        if (randCrit > 90)
        {
            int critDamage = Mathf.RoundToInt(enemy.damage * 2);

            if (isShielded)
                critDamage = Mathf.RoundToInt(critDamage / 3);
            EnemyCritString();
            Debug.Log(randCrit);
            dialogueText.text = pickCrit + "(" + critDamage + "points of damage)";
            yield return new WaitForSeconds(1);

            bool isDead = player.TakeDamage(critDamage);
            dialogueText.text = pickEnemyCrit;
            shouldPlayerShake = true;
            slowDownAmount = 3;
            power = 0.45f;
            yield return new WaitForSeconds(1);

            playerHUD.SetHP(player.currHP);

            yield return new WaitForSeconds(1f);
            if (isDead)
            {
                playerGO.transform.Rotate(Vector3.forward * 90f);
                yield return new WaitForSeconds(1f);

                //enemyGO.SetActive(false);

                state = BattleState.LOST;
                EndBattle();
            }
            else
            {
                state = BattleState.PLAYERTURN;
                PlayerTurn();
            }
        
    }

        else if (randCrit < 3.6f && !isCharmed)

        {
            EnemyMissString();
            dialogueText.text = pickEnemyMiss;
            yield return new WaitForSeconds(1f);
            bool isDead = player.TakeDamage(0);
            dialogueText.text = "The enemy misses";
            yield return new WaitForSeconds(1);

            playerHUD.SetHP(player.currHP);

            yield return new WaitForSeconds(1f);
            if (isDead)
            {
                playerGO.transform.Rotate(Vector3.forward * 90f);
                yield return new WaitForSeconds(1f);

                //enemyGO.SetActive(false);

                state = BattleState.LOST;
                EndBattle();
            }
            else
            {
                state = BattleState.PLAYERTURN;
                PlayerTurn();
            }
        }
        else if (randCrit < 25f && isCharmed)

        {
            EnemyMissString();
            dialogueText.text = pickEnemyMiss;
            yield return new WaitForSeconds(1f);
            bool isDead = player.TakeDamage(0);
            dialogueText.text = pickEnemyMiss;
            yield return new WaitForSeconds(1);

            playerHUD.SetHP(player.currHP);

            yield return new WaitForSeconds(1f);
            if (isDead)
            {
                playerGO.transform.Rotate(Vector3.forward * 90f);
                yield return new WaitForSeconds(1f);

                //enemyGO.SetActive(false);

                state = BattleState.LOST;
                EndBattle();
            }
            else
            {
                state = BattleState.PLAYERTURN;
                PlayerTurn();
            }
        }

        else
        {

            int dmg = enemy.damage;
            dmg = Mathf.RoundToInt(enemy.damage/3);
            shouldPlayerShake = true;
            slowDownAmount = 3;
            power = 0.25f;
            EnemyAttackString();
            dialogueText.text = pickEnemyAttack + "(" + dmg + "points of damage)";
            yield return new WaitForSeconds(1);
            bool isDead = player.TakeDamage(dmg);

            
            
            yield return new WaitForSeconds(1);
            playerHUD.SetHP(player.currHP);

            yield return new WaitForSeconds(1f);
            if (isDead)
            {
                playerGO.transform.Rotate(Vector3.forward * 90f);
                yield return new WaitForSeconds(1f);

                //enemyGO.SetActive(false);
                
                state = BattleState.LOST;
                EndBattle();
            }
            else
            {
                state = BattleState.PLAYERTURN;
                PlayerTurn();
            }
        }
    }

    //End Battle
    void EndBattle()
    {
        if (state == BattleState.WON)
        {
            gameOverPanel.SetActive(true);
            dialogueText.text = "You beat the " + enemy.name+" and cleared the dungeon!";
        }
        else if (state == BattleState.LOST)
        {
            gameOverPanel.SetActive(true);


            dialogueText.text = "The " + enemy.name + " stands triumphantly over your dead body";
        }
        else if (state == BattleState.FLED)
        {
            playerHUD.SetHP(player.currHP);
            gameOverPanel.SetActive(true);
            dialogueText.text = "You coward, come again another day";
        }
    }

    //Random combat strings
    void AttackString()
    {
        string[] attacks = new string[] { "You stab the " + enemy.name + " in the eye!", "You swing your staff at the " + enemy.name + "!", "You artfully stab the " + enemy.name + " when he looks away!", "En garde you " + enemy.name + " swine!", "The " + enemy.name + " is hurt by your scathing words!", "THWACK!" };
        System.Random random = new System.Random();
        int useAttack = random.Next(attacks.Length);
        pickAttack = attacks[useAttack];
    }
    void CritString()
    {
        string[] crits = new string[] { "Your attacks inflict more than just flesh wounds on the " + enemy.name + "!", "You somehow remove the " + enemy.name + "'s brain from his head without damaging the skull!", "A chandelier falls through the ceiling and lands on the " + enemy.name + "!", "Ура капиталистическая мразь!", "You made the " + enemy.name + " realize why nobody loves him!", "You finally get to use your Scroll of Untold Power!", "Look a distraction!" };
        System.Random random = new System.Random();
        int useCrit = random.Next(crits.Length);
        pickCrit = crits[useCrit];
    }
    void MissString()
    {
        string[] miss = new string[] { "A glancing blow on the " + enemy.name + "!", "Your blow skitters across the faceplate of the  " + enemy.name + "!", "A chandelier falls through the ceiling and narrowly misses the " + enemy.name + "!", "A swing and a miss!", "The " + enemy.name + " does not understand rock, paper, scissors", "The " + enemy.name + " evaded your attack like you evade taxes!" };
        System.Random random = new System.Random();
        int useMiss = random.Next(miss.Length);
        pickMiss = miss[useMiss];
    }
    void HealString()
    {
        string[] heal = new string[] { "That's the stuff!", "You shrug off the damaged caused by the " + enemy.name + "!", "You slurp up that good good!", "The " + enemy.name + " looks annoyed that you healed!", "Roll 3d8 after a long rest!", "Was it the red or the blue potion? Why not both!" };
        System.Random random = new System.Random();
        int useHeal = random.Next(heal.Length);
        pickHeal = heal[useHeal];
    }
    void EnemyAttackString()
    {
        string[] skill01 = new string[] { "The " + enemy.name + "smashes their axe into your side!", "The " + enemy.name + " lands a heavy blow on your shoulder!", "The " + enemy.name + " strikes out at you and connects!" };
        System.Random random = new System.Random();
        int useSkill01 = random.Next(skill01.Length);
        pickEnemyAttack = skill01[useSkill01];
    }
    void EnemyCritString()
    {
        string[] skill02 = new string[] { "The " + enemy.name + "slams into you with the force of 12 bulls!", "The " + enemy.name + "attempts to seperate your head from your body!" , "The " + enemy.name + "roars and charges!" ,};
        System.Random random = new System.Random();
        int useSkill02 = random.Next(skill02.Length);
        pickEnemyCrit = skill02[useSkill02];
    }
    void EnemyMissString()
    {
        string[] skill03 = new string[] { "The " + enemy.name + " narrowly misses your legs as you leap over his blade", "If it wasn't for your shield and " + enemy.name + "'s poor eyesight you'd be dead" };
        System.Random random = new System.Random();
        int useSkill03 = random.Next(skill03.Length);
        pickEnemyMiss = skill03[useSkill03];
    }

}
