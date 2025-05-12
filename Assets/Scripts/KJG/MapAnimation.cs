using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class MapAnimation : MonoBehaviour
{
    [SerializeField] private GameObject[] mapAnime;

    public void TurnOnMap(int value, bool trans)
    {
        StartCoroutine(FadeIn(value));
        if (trans == true) { StartCoroutine(ChangeMapTrans(value)); }
    }
    public void TurnOffMap(int value)
    {
        StartCoroutine(FadeOut(value));
    }

    IEnumerator FadeOut(int value)
    {
        SpriteRenderer[] renderer = mapAnime[value].GetComponentsInChildren<SpriteRenderer>();

        float time = 0f;
        while (time < 1f)
        {
            foreach (SpriteRenderer dummy in renderer)
            {

                Color dummyColor = dummy.color;
                dummyColor.a = Mathf.Lerp(1f, 0f, time);
                dummy.color = dummyColor;
            }
            time += Time.deltaTime;

            yield return null;
        }
        mapAnime[value].SetActive(false);
    }

    IEnumerator FadeIn(int value)
    {
        SpriteRenderer[] renderer = mapAnime[value].GetComponentsInChildren<SpriteRenderer>();

        float time = 0f;
        while (time < 1f)
        {
            foreach (SpriteRenderer dummy in renderer)
            {

                Color dummyColor = dummy.color;
                dummyColor.a = Mathf.Lerp(0f, 1f, time);
                dummy.color = dummyColor;
            }
            time += Time.deltaTime;
            yield return null;
        }
    }

    IEnumerator ChangeMapTrans(int value)
    {
        Transform trans = mapAnime[value].GetComponent<Transform>();
        Vector3 startVe = new Vector3(-0.1f, -0.5f, trans.position.z);
        Vector3 endVe = new Vector3(0f, 0f, trans.position.z);
        float time = 0f;
        while (time < 1f)
        {
            trans.localPosition = Vector3.Lerp(startVe, endVe, time);
            time += Time.deltaTime;

            yield return null;
        }
        trans.localPosition = endVe;
    }
}