using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapController : MonoBehaviour
{
    [SerializeField] private GameObject[] MapConditionObj;
    [SerializeField] private GameObject EndSpawnPoint;
    [SerializeField] private GameObject BattleOff;
    [SerializeField] private GameObject[] StartSign;
    // [SerializeField] private GameObject EndCollision;
    // [SerializeField] private GameObject[] StageObjects;

    void Awake()
    {
        
    }
    private void Update()
    {
        
    }

    public void BattleSetting()
    {
        MapConditionObj[0].SetActive(true);
        MapConditionObj[1].SetActive(false);
        MapConditionObj[2].SetActive(false);
        GameManager.instance.mapCondition = 0;
    }

    public void BattleStart()
    {

        MapConditionObj[0].SetActive(false);
        MapConditionObj[1].SetActive(true);
        BattleOff.SetActive(false);

        GameManager.instance.mapCondition = 1;
        if (MapConditionObj[2] != null) { MapConditionObj[2].SetActive(false); }
    }

    public void BattleEnd()
    {
        MapConditionObj[1].SetActive(false);
        MapConditionObj[2].SetActive(true);
        BattleOff.SetActive(true);

        GameManager.instance.mapCondition = 2;
    }


    public void isBattle()
    {

    }
}
