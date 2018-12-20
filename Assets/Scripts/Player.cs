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
    [SerializeField] [RangeAttribute(0f, 1f)] float deathVolume;
    [SerializeField] GameObject deathVFX;
    [SerializeField] bool isInvulnerable = false;
    bool startInvul;
    [SerializeField] bool canMove = false;
    [SerializeField] bool hasShield = false;
    [SerializeField] GameObject shield;
    [SerializeField] int bulletType = 1;
    [SerializeField] Transform firePosition;
    GameObject activeShield;
    Rigidbody2D playerRB;

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
        playerRB = GetComponent<Rigidbody2D>();
        SetUpMoveBoundaries();
        startInvul = isInvulnerable;
        //StartCoroutine(AutoFire());
	}

    void Update ()
    {       
        if (canMove)
        {
            UpdateMove();
            Fire();
        }        
	}

    /*
    private void FixedUpdate()
    {
        if (canMove)
        {
            FixedUpdateMove();            
        }
    }
    */

    private void UpdateMove()
    {
        var deltaX = Input.GetAxisRaw("Horizontal") * Time.deltaTime * moveSpeed;
        var newXPos = Mathf.Clamp(transform.position.x + deltaX, xMin, xMax);

        var deltaY = Input.GetAxisRaw("Vertical") * Time.deltaTime * moveSpeed;
        var newYPos = Mathf.Clamp(transform.position.y + deltaY, yMin, yMax);
        
        transform.position = new Vector3(newXPos, newYPos, -1); // SE DER ALGUMA MERDA PODE SER O -1 NO Z
    }
        
        
    private void FixedUpdateMove()
    {         
        var deltaX = Input.GetAxis("Horizontal") * moveSpeed;
        var deltaY = Input.GetAxis("Vertical") * moveSpeed;

        Debug.Log(new Vector2(deltaX, deltaY));
        playerRB.velocity = new Vector2(deltaX, deltaY);  
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
                GameObject newBullet = Instantiate(bulletPrefab, firePosition.position, Quaternion.identity);
                newBullet.GetComponent<Rigidbody2D>().velocity = new Vector2(0, bulletSpeed);
                AudioSource.PlayClipAtPoint(bulletSFX, Camera.main.transform.position, bulletVolume);
                yield return new WaitForSeconds(bulletCadence);
            }
            else if (bulletType == 2)
            {
                Vector3 bulletOffset = new Vector3(0.2f, 0, 0);
                GameObject newBullet1 = Instantiate(bulletPrefab, (firePosition.position + bulletOffset), Quaternion.identity);
                GameObject newBullet2 = Instantiate(bulletPrefab, (firePosition.position - bulletOffset), Quaternion.identity);
                newBullet1.GetComponent<Rigidbody2D>().velocity = new Vector2(0, bulletSpeed);
                newBullet2.GetComponent<Rigidbody2D>().velocity = new Vector2(0, bulletSpeed);
                AudioSource.PlayClipAtPoint(bulletSFX, Camera.main.transform.position, bulletVolume);
                yield return new WaitForSeconds(bulletCadence);
            }
            else if (bulletType == 3)
            {
                GameObject newBullet1 = Instantiate(bulletPrefab, firePosition.position, Quaternion.identity);
                GameObject newBullet2 = Instantiate(bulletPrefab, (firePosition.position + new Vector3(0.2f, -0.1f, 0)), Quaternion.identity);
                GameObject newBullet3 = Instantiate(bulletPrefab, (firePosition.position + new Vector3(-0.2f, -0.1f, 0)), Quaternion.identity);
                newBullet1.GetComponent<Rigidbody2D>().velocity = new Vector2(0, bulletSpeed);
                newBullet2.GetComponent<Rigidbody2D>().velocity = new Vector2(0, bulletSpeed);
                newBullet3.GetComponent<Rigidbody2D>().velocity = new Vector2(0, bulletSpeed);
                AudioSource.PlayClipAtPoint(bulletSFX, Camera.main.transform.position, bulletVolume);
                yield return new WaitForSeconds(bulletCadence);
            }
            else if (bulletType == 4)
            {                
                GameObject newBullet1 = Instantiate(bulletPrefab, firePosition.position, Quaternion.identity);
                GameObject newBullet2 = Instantiate(bulletPrefab, (firePosition.position + new Vector3(0.2f, -0.1f, 0)), Quaternion.identity);
                GameObject newBullet3 = Instantiate(bulletPrefab, (firePosition.position + new Vector3(-0.2f, -0.1f, 0)), Quaternion.identity);
                GameObject newBullet4 = Instantiate(bulletPrefab, (firePosition.position + new Vector3(0.7f, -0.4f, 0)), Quaternion.identity);
                GameObject newBullet5 = Instantiate(bulletPrefab, (firePosition.position + new Vector3(-0.7f, -0.4f, 0)), Quaternion.identity);
                newBullet1.GetComponent<Rigidbody2D>().velocity = new Vector2(0, bulletSpeed);
                newBullet2.GetComponent<Rigidbody2D>().velocity = new Vector2(0, bulletSpeed);
                newBullet3.GetComponent<Rigidbody2D>().velocity = new Vector2(0, bulletSpeed);
                newBullet4.GetComponent<Rigidbody2D>().velocity = new Vector2(0, bulletSpeed);
                newBullet5.GetComponent<Rigidbody2D>().velocity = new Vector2(0, bulletSpeed);
                AudioSource.PlayClipAtPoint(bulletSFX, Camera.main.transform.position, bulletVolume);
                yield return new WaitForSeconds(bulletCadence);
            }
        }
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {        
        if (!isInvulnerable)
            ProcessHit(collision);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (hasShield) StartCoroutine(DestroyShield());
        else if (!startInvul) PlayerDeath();   
    }

    private void ProcessHit(Collider2D collision)
    {
        var incomingCollision = collision.gameObject.GetComponent<EnemyBullet>();
        if (incomingCollision == null) return;

        if (hasShield) StartCoroutine(DestroyShield());
        else
        {            
            health -= incomingCollision.GetDamage();            
            if (health <= 0)
            {
                PlayerDeath();
            }
        }
        
    }

    public void PickUpShield()
    {
        if (!hasShield)
        {
            activeShield = Instantiate(shield, transform.position, Quaternion.identity);
            hasShield = true;
        }
    }

    public void PickUpBullet()
    {
        if (bulletType < 4)
        {
            bulletType += 1;
        }
    }

    private IEnumerator DestroyShield()
    {
        isInvulnerable = true;        
        activeShield.GetComponent<Shield>().DestroyShield();
        hasShield = false;                

        playerRB.velocity = Vector2.zero;
        for (int i = 0; i < 20; i++)
        {
            GetComponent<SpriteRenderer>().color = new Vector4(255, 255, 255, 0);
            yield return new WaitForSeconds(0.05f);
            GetComponent<SpriteRenderer>().color = new Vector4(255, 255, 255, 255);
            yield return new WaitForSeconds(0.05f);
        }
        if (!startInvul) isInvulnerable = false;  
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

    private void PlayerDeath()
    {
        AudioSource.PlayClipAtPoint(deathSFX, Camera.main.transform.position, deathVolume);
        var newExplosion = Instantiate(deathVFX, transform.position, Quaternion.identity);
        Destroy(newExplosion, 2f);
        FindObjectOfType<Game>().GameOver();
        Destroy(this.gameObject);
    }

    public int GetBulletType() { return bulletType; }
    public void SetBulletType(int type) { bulletType = type; }

}
