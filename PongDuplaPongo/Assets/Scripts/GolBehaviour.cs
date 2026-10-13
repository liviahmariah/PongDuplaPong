
using UnityEngine;
using Unity.Netcode;

public class GolBehaviour : MonoBehaviour
{
    public int playerToScore;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (NetworkManager.Singleton != null &&
            !NetworkManager.Singleton.IsServer)
        {
            return;
        }

        if (!collision.CompareTag("Ball"))
            return;

        if (GameManager.Instance == null)
            return;

        if (GameManager.Instance.PartidaFinalizada)
            return;

        GameManager.Instance.AddScore(playerToScore);

        if (GameManager.Instance.PartidaFinalizada)
            return;

        BallBehaviour ballScript =
            collision.GetComponent<BallBehaviour>();

        if (ballScript != null)
        {
            ballScript.Launch();
        }
    }
}
