using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class UIManager : MonoBehaviour
{
    #region Serialized Fields
    // Serialized fields for UI elements
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

    #endregion

    #region Private Fields
    // Private state
    private UIState currentState;
    #endregion

    #region Public Properties
    // Public property for current UI state
    public UIState CurrentState => currentState;
    #endregion

    #region Unity Lifecycle
    private void Awake()
    {
        SetUIState(UIState.Title);
    }
    #endregion

    #region UI State Management
    // UI state management
    public void SetUIState(UIState state)
    {
        titleScreen.SetActive(state == UIState.Title);
        preGameScreen.SetActive(state == UIState.PreGame);
        gameplayHUD.SetActive(state == UIState.Gameplay);
        gameOverScreen.SetActive(state == UIState.GameOver);

        currentState = state;

        // Manage cursor lock state
        if (state == UIState.Gameplay)
        {
            LockCursor();
        }
        // Do not unlock cursor for GameOver here; handled in SetClickToPlayAgainActive
    }
    #endregion

    #region Public UI Update Methods
    // UI update methods
    public void UpdateScore(int score) => scoreText.text = score.ToString("D4");
    public void UpdateTime(float timeRemaining) => timeText.text = timeRemaining.ToString("F1");

    public void ShowGameOverResults(int finalScore, int shotsFired, int targetsHit, float accuracy)
    {
        SetUIState(UIState.GameOver);
        // GameManager will handle the coroutine for revealing results and call UIManager's update methods.
    }

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
    #endregion

    #region Cursor Lock Methods
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
    #endregion
}

public enum UIState
{
    Title,
    PreGame,
    Gameplay,
    GameOver
}
