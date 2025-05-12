using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    MapController mapController;
    MapAnimation mapAnimation;

    private static MapManager mapInstance;
    public static MapManager MapInstance
    {
        get { return mapInstance; }
    }

    private void Awake()
    {
        mapInstance = this;
        if (mapController == null)
            {mapController = GetComponentInChildren<MapController>();}

        if (mapAnimation == null)
            {mapAnimation = GetComponentInChildren<MapAnimation>();}
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
                mapAnimation.TurnOnMap(1);
                mapAnimation.TurnOffMap(0);
                mapAnimation.TurnOffMap(3);
                break;

            case 2:
                mapController.BattleEnd();
                mapAnimation.TurnOnMap(2);
                mapAnimation.TurnOffMap(1);
                mapAnimation.TurnOnMap(3);
                break;
        }
    }
}
