using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Wave Config")]

public class WaveConfig : ScriptableObject
{

    [SerializeField] GameObject enemyPrefab;
    [SerializeField] GameObject pathPrefab;
    [SerializeField] float spawnInterval = 0.5f;
    [SerializeField] float randomInterval = 0.3f;
    [SerializeField] int numberOfSpawns = 5;
    [SerializeField] float moveSpeed = 2f;
    [SerializeField] float delayForNextWave = 1f;

    public GameObject GetEnemyPrefab() { return enemyPrefab; }

    public float GetSpawnInterval() { return spawnInterval;  }

    public float GetRandomInterval() { return randomInterval;  }

    public int GetNumberOfSpawns() { return numberOfSpawns;  }

    public float GetMoveSpeed() { return moveSpeed;  }

    public float GetDelayForNextWave() { return delayForNextWave; }

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
