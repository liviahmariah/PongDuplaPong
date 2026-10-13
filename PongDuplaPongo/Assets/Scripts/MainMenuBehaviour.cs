using UnityEngine;
using UnityEngine.SceneManagement; 
using Unity.Netcode;

public class MainMenuBehaviour : MonoBehaviour {
    public void StartHost() {
        NetworkManager.Singleton.StartHost();
        
        
        NetworkManager.Singleton.SceneManager.LoadScene("Gameplay", LoadSceneMode.Single);
        SceneManager.LoadScene("GameplayUI", LoadSceneMode.Additive);
    }

    public void StartClient() {

        NetworkManager.Singleton.StartClient();
    }
    
}
