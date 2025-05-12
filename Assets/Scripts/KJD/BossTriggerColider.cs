using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossTriggerColider : MonoBehaviour
{
    private bool detectPlayer;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            detectPlayer = true;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            detectPlayer = false;
        }
    }
    public bool GetDetectPlayer()
        { return detectPlayer; }
}
