using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerUnit : MonoBehaviour
{
    public string name;

    public int currHP;
    public int maxHP;

    public int currPotion;
    public int maxPotion;
    public int potionVal;

    public int curSkill01;
    public int maxSkill01;

    public int curSkill02;
    public int maxSkill02;

    public int curSkill03;
    public int maxSkill03;

    public int damage;

    void Start()
    {
        
    }

    
    void Update()
    {
        
    }

    public bool TakeDamage(int dmg)
    {
        currHP -= dmg;
       
        if (currHP <= 0)
        {
            return true;
        }
        else { return false; }
    }

    public void Heal()
    {
        potionVal = maxHP / 3;
        if (currPotion > 0)
        {
            currPotion -= 1;
            currHP += potionVal;
            
        }


        if (currHP > maxHP)
            currHP = maxHP;
      

        if (currPotion > maxPotion)
            currPotion = maxPotion;
        
    }
}
