using UnityEngine;

public class FireExtinguisher : MonoBehaviour
{
    [SerializeField] private float interactionDistance = 3f;
    [SerializeField] private Camera playerCamera;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            Ray ray = new Ray(
                playerCamera.transform.position,
                playerCamera.transform.forward
            );

            if (Physics.Raycast(ray, out RaycastHit hit, interactionDistance))
            {
                FireTree tree = hit.collider.GetComponentInParent<FireTree>();

                if (tree != null && tree.IsBurning)
                {
                    tree.ExtinguishFire();
                }
            }
        }
    }
}