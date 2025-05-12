using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneButtonSound : MonoBehaviour
{
    public void ClickSound()
    {
        SoundManager.PlayClip("ClickSfx");
    }
}
