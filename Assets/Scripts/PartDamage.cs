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
    bool partDestroyed = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        var bullet = collision.GetComponent<PlayerBullet>();
        if (bullet != null)
        {
            StartCoroutine(HitAnimation());
            health -= bullet.GetDamage();
            if (health < 0 && !partDestroyed)
            {
                DestroyPart();
                partDestroyed = true;
            }
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
        FindObjectOfType<GameSession>().AddScore(score);
        DropPowerUp();
        AudioSource.PlayClipAtPoint(deathSFX, Camera.main.transform.position, 0.1f);
        GameObject PartDestroyedVFX =  Instantiate(deathVFX, new Vector3(transform.position.x, transform.position.y, -2), Quaternion.Euler(90,0,0));
        Destroy(PartDestroyedVFX, 2f);
        if (smokeVFX != null)
        {
            GameObject newsmokeVFX = Instantiate(smokeVFX, new Vector3(transform.position.x, transform.position.y, -1), Quaternion.identity);
            newsmokeVFX.transform.parent = FindObjectOfType<RedBoss>().transform;
        }        

        if (this.name == "topGun") { Destroy(GameObject.Find("topGunHatch")); RedBoss.redBoss.AddPartDestroyed(); }        
        if (this.name == "maincannon barrel") { Destroy(GameObject.Find("maincannon")); RedBoss.redBoss.AddPartDestroyed(); }
        if (this.name == "headcannon")
        {
            Destroy(GameObject.Find("headCannonHatch"));
            if (RedBoss.redBoss.laser != null) Destroy(RedBoss.redBoss.laser);
            if (RedBoss.redBoss.crosshair != null) Destroy(RedBoss.redBoss.crosshair);
            RedBoss.redBoss.AddPartDestroyed();
        }

        Destroy(this.gameObject);
    }

    private void DropPowerUp()
    {
        if (powerUp != null) Instantiate(powerUp, transform.position, Quaternion.identity);
        powerUp = null;
    }
}
