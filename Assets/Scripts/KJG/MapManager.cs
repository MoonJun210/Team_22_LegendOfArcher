
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    MapController mapController;
    MapAnimation mapAnimation;

    public GameObject[] maps;
    public GameObject[] spawnPoints;
    [SerializeField] int stageLevel = 1;

    private static MapManager mapInstance;
    public static MapManager MapInstance
    {
        get { return mapInstance; }
    }

    private void Awake()
    {
        mapInstance = this;
        if (mapController == null)
        { mapController = GetComponentInChildren<MapController>(); }

        if (mapAnimation == null)
        { mapAnimation = GetComponentInChildren<MapAnimation>(); }
    }

    private void Start()
    {

    }

    public void ChagneMapCondition(int value)
    {
        switch (value)
        {
            case 0:
                mapController.BattleSetting();
                break;

            case 1:
                mapController.BattleStart();
                mapAnimation.TurnOnMap(1, false);
                mapAnimation.TurnOffMap(3);
                break;

            case 2:
                mapController.BattleEnd();
                mapAnimation.TurnOnMap(2, true);
                mapAnimation.TurnOffMap(1);
                mapAnimation.TurnOffMap(0);
                mapAnimation.TurnOnMap(3, false);
                break;
        }
    }

    public void TeleportNextPoint(GameObject player)
    {
        StartCoroutine(FadeEffect(player));
    }

    IEnumerator FadeEffect(GameObject player)
    {
        EventManager.Instance.TriggerEvent("FadeIn", 0.5f);
        yield return new WaitForSeconds(0.5f);
        stageLevel++;
        StageChange();
        BossSpawner.Instance.SpawnBoss();
        ChagneMapCondition(0);
        player.transform.position = spawnPoints[stageLevel - 1].transform.position + (Vector3.up * 1.2f);
        yield return new WaitForSeconds(0.6f);

        EventManager.Instance.TriggerEvent("FadeOut", 0.5f);

    }

    private void StageChange()
    {
        for (int i = 0; i < maps.Length; i++)
        {
            maps[i].SetActive(false);
        }
        maps[stageLevel - 1].SetActive(true);

        mapController = maps[stageLevel - 1].GetComponentInChildren<MapController>();
        mapAnimation = maps[stageLevel - 1].GetComponentInChildren<MapAnimation>();
    }
}


