using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStatHandler : MonoBehaviour
{
    [SerializeField] private CharacterStat statData;

    public int MaxHealth => statData.maxHealth;
    public float BaseSpeed => statData.moveSpeed;

    public int Health { get; private set; }
    public float CurrentSpeed { get; private set; }

    private void Awake()
    {
        Health = MaxHealth;
        CurrentSpeed = BaseSpeed;
    }

    public void TakeDamage(int amount = 1)
    {
        Health = Mathf.Clamp(Health - amount, 0, MaxHealth);
    }

    public void Heal(int amount = 1)
    {
        Health = Mathf.Clamp(Health + amount, 0, MaxHealth);
    }

    public void SetSpeed(float newSpeed)
    {
        CurrentSpeed = newSpeed;
    }

}
