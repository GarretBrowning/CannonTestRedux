using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// GameManager handles the core game loop and scoring mechanics.
/// - Manages game state (title, pre-game, gameplay, game over)
/// - Tracks score, shots fired, targets hit, and accuracy
/// - Controls target wave spawning and wave completion bonuses
/// - Handles input map switching between UI and gameplay
/// - Manages game timer and results display
/// </summary>
public class GameManager : MonoBehaviour
{
    private const float GAME_DURATION_SECONDS = 60f;

    // UI References
    [SerializeField] private UIManager uiManager;
    [SerializeField] private PlayerInput playerInput;

    // Player Reference
    [SerializeField] private GameObject player;

    // Target Management
    [SerializeField]
    [Tooltip("The target object pool that spawns and manages the targets.")]
    private RespawningTargets targetSpawner;

    [SerializeField]
    [Tooltip("Bonus points awarded when all targets in a wave are destroyed.")]
    private int waveClearedBonus = 100;

    private List<TargetLifecycleHandler> currentWave;
    private int targetsRemaining;

    // Game State
    private int totalScore = 0;
    private int shotsFired = 0;
    private int targetsHit = 0;
    private float timeRemaining;
    private bool isGameActive; // True when in main gameplay; false during menus or transitions.
    private Coroutine preGameCoroutine;
    private Coroutine revealResultsCoroutine;
    private bool clickToRestartRequested = false;

    public int TotalScore => totalScore;
    public int ShotsFired => shotsFired;
    public int TargetsHit => targetsHit;
    public int Accuracy => shotsFired > 0 ? Mathf.RoundToInt((float)targetsHit / shotsFired * 100f) : 0;

    /// <summary>
    /// Subscribes to scoring, target hit, and bullet fired events when this component is enabled.
    /// - OnScored: Called when points are scored, handled by HandleScore
    /// - OnTargetHit: Called when a target is hit, handled by HandleTargetHit 
    /// - OnBulletFired: Called when player fires a bullet, handled by HandleBulletFired
    /// </summary>
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

        targetSpawner.ResetTargets(); // Reset all targets before spawning new wave.
        SpawnNewWave();
        player?.SetActive(true); // 'Spawn' the player.
    }

    public void RestartGame()
    {
        uiManager?.LockCursor();
        // Restarts the game at the pre-game countdown:
        if (preGameCoroutine != null) StopCoroutine(preGameCoroutine);
        preGameCoroutine = StartCoroutine(StartPreGameCountdown());
    }
        
    /// <summary>
    /// Handles the end of the game by:
    /// - Switching to UI input controls
    /// - Stopping gameplay
    /// - Calculating final accuracy
    /// - Displaying game over screen with results
    /// - Disabling the player
    /// - Starting the results reveal animation
    /// </summary>
    private void EndGame()
    {
        SwitchToUIInputMap();
        isGameActive = false;
        int accuracy = shotsFired > 0 ? Mathf.RoundToInt((float)targetsHit / shotsFired * 100f) : 0;
        uiManager.SetUIState(UIState.GameOver);
        player?.SetActive(false);
        if (revealResultsCoroutine != null) StopCoroutine(revealResultsCoroutine);
        revealResultsCoroutine = StartCoroutine(RevealResults(totalScore, shotsFired, targetsHit, accuracy));
    }

    private IEnumerator RevealResults(int finalScore, int shotsFired, int targetsHit, float accuracy)
    {
        // Clear previous results and show each stat with 1 second delay between them
        uiManager.ClearGameOverTexts();
        
        // Show final score:
        yield return new WaitForSeconds(1f);
        uiManager.SetFinalScoreText($"{finalScore}");
        uiManager.ShowFinalScoreGroup();
        
        // Show shots fired:
        yield return new WaitForSeconds(1f); 
        uiManager.SetShotsFiredText($"{shotsFired}");
        uiManager.ShowShotsFiredGroup();
        
        // Show targets hit:
        yield return new WaitForSeconds(1f);
        uiManager.SetTargetsHitText($"{targetsHit}");
        uiManager.ShowTargetsHitGroup();
        
        // Show accuracy percentage:
        yield return new WaitForSeconds(1f);
        uiManager.SetAccuracyText($"{accuracy}%");
        uiManager.ShowAccuracyGroup();
        
        // Enable restart prompt
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
        currentWave = targetSpawner.Respawn(); // Respawn the targets.
        targetsRemaining = currentWave.Count; // Reset number of targets remaining.

        // Unsubscribe and resubscribe to OnTargetDestroyed event for each target in the new wave
        // This ensures we don't have duplicate subscriptions if targets are reused from object pooling
        // and guarantees we're subscribed exactly once to handle scoring and wave completion.
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

    /// <summary>
    /// Unsubscribes from the destroyed target's event and tracks remaining targets.
    /// Awards bonus points and spawns a new wave of targets when all targets in the current wave are destroyed.
    /// </summary>
    /// <param name="target">The target that was destroyed</param>
    private void HandleTargetDestroyed(TargetLifecycleHandler target)
    {
        target.OnTargetDestroyed -= HandleTargetDestroyed;
        targetsRemaining--;

        if (targetsRemaining <= 0)
        {
            totalScore += waveClearedBonus;
            uiManager.UpdateScore(totalScore);
            StartCoroutine(SpawnNewWaveNextFrame());
        }
    }

    /// <summary>
    /// Spawns a new wave of targets on the next frame to ensure all previous targets have been properly disabled first.
    /// This delay helps avoid Unity's SetActive timing issues where objects may not be fully deactivated if done in the same frame.
    /// </summary>
    private IEnumerator SpawnNewWaveNextFrame()
    {
        yield return null;
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

    /// <summary>
    /// Handles click input events from the Input System.
    /// When in Title screen state, starts the pre-game countdown.
    /// When in GameOver state, sets flag to request game restart.
    /// </summary>
    /// <param name="context">Input action callback context containing input state</param>
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
                // Set flag to request restart on next frame:
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