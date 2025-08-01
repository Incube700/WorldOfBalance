using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Button playButton;
    [SerializeField] private string gameSceneName = "TestScene";
    
    void Start()
    {
        // Setup button listener
        if (playButton != null)
        {
            playButton.onClick.AddListener(PlayGame);
        }
    }
    
    public void PlayGame()
    {
        Debug.Log("Loading game scene...");
        SceneManager.LoadScene(gameSceneName);
    }
    
    public void QuitGame()
    {
        Debug.Log("Quitting game...");
        Application.Quit();
    }
}