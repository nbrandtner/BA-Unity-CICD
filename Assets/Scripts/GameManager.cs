using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject winMessage;
    [SerializeField] private string mainMenuSceneName = "Scenes/mainmenu";

    private bool levelComplete = false;

    public void ShowWinMessage()
    {
        if (winMessage != null)
            winMessage.SetActive(true);

        // Disable gameplay scripts
        DisableGameplay();

        levelComplete = true;
    }

    private void Update()
    {
        if (levelComplete)
        {
            Debug.Log("Waiting for Keypress");
            if (Input.anyKey)
                Debug.Log("Something is pressed: " + Event.current);
            if (Input.anyKeyDown || Input.GetMouseButtonDown(0))
            {
                Debug.Log("Key or mouse pressed! Loading main menu...");
                SceneManager.LoadScene(mainMenuSceneName);
            }
        }
    }


    private void DisableGameplay()
    {
        foreach (var mirror in Object.FindObjectsByType<DraggableMirror>(FindObjectsSortMode.None))
        {
            mirror.DisableMirrors(true);
        }
    }
}
