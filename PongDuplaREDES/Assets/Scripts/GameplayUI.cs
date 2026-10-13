using UnityEngine;
using TMPro;

public class GameplayUI : MonoBehaviour {
    public TextMeshProUGUI scoreTextP1;
    public TextMeshProUGUI scoreTextP2;

    void Start() {
        if (GameManager.Instance != null) {
            GameManager.Instance.OnScoreChanged += UpdateScoreUI;

            UpdateScoreUI(GameManager.Instance.scorePlayer1.Value, GameManager.Instance.scorePlayer2.Value);
        }
    }

    void OnDestroy() {
        if (GameManager.Instance != null) {
            GameManager.Instance.OnScoreChanged -= UpdateScoreUI;
        }
    }

    private void UpdateScoreUI(int p1Score, int p2Score) {
        scoreTextP1.text = p1Score.ToString();
        scoreTextP2.text = p2Score.ToString();
    }
}