using UnityEngine;

public class KeyPickUp : MonoBehaviour
{
    public GameObject handUI;
    public GameObject objToActivate;

    private GameObject ob;
    private bool inReach;
    private bool isCollected;

    void Start()
    {
        if (handUI != null)
            handUI.SetActive(false);

        if (objToActivate != null)
            objToActivate.SetActive(false);

        ob = this.gameObject;
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

        if (objToActivate != null)
            objToActivate.SetActive(true);

        MeshRenderer meshRenderer = ob.GetComponent<MeshRenderer>();

        if (meshRenderer != null)
            meshRenderer.enabled = false;
    }
}