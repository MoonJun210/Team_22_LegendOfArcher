using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatHandler : MonoBehaviour
{
    // 최대 체력 (Inspector에서 설정 가능)
    [field: SerializeField] public float MaxHP { get; private set; } = 300f;
    [field: SerializeField] public float MoveSpeed { get; private set; } = 3f;

    [SerializeField] private Slider healthSlider;

    // 현재 체력 (초기값은 MaxHP로 설정)
    public float CurrentHP { get; set; }

    private void Awake()
    {
        CurrentHP = MaxHP; // 게임 시작 시 체력 초기화

        if(healthSlider == null)
        {
            return;
        }
        healthSlider.maxValue = MaxHP;
        healthSlider.value = CurrentHP;
    }

    // 데미지 받기
    public void TakeDamage(float damage)
    {
        CurrentHP -= damage;
        CurrentHP = Mathf.Clamp(CurrentHP, 0, MaxHP); // 체력 최소 0 보정

        if (healthSlider == null)
        {
            return;
        }
        healthSlider.value = CurrentHP;

    }

    // 체력 회복
    public void Heal(float amount)
    {
        CurrentHP = Mathf.Clamp(CurrentHP + amount, 0, MaxHP); // 체력 최대 초과 방지
    }
}
