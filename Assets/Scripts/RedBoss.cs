using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RedBoss : MonoBehaviour
{
    [Header("Boss")]
    [SerializeField] GameObject boss;
    //[SerializeField] int health = 50000;
    [SerializeField] int stage = 1;

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
    [SerializeField] GameObject headGunL;
    [SerializeField] GameObject headGunR;
    [SerializeField] GameObject headGunsBullet;
    [SerializeField] AudioClip headGunsSFX;
    [SerializeField] int headGunsBulletSpeed;
    [SerializeField] float headGunsFireCountdown;
    [SerializeField] float headGunsCadence;
    [SerializeField] int headGunsSalvo;
    bool headGunsAreFiring = false;
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
    float headGunOriginalCD;

    [Header("Fire Points")]
    [SerializeField] Transform topGunFPL;
    [SerializeField] Transform topGunFPR;
    [SerializeField] Transform headGunLFP;
    [SerializeField] Transform headGunRFP;
    [SerializeField] Transform headCannonFP;
    [SerializeField] Transform wingGunLFP;
    [SerializeField] Transform wingGunRFP;
    [SerializeField] Transform mainCannonFP;

    [Header("Debug")]
    Transform player;
    public static RedBoss redBoss;
    public int partsDestroyed = 0;


    void Start ()
    {
        redBoss = this;
        topGunOriginalCD = topGunFireCountdown;
        headGunOriginalCD = headGunsFireCountdown;
        player = FindObjectOfType<Player>().transform;        
	}
	
	
	void Update ()
    {
        MoveToPlayer();
        if (topGun != null) topGunFireCountdown -= Time.deltaTime;
        if (topGunFireCountdown < 0 && !topGunIsFiring) { StartCoroutine(TopGunFire()); topGunIsFiring = true; }        
        if (headGunL != null || headGunR != null) headGunsFireCountdown -= Time.deltaTime;
        if (headGunsFireCountdown < 0 && !headGunsAreFiring) { StartCoroutine(HeadGunsFire()); headGunsAreFiring = true; }
        //headCannonFireCountdown -= Time.deltaTime;
        //if (headCannonFireCountdown < 0) StartCoroutine(HeadCannonFire());
        //wingGunsFireCountdown -= Time.deltaTime;
        //if (wingGunsFireCountdown < 0) StartCoroutine(WingGunsFire());
        //mainCannonFireCountdown -= Time.deltaTime;
        //if (mainCannonFireCountdown < 0) StartCoroutine(MainCannonFire());

    }

    private void MoveToPlayer()
    {
        float distToPlayer = player.position.x - transform.position.x;
        transform.position += new Vector3(distToPlayer, 0, 0).normalized * Time.deltaTime * 0.5f;
        if (transform.position.y > 5.0f)
        {
            transform.position += new Vector3(0, -0.5f, 0) * Time.deltaTime;
        }
    }

    IEnumerator TopGunFire()
    {
        transform.Find("topGunHatch").gameObject.GetComponent<Animator>().SetTrigger("open");
        yield return new WaitForSeconds(1.5f);
        do
        {
            topGun.transform.localScale += new Vector3(0.01f, 0.01f, 0);
            yield return new WaitForSeconds(Time.deltaTime);            
        } while (topGun.transform.localScale.x < 1);

        AudioSource.PlayClipAtPoint(topGunMoveSFX, Camera.main.transform.position, 0.2f);
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
            //GameObject newTopGunBulletL = Instantiate(topGunBullet, topGun.transform.position + new Vector3(-0.13f, -0.6f, 0), topGun.transform.rotation);            
            newTopGunBulletL.GetComponent<Rigidbody2D>().velocity = newTopGunBulletL.transform.up * -topGunBulletSpeed;
            newTopGunBulletL.GetComponent<Rigidbody2D>().AddTorque(400f);
            GameObject newTopGunBulletR = Instantiate(topGunBullet, topGunFPR.position, topGunFPR.rotation);
            //GameObject newTopGunBulletR = Instantiate(topGunBullet, topGun.transform.position + new Vector3(0.13f, -0.6f, 0), topGun.transform.rotation);
            newTopGunBulletR.GetComponent<Rigidbody2D>().velocity = newTopGunBulletR.transform.up * -topGunBulletSpeed;
            newTopGunBulletR.GetComponent<Rigidbody2D>().AddTorque(400f);
            AudioSource.PlayClipAtPoint(topGunBulletSFX, Camera.main.transform.position, 0.07f);
            
            t += (Time.deltaTime + topGunCadence) / (topGunCadence * topGunSalvo);
            float lerp = Mathf.Lerp(topGunRotation, topGunRotation*-1, t);            
            topGun.transform.rotation = Quaternion.Euler(0, 0, lerp);
            yield return new WaitForSeconds(topGunCadence);

        }

        
        AudioSource.PlayClipAtPoint(topGunMoveSFX, Camera.main.transform.position, 0.2f);
        t = 0;
        do
        {
            t += Time.deltaTime;
            float smooth = Mathf.SmoothStep(topGunRotation*-1, 0, t);
            topGun.transform.rotation = Quaternion.Euler(0, 0, smooth);
            yield return new WaitForSeconds(Time.deltaTime);

        } while (t < 1);

        yield return new WaitForSeconds(1.3f);
        do
        {
            topGun.transform.localScale += new Vector3(-0.01f, -0.01f, 0);
            yield return new WaitForSeconds(Time.deltaTime);
        } while (topGun.transform.localScale.x > 0.4);
        transform.Find("topGunHatch").gameObject.GetComponent<Animator>().SetTrigger("close");
        

        topGunRotation *= -1;
        topGunFireCountdown = topGunOriginalCD;            
        topGunIsFiring = false;       
    }

    IEnumerator HeadGunsFire()
    {
        for (int i = 0; i < headGunsSalvo; i++)
        {
            if (headGunL != null)
            {
                GameObject newHeadGunBulletL = Instantiate(headGunsBullet, headGunLFP.position, Quaternion.identity);
                newHeadGunBulletL.GetComponent<Rigidbody2D>().velocity = newHeadGunBulletL.transform.up * -headGunsBulletSpeed;
                AudioSource.PlayClipAtPoint(headGunsSFX, Camera.main.transform.position, 0.05f);
                //headGunL.transform.position += new Vector3(0, 0.1f, 0);
                //yield return new WaitForSeconds(Time.deltaTime);
                //headGunL.transform.position -= new Vector3(0, 0.1f, 0);
                yield return new WaitForSeconds(headGunsCadence);
            }            
            
            if (headGunR != null)
            {
                GameObject newHeadGunBulletR = Instantiate(headGunsBullet, headGunRFP.position, Quaternion.identity);
                newHeadGunBulletR.GetComponent<Rigidbody2D>().velocity = newHeadGunBulletR.transform.up * -headGunsBulletSpeed;
                AudioSource.PlayClipAtPoint(headGunsSFX, Camera.main.transform.position, 0.05f);
                //headGunR.transform.position += new Vector3(0, 0.1f, 0);                
                //yield return new WaitForSeconds(Time.deltaTime);
                //headGunR.transform.position -= new Vector3(0, 0.1f, 0);
                yield return new WaitForSeconds(headGunsCadence);
            }
        }
        headGunsFireCountdown = headGunOriginalCD;
        headGunsAreFiring = false;
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

    public void AddPartDestroyed()
    {
        partsDestroyed += 1;
        if (partsDestroyed == 3) SetStage(2);
    }
    
}
