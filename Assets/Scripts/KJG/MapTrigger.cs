using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapTrigger : MonoBehaviour
{
    [SerializeField] private GameObject[] floorCollision;
    [SerializeField] Collider2D[] MapTigger;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (MapTigger[0] == collision)
        {
            if (collision.CompareTag("Player"))
            {
                floorCollision[0].SetActive(true);
                floorCollision[1].SetActive(false);
            }
        }
        else if (MapTigger[1] == collision)
        {
            if (collision.CompareTag("Player"))
            {
                floorCollision[0].SetActive(false);
                floorCollision[1].SetActive(true);
            }
        }
    }
}
