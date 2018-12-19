using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPathing : MonoBehaviour
{
    WaveConfig waveConfig;
    List<Transform> waypointList;    
    int waypointIndex = 1;    

    [SerializeField] bool blackSaucerBehavior = false;
    [SerializeField] bool voidShipBehavior = false;
    bool isFiring = false;
    Enemy enemyScript;

    // Use this for initialization
	void Start ()
    {
        if (blackSaucerBehavior || voidShipBehavior)
        {
            enemyScript = GetComponent<Enemy>();
        }
        if (waveConfig)
        {
            waypointList = waveConfig.GetWaypointList();
            //transform.position = waypointList[waypointIndex].transform.position;
        }            
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (waveConfig)
            if (!isFiring)
                StartCoroutine(Move());
    }

    public void SetWaveConfig(WaveConfig waveConfigPassed)
    {
        waveConfig = waveConfigPassed;
    }

    private IEnumerator Move()
    {
        if (waypointIndex <= waypointList.Count - 1)
        {
            var targetPosition = waypointList[waypointIndex].transform.position;
            var movementThisFrame = waveConfig.GetMoveSpeed() * Time.deltaTime;
            transform.position = Vector2.MoveTowards(transform.position, targetPosition, movementThisFrame);
            if (transform.position == targetPosition)
            {
                if (blackSaucerBehavior && waypointIndex < waypointList.Count -1)
                {
                    isFiring = true;
                    yield return StartCoroutine(enemyScript.SaucerFire());
                    isFiring = false;
                }
                else if (voidShipBehavior && waypointIndex < waypointList.Count - 1)
                {
                    isFiring = true;
                    yield return StartCoroutine(enemyScript.VoidFire());
                    isFiring = false;
                }

                if (voidShipBehavior && waypointIndex == waypointList.Count - 2)
                {
                    waypointIndex = 0;
                }

                waypointIndex++;
            }
        }
        else
        {
            Destroy(gameObject);
        }
        yield return null;
    }
   
}
