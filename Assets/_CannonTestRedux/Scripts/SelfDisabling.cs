using UnityEngine;

/// <summary>
/// SelfDisabling is a component that disables a game object (bullet) after a specified amount of time.
/// </summary>
public class SelfDisabling : MonoBehaviour
{
    [SerializeField] private float disableAfterSeconds = 2f;
    private float timer;
    private bool timerActive = false;

    private void OnEnable()
    {
        timer = 0f;
        timerActive = true;
    }

    private void Update()
    {
        if (!timerActive) return;
        timer += Time.deltaTime;
        if (timer >= disableAfterSeconds)
        {
            gameObject.SetActive(false);
            timerActive = false;
        }
    }
} 