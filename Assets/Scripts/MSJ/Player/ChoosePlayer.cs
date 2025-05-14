using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ChoosePlayer : MonoBehaviour
{
    Animator[] anims;

    private void Awake()
    {
        anims = GetComponentsInChildren<Animator>();
    }

    public void ChoosePlayerNum(int num)
    {
        GameManager.instance.playerNum = num;
        anims[num].SetTrigger("IsClick");
        StartCoroutine(LoadNextScene());
    }

    IEnumerator LoadNextScene()
    {
        EventManager.Instance.TriggerEvent("FadeIn", 0.7f);
        yield return new WaitForSeconds(1f);

        SceneManager.LoadScene("TestCombine_3");
    }
}
