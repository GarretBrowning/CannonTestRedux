using UnityEngine;
using System;

/// <summary>
/// Scorable is a component that manages scoring for objects that can be scored (targets).
/// </summary>
public class Scorable : MonoBehaviour
{
    [SerializeField] private int scoreValue = 10;

    public static event Action<int> OnScored;
    public static event Action OnTargetHit;

    public void Score()
    {
        OnScored?.Invoke(scoreValue);
        OnTargetHit?.Invoke();
    }
}
