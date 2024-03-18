using Unity_UI_Samples.Scripts;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Controla el flujo de la aplicación.
/// Implementa el patrón observer para manejar los eventos de los botones.
/// </summary>
public class ApplicationManager : MonoBehaviour
{
    [SerializeField] private UIControl uiControl;

    [SerializeField] private PanelManager menuManager;
    [SerializeField] private Animator mainMenu;

    private bool _isGamePaused;

    private void Start()
    {
        uiControl.OnNewGameButtonClicked += NewGame;
        uiControl.OnEndGameButtonClicked += EndGame;
        uiControl.OnReturnToMainMenuButtonClicked += ReturnToMainMenu;
        uiControl.OnResumeButtonClicked += Resume;
        uiControl.OnQuitButtonClicked += Quit;
    }

    private void Update()
    {
        if (!Input.GetKeyDown(KeyCode.Escape)) return;

        if (!_isGamePaused) Pause();
        else Resume();
    }

    private void OnDestroy()
    {
        uiControl.OnNewGameButtonClicked -= NewGame;
        uiControl.OnEndGameButtonClicked -= EndGame;
        uiControl.OnReturnToMainMenuButtonClicked -= ReturnToMainMenu;
        uiControl.OnResumeButtonClicked -= Resume;
        uiControl.OnQuitButtonClicked -= Quit;
    }

    private void OnApplicationFocus(bool hasFocus)
    {
        if (hasFocus) return;

        Pause();
    }

    public void NewGame()
    {
        LevelLoader.LoadLevel(1);
    }

    public void EndGame()
    {
        Resume();
        LevelLoader.LoadLevel(3);
    }

    public void ReturnToMainMenu()
    {
        Resume();
        LevelLoader.LoadLevel(0);
    }

    private void Pause()
    {
        if (SceneManager.GetActiveScene().buildIndex != 1) return;
        
        if (_isGamePaused) return;

        _isGamePaused = true;

        menuManager.OpenPanel(mainMenu);
        Time.timeScale = 0;
    }

    public void Resume()
    {
        if (!_isGamePaused) return;

        _isGamePaused = false;
        menuManager.CloseCurrent();
        Time.timeScale = 1;
    }

    public void Quit()
    {
        Application.Quit();
    }
}