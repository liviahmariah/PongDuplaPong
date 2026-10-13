using UnityEngine;
using Unity.Netcode;

public class PartidaManager : NetworkBehaviour {
    public GameObject ballPrefab;

    public override void OnNetworkSpawn() {
        if (!IsServer) return;

        if (NetworkManager.Singleton.ConnectedClientsIds.Count == 2) {
            SpawnBall();
        } else {
            NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
        }
    }

    private void OnClientConnected(ulong clientId) {
        if (NetworkManager.Singleton.ConnectedClientsIds.Count == 2) {
            SpawnBall();
            
            NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnected;
        }
    }

    private void SpawnBall()
    {
        GameObject ball = Instantiate(ballPrefab);
        ball.GetComponent<NetworkObject>().Spawn();
    }

    public override void OnNetworkDespawn() {
        if (IsServer) {
            if (NetworkManager.Singleton != null) {
                NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnected;
            }
        }
    }
}