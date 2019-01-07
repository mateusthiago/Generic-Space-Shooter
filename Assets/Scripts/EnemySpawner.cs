using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class EnemySpawner : MonoBehaviour
{
    [SerializeField] int startingWave = 0;
    [SerializeField] List<WaveConfig> waveConfigList;
    [SerializeField] GameObject redBoss;

    int enemyCount;

    private void Start()
    {
        StartCoroutine(SpawnAllWaves());
    }

    private IEnumerator SpawnAllWaves()
    {
        for (int waveIndex = startingWave; waveIndex < waveConfigList.Count; waveIndex++)
        {
            var currentWave = waveConfigList[waveIndex];
            if (currentWave.GetWaitForAllEnemiesToSpawn()) yield return StartCoroutine(SpawnAllEnemiesInWave(currentWave));
            else StartCoroutine(SpawnAllEnemiesInWave(currentWave));
            if (currentWave.GetWaitForAllEnemiesToDie())
            {
                while (enemyCount > 0) yield return new WaitForSeconds(0.2f);                
            }            
            yield return new WaitForSeconds(currentWave.GetDelayForNextWave());
        }
    }

    private IEnumerator SpawnAllEnemiesInWave(WaveConfig waveConfig)
    {
        yield return new WaitForSeconds(waveConfig.GetWaitBeforeStart());

        for (int spawnCount = 1; spawnCount <= waveConfig.GetNumberOfSpawns(); spawnCount++)
        {
            if (waveConfig.isBoss == false)
            {
                GameObject newEnemy = Instantiate(waveConfig.GetEnemyPrefab(), waveConfig.GetWaypointList()[0].transform.position, waveConfig.GetEnemyPrefab().transform.rotation);
                newEnemy.GetComponent<EnemyPathing>().SetWaveConfig(waveConfig);
                if (spawnCount == waveConfig.GetNumberOfSpawns())
                {
                    Enemy newEnemyScript = newEnemy.GetComponent<Enemy>();

                    if (waveConfig.DifDropChance() == true) newEnemyScript.SetDropChance(waveConfig.GetDropChance());

                    if (waveConfig.IsSectorEnd() == true)
                    {
                        newEnemyScript.SetLastInSector(true);
                        newEnemyScript.SetNextSector(waveConfig.GetNextSector());
                    }
                }
            }                
            else if (waveConfig.isBoss == true) redBoss.SetActive(true);
            
            
            yield return new WaitForSeconds(waveConfig.GetSpawnInterval());
        }
    }    

    public void SetLastSector(int sector)
    {
        switch (sector)
        {
            case 1: break;
            case 2: startingWave = 14; break;
            case 3: startingWave = 41; break;
            case 4: startingWave = 58; break;
        }
    }

    public void AddEnemyCount() { enemyCount += 1; }
    public void SubtractEnemyCount() { enemyCount -= 1; }

    //private void OnGUI()
    //{
    //    GUI.Label(new Rect(10, 10, 100, 20), enemyCount.ToString());
    //}
}
