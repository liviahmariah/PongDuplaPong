using UnityEngine;
using UnityEngine.SceneManagement; // Necessário para carregar cenas locais
using Unity.Netcode;

public class MainMenuBehaviour : MonoBehaviour {
    public void StartHost() {
        // Inicia como Servidor e Jogador 1
        NetworkManager.Singleton.StartHost();
        
        // GameManager.Instance.StartMatch();
        
        NetworkManager.Singleton.SceneManager.LoadScene("Gameplay", LoadSceneMode.Single);
        SceneManager.LoadScene("GameplayUI", LoadSceneMode.Additive);
    }

    public void StartClient() {
        // Conecta-se à sala existente como Jogador 2
        NetworkManager.Singleton.StartClient();
    }
    
    // public void OnPlayButtonClicked() {
    //     GameManager.Instance.StartLocalMatch();
    // }
}
