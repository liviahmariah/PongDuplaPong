using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Netcode;

public class PaddleBehaviour : NetworkBehaviour {
    public float speed = 8f;
    public float yBound = 3.8f; 
    
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;

    private readonly Vector2[] spawnPositions = new Vector2[]
    {
        new Vector2(-5f,  1.8f),
        new Vector2( 5f,  1.8f),
        new Vector2(-7f, -1.8f),
        new Vector2( 7f, -1.8f)
    };

    public NetworkVariable<float> netInputY = new NetworkVariable<float>(0f, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    void Awake() {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public override void OnNetworkSpawn() {
        if (IsServer) {
            int playerIndex = (int)OwnerClientId;

            if (playerIndex < spawnPositions.Length) {
                transform.position = spawnPositions[playerIndex];
            } else {
                float x = (playerIndex % 2 == 0) ? -5f : 5f;
                transform.position = new Vector2(x, 0f);
            }
        }
    }

    void Update() {
        if (!IsOwner) return;

        float moveY = 0f;

        if (Keyboard.current != null) {
            if (Keyboard.current.wKey.isPressed || Keyboard.current.upArrowKey.isPressed)
                moveY = 1f;
            else if (Keyboard.current.sKey.isPressed || Keyboard.current.downArrowKey.isPressed)
                moveY = -1f;
        }

        netInputY.Value = moveY;
    }

    void FixedUpdate() {
        if (IsOwner) {
            Vector2 novaPos = rb.position + new Vector2(0, netInputY.Value * speed * Time.fixedDeltaTime);
            novaPos.y = Mathf.Clamp(novaPos.y, -yBound, yBound);
            rb.MovePosition(novaPos);
        }

        if (IsServer && !IsOwner) {
            Vector2 novaPos = rb.position + new Vector2(0, netInputY.Value * speed * Time.fixedDeltaTime);
            novaPos.y = Mathf.Clamp(novaPos.y, -yBound, yBound);
            rb.MovePosition(novaPos);
        }
    }
}