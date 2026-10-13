using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.Netcode;
using System;

public class GameManager : NetworkBehaviour {
    public static GameManager Instance { get; private set; }

    public NetworkVariable<int> scorePlayer1 = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    public NetworkVariable<int> scorePlayer2 = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    public event Action<int, int> OnScoreChanged;

    void Awake() {
        if (Instance == null) {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject);
        }

        SceneManager.LoadScene("MainMenu");
    }

    public override void OnNetworkSpawn() {
        scorePlayer1.OnValueChanged += OnScoreValueChanged;
        scorePlayer2.OnValueChanged += OnScoreValueChanged;

        OnScoreChanged?.Invoke(scorePlayer1.Value, scorePlayer2.Value);
    }

    public override void OnNetworkDespawn() {
        scorePlayer1.OnValueChanged -= OnScoreValueChanged;
        scorePlayer2.OnValueChanged -= OnScoreValueChanged;
    }

    private void OnScoreValueChanged(int previousValue, int newValue) {
        OnScoreChanged?.Invoke(scorePlayer1.Value, scorePlayer2.Value);
    }

    public void StartMatch() {
        SceneManager.LoadScene("GameplayUI", LoadSceneMode.Additive);
    }

    public void AddScore(int playerID) {
        if (!IsServer) return;

        if (playerID == 0) scorePlayer1.Value++;
        else if (playerID == 1) scorePlayer2.Value++;
    }
}