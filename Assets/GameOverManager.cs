using UnityEngine;

public class GameOverManager : MonoBehaviour
{
    public static GameOverManager Instance { get; private set; }

    // Consultable partout (ex: EnemyScript) pour savoir si la partie est finie
    public static bool IsGameOver { get; private set; } = false;

    [Header("UI")]
    [SerializeField] private GameObject gameOverPanel;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        // Sécurité : force le reset à chaque chargement de scène,
        // au cas où le static aurait survécu à une session précédente
        ResetGameOverState();
    }

    public void TriggerGameOver()
    {
        if (IsGameOver) return;
        IsGameOver = true;

        if (gameOverPanel != null)
            gameOverPanel.SetActive(true);

        // Fige toute la scène (mouvement, animations, timers bases sur Time.deltaTime)
        Time.timeScale = 0f;
    }

    // A appeler plus tard depuis le bouton "Rejouer" / "Menu"
    public void ResetGameOverState()
    {
        IsGameOver = false;
        Time.timeScale = 1f;

        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);
    }
}
