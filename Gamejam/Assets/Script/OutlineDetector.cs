using UnityEngine;

public class OutlineDetector : MonoBehaviour
{
    [SerializeField] private float distance = 3f;
    [SerializeField] private Camera playerCamera;
    [SerializeField] private GameObject interactionText;

    private Outline currentOutline;

    void Start()
    {
        interactionText.SetActive(false);
    }

    void Update()
    {
        if (Physics.Raycast(
            playerCamera.transform.position,
            playerCamera.transform.forward,
            out RaycastHit hit,
            distance))
        {
            Outline outline = hit.collider.GetComponentInParent<Outline>();

            if (outline != currentOutline)
            {
                if (currentOutline != null)
                    currentOutline.enabled = false;

                currentOutline = outline;

                if (currentOutline != null)
                    currentOutline.enabled = true;
            }

            interactionText.SetActive(currentOutline != null);
        }
        else
        {
            if (currentOutline != null)
            {
                currentOutline.enabled = false;
                currentOutline = null;
            }

            interactionText.SetActive(false);
        }
    }
}