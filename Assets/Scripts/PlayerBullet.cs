using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBullet : MonoBehaviour
{

    [SerializeField] GameObject hitFX;
    [SerializeField] int damage = 100;

    public int GetDamage()
    {
        return damage;
    }    

    private void OnTriggerEnter2D(Collider2D collision)
    {
        var newHitFX = Instantiate(hitFX, transform.position, Quaternion.Euler(90, 0, 0));
        Destroy(newHitFX, 0.2f);        
        Destroy(this.gameObject);        
    }

    private void OnDestroy()
    {
        if (transform.parent != null && transform.parent.childCount >= 1) Destroy(transform.parent.gameObject);
    }
}
