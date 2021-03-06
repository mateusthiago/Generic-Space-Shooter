﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RedBoss : MonoBehaviour
{
    [Header("Guns")]
    [SerializeField] GameObject topGun;
    [SerializeField] AudioClip topGunMoveSFX;
    [SerializeField] GameObject topGunBullet;
    [SerializeField] AudioClip topGunBulletSFX;
    [SerializeField] float topGunBulletSpeed;
    [SerializeField] float topGunFireCountdown;
    [SerializeField] int topGunSalvo;
    [SerializeField] float topGunCadence;
    float topGunRotation = 45;
    bool topGunIsFiring = false;
    [Space]
    [SerializeField] GameObject headGunL;
    [SerializeField] GameObject headGunR;
    [SerializeField] GameObject headGunsBullet;
    [SerializeField] AudioClip headGunsSFX;
    [SerializeField] int headGunsBulletSpeed;
    [SerializeField] float headGunsFireCountdown;
    [SerializeField] float headGunsCadence;
    [SerializeField] int headGunsSalvo;
    bool headGunsAreFiring = false;
    [Space]
    [SerializeField] GameObject headCannon;
    [SerializeField] GameObject headCannonBullet;
    [SerializeField] GameObject headCannonCrosshair;
    [SerializeField] GameObject headCannonLine;
    public GameObject laser; //declarado aqui para poder ser destruído na função de morte do chefe - partdamage também destroi
    public GameObject crosshair;
    [SerializeField] float headCannonBulletSpeed;
    [SerializeField] float headCannonFireCountdown;
    [SerializeField] float headCannonCadence;
    [SerializeField] int headCannonSalvo;
    bool headCannonIsFiring = false;
    [Space]
    [SerializeField] GameObject wingGunL;    
    [SerializeField] GameObject wingGunR;
    [SerializeField] float wingGunsBulletSpeed;
    [SerializeField] float wingGunsFireCountdown;
    [SerializeField] float wingGunsCadence;
    [SerializeField] int wingGunsSalvo;
    bool wingGunsAreFiring = false;
    [Space]
    [SerializeField] GameObject mainCannon;
    [SerializeField] GameObject mainCannonBarrel;
    [SerializeField] GameObject mainCannonBullet;    
    [SerializeField] AudioClip mainCannonSFX;
    [SerializeField] AudioClip mainCannonCharge;
    [SerializeField] float mainCannonBulletSpeed;
    [SerializeField] float mainCannonFireCountdown;
    [SerializeField] float mainCannonCadence;
    [SerializeField] int mainCannonSalvo;
    bool mainCannonIsFiring;    

    int weaponTurn = 0; // 1= topgun, 2= headcannon, 3= maincannon
    int gunTurn = 1; //1= headguns, 2=wingguns
    
    float topGunOriginalCD;
    float headGunsOriginalCD;
    float headCannonOriginalCD;
    float wingGunsOriginalCD;
    float mainCannonOriginalCD;

    bool isDying = false;

    [Header("Fire Points")]
    [SerializeField] Transform topGunFPL;
    [SerializeField] Transform topGunFPR;
    [SerializeField] Transform headGunLFP;
    [SerializeField] Transform headGunRFP;
    [SerializeField] Transform headCannonFP;
    [SerializeField] Transform wingGunLFP;
    [SerializeField] Transform wingGunRFP;
    [SerializeField] Transform mainCannonFP;

    [Header("Death")]
    [SerializeField] GameObject smallExplosion;
    [SerializeField] GameObject bigExplosion;
    [SerializeField] AudioClip sexpAudio;
    [SerializeField] AudioClip bexpAudio;
    [SerializeField] Image flashScreen;

    [Header("Debug - DO NOT CHANGE")]
    [SerializeField]Transform player;    
    public int partsDestroyed = 0;

    public static RedBoss redBoss;

    CamShake camshake;

    void Start ()
    {
        redBoss = this;
        topGunOriginalCD = topGunFireCountdown;
        headGunsOriginalCD = headGunsFireCountdown;
        headCannonOriginalCD = headCannonFireCountdown;
        wingGunsOriginalCD = wingGunsFireCountdown;
        mainCannonOriginalCD = mainCannonFireCountdown;
        player = FindObjectOfType<Player>().transform;
        camshake = FindObjectOfType<Camera>().GetComponent<CamShake>();

    }
	
	
	void Update ()
    {
        if (!isDying)
        {
            if (partsDestroyed >= 3)
            {
                StopAllCoroutines();
                StartCoroutine(Death());
                isDying = true;
            }

            MoveToPlayer();
            
            if ((headGunL != null || headGunR != null) && gunTurn == 1) headGunsFireCountdown -= Time.deltaTime; else if (gunTurn == 1) gunTurn = 2;
            if (headGunsFireCountdown < 0 && !headGunsAreFiring) { StartCoroutine(HeadGunsFire()); headGunsAreFiring = true; }

            if ((wingGunL != null || wingGunR != null) && gunTurn == 2) wingGunsFireCountdown -= Time.deltaTime; else if (gunTurn == 2) gunTurn = 1;
            if (wingGunsFireCountdown < 0 && !wingGunsAreFiring) { StartCoroutine(WingGunsFire()); wingGunsAreFiring = true; }

            if (topGun != null && weaponTurn == 1) topGunFireCountdown -= Time.deltaTime; else if (weaponTurn == 1) weaponTurn = 2;
            if (topGunFireCountdown < 0 && !topGunIsFiring) { StartCoroutine(TopGunFire()); topGunIsFiring = true; }

            if (headCannon != null && weaponTurn == 2) headCannonFireCountdown -= Time.deltaTime; else if (weaponTurn == 2) weaponTurn = 3;
            if (headCannonFireCountdown < 0 && !headCannonIsFiring) { StartCoroutine(HeadCannonFire()); headCannonIsFiring = true; }

            if (mainCannonBarrel != null && weaponTurn == 3) mainCannonFireCountdown -= Time.deltaTime; else if (weaponTurn == 3) weaponTurn = 1;
            if (mainCannonFireCountdown < 0 && !mainCannonIsFiring) { StartCoroutine(MainCannonFire()); mainCannonIsFiring = true; }
        }
    }

    private void MoveToPlayer()
    {
        if (transform.position.y > 5.0f)
        {
            transform.position += new Vector3(0, -0.5f, 0) * Time.deltaTime;
        }
        if (transform.position.y >= 5.0f && weaponTurn == 0) weaponTurn = 1;

        if (player != null)
        {
            float distToPlayer = player.position.x - transform.position.x;
            if (Mathf.Abs(distToPlayer) < 0.2f) return;
            transform.position += new Vector3(distToPlayer, 0, 0).normalized * Time.deltaTime * 0.6f;
        }
    }

    IEnumerator TopGunFire()
    {
        AudioSource.PlayClipAtPoint(topGunMoveSFX, Camera.main.transform.position, 0.3f);
        transform.Find("topGunHatch").gameObject.GetComponent<Animator>().SetTrigger("open");
        yield return new WaitForSeconds(1.5f);
        do
        {
            topGun.transform.localScale += new Vector3(0.01f, 0.01f, 0);
            yield return null;            
        } while (topGun.transform.localScale.x < 1);

        topGun.layer = 10;

        float t = 0;
        do
        {
            t += Time.deltaTime;            
            float smooth = Mathf.SmoothStep(0, topGunRotation, t);
            if (topGun != null) topGun.transform.rotation = Quaternion.Euler(0, 0, smooth);            
            yield return null;

        } while (t < 1);

        yield return new WaitForSeconds(0.5f);

        t = 0;
        for (int i = 0; i < topGunSalvo; i++)
        {
            if (topGun != null)
            {
                GameObject newTopGunBulletL = Instantiate(topGunBullet, topGunFPL.position, topGunFPL.rotation);
                newTopGunBulletL.GetComponent<Rigidbody2D>().velocity = newTopGunBulletL.transform.up * -topGunBulletSpeed;
                newTopGunBulletL.GetComponent<Rigidbody2D>().AddTorque(400f);
                GameObject newTopGunBulletR = Instantiate(topGunBullet, topGunFPR.position, topGunFPR.rotation);
                newTopGunBulletR.GetComponent<Rigidbody2D>().velocity = newTopGunBulletR.transform.up * -topGunBulletSpeed;
                newTopGunBulletR.GetComponent<Rigidbody2D>().AddTorque(400f);
                AudioSource.PlayClipAtPoint(topGunBulletSFX, Camera.main.transform.position, 0.05f);

                t += topGunCadence / (topGunCadence * (topGunSalvo - 1));
                float lerp = Mathf.Lerp(topGunRotation, topGunRotation * -1, t);
                topGun.transform.rotation = Quaternion.Euler(0, 0, lerp);
                yield return new WaitForSeconds(topGunCadence);
            }
        }        
        
        t = 0;       
        
        do
        {
            t += Time.deltaTime;
            float smooth = Mathf.SmoothStep(topGunRotation * -1, 0, t);
            if (topGun != null) topGun.transform.rotation = Quaternion.Euler(0, 0, smooth);
            yield return null;

        } while (t < 1);        

        yield return new WaitForSeconds(1f);
        if (topGun != null) topGun.layer = 2;

        do
        {
            if (topGun != null) topGun.transform.localScale += new Vector3(-0.01f, -0.01f, 0);
            yield return null;
        } while (topGun != null && topGun.transform.localScale.x > 0.4);

        if (topGun != null)
        {
            AudioSource.PlayClipAtPoint(topGunMoveSFX, Camera.main.transform.position, 0.3f);
            transform.Find("topGunHatch").gameObject.GetComponent<Animator>().SetTrigger("close");
        }

        topGunRotation *= -1;
        topGunFireCountdown = topGunOriginalCD;            
        topGunIsFiring = false;
        weaponTurn = 2;
    }

    IEnumerator HeadGunsFire()
    {
        for (int i = 0; i < headGunsSalvo; i++)
        {
            if (headGunL != null)
            {
                GameObject newHeadGunBulletL = Instantiate(headGunsBullet, headGunLFP.position, Quaternion.identity);
                newHeadGunBulletL.GetComponent<Rigidbody2D>().velocity = newHeadGunBulletL.transform.up * -headGunsBulletSpeed;
                AudioSource.PlayClipAtPoint(headGunsSFX, Camera.main.transform.position, 0.03f);
                yield return new WaitForSeconds(headGunsCadence);
            }            
            
            if (headGunR != null)
            {
                GameObject newHeadGunBulletR = Instantiate(headGunsBullet, headGunRFP.position, Quaternion.identity);
                newHeadGunBulletR.GetComponent<Rigidbody2D>().velocity = newHeadGunBulletR.transform.up * -headGunsBulletSpeed;
                AudioSource.PlayClipAtPoint(headGunsSFX, Camera.main.transform.position, 0.03f);
                yield return new WaitForSeconds(headGunsCadence);
            }
        }
        headGunsFireCountdown = headGunsOriginalCD;
        headGunsAreFiring = false;
        gunTurn = 2;
    }

    IEnumerator HeadCannonFire()
    {
        float rotationSpeed = 8f;

        AudioSource.PlayClipAtPoint(topGunMoveSFX, Camera.main.transform.position, 0.3f);
        transform.Find("headCannonHatch").gameObject.GetComponent<Animator>().SetTrigger("open");
        yield return new WaitForSeconds(1.5f);
        do
        {
            headCannon.transform.localScale += new Vector3(0.01f, 0.01f, 0);
            yield return null;
        }
        while (headCannon.transform.localScale.x < 1);
        headCannon.layer = 10;

        Vector3 putCrosshairAbovePlayer = new Vector3(0, 0, -1);
        crosshair = Instantiate(headCannonCrosshair, player.transform.position + putCrosshairAbovePlayer, Quaternion.identity) as GameObject;
        laser = Instantiate(headCannonLine, Vector3.zero, Quaternion.identity) as GameObject;        
        for (int salvoCount = 1; salvoCount <= headCannonSalvo; salvoCount++)
        {            
            float lockOnCD = headCannonCadence;            
            Vector3 direction = Vector3.zero;            
            var laserLine = laser.GetComponent<LineRenderer>();            
            do
            {
                lockOnCD -= Time.deltaTime;
                if (headCannon != null)
                {
                    if (headCannon == null || player == null) { Destroy(laser); Destroy(crosshair); }
                    crosshair.transform.Rotate(0, 0, -1f);
                    crosshair.GetComponent<SpriteRenderer>().color = Color.Lerp(crosshair.GetComponent<SpriteRenderer>().color, new Color(255, 0, 0, 0.5f), Time.deltaTime);                    
                    crosshair.transform.position = player.transform.position + putCrosshairAbovePlayer;
                    laserLine.SetPosition(0, headCannon.transform.position);
                    laserLine.SetPosition(1, player.transform.position);
                    laserLine.endWidth = (lockOnCD / headCannonCadence) * headCannonCadence;
                    direction = new Vector3(player.transform.position.x - headCannon.transform.position.x, player.transform.position.y - headCannon.transform.position.y);
                    headCannon.transform.up = Vector3.Lerp(headCannon.transform.up, -direction, Time.deltaTime); //multiply deltaTime * rotationspeed for smoothing
                }
                else { Destroy(laser); Destroy(crosshair); }

                yield return null;
            } while (lockOnCD > 0);

            if (headCannon != null)
            {
                GameObject newHeadCannonBullet = Instantiate(headCannonBullet, headCannonFP.position, headCannonFP.rotation);
                newHeadCannonBullet.GetComponent<Rigidbody2D>().velocity = -direction * -headCannonBulletSpeed;
                newHeadCannonBullet.GetComponent<Rigidbody2D>().AddTorque(400f);
                AudioSource.PlayClipAtPoint(topGunBulletSFX, Camera.main.transform.position, 0.5f);
            }
        }
        yield return new WaitForSeconds(0.4f);
        Destroy(crosshair);
        Destroy(laser);
        yield return new WaitForSeconds(1f);

        do
        {
            if (headCannon != null) headCannon.transform.up = Vector3.Slerp(headCannon.transform.up, Vector3.up, rotationSpeed * Time.deltaTime);
            yield return null;
        } while (headCannon != null && headCannon.transform.up != Vector3.up);

        if (headCannon != null)
        {
            headCannon.layer = 2;
            do
            {
                headCannon.transform.localScale += new Vector3(-0.01f, -0.01f, 0);
                yield return null;
            }
            while (headCannon.transform.localScale.x > 0.7);
            AudioSource.PlayClipAtPoint(topGunMoveSFX, Camera.main.transform.position, 0.3f);
            transform.Find("headCannonHatch").gameObject.GetComponent<Animator>().SetTrigger("close");

            headCannonFireCountdown = headCannonOriginalCD;
            headCannonIsFiring = false;
        }
        weaponTurn = 3;
    }

    IEnumerator WingGunsFire()
    {
        for (int i = 0; i < wingGunsSalvo; i++)
        {
            if (wingGunL != null)
            {
                GameObject newWingGunBulletL = Instantiate(headGunsBullet, wingGunLFP.position, Quaternion.identity);
                newWingGunBulletL.GetComponent<Rigidbody2D>().velocity = newWingGunBulletL.transform.up * -wingGunsBulletSpeed;
                AudioSource.PlayClipAtPoint(headGunsSFX, Camera.main.transform.position, 0.03f);                
            }

            if (wingGunR != null)
            {
                GameObject newWingGunBulletR = Instantiate(headGunsBullet, wingGunRFP.position, Quaternion.identity);
                newWingGunBulletR.GetComponent<Rigidbody2D>().velocity = newWingGunBulletR.transform.up * -wingGunsBulletSpeed;
                AudioSource.PlayClipAtPoint(headGunsSFX, Camera.main.transform.position, 0.03f);
            }
            yield return new WaitForSeconds(wingGunsCadence);
        }
        wingGunsFireCountdown = wingGunsOriginalCD;
        wingGunsAreFiring = false;
        gunTurn = 1;
    }

    IEnumerator MainCannonFire()
    {
        // ANIMAÇÃO DE ARMAR
        AudioSource.PlayClipAtPoint(topGunMoveSFX, Camera.main.transform.position, 0.2f);
        if (mainCannonBarrel != null)
        {
            do
            {
                mainCannon.transform.position += new Vector3(0, 0.01f, 0);
                yield return null;
            }
            while (mainCannon.transform.localPosition.y < 1.75f);
            do
            {
                mainCannonBarrel.transform.localScale += new Vector3(0, 0.01f, 0);
                yield return null;
            }
            while (mainCannonBarrel.transform.localScale.y < 1);
            mainCannonBarrel.layer = 10;
        }
        new WaitForSeconds(0.5f);

        // SEGUIR PLAYER
        float lockOnCD = headCannonCadence -0.5f;
        Vector3 direction;
        if(mainCannonBarrel != null) AudioSource.PlayClipAtPoint(mainCannonCharge, Camera.main.transform.position, 0.3f);
        GetComponent<ParticleSystem>().Play();
        do
        {
            lockOnCD -= Time.deltaTime;
            if (mainCannonBarrel != null)
            {                
                direction = new Vector3(player.transform.position.x - mainCannon.transform.position.x, player.transform.position.y - mainCannon.transform.position.y);
                mainCannon.transform.up = Vector3.Lerp(mainCannon.transform.up, -direction, Time.deltaTime);
            }
            yield return null;
        } while (lockOnCD > 0);

        //ATIRAR
        for (int i = 0; i < mainCannonSalvo; i++)
        {
            if (player != null && mainCannonBarrel != null)
            {
                GameObject newMainCannonBullet = Instantiate(mainCannonBullet, mainCannonFP.transform.position, mainCannonFP.rotation);
                newMainCannonBullet.GetComponent<Rigidbody2D>().velocity = newMainCannonBullet.transform.up * -mainCannonBulletSpeed;
                AudioSource.PlayClipAtPoint(mainCannonSFX, Camera.main.transform.position, 0.1f);
                yield return new WaitForSeconds(mainCannonCadence);
            }

        }
        yield return new WaitForSeconds(0.5f);

        // VOLTAR À ROTAÇÃO 0
        do
        {
            if (mainCannonBarrel != null) mainCannon.transform.up = Vector3.Lerp(mainCannon.transform.up, Vector3.up, Time.deltaTime * 5);
            yield return null;
        } while (mainCannonBarrel != null && mainCannon.transform.up != Vector3.up);


        // ANIMAÇÃO DE DESARMAR
        yield return new WaitForSeconds(1f);
        if (mainCannonBarrel != null) AudioSource.PlayClipAtPoint(topGunMoveSFX, Camera.main.transform.position, 0.3f);
        if (mainCannonBarrel != null)
        {
            do
            {
                if (mainCannonBarrel!=null) mainCannonBarrel.transform.localScale -= new Vector3(0, 0.01f, 0);
                yield return null;
            }
            while (mainCannonBarrel.transform.localScale.y > 0.2f);
            do
            {
                if (mainCannonBarrel != null) mainCannon.transform.position -= new Vector3(0, 0.01f, 0);
                yield return null;
            }
            while (mainCannonBarrel != null && mainCannon.transform.localPosition.y > 0.75f);
            if (mainCannonBarrel != null) mainCannonBarrel.layer = 2;
        }

        // FIM
        yield return new WaitForSeconds(0.5f);
        mainCannonFireCountdown = mainCannonOriginalCD;
        mainCannonIsFiring = false;
        weaponTurn = 1;
    }

    public void AddPartDestroyed()
    {
        partsDestroyed += 1;
    }

    private IEnumerator Death()
    {
        if (laser != null) Destroy(laser);
        camshake.CameraShake(0.2f, 3f);
        float explosionTimer;
        float startTime = Time.time;
        do
        {
            explosionTimer = Time.time - startTime;
            float randomX = Random.Range(transform.position.x - 1, transform.position.x + 1);
            float randomY = Random.Range(transform.position.y - 3, transform.position.y + 3);
            Vector3 randomPos = new Vector3(randomX, randomY, -1);
            GameObject newExplosion = Instantiate(smallExplosion, randomPos, Quaternion.identity);
            Destroy(newExplosion, 0.6f);
            AudioSource.PlayClipAtPoint(sexpAudio, randomPos, 1f);
            yield return new WaitForSeconds(0.25f);

        } while (explosionTimer < 3);

        Player.instance.StopAllCoroutines();
        Player.instance.ToggleMove(false);
        camshake.CameraShake(1f, 3f);
        GameObject bigExp = Instantiate(bigExplosion, transform.position, Quaternion.identity);
        Destroy(bigExp, 6f);
        AudioSource.PlayClipAtPoint(bexpAudio, Camera.main.transform.position, 1f);
        yield return new WaitForSeconds(0.3f);
        flashScreen.color = Color.white;
        flashScreen.CrossFadeAlpha(1, 0.1f, false);
        yield return new WaitForSeconds(0.2f);
        Destroy(this.gameObject);
        flashScreen.CrossFadeAlpha(0, 0.5f, false);        

        Game.game.CallEnding();
    }
    
}
