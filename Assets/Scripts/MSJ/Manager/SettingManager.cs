using UnityEngine;
using UnityEngine.UI;

public class SettingManager : MonoBehaviour
{
    [SerializeField] private Slider bgmSlider;
    [SerializeField] private Slider sfxSlider;

    private void Start()
    {
        // 초기 슬라이더 값 설정
        bgmSlider.value = SoundManager.instance.MusicVolume;
        sfxSlider.value = SoundManager.instance.SoundEffectVolume;

        // 이벤트 등록
        bgmSlider.onValueChanged.AddListener(ChangeBGMVolume);
        sfxSlider.onValueChanged.AddListener(ChangeSFXVolume);
    }

    public void ChangeBGMVolume(float value)
    {
        SoundManager.instance.SetBGMVolume(value);
    }

    public void ChangeSFXVolume(float value)
    {
        SoundManager.instance.SetSFXVolume(value);
    }


}
