using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PartDamage : MonoBehaviour
{
    [SerializeField] int health;
    [SerializeField] AudioClip deathSFX;
    [SerializeField] GameObject deathVFX;
    [SerializeField] GameObject smokeVFX;
    [SerializeField] GameObject powerUp;
    [SerializeField] int score;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        var bullet = collision.GetComponent<PlayerBullet>();        
        if (bullet != null)
        {
            StartCoroutine(HitAnimation());
            health -= bullet.GetDamage();
            if (health < 0) DestroyPart();
        }
    }

    private IEnumerator HitAnimation()
    {
        GetComponent<SpriteRenderer>().color = new Vector4(0, 255, 255, 255);
        yield return new WaitForSeconds(0.05f);
        GetComponent<SpriteRenderer>().color = new Vector4(255, 255, 255, 255);
    }

    private void DestroyPart()
    {
        AudioSource.PlayClipAtPoint(deathSFX, Camera.main.transform.position, 0.1f);
        GameObject PartDestroyedVFX =  Instantiate(deathVFX, new Vector3(transform.position.x, transform.position.y, -1), Quaternion.identity);
        GameObject newsmokeVFX = Instantiate(smokeVFX, new Vector3(transform.position.x, transform.position.y, -1), Quaternion.identity);
        newsmokeVFX.transform.parent = FindObjectOfType<RedBoss>().transform;
        Destroy(PartDestroyedVFX, 2f);
        //FindObjectOfType<GameSession>().AddScore(score);
        RedBoss.redBoss.AddPartDestroyed();
        DropPowerUp();
        Destroy(this.gameObject);
    }

    private void DropPowerUp()
    {
        if (powerUp != null) Instantiate(powerUp, transform.position, Quaternion.identity);
        powerUp = null;
    }
}
