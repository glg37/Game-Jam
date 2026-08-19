using UnityEngine;

public class CabinTrigger : MonoBehaviour
{
    [SerializeField] private GameVictory gameVictory;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            gameVictory.PlayerEnteredCabin();
        }
    }
}