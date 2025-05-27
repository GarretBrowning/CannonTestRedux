using UnityEngine;
using TMPro;

/// <summary>
/// UIManager handles the UI elements and state management for the game.
/// - Manages the visibility of UI elements based on the current UI state
/// - Manages cursor lock state
/// - Updates the score and time display
/// - Shows the game over results
/// - Manages the click-to-play-again prompt
/// </summary>
public class UIManager : MonoBehaviour
{
    // UI Elements
    public GameObject titleScreen;
    public GameObject preGameScreen;
    public GameObject gameplayHUD;
    public GameObject gameOverScreen;

    [Header("PreGame Countdown")]
    public TextMeshProUGUI numberText;

    [Header("Gameplay HUD")]
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI timeText;

    [Header("Game Over Elements")]
    public GameObject finalScoreGroup;
    public GameObject shotsFiredGroup;
    public GameObject targetsHitGroup;
    public GameObject accuracyGroup;
    public TextMeshProUGUI finalScoreText;
    public TextMeshProUGUI shotsFiredResultText;
    public TextMeshProUGUI targetsHitResultText;
    public TextMeshProUGUI accuracyResultText;
    public TextMeshProUGUI clickToPlayAgainText;

    private UIState currentState;
    public UIState CurrentState => currentState;
    private void Awake()
    {
        SetUIState(UIState.Title);
    }

    public void SetUIState(UIState state)
    {
        titleScreen.SetActive(state == UIState.Title);
        preGameScreen.SetActive(state == UIState.PreGame);
        gameplayHUD.SetActive(state == UIState.Gameplay);
        gameOverScreen.SetActive(state == UIState.GameOver);

        currentState = state;

        // Manage cursor lock state:
        if (state == UIState.Gameplay)
        {
            LockCursor();
        }
    }

    // UI update methods
    public void UpdateScore(int score) => scoreText.text = score.ToString("D4");
    public void UpdateTime(float timeRemaining) => timeText.text = timeRemaining.ToString("F1");
    public void SetFinalScoreText(string text) => finalScoreText.text = text;
    public void SetShotsFiredText(string text) => shotsFiredResultText.text = text;
    public void SetTargetsHitText(string text) => targetsHitResultText.text = text;
    public void SetAccuracyText(string text) => accuracyResultText.text = text;
    public void SetClickToPlayAgainActive(bool active)
    {
        clickToPlayAgainText.gameObject.SetActive(active);
        if (active)
        {
            UnlockCursor();
        }
        else
        {
            LockCursor();
        }
    }
    public void ClearGameOverTexts()
    {
        finalScoreText.text = "";
        shotsFiredResultText.text = "";
        targetsHitResultText.text = "";
        accuracyResultText.text = "";
        clickToPlayAgainText.gameObject.SetActive(false);

        // Hide groups
        if (finalScoreGroup) finalScoreGroup.SetActive(false);
        if (shotsFiredGroup) shotsFiredGroup.SetActive(false);
        if (targetsHitGroup) targetsHitGroup.SetActive(false);
        if (accuracyGroup) accuracyGroup.SetActive(false);
    }

    public void ShowFinalScoreGroup() { if (finalScoreGroup) finalScoreGroup.SetActive(true); }
    public void ShowShotsFiredGroup() { if (shotsFiredGroup) shotsFiredGroup.SetActive(true); }
    public void ShowTargetsHitGroup() { if (targetsHitGroup) targetsHitGroup.SetActive(true); }
    public void ShowAccuracyGroup() { if (accuracyGroup) accuracyGroup.SetActive(true); }

    // Cursor lock methods
    public void LockCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void UnlockCursor()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}

public enum UIState
{
    Title,
    PreGame,
    Gameplay,
    GameOver
}
