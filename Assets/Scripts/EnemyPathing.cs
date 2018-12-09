using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPathing : MonoBehaviour
{
    WaveConfig waveConfig;
    List<Transform> waypointList;    
    int waypointIndex = 0;    

    // Use this for initialization
	void Start ()
    {
        if (waveConfig)
        {
            waypointList = waveConfig.GetWaypointList();
            transform.position = waypointList[waypointIndex].transform.position;
        }            
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (waveConfig)
            Move();
    }

    public void SetWaveConfig(WaveConfig waveConfigPassed)
    {
        waveConfig = waveConfigPassed;
    }

    private void Move()
    {
        if (waypointIndex <= waypointList.Count - 1)
        {
            var targetPosition = waypointList[waypointIndex].transform.position;
            var movementThisFrame = waveConfig.GetMoveSpeed() * Time.deltaTime;
            transform.position = Vector2.MoveTowards(transform.position, targetPosition, movementThisFrame);
            if (transform.position == targetPosition)
            {
                waypointIndex++;
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
