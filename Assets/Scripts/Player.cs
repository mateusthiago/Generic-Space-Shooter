using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("Payer")]
    [SerializeField] float moveSpeed = 8f;
    [SerializeField] float screenEdgePadding = 0.8f;
    [SerializeField] int health = 10;
    [SerializeField] AudioClip deathSFX;
    [SerializeField] [RangeAttribute(0f, 1f)] float deahVolume;
    [SerializeField] GameObject deathVFX;
    [SerializeField] bool isInvulnerable = false;
    [SerializeField] bool canMove = false;
    [SerializeField] bool hasShield = false;
    [SerializeField] GameObject shield;
    [SerializeField] int bulletType = 1;
    GameObject activeShield;

    [Header("Bullet")]
    [SerializeField] GameObject bulletPrefab;
    [SerializeField] AudioClip bulletSFX;
    [SerializeField] [RangeAttribute(0f, 1f)] float bulletVolume;
    [SerializeField] float bulletSpeed = 20f;
    [SerializeField] public float bulletCadence = 0.2f;
    
    float xMin, yMin;
    float xMax, yMax;

    Coroutine autoFire;
    bool isFiring;

    void Start ()
    {        
        SetUpMoveBoundaries();        
        //StartCoroutine(AutoFire());
	}

    void Update ()
    {       
        if (canMove)
        {
            Move();
            Fire();
        }        
	}

    private void Move()
    {
        var deltaX = Input.GetAxisRaw("Horizontal") * Time.deltaTime * moveSpeed;
        var newXPos = Mathf.Clamp(transform.position.x + deltaX, xMin, xMax);

        var deltaY = Input.GetAxisRaw("Vertical") * Time.deltaTime * moveSpeed;
        var newYPos = Mathf.Clamp(transform.position.y + deltaY, yMin, yMax);

        transform.position = new Vector2(newXPos, newYPos);
    }

    private void Fire()
    {
        if (Input.GetButtonDown("Fire1") && !isFiring)
        {
            isFiring = true;
            autoFire = StartCoroutine(AutoFire());
        }
        else if (Input.GetButtonUp("Fire1") && isFiring)
        {
            isFiring = false;
            StopCoroutine(autoFire);
        }
    }

    IEnumerator AutoFire()
    {
        while (true)
        {
            if (bulletType == 1)
            {
                GameObject newBullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity);
                newBullet.GetComponent<Rigidbody2D>().velocity = new Vector2(0, bulletSpeed);
                AudioSource.PlayClipAtPoint(bulletSFX, Camera.main.transform.position, bulletVolume);
                yield return new WaitForSeconds(bulletCadence);
            }
            else if (bulletType == 2)
            {
                Vector3 bulletPos = new Vector3(0.2f, 0, 0);
                GameObject newBullet1 = Instantiate(bulletPrefab, (transform.position + bulletPos), Quaternion.identity);
                GameObject newBullet2 = Instantiate(bulletPrefab, (transform.position - bulletPos), Quaternion.identity);
                newBullet1.GetComponent<Rigidbody2D>().velocity = new Vector2(0, bulletSpeed);
                newBullet2.GetComponent<Rigidbody2D>().velocity = new Vector2(0, bulletSpeed);
                AudioSource.PlayClipAtPoint(bulletSFX, Camera.main.transform.position, bulletVolume);
                yield return new WaitForSeconds(bulletCadence);
            }
        }
        
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {        
        if (!isInvulnerable)
            ProcessHit(collision);
    }

    private void ProcessHit(Collision2D collision)
    {
        var incomingCollision = collision.gameObject.GetComponent<Bullet>();

        if (hasShield)
        {
            incomingCollision.Hit();
            Destroy(activeShield);
            hasShield = false;
            GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        }
        else
        {            
            health -= incomingCollision.GetDamage();
            incomingCollision.Hit();
            if (health <= 0)
            {
                PlayerDeath();
            }
        }
        
    }

    private void PlayerDeath()
    {
        AudioSource.PlayClipAtPoint(deathSFX, Camera.main.transform.position, deahVolume);
        var newExplosion = Instantiate(deathVFX, transform.position, Quaternion.identity);
        Destroy(newExplosion, 2f);        
        FindObjectOfType<Game>().GameOver();
        Destroy(this.gameObject);
    }

    public void PickUpShield()
    {
        if (!hasShield)
        {
            activeShield = Instantiate(shield, transform.position, Quaternion.identity);
            hasShield = true;
        }        
    }



    void OnGUI()
    {
        GUI.Label(new Rect(10, 10, 200, 40), health.ToString());
    }

    private void SetUpMoveBoundaries()
    {
        xMin = Camera.main.ViewportToWorldPoint(new Vector3(0, 0, 0)).x + screenEdgePadding;
        xMax = Camera.main.ViewportToWorldPoint(new Vector3(1, 0, 0)).x - screenEdgePadding;

        yMin = Camera.main.ViewportToWorldPoint(new Vector3(0, 0, 0)).y + screenEdgePadding;
        yMax = Camera.main.ViewportToWorldPoint(new Vector3(0, 1, 0)).y - screenEdgePadding;
    }

    public float GetMoveSpeed()
    {
        return moveSpeed;
    }

    public void ToggleMove(bool toggle)
    {
        canMove = toggle;
    }

}
