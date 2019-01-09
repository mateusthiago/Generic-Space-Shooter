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
            if (transform.childCount > 0) // contar children em caso de squads e destrui-los antes de destruir o object squad
            {
                for (int i = 0; i < transform.childCount; i++)
                {
                    var squadMember = transform.GetChild(i).GetComponent<Enemy>();
                    if (squadMember != null) squadMember.EnemyDeath(false);
                }
            }

            var enemy = GetComponent<Enemy>(); // verificar se o objeto é um inimigo
            if (enemy != null) enemy.EnemyDeath(false); // destruir o inimigo corretamente para subtrair do enemyCount em EnemySpawner

            else Destroy(this.gameObject); // destruir o objeto com este script (provavelmente um objeto vazio utilizado para formar squads)
        }
        yield return null;
    }
   
}
