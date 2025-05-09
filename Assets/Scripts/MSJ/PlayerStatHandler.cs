using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStatHandler : MonoBehaviour
{
    [Range(1, 5)][SerializeField] private int health = 5;
    public int Health
    {
        get => health;
        set => health = Mathf.Clamp(value, 0, 5);
    }

    [Range(1f, 20f)][SerializeField] private float speed = 3;
    public float Speed
    {
        get => speed;
        set => speed = Mathf.Clamp(value, 0, 20);
    }
}
