using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerSpawner : MonoBehaviour
{

    public GameObject[] playesr;
    public GameObject spawnPos;
    [SerializeField] private Button basicPlayerBtn;
    [SerializeField] private Button elfBtn;
    [SerializeField] private Button dwarfBtn;

    GameObject _player;

    private void Start()
    {
        basicPlayerBtn.onClick.AddListener(SpawnBasicPlayer);
        elfBtn.onClick.AddListener(SpawnElfPlayer);
        dwarfBtn.onClick.AddListener(SpawnDwarfPlayer);
    }

    public void SpawnBasicPlayer()
    {
        this.gameObject.SetActive(false);
        _player = Instantiate(playesr[0], spawnPos.transform.position, Quaternion.identity);
        EventManager.Instance.TriggerEvent("SearchTarget", _player);
    }

    public void SpawnElfPlayer()
    {
        this.gameObject.SetActive(false);
        _player = Instantiate(playesr[1], spawnPos.transform.position, Quaternion.identity);
        EventManager.Instance.TriggerEvent("SearchTarget", _player);
    }

    public void SpawnDwarfPlayer()
    {
        this.gameObject.SetActive(false);
        _player = Instantiate(playesr[2], spawnPos.transform.position, Quaternion.identity);
        EventManager.Instance.TriggerEvent("SearchTarget", _player);
    }
}
