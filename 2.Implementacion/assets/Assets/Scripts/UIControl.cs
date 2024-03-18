using System;
using System.Linq;
using TMPro;
using UnitScripts;
using UnitScripts.ShipScripts;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Gestiona la interfaz de usuario
/// </summary>
public class UIControl : MonoBehaviour
{
    public event Action<bool> OnFullScreenToggleValueChanged;
    public event Action<float> OnVolumeSliderValueChanged;
    public event Action<float> OnQualitySliderValueChanged;
    public event Action OnNewGameButtonClicked;
    public event Action OnEndGameButtonClicked;
    public event Action OnReturnToMainMenuButtonClicked;
    public event Action OnResumeButtonClicked;
    public event Action OnQuitButtonClicked;

    private const string WinText = "¡Has ganado!";
    private const string LoseText = "¡Has perdido!";
    private const string TieText = "¡Empate!";

    //Escena 1 (juego)
    [SerializeField] private TextMeshProUGUI txtPlayerCredits;
    [SerializeField] private TextMeshProUGUI txtPlayerMetal;
    [SerializeField] private TextMeshProUGUI txtPlayerEnergy;
    [SerializeField] private TextMeshProUGUI txtPlayerPopulation;
    private bool _onGui;

    //Escena 3 (resumen de parida)
    [SerializeField] private Text txtResultado;

    private void Start()
    {
        var buildIndex = SceneManager.GetActiveScene().buildIndex;
        switch (buildIndex)
        {
            case 0 or 2 or 3:
                _onGui = false;
                if (buildIndex == 3) EndGameGUI();
                break;
            case 1:
                _onGui = true;
                break;
            default:
                throw new Exception($"Invalid scene index: {buildIndex}");
        }
    }

    private void OnGUI()
    {
        if (!_onGui) return;

        var gameManager = GameManager.Instance;
        if (!gameManager) throw new Exception("GameManager not found");

        var playerResources = gameManager.PlayerResources;

        txtPlayerCredits.text = playerResources[ResourceType.Credits].Amount.ToString();
        txtPlayerMetal.text = playerResources[ResourceType.Metal].Amount.ToString();
        txtPlayerEnergy.text = playerResources[ResourceType.Energy].Amount.ToString();
        txtPlayerPopulation.text = $"{playerResources[ResourceType.Population].Amount}/" +
                                   $"{playerResources[ResourceType.Population].MaxAmount}";
    }

