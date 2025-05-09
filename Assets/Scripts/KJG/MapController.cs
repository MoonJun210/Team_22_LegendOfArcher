using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapController : MonoBehaviour
{
    [SerializeField] private GameObject StartDesign;
    [SerializeField] private GameObject StartCollision;
    [SerializeField] private GameObject[] EndDesigns;
    [SerializeField] private GameObject EndCollision;
    [SerializeField] private GameObject[] StageObjects;

    public void BattleStart(int stage)
    {
        StartDesign.SetActive(true);
        StartCollision.SetActive(true);

        MapStageStart(stage);

        if (EndDesigns != null)
        {
            foreach (GameObject dummy in EndDesigns)
            {
                dummy.SetActive(false);
            }
        }

        if (EndCollision != null) { EndCollision.SetActive(false); }
    }

    public void BattleEnd(int stage)
    {
        StartDesign.SetActive(false);
        StartCollision.SetActive(false);

        MapStageEnd(stage);

        foreach (GameObject dummy in EndDesigns)
        {
            dummy.SetActive(true);
        }

        EndCollision.SetActive(true);
    }

    public void MapStageStart(int stage)
    {
        switch (stage)
        {
            case 1:
                StageObjects[0].SetActive(false);
                break;
        }

    }

    public void MapStageEnd(int stage)
    {
        switch (stage)
        {
            case 1:
                StageObjects[0].SetActive(true);
                StageObjects[1].SetActive(true);
                StageObjects[2].SetActive(true);
                break;
        }

    }
}
