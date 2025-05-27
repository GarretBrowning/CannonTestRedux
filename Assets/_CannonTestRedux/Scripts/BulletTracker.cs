using UnityEngine;
using System;

/// <summary>
/// BulletTracker is a singleton that tracks the number of bullets fired.
/// It raises an event when a bullet is fired to notify other components.
/// </summary>
public class BulletTracker : MonoBehaviour
{
    public static event Action OnBulletFired;

    public static void BulletFired()
    {
        OnBulletFired?.Invoke();
    }
}