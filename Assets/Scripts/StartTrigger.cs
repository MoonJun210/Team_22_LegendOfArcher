using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

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
            EventManager.Instance.TriggerEvent("GetPlayerPosition", _player);
            EventManager.Instance.TriggerEvent("InitPlayerSpawned", _player);
            
        }
    }

    void OnPlayerSpawned(GameObject player)
    {
        _player = player;   
    }
}
