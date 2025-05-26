using UnityEngine;
using System;

public class BulletTracker : MonoBehaviour
{
    public static event Action OnBulletFired;

    public static void BulletFired()
    {
        OnBulletFired?.Invoke();
    }
}