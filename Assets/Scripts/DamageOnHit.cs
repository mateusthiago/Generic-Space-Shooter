using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageOnHit : MonoBehaviour
{       
    [SerializeField] int damage = 100;

    public int GetDamage()
    {
        return damage;
    }
    

}
