using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireEventManager : MonoBehaviour
{
    [Header("Árvores")]
    [SerializeField] private FireTree[] trees;

    [Header("Tempo até um incêndio")]
    [SerializeField] private float minTimeUntilFire = 30f;
    [SerializeField] private float maxTimeUntilFire = 90f;

    [Header("Tempo para apagar")]
    [SerializeField] private float timeToExtinguish = 30f;

    [Header("Música")]
    [SerializeField] private AudioSource fireMusic;

    [Header("Derrota")]
    [SerializeField] private GameObject defeatScreen;

    [SerializeField] private GameObject player;

    private FireTree currentTree;
    private Coroutine fireRoutine;

    // Árvores que já pegaram fogo
    private List<FireTree> treesAlreadyUsed = new List<FireTree>();

    private void Start()
    {
        if (defeatScreen != null)
            defeatScreen.SetActive(false);

        fireRoutine = StartCoroutine(FireCycle());
    }

    private IEnumerator FireCycle()
    {
        while (true)
        {
            // Espera antes do próximo incêndio
            float waitTime = Random.Range(
                minTimeUntilFire,
                maxTimeUntilFire
            );

            yield return new WaitForSeconds(waitTime);

            // Escolhe uma árvore que ainda não foi usada
            currentTree = GetRandomUnusedTree();

            // Se não houver mais árvores
            if (currentTree == null)
            {
                Debug.Log("Todas as árvores já pegaram fogo!");

                yield break;
            }

            // Registra a árvore como usada
            treesAlreadyUsed.Add(currentTree);

            // Inicia o incêndio
            currentTree.StartFire();

            Debug.Log(
                "Nova árvore pegando fogo: " +
                currentTree.gameObject.name
            );

            // Música
            if (fireMusic != null)
                fireMusic.Play();

            // Tempo para o jogador apagar
            float timer = timeToExtinguish;

            while (timer > 0f)
            {
                // Jogador apagou
                if (!currentTree.IsBurning)
                {
                    if (fireMusic != null)
                        fireMusic.Stop();

                    break;
                }

                timer -= Time.deltaTime;

                yield return null;
            }

            // Se o tempo acabou e ainda está queimando
            if (currentTree != null &&
                currentTree.IsBurning)
            {
                Defeat();

                yield break;
            }

            // Intervalo antes do próximo incêndio
            yield return new WaitForSeconds(5f);
        }
    }

    private FireTree GetRandomUnusedTree()
    {
        if (trees == null || trees.Length == 0)
            return null;

        // Cria uma lista somente com árvores ainda não usadas
        List<FireTree> availableTrees =
            new List<FireTree>();

        foreach (FireTree tree in trees)
        {
            if (tree != null &&
                !treesAlreadyUsed.Contains(tree))
            {
                availableTrees.Add(tree);
            }
        }

        // Nenhuma disponível
        if (availableTrees.Count == 0)
            return null;

        // Escolhe aleatoriamente
        int index =
            Random.Range(0, availableTrees.Count);

        return availableTrees[index];
    }

    private void Defeat()
    {
        Debug.Log(
            "O jogador não conseguiu apagar o incêndio!"
        );

        if (fireMusic != null)
            fireMusic.Stop();

        if (defeatScreen != null)
            defeatScreen.SetActive(true);

        // Desativa todos os scripts do Player
        if (player != null)
        {
            MonoBehaviour[] playerScripts =
                player.GetComponentsInChildren<MonoBehaviour>();

            foreach (MonoBehaviour script in playerScripts)
            {
                script.enabled = false;
            }
        }

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        Time.timeScale = 0f;
    }
}