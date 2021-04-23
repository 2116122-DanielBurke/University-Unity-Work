using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyUnit : MonoBehaviour
{
    public int maxHP;
    public int currHP;

    public int damage;

    public int healValue;

    public string name;
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
        currHP += healValue;
    }

}
