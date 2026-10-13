
using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Netcode;
using System.Collections.Generic;

public class PaddleBehaviour : NetworkBehaviour
{
    [Header("Movimentação")]
    public float speed = 8f;
    public float yBound = 3.8f;

    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;

    private readonly Vector2[] spawnPositions =
    {
        new Vector2(-5f,  1.8f),
        new Vector2( 5f,  1.8f),
        new Vector2(-7f, -1.8f),
        new Vector2( 7f, -1.8f)
    };

    [Header("Identificação do jogador")]
    public NetworkVariable<int> indiceJogador =
        new NetworkVariable<int>(
            -1,
            NetworkVariableReadPermission.Everyone,
            NetworkVariableWritePermission.Server
        );

    private readonly Color[] coresJogadores =
    {
        new Color(0.14f, 0.47f, 1f),   
        new Color(0.94f, 0.20f, 0.20f), 
        new Color(0.14f, 0.72f, 0.42f), 
        new Color(0.66f, 0.33f, 0.97f)  
    };

    private static readonly Dictionary<ulong, int>
        indicesAtribuidos = new Dictionary<ulong, int>();

    public NetworkVariable<float> netInputY =
        new NetworkVariable<float>(
            0f,
            NetworkVariableReadPermission.Everyone,
            NetworkVariableWritePermission.Owner
        );

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public override void OnNetworkSpawn()
    {
        indiceJogador.OnValueChanged += OnIndiceJogadorChanged;

        if (IsServer)
        {
            int indice = ObterOuAtribuirIndice(OwnerClientId);

            indiceJogador.Value = indice;

            if (indice >= 0 && indice < spawnPositions.Length)
            {
                transform.position = spawnPositions[indice];
            }
            else
            {
                Debug.LogWarning(
                    "Não foi possível atribuir um índice de jogador. " +
                    "O limite é de 4 jogadores."
                );
            }
        }

        AtualizarCor(indiceJogador.Value);
    }

    private int ObterOuAtribuirIndice(ulong clientId)
    {

        if (indicesAtribuidos.TryGetValue(clientId, out int existente))
        {
            return existente;
        }

        for (int i = 0; i < coresJogadores.Length; i++)
        {
            bool ocupado = false;

            foreach (int indice in indicesAtribuidos.Values)
            {
                if (indice == i)
                {
                    ocupado = true;
                    break;
                }
            }

            if (!ocupado)
            {
                indicesAtribuidos[clientId] = i;
                return i;
            }
        }

        return -1;
    }

    private void OnIndiceJogadorChanged(int anterior, int atual)
    {
        AtualizarCor(atual);
    }

    private void AtualizarCor(int indice)
    {
        if (spriteRenderer == null)
            return;

        if (indice >= 0 && indice < coresJogadores.Length)
        {
            spriteRenderer.color = coresJogadores[indice];
        }
    }

    public override void OnNetworkDespawn()
    {
        indiceJogador.OnValueChanged -= OnIndiceJogadorChanged;

        if (IsServer)
        {
            indicesAtribuidos.Remove(OwnerClientId);
        }
    }

    private void Update()
    {
        if (!IsOwner)
            return;

        float moveY = 0f;

        if (Keyboard.current != null)
        {
            if (Keyboard.current.wKey.isPressed ||
                Keyboard.current.upArrowKey.isPressed)
            {
                moveY = 1f;
            }
            else if (Keyboard.current.sKey.isPressed ||
                     Keyboard.current.downArrowKey.isPressed)
            {
                moveY = -1f;
            }
        }

        netInputY.Value = moveY;
    }

    private void FixedUpdate()
    {
        if (IsOwner)
        {
            Vector2 novaPos = rb.position +
                new Vector2(
                    0,
                    netInputY.Value * speed * Time.fixedDeltaTime
                );

            novaPos.y = Mathf.Clamp(novaPos.y, -yBound, yBound);
            rb.MovePosition(novaPos);
        }

        if (IsServer && !IsOwner)
        {
            Vector2 novaPos = rb.position +
                new Vector2(
                    0,
                    netInputY.Value * speed * Time.fixedDeltaTime
                );

            novaPos.y = Mathf.Clamp(novaPos.y, -yBound, yBound);
            rb.MovePosition(novaPos);
        }
    }
}
