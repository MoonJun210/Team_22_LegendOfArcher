using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossSpawner : MonoBehaviour
{
    public static BossSpawner Instance;

    public GameObject[] bosses;
    public GameObject[] spawnPos;
    public int bossNum = 0;

    public GameObject curBoss;

    private void Awake()
    {
        Instance = this;
    }

    public void SpawnBoss()
    {
        curBoss = Instantiate(bosses[bossNum], spawnPos[bossNum].transform.position, Quaternion.identity);
        bossNum++;
    }
}
