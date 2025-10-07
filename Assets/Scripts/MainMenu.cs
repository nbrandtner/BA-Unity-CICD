using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    // Called when "Level 1" button is clicked
    public void LoadLevel1()
    {
        SceneManager.LoadScene("Level1");
    }

    // Called when "Level 2" button is clicked
    public void LoadLevel2()
    {
        SceneManager.LoadScene("Level2"); 
    }

    // Called when "Level 3" button is clicked
    public void LoadLevel3()
    {
        SceneManager.LoadScene("Level 3"); 
    }

    // Called when "Quit" button is clicked
    public void QuitGame()
    {
        Debug.Log("Quitting game...");

        #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
}
