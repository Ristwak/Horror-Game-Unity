using UnityEngine;

public class UseChest : MonoBehaviour
{
    private GameObject OB;
    public GameObject handUI;
    public GameObject objToActivate;

    private bool inReach;
    private bool isOpened;

    void Start()
    {
        OB = this.gameObject;

        if (handUI != null)
            handUI.SetActive(false);

        if (objToActivate != null)
            objToActivate.SetActive(false);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Reach") && !isOpened)
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
        if (!inReach || isOpened || !Input.GetButtonDown("Interact"))
            return;

        isOpened = true;

        if (handUI != null)
            handUI.SetActive(false);

        if (objToActivate != null)
            objToActivate.SetActive(true);

        Animator chestAnimator = OB.GetComponent<Animator>();
        if (chestAnimator != null)
            chestAnimator.SetBool("open", true);

        BoxCollider chestCollider = OB.GetComponent<BoxCollider>();
        if (chestCollider != null)
            chestCollider.enabled = false;
    }
}