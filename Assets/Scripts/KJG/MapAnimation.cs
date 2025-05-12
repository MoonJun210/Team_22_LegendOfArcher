using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class MapAnimation : MonoBehaviour
{
    [SerializeField] private GameObject[] mapAnime;

    public void TurnOnMap(int value)
    {
        StartCoroutine(FadeIn(value));
        StartCoroutine(ChangeMapTrans(value));
    }
    public void TurnOffMap(int value)
    {
        StartCoroutine(FadeOut(value));
    }

    IEnumerator FadeOut(int value)
    {
        SpriteRenderer[] renderer = mapAnime[value].GetComponentsInChildren<SpriteRenderer>();

        float time = 1f;

        while (time > 0f)
        {
            time -= Time.deltaTime;
            foreach (SpriteRenderer dummy in renderer)
            {
                Color dummyColor = dummy.color;
                dummyColor.a = Mathf.Lerp(1f, 0f, Time.deltaTime);
                dummy.color = dummyColor;
            }
        }
        yield return null;
    }

    IEnumerator FadeIn(int value)
    {
        SpriteRenderer[] renderer = mapAnime[value].GetComponentsInChildren<SpriteRenderer>();

        float time = 1f;
        while (time > 0f)
        {
            time -= Time.deltaTime;
            foreach (SpriteRenderer dummy in renderer)
            {
                Color dummyColor = dummy.color;
                dummyColor.a = Mathf.Lerp(0f, 1f, Time.deltaTime);
                dummy.color = dummyColor;
            }
        }
        yield return null;
    }

    IEnumerator ChangeMapTrans(int value)
    {
        Transform[] trans = mapAnime[value].GetComponentsInChildren<Transform>();

        foreach (Transform dummy in trans)
        {
            Vector3 dummyPostion = dummy.position;
            dummyPostion.x = dummyPostion.x - 0.1f;
            dummyPostion.y = dummyPostion.y - 0.5f;

            dummyPostion.x = dummyPostion.x + Mathf.Lerp(0f, 0.1f, Time.deltaTime);
            dummyPostion.y = dummyPostion.y + Mathf.Lerp(0f, 0.5f, Time.deltaTime);
            dummy.position = dummyPostion;
        }
        yield return null;
    }
}