    private void EndGameGUI()
    {
        var gameManager = GameManager.Instance;
        if (!gameManager) throw new Exception("GameManager not found");

        txtResultado.text = gameManager.Winner switch
        {
            PlayerType.Player => WinText,
            PlayerType.IA => LoseText,
            PlayerType.Neutral => TieText,
            _ => throw new Exception($"Invalid player type: {gameManager.Winner}")
        };

        //Player
        var playerTotalResources = gameManager.PlayerTotalResources;
        var playerUsedResources = gameManager.PlayerUsedResources;
        var playerCreatedShips = gameManager.PlayerCreatedShips;
        var playerDestroyedShips = gameManager.PlayerDestroyedShips;

        //IA
        var iaTotalResources = gameManager.IaTotalResources;
        var iaUsedResources = gameManager.IaUsedResources;
        var iaCreatedShips = gameManager.IaCreatedShips;
        var iaDestroyedShips = gameManager.IaDestroyedShips;

        txtResultado.text += Environment.NewLine;

        txtResultado.text += $"{Environment.NewLine}Recursos generados{Environment.NewLine}";

        txtResultado.text += $"Jugador:\tcreditos: {playerTotalResources[ResourceType.Credits]}" +
                             $"\t\tmetal: {playerTotalResources[ResourceType.Metal]}" +
                             $"\t\tenergia: {playerTotalResources[ResourceType.Energy]}" +
                             $"\t\tpoblacion: {playerTotalResources[ResourceType.Population]}" +
                             $"\t\ttotal: {playerTotalResources.Sum(kv => kv.Value)}" +
                             $"{Environment.NewLine}";

        txtResultado.text += $"IA:\t\t\t\tcreditos: {iaTotalResources[ResourceType.Credits]}" +
                             $"\t\tmetal: {iaTotalResources[ResourceType.Metal]}" +
                             $"\t\tenergia: {iaTotalResources[ResourceType.Energy]}" +
                             $"\t\tpoblacion: {iaTotalResources[ResourceType.Population]}" +
                             $"\t\ttotal: {iaTotalResources.Sum(pair => pair.Value)}" +
                             $"{Environment.NewLine}";

        txtResultado.text += Environment.NewLine;

        txtResultado.text += $"{Environment.NewLine}Recursos utilizados{Environment.NewLine}";

        txtResultado.text += $"Jugador:\tcreditos: {playerUsedResources[ResourceType.Credits]}" +
                             $"\t\tmetal: {playerUsedResources[ResourceType.Metal]}" +
                             $"\t\tenergia: {playerUsedResources[ResourceType.Energy]}" +
                             $"\t\tpoblacion: {playerUsedResources[ResourceType.Population]}" +
                             $"\t\ttotal: {playerUsedResources.Sum(kv => kv.Value)}" +
                             $"{Environment.NewLine}";

        txtResultado.text += $"IA:\t\t\t\tcreditos: {iaUsedResources[ResourceType.Credits]}" +
                             $"\t\tmetal: {iaUsedResources[ResourceType.Metal]}" +
                             $"\t\tenergia: {iaUsedResources[ResourceType.Energy]}" +
                             $"\t\tpoblacion: {iaUsedResources[ResourceType.Population]}" +
                             $"\t\ttotal: {iaUsedResources.Sum(kv => kv.Value)}" +
                             $"{Environment.NewLine}";

        txtResultado.text += Environment.NewLine;

        txtResultado.text += $"{Environment.NewLine}Naves creadas{Environment.NewLine}";

        txtResultado.text += $"Jugador:\tRapidas: {playerCreatedShips[ShipType.FastShip]}" +
                             $"\t\tNormales: {playerCreatedShips[ShipType.NormalShip]}" +
                             $"\t\tBombarderos: {playerCreatedShips[ShipType.BomberShip]}" +
                             $"\t\ttotal: {playerCreatedShips.Sum(kv => kv.Value)}" +
                             $"{Environment.NewLine}";

        txtResultado.text += $"IA:\t\t\t\tRapidas: {iaCreatedShips[ShipType.FastShip]}" +
                             $"\t\tNormales: {iaCreatedShips[ShipType.NormalShip]}" +
                             $"\t\tBombarderos: {iaCreatedShips[ShipType.BomberShip]}" +
                             $"\t\ttotal: {iaCreatedShips.Sum(kv => kv.Value)}" +
                             $"{Environment.NewLine}";

        txtResultado.text += Environment.NewLine;

        txtResultado.text += $"{Environment.NewLine}Naves destruidas{Environment.NewLine}";

        txtResultado.text += $"Jugador:\tRapidas: {playerDestroyedShips[ShipType.FastShip]}" +
                             $"\t\tNormales: {playerDestroyedShips[ShipType.NormalShip]}" +
                             $"\t\tBombarderos: {playerDestroyedShips[ShipType.BomberShip]}" +
                             $"\t\ttotal: {playerDestroyedShips.Sum(kv => kv.Value)}" +
                             $"{Environment.NewLine}";

        txtResultado.text += $"IA:\t\t\t\tRapidas: {iaDestroyedShips[ShipType.FastShip]}" +
                             $"\t\tNormales: {iaDestroyedShips[ShipType.NormalShip]}" +
                             $"\t\tBombarderos: {iaDestroyedShips[ShipType.BomberShip]}" +
                             $"\t\ttotal: {iaDestroyedShips.Sum(kv => kv.Value)}" +
                             $"{Environment.NewLine}";
    }

    public void FullScreenChanged(bool newValue)
    {
        OnFullScreenToggleValueChanged?.Invoke(newValue);
    }

    public void VolumeChanged(float newValue)
    {
        OnVolumeSliderValueChanged?.Invoke(newValue);
    }

    public void QualityChanged(float newValue)
    {
        OnQualitySliderValueChanged?.Invoke(newValue);
    }

    public void NewGame()
    {
        OnNewGameButtonClicked?.Invoke();
    }

    public void EndGame()
    {
        OnEndGameButtonClicked?.Invoke();
    }

    public void ReturnToMainMenu()
    {
        OnReturnToMainMenuButtonClicked?.Invoke();
    }

    public void Resume()
    {
        OnResumeButtonClicked?.Invoke();
    }

    public void Quit()
    {
        OnQuitButtonClicked?.Invoke();
    }
}