using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public GameObject[] enemyPrefabs;
    public GameObject[] powerUpPrefabs;
    float rangeSpawn = 9.0f;
    int enemyCount = 0;
    public int waveNumber = 1;
    public int waveBoss = 2;
    public GameObject bossPrefab;
    public GameObject[] miniEnemyPrefabs;
    // Start is called before the first frame update
    void Start()
    {
        int index = generateRandomIndex(powerUpPrefabs.Length);

        Instantiate(powerUpPrefabs[index], GenerateSpawnPosition(), powerUpPrefabs[index].transform.rotation);
        spawnEnemyWave(waveNumber);
    }

    int generateRandomIndex(int size)
    {
        return UnityEngine.Random.Range(0, size);
    }

    void spawnEnemyWave(int enemyNumber)
    {
        for(int i = 0; i < enemyNumber; i++)
        {
            int index = generateRandomIndex(enemyPrefabs.Length);

            Instantiate(enemyPrefabs[index], GenerateSpawnPosition(), enemyPrefabs[index].transform.rotation);
            
        }
    }

    public void SpawnMiniEnemy(int enemyNumber)
    {
        for (int i = 0; i < enemyNumber; i++)
        {
            int index = generateRandomIndex(miniEnemyPrefabs.Length);

            Instantiate(miniEnemyPrefabs[index], GenerateSpawnPosition(), miniEnemyPrefabs[index].transform.rotation);

        }
    }

    void spawnBossWave(int currentWave)
    {
        int miniEnemiesCount;

        if (waveBoss != 0)
        {
            miniEnemiesCount = currentWave / waveBoss;
        }
        else
        {
            miniEnemiesCount = 1;
        }
        var boss = Instantiate(bossPrefab, GenerateSpawnPosition(), bossPrefab.transform.rotation);
        boss.GetComponent<Enemy>().miniEnemySpawnCount = miniEnemiesCount;
    }

    // Update is called once per frame
    void Update()
    {
        enemyCount = FindObjectsOfType<Enemy>().Length;
        if(enemyCount == 0)
        {
            int index = generateRandomIndex(powerUpPrefabs.Length);
            Instantiate(powerUpPrefabs[index], GenerateSpawnPosition(), powerUpPrefabs[index].transform.rotation);
            if(waveNumber % waveBoss == 0)
            {
                spawnBossWave(++waveNumber);
            }else
            spawnEnemyWave(++waveNumber);
        }
    }

    Vector3 GenerateSpawnPosition() {
        float spawnPosX = UnityEngine.Random.Range(-rangeSpawn, rangeSpawn);
        float spawnPosZ = UnityEngine.Random.Range(-rangeSpawn, rangeSpawn);

        Vector3 spawnPos = new Vector3(spawnPosX, 0, spawnPosZ);

        return spawnPos; 
    }
}
