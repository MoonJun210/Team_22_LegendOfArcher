using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public int mapCondition = 0;
    public int currentStatge = 0;

    public static GameManager instance;

    GameObject _player;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);

      
    }

    public void OnPlayerSpawned(GameObject player)
    {
        _player = player;

        EventManager.Instance.TriggerEvent("SearchTarget", _player);
        EventManager.Instance.TriggerEvent("OnPlayerSpawned", _player);
    }

    private void OnDisable()
    {
        EventManager.Instance.UnregisterAllEvents();
    }
}
