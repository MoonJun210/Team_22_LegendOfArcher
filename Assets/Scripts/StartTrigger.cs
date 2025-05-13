using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartTrigger : MonoBehaviour
{
    GameObject _player;
    private void Awake()
    {
        EventManager.Instance.RegisterEvent<GameObject>("OnPlayerSpawned", OnPlayerSpawned);

    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Player")
        {
            MapManager.MapInstance.ChagneMapCondition(1);
            EventManager.Instance.TriggerEvent("GetPlayerPosition", _player);
            EventManager.Instance.TriggerEvent("InitPlayerSpawned", _player);
            gameObject.SetActive(false);
        }
    }

    void OnPlayerSpawned(GameObject player)
    {
        _player = player;
        Debug.Log("플레이어 등록");
    }
}
