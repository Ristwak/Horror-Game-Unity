using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Door : MonoBehaviour
{
    public GameObject handUI;
    public GameObject UIText;
    public GameObject invKey;
    public GameObject fadeFX;

    public string nextSceneName;

    private bool inReach;
    private bool transitionStarted;

    void Start()
    {
        if (handUI != null)
            handUI.SetActive(false);

        if (UIText != null)
            UIText.SetActive(false);

        if (invKey != null)
            invKey.SetActive(false);

        if (fadeFX != null)
            fadeFX.SetActive(false);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Reach") && !transitionStarted)
        {
            inReach = true;

            if (handUI != null)
                handUI.SetActive(true);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Reach"))
        {
            inReach = false;

            if (!transitionStarted)
            {
                if (handUI != null)
                    handUI.SetActive(false);

                if (UIText != null)
                    UIText.SetActive(false);
            }
        }
    }

    void Update()
    {
        if (!inReach || transitionStarted ||
            !Input.GetButtonDown("Interact"))
            return;

        if (invKey == null)
            return;

        if (!invKey.activeInHierarchy)
        {
            if (handUI != null)
                handUI.SetActive(true);

            if (UIText != null)
                UIText.SetActive(true);
        }
        else
        {
            transitionStarted = true;

            if (handUI != null)
                handUI.SetActive(false);

            if (UIText != null)
                UIText.SetActive(false);

            if (fadeFX != null)
                fadeFX.SetActive(true);

            StartCoroutine(ending());
        }
    }

    IEnumerator ending()
    {
        yield return new WaitForSeconds(0.6f);

        if (!string.IsNullOrEmpty(nextSceneName))
            SceneManager.LoadScene(nextSceneName);
    }
}