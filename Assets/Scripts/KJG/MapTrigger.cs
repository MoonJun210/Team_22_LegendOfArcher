using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapTrigger : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            MapManager.MapInstance.ChagneMapCondition(1);

        }
        else if (collision.CompareTag("Key"))
        {
            MapManager.MapInstance.ChagneMapCondition(2);
            gameObject.SetActive(false);
        }
    }
}
