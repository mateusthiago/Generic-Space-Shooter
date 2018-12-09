using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] int startingWave = 0;
    [SerializeField] List<WaveConfig> waveConfigList;
    [SerializeField] bool looping = false;
	
	IEnumerator Start ()
    {
        do
        {
            yield return StartCoroutine(SpawnAllWaves());
        }
        while (looping);
	}

    private IEnumerator SpawnAllWaves()
    {
        for (int waveIndex = startingWave; waveIndex < waveConfigList.Count; waveIndex++)
        {
            var currentWave = waveConfigList[waveIndex];
            if (currentWave.GetWait()) yield return StartCoroutine(SpawnAllEnemiesInWave(currentWave));
            else StartCoroutine(SpawnAllEnemiesInWave(currentWave));
            yield return new WaitForSeconds(currentWave.GetDelayForNextWave());
        }
    }

    private IEnumerator SpawnAllEnemiesInWave(WaveConfig waveConfig)
    {
        for (int spawnCount = 1; spawnCount <= waveConfig.GetNumberOfSpawns(); spawnCount++)
        {
            GameObject newEnemy = Instantiate(waveConfig.GetEnemyPrefab(), waveConfig.GetWaypointList()[0].transform.position, Quaternion.identity);
            newEnemy.GetComponent<EnemyPathing>().SetWaveConfig(waveConfig);
            yield return new WaitForSeconds(waveConfig.GetSpawnInterval());
        }
    }
}
