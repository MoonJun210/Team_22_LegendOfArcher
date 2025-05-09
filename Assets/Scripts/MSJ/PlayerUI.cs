using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
    public GameObject healthPanel;
    public GameObject healthImg; // 프리펩

    private PlayerStatHandler playerStatHandler;

    private void Awake()
    {
        playerStatHandler = GetComponent<PlayerStatHandler>();
       
    }

    private void Start()
    {
        InitHealthImg();
    }

    void InitHealthImg()
    {
        UpdateHealthImg();
    }

    public void UpdateHealthImg()
    {
        foreach (Transform child in healthPanel.transform)
        {
            Destroy(child.gameObject);
        }

        for (int i = 0; i < playerStatHandler.Health; i++)
        {
            Instantiate(healthImg, healthPanel.transform);
        }
    }

}
