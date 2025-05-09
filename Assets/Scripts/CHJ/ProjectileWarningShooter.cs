using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileWarningShooter : MonoBehaviour
{
    [SerializeField] private GameObject directionLinePrefab;
    [SerializeField] private GameObject projectilePrefab;

    public IEnumerator Fire(Vector3 origin, Vector2 direction, float delay = 1f, float lineLength = 8f)
    {
        GameObject line = Instantiate(directionLinePrefab, origin, Quaternion.identity);
        LineRenderer lr = line.GetComponent<LineRenderer>();
        lr.positionCount = 2;
        lr.SetPosition(0, origin);
        lr.SetPosition(1, origin + (Vector3)(direction * lineLength));
        lr.startColor = lr.endColor = new Color(1f, 0f, 0f, 0.4f);

        yield return new WaitForSeconds(delay);
        Destroy(line);

        GameObject projectile = Instantiate(projectilePrefab, origin, Quaternion.identity);
        projectile.GetComponent<Projectile>().SetDirection(direction);
    }
}
