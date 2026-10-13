using UnityEngine;
using Unity.Netcode;

public class GolBehaviour : MonoBehaviour {
    public int playerToScore;

    void OnTriggerEnter2D(Collider2D collision) {
        if (NetworkManager.Singleton != null && !NetworkManager.Singleton.IsServer) return;

        if (collision.CompareTag("Ball")) {
            GameManager.Instance.AddScore(playerToScore);
            BallBehaviour ballScript = collision.GetComponent<BallBehaviour>();

            if (ballScript != null) {
                ballScript.Launch();
            }
        }
    }
}