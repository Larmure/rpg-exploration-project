using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField] private Button continueButton;
    [SerializeField] private Button settingsButton;
    [SerializeField] private string firstLevelSceneName = "SampleScene";

    private void Start()
    {
        // Pas de système de sauvegarde pour l'instant : bouton désactivé
        continueButton.interactable = false;

        // Paramètres pas encore décidés : désactivé aussi pour éviter un menu vide
        settingsButton.interactable = false;
    }

    public void OnNewGameClicked()
    {
        SceneManager.LoadScene(firstLevelSceneName);
    }

    public void OnContinueClicked()
    {
        // TODO: charger la sauvegarde une fois le système en place
    }

    public void OnSettingsClicked()
    {
        // TODO: écran de paramètres une fois défini
    }
}