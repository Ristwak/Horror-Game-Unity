using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuStuff : MonoBehaviour
{
    public string nextSceneName;

    public void B_LoadScene()
    {
        if (!string.IsNullOrEmpty(nextSceneName))
            SceneManager.LoadScene(nextSceneName);
    }

    public void B_QuitGame()
    {
        Application.Quit();
    }
}