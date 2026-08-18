using UnityEngine;

public class VictoryTrigger : MonoBehaviour
{
    [SerializeField] private GameVictory gameVictory;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        if (gameVictory != null)
        {
            gameVictory.PlayerEnteredCabin();
        }
    }
}