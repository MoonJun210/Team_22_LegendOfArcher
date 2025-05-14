using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public int mapCondition = 0;
    public int currentStatge = 0;

    public static GameManager instance;

    public int playerNum;
    public GameObject _player;
    public PlayerController _playerController;

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
        _playerController = _player.GetComponent<PlayerController>();

        EventManager.Instance.TriggerEvent("SearchTarget", _player);
    }

    public void CameraTargetToPlayer()
    {
        EventManager.Instance.TriggerEvent("SearchTarget", _player);
    }

    private void OnDisable()
    {
        EventManager.Instance.UnregisterAllEvents();
    }
}
