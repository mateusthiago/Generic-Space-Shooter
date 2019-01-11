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

    public static Player instance;
    DeviceType deviceType;
    Vector3 previousTouchPos;


    void Start ()
    {
        deviceType = SystemInfo.deviceType;
        instance = this;
        playerRB = GetComponent<Rigidbody2D>();
        SetUpMoveBoundaries();
        startInvul = isInvulnerable;                
	}

    void Update ()
    {       
        if (canMove && !Game.game.gameIsPaused)
        {
            UpdateMove();
            if(deviceType == DeviceType.Handheld && !isFiring) // correto é handheld
            {
                isFiring = true;
                StartCoroutine(AutoFire());
            }
            else if(deviceType == DeviceType.Desktop) Fire();
        }        
	}

    public void UpdateMove()
    {
        Vector3 inputVector;

        if (deviceType == DeviceType.Handheld) // correto é handheld
        {
            if (Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0);                
                if (touch.phase == TouchPhase.Began || touch.phase == TouchPhase.Stationary)
                {
                    previousTouchPos = Camera.main.ScreenToWorldPoint(touch.position);                    
                }
                else if (touch.phase == TouchPhase.Moved)
                {
                    Vector3 newTouchPos = Camera.main.ScreenToWorldPoint(touch.position);
                    inputVector = newTouchPos;
                    inputVector.z = 0;
                    previousTouchPos.z = 0;
                    Vector3 direction = inputVector - previousTouchPos;
                    transform.Translate(direction);
                    if (transform.position.x < xMin || transform.position.x > xMax || transform.position.y < yMin || transform.position.y > yMax)
                    {
                        Vector3 clampedNewPos = transform.position;
                        clampedNewPos.x = Mathf.Clamp(clampedNewPos.x, xMin, xMax);
                        clampedNewPos.y = Mathf.Clamp(clampedNewPos.y, yMin, yMax);
                        transform.position = clampedNewPos;
                    }
                    
                    previousTouchPos = newTouchPos;
                }
            }
            else return;          
        }
        else if (deviceType == DeviceType.Desktop)
        {
            inputVector = new Vector3(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized * Time.deltaTime * moveSpeed;
            Vector3 clampedNewPos = transform.position + inputVector;
            clampedNewPos.x = Mathf.Clamp(clampedNewPos.x, xMin, xMax);
            clampedNewPos.y = Mathf.Clamp(clampedNewPos.y, yMin, yMax);

            transform.position = clampedNewPos;
        }
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
        Camera.main.GetComponent<CamShake>().CameraShake(2, 1);
        AudioSource.PlayClipAtPoint(deathSFX, Camera.main.transform.position, deathVolume);
        var newExplosion = Instantiate(deathVFX, transform.position, Quaternion.identity);
        Destroy(newExplosion, 2f);
        FindObjectOfType<Game>().GameOver();
        Destroy(this.gameObject);
        Game.game.SetGameIsOn(false);
    }

    public int GetBulletType() { return bulletType; }
    public void SetBulletType(int type) { bulletType = type; }

}
