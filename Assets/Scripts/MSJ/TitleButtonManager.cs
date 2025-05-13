using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleButtonManager : MonoBehaviour
{
    public GameObject settingPanel;
    public GameObject startPanel;

    public void TurnOnSettingPanel()
    {
        settingPanel.SetActive(true);
    }

    public void TurnOffSettingPanel()
    {
        settingPanel.SetActive(false);
    }

    public void StartButton()
    {
        startPanel.SetActive(true);
    }

    public void ExitButton()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;  // 에디터에서 플레이 모드 종료
#else
    Application.Quit();  // 빌드된 게임 종료
#endif
    }

}
