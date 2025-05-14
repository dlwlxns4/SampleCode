/// 2025-05-14

using UnityEngine;

public static class GameObjectExntensions
{
    public static void SetParent(this GameObject gameObject, GameObject parent)
    {
        gameObject.transform.SetParent(parent.transform);
    }
    
    public static void SetParent(this GameObject gameObject, Component parent)
    {
        gameObject.transform.SetParent(parent.transform);
    }
}