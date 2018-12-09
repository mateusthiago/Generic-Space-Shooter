using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Enemy")]
    [SerializeField] int health = 200;
    [SerializeField] int score = 100;    
    [SerializeField] GameObject deathVFX;
    [SerializeField] AudioClip deathSFX;
    [SerializeField] [Range(0.1f, 1f)] float deathVolume;
    [SerializeField] GameObject powerUp;

    [Header("Bullet")]
    [SerializeField] float fireCountdown;
    [SerializeField] float minFireDelay = 0.3f;
    [SerializeField] float maxFireDelay = 2f;
    [SerializeField] int salvo = 1;
    [SerializeField] float salvoCadence = 1f;
    [SerializeField] float bulletSpeed = 8f;
    [SerializeField] GameObject bulletPrefab;    
    [SerializeField] AudioClip bulletSFX;
    [SerializeField] [Range(0.1f, 1f)] float bulletVolume;

    GameObject player = null;

    // Use this for initialization
    void Start ()
    {
        fireCountdown = Random.Range(minFireDelay, maxFireDelay);        
        if (FindObjectOfType<Player>() != null)
        {
            player = FindObjectOfType<Player>().gameObject;
        }
	}
	
	// Update is called once per frame
	void Update ()
    {
        fireCountdown -= Time.deltaTime;
        if (fireCountdown <= 0) { StartCoroutine(Fire());  };
	}

    private IEnumerator Fire()
    {

        fireCountdown = Random.Range(minFireDelay, maxFireDelay);

        for (int i = 0; i < salvo; i++)
        {
            if (player != null && transform.position.y > player.transform.position.y)
            {
                GameObject newEnemyBullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity);
                newEnemyBullet.GetComponent<Rigidbody2D>().velocity = (player.transform.position - transform.position).normalized * bulletSpeed;
                //newEnemyBullet.GetComponent<Rigidbody2D>().AddForce((FindObjectOfType<Player>().transform.position - transform.position) * bulletSpeed);
                newEnemyBullet.GetComponent<Rigidbody2D>().AddTorque(400f);
                AudioSource.PlayClipAtPoint(bulletSFX, Camera.main.transform.position, bulletVolume);
                yield return new WaitForSeconds(salvoCadence);
            }
            
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        ProcessHit(collision);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {        
        EnemyDeath();
    }

    private void ProcessHit(Collider2D collision)
    {
        var incomingCollision = collision.gameObject.GetComponent<PlayerBullet>();
        Debug.Log(collision.gameObject);
        health -= incomingCollision.GetDamage();        
        StartCoroutine (HitAnimation());
        if (health <= 0)
        {
            EnemyDeath();
        }
    }

    public void EnemyDeath()
    {
        AudioSource.PlayClipAtPoint(deathSFX, Camera.main.transform.position, deathVolume);
        var newExplosion = Instantiate(deathVFX, transform.position, Quaternion.Euler(-90, 0, 0));
        Destroy(newExplosion, 0.5f);
        FindObjectOfType<GameSession>().AddScore(score);
        if (powerUp != null) { Instantiate(powerUp, transform.position, Quaternion.identity); powerUp = null; }
        Destroy(this.gameObject);
    }

    private IEnumerator HitAnimation()
    {        
        GetComponent<SpriteRenderer>().color = new Vector4(0, 255, 255, 255);
        yield return new WaitForSeconds(0.05f);
        GetComponent<SpriteRenderer>().color = new Vector4(255, 255, 255, 255);
    }
}
