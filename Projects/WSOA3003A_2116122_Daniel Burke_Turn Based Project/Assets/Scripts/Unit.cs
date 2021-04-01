using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    public string unitName;
    public int unitLevel;
    public int healChargeMax;

    public int healChargeValue;

    public int currHealCharge;

    public int damage;

    public int maxHP;
    public int currHP;

    private void Start()
    {
        damage = Mathf.RoundToInt(5 * (unitLevel/2));
        maxHP += unitLevel;
        currHP = maxHP;
        
    }

    public bool TakeDamage(int dmg)
    {
        currHP -= dmg;

        if(currHP <= 0)
        {
            return true;
        }
        else { return false; }
    }

    public void Heal()
    {
        healChargeValue = maxHP / 3;
        if (currHealCharge > 0)
        {
            currHealCharge -= 1;
            currHP += healChargeValue;
        }
        

        if (currHP > maxHP)
            currHP = maxHP;

        if (currHealCharge > healChargeMax)
            currHealCharge = healChargeMax;
    }
}
