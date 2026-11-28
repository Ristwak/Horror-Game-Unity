using UnityEngine;
using UnityEngine.SceneManagement;

public class KillPlayer : MonoBehaviour
{
    public string nextSceneName;
    public float delay = 0.5f;
    public GameObject fadeout;

    private bool playerInsideTrigger;
    private bool transitionStarted;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player") || transitionStarted)
            return;

        playerInsideTrigger = true;

        if (fadeout != null)
            fadeout.SetActive(true);

        CancelInvoke(nameof(LoadNextScene));
        Invoke(nameof(LoadNextScene), delay);
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player") || transitionStarted)
            return;

        playerInsideTrigger = false;
        CancelInvoke(nameof(LoadNextScene));

        if (fadeout != null)
            fadeout.SetActive(false);
    }

    private void LoadNextScene()
    {
        if (!playerInsideTrigger || transitionStarted)
            return;

        transitionStarted = true;
        SceneManager.LoadScene(nextSceneName);
    }
}