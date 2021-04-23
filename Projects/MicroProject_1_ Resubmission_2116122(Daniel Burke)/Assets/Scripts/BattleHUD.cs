using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleHUD : MonoBehaviour
{
    //global vars
    int currHP, maxHP;

    public Text nameText;

    public Slider hpSlider;


    //player vars
    
    public void SetHUD(EnemyUnit unit)
    {
        nameText.text = unit.name;

        if (unit.currHP < 0)
            unit.currHP = 0;
        hpSlider.maxValue = unit.maxHP;
        hpSlider.value = unit.currHP;
        if (unit.currHP >= unit.maxHP)
            currHP = maxHP;
    }
    public void SetPlayerHUD(PlayerUnit unit)
    {
        nameText.text = unit.name;

        if (unit.currHP >= unit.maxHP)
            currHP = maxHP;

        if (unit.currHP < 0)
            unit.currHP = 0;
        hpSlider.maxValue = unit.maxHP;
        hpSlider.value = unit.currHP;

    }


    public void SetHP(int HP)
    {
        hpSlider.value = HP;
    }

    


}
