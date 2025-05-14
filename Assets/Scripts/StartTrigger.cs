using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartTrigger : MonoBehaviour
{

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Player")
        {
            SoundManager.instance.ChangeBackGroundMusic($"Boss{BossSpawner.Instance.bossNum + 1}BGM");
            MapManager.MapInstance.ChagneMapCondition(1);
            BossSpawner.Instance.SpawnBoss();
            EventManager.Instance.TriggerEvent("InitPlayerSpawned", GameManager.instance._player);
            gameObject.SetActive(false);
           
        }
    }

  
}
