﻿using System;
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
    [SerializeField] bool canMove = false;
    [SerializeField] bool hasShield = false;
    [SerializeField] GameObject shield;
    [SerializeField] int bulletType = 1;
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
        
        

        /*
        var deltaX = Input.GetAxis("Horizontal") * moveSpeed;
        var deltaY = Input.GetAxis("Vertical") * moveSpeed;

        Debug.Log(new Vector2(deltaX, deltaY));
        playerRB.velocity = new Vector2(deltaX, deltaY);
        */
        
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

    private void OnTriggerEnter2D(Collider2D collision)
    {        
        if (!isInvulnerable)
            ProcessHit(collision);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (hasShield) DestroyShield();
        else  PlayerDeath();   
    }

    private void ProcessHit(Collider2D collision)
    {
        var incomingCollision = collision.gameObject.GetComponent<EnemyBullet>();
        if (incomingCollision == null) return;

        if (hasShield) DestroyShield();
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

    private void DestroyShield()
    {
        Destroy(activeShield);
        hasShield = false;
        playerRB.velocity = Vector2.zero;
        return;
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

}
