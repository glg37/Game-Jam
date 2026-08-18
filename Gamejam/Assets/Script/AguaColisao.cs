using UnityEngine;

public class AguaColisao : MonoBehaviour
{
    private void OnParticleCollision(GameObject other)
    {
        FireTree fogo = other.GetComponentInParent<FireTree>();

        if (fogo != null && fogo.IsBurning)
        {
            fogo.AguaAtingindo();
        }
    }
}