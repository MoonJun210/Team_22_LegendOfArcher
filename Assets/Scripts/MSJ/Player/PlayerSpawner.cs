using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerSpawner : MonoBehaviour
{
    public GameObject spawnPos;
    public GameObject[] players;
    GameObject _player;

    private void Start()
    {
        SpawnPlayer();
        BossSpawner.Instance.SpawnBoss();
        EventManager.Instance.TriggerEvent("FadeOut", 0.7f);
    }

    public void SpawnPlayer()
    {
        _player = Instantiate(players[GameManager.instance.playerNum], spawnPos.transform.position, Quaternion.identity);
        GameManager.instance.OnPlayerSpawned(_player);
    }

}
