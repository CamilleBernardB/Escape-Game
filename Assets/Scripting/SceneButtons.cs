using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SceneButtons : MonoBehaviour
{
    [Header("Buttons")]
    [SerializeField] private Button taquinButton;
    [SerializeField] private Button zipButton;
    [SerializeField] private Button connectionsButton;

    [Header("Scene names (must match Build Settings)")]
    [SerializeField] private string taquinSceneName = "TaquinScene";
    [SerializeField] private string zipSceneName = "ZipScene";
    [SerializeField] private string connectionsSceneName = "ConnectionsScene";

    private void Awake()
    {
        taquinButton.onClick.AddListener(() => LoadScene(taquinSceneName));
        zipButton.onClick.AddListener(() => LoadScene(zipSceneName));
        connectionsButton.onClick.AddListener(() => LoadScene(connectionsSceneName));
    }




    public void SetButtonsInteractable(bool taquin, bool zip, bool connections)
    {
        taquinButton.interactable = taquin;
        zipButton.interactable = zip;
        connectionsButton.interactable = connections;
    }

    private void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}
