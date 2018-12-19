using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RedBoss : MonoBehaviour
{
    [Header("Boss")]
    [SerializeField] GameObject boss;
    [SerializeField] int health = 50000;
    [SerializeField] int stage = 1;

    [Header("Guns")]
    [SerializeField] GameObject topGun;
    [SerializeField] GameObject topGunBullet;
    [SerializeField] float topGunBulletSpeed;
    [SerializeField] float topGunFireCountdown;
    [SerializeField] int topGunSalvo;
    [SerializeField] float topGunCadence;
    float topGunRotation = 45;
    bool topGunIsFiring = false;
    [SerializeField] GameObject headGunL;
    [SerializeField] GameObject headGunR;
    [SerializeField] float headGunsFireCountdown;
    [SerializeField] float headGunsCadence;
    [SerializeField] int headGunsSalvo;
    [SerializeField] GameObject headCannon;
    [SerializeField] float headCannonFireCountdown;
    [SerializeField] float headCannonCadence;
    [SerializeField] int headCannonSalvo;
    [SerializeField] GameObject wingGunL;    
    [SerializeField] GameObject wingGunR;
    [SerializeField] float wingGunsFireCountdown;
    [SerializeField] float wingGunsCadence;
    [SerializeField] int wingGunsSalvo;
    [SerializeField] GameObject mainCannon;
    [SerializeField] float mainCannonFireCountdown;
    [SerializeField] float mainCannonCadence;
    [SerializeField] int mainCannonSalvo;
    
    float topGunOriginalCD;

    [Header("Fire Points")]
    [SerializeField] Transform topGunFPL;
    [SerializeField] Transform topGunFPR;
    [SerializeField] Transform headGunLFP;
    [SerializeField] Transform headGunRFP;
    [SerializeField] Transform headCannonFP;
    [SerializeField] Transform wingGunLFP;
    [SerializeField] Transform wingGunRFP;
    [SerializeField] Transform mainCannonFP;


    Transform player;   


    void Start ()
    {        
        topGunOriginalCD = topGunFireCountdown;
        player = FindObjectOfType<Player>().transform;        
	}
	
	
	void Update ()
    {
        topGunFireCountdown -= Time.deltaTime;
        if (topGunFireCountdown < 0 && !topGunIsFiring) { StartCoroutine(TopGunFire()); topGunIsFiring = true; }        
        //headGunsFireCountdown -= Time.deltaTime;
        //if (headGunsFireCountdown < 0) StartCoroutine(HeadGunsFire());
        //headCannonFireCountdown -= Time.deltaTime;
        //if (headCannonFireCountdown < 0) StartCoroutine(HeadCannonFire());
        //wingGunsFireCountdown -= Time.deltaTime;
        //if (wingGunsFireCountdown < 0) StartCoroutine(WingGunsFire());
        //mainCannonFireCountdown -= Time.deltaTime;
        //if (mainCannonFireCountdown < 0) StartCoroutine(MainCannonFire());

    }

    IEnumerator TopGunFire()
    {                       
        float t = 0;
                
        do
        {
            t += Time.deltaTime;            
            float smooth = Mathf.SmoothStep(0, topGunRotation, t);
            topGun.transform.rotation = Quaternion.Euler(0, 0, smooth);            
            yield return new WaitForSeconds(Time.deltaTime);

        } while (t < 1);

        yield return new WaitForSeconds(0.5f);

        t = 0;
        for (int i = 0; i < topGunSalvo; i++)
        {            
            GameObject newTopGunBulletL = Instantiate(topGunBullet, topGunFPL.position, topGunFPL.rotation);
            GameObject newTopGunBulletR = Instantiate(topGunBullet, topGunFPR.position, topGunFPR.rotation);
            newTopGunBulletL.GetComponent<Rigidbody2D>().velocity = newTopGunBulletL.transform.up * -topGunBulletSpeed;
            newTopGunBulletL.GetComponent<Rigidbody2D>().AddTorque(400f);
            newTopGunBulletR.GetComponent<Rigidbody2D>().velocity = newTopGunBulletL.transform.up * -topGunBulletSpeed;
            newTopGunBulletR.GetComponent<Rigidbody2D>().AddTorque(400f);
            
            t += (Time.deltaTime + topGunCadence) / (topGunCadence * topGunSalvo);
            float lerp = Mathf.Lerp(topGunRotation, topGunRotation*-1, t);            
            topGun.transform.rotation = Quaternion.Euler(0, 0, lerp);
            yield return new WaitForSeconds(topGunCadence);

        }

        t = 0;
        do
        {
            t += Time.deltaTime;
            float smooth = Mathf.SmoothStep(topGunRotation*-1, 0, t);
            topGun.transform.rotation = Quaternion.Euler(0, 0, smooth);
            yield return new WaitForSeconds(Time.deltaTime);

        } while (t < 1);

        topGunRotation *= -1;
        topGunFireCountdown = topGunOriginalCD;            
        topGunIsFiring = false;       
    }

    IEnumerator HeadGunsFire()
    {
        return null;
    }

    IEnumerator HeadCannonFire()
    {
        return null;
    }

    IEnumerator WingGunsFire()
    {
        return null;
    }

    IEnumerator MainCannonFire()
    {
        return null;
    }

    public void SetStage(int newStage)
    {
        stage = newStage;
    }

    
}
