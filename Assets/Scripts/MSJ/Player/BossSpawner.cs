using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossSpawner : MonoBehaviour
{
    public static BossSpawner Instance;

    public GameObject[] bosses;
    public GameObject[] spawnPos;
    int bossNum = 0;

    private void Awake()
    {
        Instance = this;
    }

    public void SpawnBoss()
    {
        Instantiate(bosses[bossNum], spawnPos[bossNum].transform.position, Quaternion.identity);
        bossNum++;
    }
}
