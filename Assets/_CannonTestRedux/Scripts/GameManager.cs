using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    [SerializeField] private UIManager uiManager;
    [SerializeField] private RespawningTargets targetSpawner;
    [SerializeField] private int waveClearedBonus = 100;
    [SerializeField] private GameObject player;
    [SerializeField] private PlayerInput playerInput;

    [Header("Debug")]
    [SerializeField] private bool debug;
    private List<TargetLifecycleHandler> currentWave;
    private int targetsRemaining;
    private int totalScore = 0;
    private int shotsFired = 0;
    private int targetsHit = 0;
    private float timeRemaining;
    private bool isGameActive;
    private Coroutine preGameCoroutine;
    private Coroutine revealResultsCoroutine;
    private bool clickToRestartRequested = false;
    private const float GAME_DURATION_SECONDS = 5f; // TODO: Change to 60f for final game

    public int TotalScore => totalScore;
    public int ShotsFired => shotsFired;
    public int TargetsHit => targetsHit;
    public int Accuracy => shotsFired > 0 ? Mathf.RoundToInt((float)targetsHit / shotsFired * 100f) : 0;

    private void OnEnable()
    {
        Scorable.OnScored += HandleScore;
        Scorable.OnTargetHit += HandleTargetHit;
        BulletTracker.OnBulletFired += HandleBulletFired;
    }

    private void OnDisable()
    {
        Scorable.OnScored -= HandleScore;
        Scorable.OnTargetHit -= HandleTargetHit;
        BulletTracker.OnBulletFired -= HandleBulletFired;
    }

    private void Start()
    {
        player?.SetActive(false);
        SpawnNewWave();
    }

    private void Update()
    {
        // Only allow click-to-start in Title state (handled by OnClick now)
        // Existing game logic
        if (!isGameActive) return;
        timeRemaining -= Time.deltaTime;
        uiManager.UpdateTime(timeRemaining);
        if (timeRemaining <= 0f)
        {
            EndGame();
        }
    }

    private IEnumerator StartPreGameCountdown()
    {
        SwitchToUIInputMap();
        uiManager.SetUIState(UIState.PreGame);
        float countdown = 3f;
        while (countdown > 0f)
        {
            uiManager.numberText.text = Mathf.Ceil(countdown).ToString();
            yield return new WaitForSeconds(1f);
            countdown -= 1f;
        }
        uiManager.numberText.text = "";
        uiManager.SetUIState(UIState.Gameplay);
        StartGame();
    }

    public void StartGame()
    {
        SwitchToPlayerInputMap();
        timeRemaining = GAME_DURATION_SECONDS;
        isGameActive = true;
        totalScore = 0;
        targetsHit = 0;
        shotsFired = 0;
        uiManager.UpdateScore(0);

        targetSpawner.ResetTargets(); // Reset all targets before spawning new wave
        SpawnNewWave();
        player?.SetActive(true);
    }

    public void RestartGame()
    {
        // Start the pre-game countdown again, skipping the title screen
        if (preGameCoroutine != null) StopCoroutine(preGameCoroutine);
        if (uiManager != null) uiManager.LockCursor(); // Hide/lock cursor immediately
        preGameCoroutine = StartCoroutine(StartPreGameCountdown());
    }
        
    private void EndGame()
    {
        SwitchToUIInputMap();
        isGameActive = false;
        int accuracy = shotsFired > 0 ? Mathf.RoundToInt((float)targetsHit / shotsFired * 100f) : 0;
        uiManager.ShowGameOverResults(totalScore, shotsFired, targetsHit, accuracy);
        player?.SetActive(false);
        if (revealResultsCoroutine != null) StopCoroutine(revealResultsCoroutine);
        revealResultsCoroutine = StartCoroutine(RevealResults(totalScore, shotsFired, targetsHit, accuracy));
    }

    private IEnumerator RevealResults(int finalScore, int shotsFired, int targetsHit, float accuracy)
    {
        uiManager.ClearGameOverTexts();
        yield return new WaitForSeconds(1f);
        uiManager.SetFinalScoreText($"{finalScore}");
        uiManager.ShowFinalScoreGroup();
        yield return new WaitForSeconds(1f);
        uiManager.SetShotsFiredText($"{shotsFired}");
        uiManager.ShowShotsFiredGroup();
        yield return new WaitForSeconds(1f);
        uiManager.SetTargetsHitText($"{targetsHit}");
        uiManager.ShowTargetsHitGroup();
        yield return new WaitForSeconds(1f);
        uiManager.SetAccuracyText($"{accuracy}%");
        uiManager.ShowAccuracyGroup();
        yield return new WaitForSeconds(1f);
        uiManager.SetClickToPlayAgainActive(true);
        StartCoroutine(WaitForClickToRestart());
    }

    private IEnumerator WaitForClickToRestart()
    {
        clickToRestartRequested = false;
        while (!clickToRestartRequested) yield return null;
        RestartGame();
    }

    private void SpawnNewWave()
    {
        currentWave = targetSpawner.Respawn();
        targetsRemaining = currentWave.Count;
        if (debug) Debug.Log($"Spawned {targetsRemaining} targets in new wave");

        foreach (var target in currentWave)
        {
            target.OnTargetDestroyed -= HandleTargetDestroyed;
            target.OnTargetDestroyed += HandleTargetDestroyed;
        }
    }

    private void HandleScore(int score)
    {
        totalScore += score;
        uiManager.UpdateScore(totalScore);
    }

    private void HandleTargetDestroyed(TargetLifecycleHandler target)
    {
        target.OnTargetDestroyed -= HandleTargetDestroyed;
        targetsRemaining--;

        if (targetsRemaining <= 0)
        {
            totalScore += waveClearedBonus; // Wave cleared bonus
            uiManager.UpdateScore(totalScore); // Update UI after bonus
            // Delay spawning the new wave by one frame to avoid Unity's SetActive timing issue.
            // If you deactivate and immediately reactivate a GameObject in the same frame,
            // Unity may keep it inactive. Waiting one frame ensures all objects are properly deactivated first.
            StartCoroutine(DelayedSpawnNewWave());
            if (debug) Debug.Log("Spawned new wave of targets (delayed)");
        }
    }

    private IEnumerator DelayedSpawnNewWave()
    {
        yield return null; // Wait one frame
        SpawnNewWave();
    }

    private void HandleBulletFired()
    {
        shotsFired++;
    }

    private void HandleTargetHit()
    {
        targetsHit++;
    }

    public void OnClick(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (uiManager != null && uiManager.CurrentState == UIState.Title)
            {
                if (preGameCoroutine != null) StopCoroutine(preGameCoroutine);
                preGameCoroutine = StartCoroutine(StartPreGameCountdown());
            }
            else if (uiManager != null && uiManager.CurrentState == UIState.GameOver)
            {
                clickToRestartRequested = true;
            }
        }
    }

    private void SwitchToPlayerInputMap()
    {
        if (playerInput != null && playerInput.currentActionMap != null && playerInput.currentActionMap.name != "Player")
            playerInput.SwitchCurrentActionMap("Player");
    }

    private void SwitchToUIInputMap()
    {
        if (playerInput != null && playerInput.currentActionMap != null && playerInput.currentActionMap.name != "UI")
            playerInput.SwitchCurrentActionMap("UI");
    }
}