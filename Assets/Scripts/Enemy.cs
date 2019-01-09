using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Enemy : MonoBehaviour
{
    [Header("Enemy")]
    [SerializeField] int health = 200;
    [SerializeField] int score = 100;    
    [SerializeField] GameObject deathVFX;
    [SerializeField] AudioClip deathSFX;
    [SerializeField] [Range(0.1f, 1f)] float deathVolume;
    [SerializeField] GameObject powerUp;
    [SerializeField] [Range(0,1)] float chance;
    [SerializeField] int powerUpType = 0; // 0 is for shields (will always drop), 1-3 will only drop if player at or bellow bulletType 1-3
    [SerializeField] bool blackSaucerBehavior = false;
    [SerializeField] bool redSaucerBehavior = false;
    [SerializeField] bool scoutBehavior = false;
    [SerializeField] bool voidBehavior = false;
    [SerializeField] bool destroyerBehavior = false;
    bool isFiring = false;

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

    [Header("VoidShip")]
    [SerializeField] AudioClip voidCharge;

    bool isInDeathFunction = false;
    bool amILastInSector = false;
    int nextSector = 0;

    GameObject player = null;
    

    void Start ()
    {
        FindObjectOfType<EnemySpawner>().AddEnemyCount();
        fireCountdown = Random.Range(minFireDelay, maxFireDelay);
        if (FindObjectOfType<Player>() != null)
        {
            player = FindObjectOfType<Player>().gameObject;
        }
    }   
	
	void Update ()
    {
        if (destroyerBehavior && !isFiring)
        {
            fireCountdown -= Time.deltaTime;
            if (fireCountdown <= 0 && maxFireDelay != 0) StartCoroutine(DestroyerFire());
        }
        else if (redSaucerBehavior && !isFiring)
        {
            StartCoroutine(SaucerFire());
            isFiring = true;
        }
        else if (!voidBehavior && !blackSaucerBehavior && bulletPrefab != null && !isFiring)
        {
            fireCountdown -= Time.deltaTime;
            if (fireCountdown <= 0 && maxFireDelay != 0) StartCoroutine(Fire());
        }
        
        
	}

    private IEnumerator Fire()
    {
        if (player != null && transform.position.y > player.transform.position.y)
        {
            isFiring = true;

            for (int i = 0; i < salvo; i++)
            {
                if (player != null)
                {
                    GameObject newEnemyBullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity);
                    if (!scoutBehavior && !voidBehavior) newEnemyBullet.GetComponent<Rigidbody2D>().velocity = (player.transform.position - transform.position).normalized * bulletSpeed;
                    if (scoutBehavior || voidBehavior) newEnemyBullet.GetComponent<Rigidbody2D>().velocity = newEnemyBullet.transform.up * -bulletSpeed;
                    newEnemyBullet.GetComponent<Rigidbody2D>().AddTorque(400f);
                    AudioSource.PlayClipAtPoint(bulletSFX, Camera.main.transform.position, bulletVolume);
                    yield return new WaitForSeconds(salvoCadence);
                }

            }

            isFiring = false;
        }

        fireCountdown = Random.Range(minFireDelay, maxFireDelay);
    }

    public IEnumerator VoidFire()
    {        
        isFiring = true;
        Animator anim = GetComponent<Animator>();
        anim.SetTrigger("openvoid");
        AudioSource.PlayClipAtPoint(voidCharge, Camera.main.transform.position, 0.3f);
        GetComponent<ParticleSystem>().Play();
        yield return new WaitForSeconds(1f);
        GetComponent<VoidShip>().isFiring = true;
        yield return new WaitForSeconds(0.5f);
         

        for (int i = 0; i < salvo; i++)
        {
            if (player != null)
            {
                GameObject newEnemyBullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity);
                if (!scoutBehavior && !voidBehavior) newEnemyBullet.GetComponent<Rigidbody2D>().velocity = (player.transform.position - transform.position).normalized * bulletSpeed;
                if (scoutBehavior || voidBehavior) newEnemyBullet.GetComponent<Rigidbody2D>().velocity = transform.up * -bulletSpeed;
                newEnemyBullet.GetComponent<Rigidbody2D>().AddTorque(400f);
                AudioSource.PlayClipAtPoint(bulletSFX, Camera.main.transform.position, bulletVolume);
                yield return new WaitForSeconds(salvoCadence);
            }

        }

        yield return new WaitForSeconds(0.5f);
        anim.SetTrigger("closevoid");
        yield return new WaitForSeconds(0.5f);
        GetComponent<VoidShip>().isFiring = false;
        isFiring = false;

    }

    public IEnumerator SaucerFire()
    {
        Animator anim = GetComponent<Animator>();
        if (anim != null ) anim.SetTrigger("arming");
        yield return new WaitForSeconds(1);

        Transform firePoint1 = transform.GetChild(0); //GameObject.Find("FirePoint1").transform; 
        Transform firePoint2 = transform.GetChild(1); //GameObject.Find("FirePoint2").transform;
        Transform firePoint3 = transform.GetChild(2); //GameObject.Find("FirePoint3").transform;

        for (int i = 0; i < salvo; i++)
        {
            GameObject newSaucerBullet1 = Instantiate(bulletPrefab, firePoint1.position, firePoint1.rotation);
            newSaucerBullet1.GetComponent<Rigidbody2D>().velocity = newSaucerBullet1.transform.up * bulletSpeed;                
            newSaucerBullet1.GetComponent<Rigidbody2D>().AddTorque(400f);

            
            GameObject newSaucerBullet2 = Instantiate(bulletPrefab, firePoint2.position, firePoint2.rotation);            
            newSaucerBullet2.GetComponent<Rigidbody2D>().velocity = newSaucerBullet2.transform.up * bulletSpeed;
            newSaucerBullet2.GetComponent<Rigidbody2D>().AddTorque(400f);            

            GameObject newSaucerBullet3 = Instantiate(bulletPrefab, firePoint3.position, firePoint3.rotation);
            newSaucerBullet3.GetComponent<Rigidbody2D>().velocity = newSaucerBullet3.transform.up * bulletSpeed;
            newSaucerBullet3.GetComponent<Rigidbody2D>().AddTorque(400f);
            

            AudioSource.PlayClipAtPoint(bulletSFX, Camera.main.transform.position, bulletVolume);
            yield return new WaitForSeconds(salvoCadence);
        }

        yield return new WaitForSeconds(0.5f);
        isFiring = false;
        if (anim != null)  anim.SetTrigger("disarming");
        yield return new WaitForSeconds(1f);        

    }

    public IEnumerator DestroyerFire()
    {
        Transform firePoint1 = GameObject.Find("FirePoint1").transform;
        Transform firePoint2 = GameObject.Find("FirePoint2").transform;
        Transform firePoint3 = GameObject.Find("FirePoint3").transform;

        isFiring = true;

        for (int i = 0; i < salvo; i++)
        {
            GameObject newDestroyerBullet1 = Instantiate(bulletPrefab, firePoint1.position, firePoint1.rotation);            
            newDestroyerBullet1.GetComponent<Rigidbody2D>().velocity = transform.up * -bulletSpeed; 
            //newDestroyerBullet1.GetComponent<Rigidbody2D>().AddTorque(400f);


            GameObject newDestroyerBullet2 = Instantiate(bulletPrefab, firePoint2.position, firePoint2.rotation);
            newDestroyerBullet2.GetComponent<Rigidbody2D>().velocity = transform.up * -bulletSpeed;
            //newDestroyerBullet2.GetComponent<Rigidbody2D>().AddTorque(400f);

            GameObject newDestroyerBullet3 = Instantiate(bulletPrefab, firePoint3.position, firePoint3.rotation);
            newDestroyerBullet3.GetComponent<Rigidbody2D>().velocity = transform.up * -bulletSpeed;
            //newDestroyerBullet3.GetComponent<Rigidbody2D>().AddTorque(400f);


            AudioSource.PlayClipAtPoint(bulletSFX, Camera.main.transform.position, bulletVolume);
            yield return new WaitForSeconds(salvoCadence);
        }

        isFiring = false;
        fireCountdown = Random.Range(minFireDelay, maxFireDelay);

    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        ProcessHit(collision);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {        
        EnemyDeath(true);
    }

    private void ProcessHit(Collider2D collision)
    {
        var incomingCollision = collision.gameObject.GetComponent<PlayerBullet>();
        if (incomingCollision != null) health -= incomingCollision.GetDamage();        
        StartCoroutine (HitAnimation());
        if (health <= 0 && !isInDeathFunction)
        {
            isInDeathFunction = true;
            EnemyDeath(true);
        }
    }

    public void EnemyDeath(bool deathByPlayer)
    {
        FindObjectOfType<EnemySpawner>().SubtractEnemyCount();

        if (deathByPlayer)
        {
            AudioSource.PlayClipAtPoint(deathSFX, Camera.main.transform.position, deathVolume);
            FindObjectOfType<Camera>().GetComponent<CamShake>().CameraShake(score/100f, Mathf.Clamp(score/500f, 0.1f, 0.5f));
            var newExplosion = Instantiate(deathVFX, transform.position, Quaternion.Euler(90, 0, 0));
            Destroy(newExplosion, 2f);
            FindObjectOfType<GameSession>().AddScore(score);
            DropPowerUp();
        }        

        if (amILastInSector == true)
        {
            if (nextSector == 4) FindObjectOfType<Game>().GetComponent<Game>().CallFinalSector();
            else FindObjectOfType<Game>().GetComponent<Game>().SetSectorStarsAndShowBanner(nextSector, 2);
            FindObjectOfType<GameSession>().SetLastSector(nextSector);
        }              

        Destroy(this.gameObject);
    }

    private void DropPowerUp()
    {
        if (powerUp != null)
        {
            float powerUpDrop = Random.value;

            switch (powerUpType)
            {
                case 0:                    
                    if (powerUpDrop <= chance) Instantiate(powerUp, transform.position, Quaternion.identity); powerUp = null;
                    break;

                case 1:
                    if (powerUpDrop <= chance && player.GetComponent<Player>().GetBulletType() == 1) Instantiate(powerUp, transform.position, Quaternion.identity); powerUp = null;
                    break;

                case 2:
                    if (powerUpDrop <= chance && player.GetComponent<Player>().GetBulletType() <= 2) Instantiate(powerUp, transform.position, Quaternion.identity); powerUp = null;
                    break;

                case 3:
                    if (powerUpDrop <= chance && player.GetComponent<Player>().GetBulletType() <= 3) Instantiate(powerUp, transform.position, Quaternion.identity); powerUp = null;
                    break;

                default: break;
            }
                   
        }
    }

    public void SetDropChance(float setChance)
    {
        chance = setChance;
    }


    private IEnumerator HitAnimation()
    {        
        GetComponent<SpriteRenderer>().color = new Vector4(0, 255, 255, 255);
        yield return new WaitForSeconds(0.05f);
        GetComponent<SpriteRenderer>().color = new Vector4(255, 255, 255, 255);
    }

    //public void SetLastInWave(bool whatSpawnerSaid) { amILastInWave = whatSpawnerSaid; }
    public void SetLastInSector(bool whatSpawnerSaid) { amILastInSector = whatSpawnerSaid; }
    public void SetNextSector(int sector) { nextSector = sector; }

    private void OnDestroy()
    {
        
    }

    public bool GetIsFiring()
    {
        return isFiring;
    }
}
