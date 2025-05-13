using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
    public GameObject healthPanel;
    public GameObject healthImg; // 프리펩
    public GameObject gameOverUI;

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

    public void OnDeath()
    {
        gameOverUI.SetActive(true);
    }

    public void RetryBtnClick()
    {
        StartCoroutine(GoToTitleScene());
    }
    IEnumerator GoToTitleScene()
    {
        EventManager.Instance.TriggerEvent("FadeIn", 0.7f);

        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene("MSJ_TitleScene");
    }

    public void ExitBtnClick()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;  // 에디터에서 플레이 모드 종료
#else
    Application.Quit();  // 빌드된 게임 종료
#endif
    }

}
