using UnityEngine;
using System.Collections.Generic;

public static class InactiveObjectFinder
{

    public static GameObject FindInactiveObjectWithTag(string tag)
    {
        GameObject[] rootObjects = UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects();

        foreach (GameObject root in rootObjects)
        {
            GameObject found = FindInChildrenByTag(root.transform, tag);
            if (found != null)
                return found;
        }

        return null;
    }


    public static GameObject FindInactiveObjectWithName(string name)
    {
        GameObject[] rootObjects = UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects();

        foreach (GameObject root in rootObjects)
        {
            GameObject found = FindInChildrenByName(root.transform, name);
            if (found != null)
                return found;
        }

        return null;
    }

    private static GameObject FindInChildrenByTag(Transform parent, string tag)
    {
        if (parent.CompareTag(tag))
            return parent.gameObject;

        foreach (Transform child in parent)
        {
            GameObject found = FindInChildrenByTag(child, tag);
            if (found != null)
                return found;
        }

        return null;
    }

    private static GameObject FindInChildrenByName(Transform parent, string name)
    {
        if (parent.name == name)
            return parent.gameObject;

        foreach (Transform child in parent)
        {
            GameObject found = FindInChildrenByName(child, name);
            if (found != null)
                return found;
        }

        return null;
    }
}
