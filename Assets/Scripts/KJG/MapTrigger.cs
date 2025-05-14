using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapTrigger : MonoBehaviour
{
    [SerializeField] private GameObject[] floorCollision;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (this.tag == "floor1")
        {
            if (collision.CompareTag("Player"))
            {
                floorCollision[0].SetActive(true);
                floorCollision[1].SetActive(false);
            }
        }
        else if (this.tag == "floor2")
        {
            if (collision.CompareTag("Player"))
            {
                floorCollision[0].SetActive(false);
                floorCollision[1].SetActive(true);
            }
        }
    }
}
