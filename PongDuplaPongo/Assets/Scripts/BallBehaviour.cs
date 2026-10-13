using UnityEngine;
using Unity.Netcode;

public class BallBehaviour : NetworkBehaviour {
    public float speed = 10f;
    public int requiredPlayers = 4;
    
    private Rigidbody2D rb;
    private bool isGameStarted = false;

    void Awake() {
        rb = GetComponent<Rigidbody2D>();
    }

    public override void OnNetworkSpawn() {
        if (!IsServer) return;

        rb.linearVelocity = Vector2.zero;
        transform.position = Vector2.zero;

        NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;

        CheckStartGame();
    }

    public override void OnNetworkDespawn() {
        if (IsServer && NetworkManager.Singleton != null) {
            NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnected;
        }
    }

    private void OnClientConnected(ulong clientId) {
        if (!IsServer) return;
        CheckStartGame();
    }

    private void CheckStartGame() {
        if (!isGameStarted && NetworkManager.Singleton.ConnectedClients.Count >= requiredPlayers) {
            isGameStarted = true;
            Launch();
        }
    }

    public void Launch() {
        if (!IsServer) return;

        if (!isGameStarted) {
            rb.linearVelocity = Vector2.zero;
            transform.position = Vector2.zero;
            return;
        }

        transform.position = Vector2.zero;

        float x = Random.Range(0, 2) == 0 ? -1f : 1f;
        float y = Random.Range(-0.5f, 0.5f);

        rb.linearVelocity = new Vector2(x, y).normalized * speed;
    }
}