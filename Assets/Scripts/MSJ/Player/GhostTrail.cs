using UnityEngine;

public class GhostTrail : MonoBehaviour
{
    public float lifetime = 0.3f;

    void Start()
    {
        Destroy(gameObject, lifetime);
    }
}
