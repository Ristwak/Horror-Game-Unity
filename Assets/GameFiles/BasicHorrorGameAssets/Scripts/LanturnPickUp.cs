using System.Collections;
using UnityEngine;

public class LanturnPickUp : MonoBehaviour
{
    private GameObject OB;
    public GameObject handUI;
    public GameObject lanturn;

    private bool inReach;
    private bool isCollected;

    void Start()
    {
        OB = this.gameObject;

        if (handUI != null)
            handUI.SetActive(false);

        if (lanturn != null)
            lanturn.SetActive(false);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Reach") && !isCollected)
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

            if (handUI != null)
                handUI.SetActive(false);
        }
    }

    void Update()
    {
        if (!inReach || isCollected || !Input.GetButtonDown("Interact"))
            return;

        isCollected = true;

        if (handUI != null)
            handUI.SetActive(false);

        if (lanturn != null)
            lanturn.SetActive(true);

        StartCoroutine(end());
    }

    IEnumerator end()
    {
        yield return new WaitForSeconds(0.01f);

        if (OB != null)
            Destroy(OB);
    }
}