using UnityEngine;

public class ProjectileManager : MonoBehaviour
{
    private static ProjectileManager instance;
    public static ProjectileManager Instance { get { return instance; } }

    [SerializeField] private GameObject[] projectilePrefabs;

    private void Awake()
    {
        instance = this;
    }

    public void ShootBullet(RangeWeaponHandler rangeWeaponHandler, Vector2 startPostiion, Vector2 direction)
    {
        GameObject origin = projectilePrefabs[rangeWeaponHandler.BulletIndex];
        GameObject obj = Instantiate(origin, startPostiion, Quaternion.identity);

        var sr = obj.GetComponentInChildren<SpriteRenderer>();
        Debug.Log($"Sprite: {sr.sprite}, sortingOrder: {sr.sortingOrder}, enabled: {sr.enabled}");

        ProjectileController projectileController = obj.GetComponent<ProjectileController>();
        projectileController.Init(direction, rangeWeaponHandler);
    }
}
