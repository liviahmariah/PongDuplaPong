
using UnityEngine;
using TMPro;

public class GameplayUI : MonoBehaviour
{
    [Header("Pontuação")]
    public TextMeshProUGUI scoreTextP1;
    public TextMeshProUGUI scoreTextP2;

    [Header("Tela de vitória")]
    public GameObject gameOverPanel;
    public TextMeshProUGUI winnerText;

    private GameManager gameManager;

    void Start()
    {
        gameManager = GameManager.Instance;

        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);

        if (gameManager == null)
        {
            Debug.LogWarning("GameManager não encontrado.");
            return;
        }

        gameManager.OnScoreChanged += UpdateScoreUI;
        gameManager.OnMatchEnded += ShowWinner;

        UpdateScoreUI(
            gameManager.scorePlayer1.Value,
            gameManager.scorePlayer2.Value
        );

        if (gameManager.PartidaFinalizada)
        {
            ShowWinner(gameManager.vencedor.Value);
        }
    }

    void OnDestroy()
    {
        if (gameManager != null)
        {
            gameManager.OnScoreChanged -= UpdateScoreUI;
            gameManager.OnMatchEnded -= ShowWinner;
        }
    }

    private void UpdateScoreUI(int p1Score, int p2Score)
    {
        if (scoreTextP1 != null)
            scoreTextP1.text = p1Score.ToString();

        if (scoreTextP2 != null)
            scoreTextP2.text = p2Score.ToString();
    }

    private void ShowWinner(int playerID)
    {
        if (gameOverPanel != null)
            gameOverPanel.SetActive(true);

        if (winnerText != null)
        {
            winnerText.text =
                "Jogador " + (playerID + 1) + " venceu!";
        }

        Debug.Log("Exibindo tela de vitória.");
    }
}
