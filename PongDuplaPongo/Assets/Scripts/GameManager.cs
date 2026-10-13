
using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.Netcode;
using System;

public class GameManager : NetworkBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Configuração da partida")]
    public int limitePontuacao = 5;

    public NetworkVariable<int> scorePlayer1 =
        new NetworkVariable<int>(
            0,
            NetworkVariableReadPermission.Everyone,
            NetworkVariableWritePermission.Server
        );

    public NetworkVariable<int> scorePlayer2 =
        new NetworkVariable<int>(
            0,
            NetworkVariableReadPermission.Everyone,
            NetworkVariableWritePermission.Server
        );

    // -1 significa que ainda não existe vencedor.
    public NetworkVariable<int> vencedor =
        new NetworkVariable<int>(
            -1,
            NetworkVariableReadPermission.Everyone,
            NetworkVariableWritePermission.Server
        );

    public NetworkVariable<bool> partidaFinalizada =
        new NetworkVariable<bool>(
            false,
            NetworkVariableReadPermission.Everyone,
            NetworkVariableWritePermission.Server
        );

    public event Action<int, int> OnScoreChanged;
    public event Action<int> OnMatchEnded;

    private bool matchEndNotified = false;

    public bool PartidaFinalizada => partidaFinalizada.Value;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        SceneManager.LoadScene("MainMenu");
    }

    public override void OnNetworkSpawn()
    {
        scorePlayer1.OnValueChanged += OnScoreValueChanged;
        scorePlayer2.OnValueChanged += OnScoreValueChanged;
        vencedor.OnValueChanged += OnWinnerValueChanged;
        partidaFinalizada.OnValueChanged += OnMatchStateChanged;

        OnScoreChanged?.Invoke(
            scorePlayer1.Value,
            scorePlayer2.Value
        );

        // Também funciona se a interface for criada
        // depois de a partida já ter terminado.
        NotifyMatchEndedIfNeeded();
    }

    public override void OnNetworkDespawn()
    {
        scorePlayer1.OnValueChanged -= OnScoreValueChanged;
        scorePlayer2.OnValueChanged -= OnScoreValueChanged;
        vencedor.OnValueChanged -= OnWinnerValueChanged;
        partidaFinalizada.OnValueChanged -= OnMatchStateChanged;

        matchEndNotified = false;
    }

    private void OnScoreValueChanged(int previousValue, int newValue)
    {
        OnScoreChanged?.Invoke(
            scorePlayer1.Value,
            scorePlayer2.Value
        );
    }

    private void OnWinnerValueChanged(int previousValue, int newValue)
    {
        NotifyMatchEndedIfNeeded();
    }

    private void OnMatchStateChanged(bool previousValue, bool newValue)
    {
        NotifyMatchEndedIfNeeded();
    }

    private void NotifyMatchEndedIfNeeded()
    {
        if (!partidaFinalizada.Value || matchEndNotified)
            return;

        matchEndNotified = true;
        OnMatchEnded?.Invoke(vencedor.Value);
    }

    public void StartMatch()
    {
        SceneManager.LoadScene(
            "GameplayUI",
            LoadSceneMode.Additive
        );
    }

    public void AddScore(int playerID)
    {
        // Somente o servidor pode alterar a pontuação.
        if (!IsServer || partidaFinalizada.Value)
            return;

        if (playerID == 0)
        {
            scorePlayer1.Value++;
        }
        else if (playerID == 1)
        {
            scorePlayer2.Value++;
        }
        else
        {
            return;
        }

        // Verifica se alguém atingiu o limite.
        if (scorePlayer1.Value >= limitePontuacao)
        {
            FinalizarPartida(0);
        }
        else if (scorePlayer2.Value >= limitePontuacao)
        {
            FinalizarPartida(1);
        }
    }

    private void FinalizarPartida(int playerID)
    {
        if (!IsServer || partidaFinalizada.Value)
            return;

        vencedor.Value = playerID;
        partidaFinalizada.Value = true;

        Debug.Log(
            "Partida finalizada! Vencedor: Jogador " +
            (playerID + 1)
        );
    }
}
