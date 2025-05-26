using UnityEngine;
using System;
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
