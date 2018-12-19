using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Wave Config")]

public class WaveConfig : ScriptableObject
{

    [SerializeField] GameObject enemyPrefab;
    [SerializeField] GameObject pathPrefab;
    [SerializeField] float waitBeforeStart = 0f;
    [SerializeField] float spawnInterval = 0.5f;
    [SerializeField] float randomInterval = 0.3f;
    [SerializeField] int numberOfSpawns = 5;
    [SerializeField] float moveSpeed = 2f;
    [SerializeField] float delayForNextWave = 1f;    
    [SerializeField] bool waitForAllEnemiesToSpawn = true;
    [SerializeField] bool waitForAllEnemiesToDie = false;
    [SerializeField] bool AlterDropChance;
    [SerializeField] [Range(0, 1f)] float dropChance;    
    [SerializeField] bool isSectorEnd = false;
    [SerializeField] int nextSector = 0;
    public bool isBoss = false;
    

    public GameObject GetEnemyPrefab() { return enemyPrefab; }

    public float GetWaitBeforeStart() { return waitBeforeStart; }

    public float GetSpawnInterval() { return spawnInterval;  }

    public float GetRandomInterval() { return randomInterval;  }

    public int GetNumberOfSpawns() { return numberOfSpawns;  }

    public float GetMoveSpeed() { return moveSpeed;  }

    public float GetDelayForNextWave() { return delayForNextWave; }

    public bool GetWaitForAllEnemiesToSpawn() { return waitForAllEnemiesToSpawn; }

    public bool GetWaitForAllEnemiesToDie() { return waitForAllEnemiesToDie; }

    public bool DifDropChance()
    {
        return AlterDropChance;
    }

    public float GetDropChance()
    {
        return dropChance;
    }

    public bool IsSectorEnd() { return isSectorEnd; }

    public int GetNextSector() { return nextSector; }

    public List<Transform> GetWaypointList()
    {
        var waveWaypoints = new List<Transform>();
        foreach (Transform waypoint in pathPrefab.transform)
        {
            waveWaypoints.Add(waypoint);
        }
        return waveWaypoints;
    }

}
