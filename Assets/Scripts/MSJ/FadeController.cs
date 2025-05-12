using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FadeController : MonoBehaviour
{
    Image sr;

    private void Awake()
    {
        EventManager.Instance.RegisterEvent<float>("FadeIn", FadeIn);
        EventManager.Instance.RegisterEvent<float>("FadeOut", FadeOut);

        sr = GetComponent<Image>();


    }

    private void Start()
    {
        EventManager.Instance.TriggerEvent("FadeOut", 1f);
    }

    private void OnDestroy()
    {
        EventManager.Instance.UnregisterEvent<float>("FadeIn", FadeIn);
        EventManager.Instance.UnregisterEvent<float>("FadeOut", FadeOut);

    }

    public void FadeIn(float fadeOutTime)
    {
        StartCoroutine(CoFadeIn(fadeOutTime));
    }

    public void FadeOut(float fadeOutTime)
    {
        StartCoroutine(CoFadeOut(fadeOutTime));
    }

    // 투명 -> 불투명
    IEnumerator CoFadeIn(float fadeOutTime, System.Action nextEvent = null)
    {

        while (sr.fillAmount < 1f)
        {
            sr.fillAmount += Time.deltaTime / fadeOutTime;

            if (sr.fillAmount >= 1f) sr.fillAmount = 1f;

            yield return null;
        }

        if (nextEvent != null) nextEvent();
    }

    // 불투명 -> 투명
    IEnumerator CoFadeOut(float fadeOutTime, System.Action nextEvent = null)
    {

        while (sr.fillAmount > 0f)
        {
            sr.fillAmount -= Time.deltaTime / fadeOutTime;

            if (sr.fillAmount <= 0f) sr.fillAmount = 0f;

            yield return null;
        }
        if (nextEvent != null) nextEvent();
    }
}

