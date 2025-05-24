using UnityEngine;

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