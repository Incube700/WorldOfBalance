using UnityEngine;
using UnityEngine.UI;

public class MenuUI : MonoBehaviour
{
    [Header("Menu Buttons")]
    [SerializeField] private Button startButton;
    [SerializeField] private Button quitButton;
    [SerializeField] private Button restartButton;
    
    [Header("Menu References")]
    [SerializeField] private MainMenu mainMenu;
    
    void Start()
    {
        SetupButtons();
    }
    
    void SetupButtons()
    {
        if (startButton != null)
        {
            startButton.onClick.AddListener(OnStartButtonClicked);
        }
        
        if (quitButton != null)
        {
            quitButton.onClick.AddListener(OnQuitButtonClicked);
        }
        
        if (restartButton != null)
        {
            restartButton.onClick.AddListener(OnRestartButtonClicked);
        }
    }
    
    void OnStartButtonClicked()
    {
        if (mainMenu != null)
        {
            mainMenu.StartGame();
        }
    }
    
    void OnQuitButtonClicked()
    {
        if (mainMenu != null)
        {
            mainMenu.QuitGame();
        }
    }
    
    void OnRestartButtonClicked()
    {
        if (mainMenu != null)
        {
            mainMenu.RestartGame();
        }
    }
} 