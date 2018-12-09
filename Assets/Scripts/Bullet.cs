using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{

    [SerializeField] int damage = 100;

    Enemy enemyScript;

    private void Start()
    {
        enemyScript = GetComponent<Enemy>();
    }

    public int GetDamage()
    {
        return damage;
    }

    public void Hit()
    {
        if (this.gameObject.transform.parent != null)
            Destroy(this.gameObject.transform.parent.gameObject);
        else
        {
            if (enemyScript != null)
            {
                enemyScript.EnemyDeath();
            }
            else
                Destroy(this.gameObject);
        }
            
    }
	
}
