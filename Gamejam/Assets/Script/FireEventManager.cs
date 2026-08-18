using System.Collections;
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
            // Espera um tempo aleatório antes do próximo incêndio
            float waitTime = Random.Range(minTimeUntilFire, maxTimeUntilFire);

            yield return new WaitForSeconds(waitTime);

            // Escolhe uma árvore
            currentTree = GetRandomAvailableTree();

            if (currentTree == null)
            {
                Debug.LogWarning("Não há árvores disponíveis para pegar fogo.");
                yield return new WaitForSeconds(5f);
                continue;
            }

            // Inicia o incêndio
            currentTree.StartFire();

            // Começa a música
            if (fireMusic != null)
                fireMusic.Play();

            // Espera o jogador apagar
            float timer = timeToExtinguish;

            while (timer > 0f)
            {
                // Se o fogo foi apagado
                if (!currentTree.IsBurning)
                {
                    if (fireMusic != null)
                        fireMusic.Stop();

                    break;
                }

                timer -= Time.deltaTime;
                yield return null;
            }

            // Se o tempo acabou e a árvore ainda está pegando fogo
            if (currentTree != null && currentTree.IsBurning)
            {
                Defeat();
                yield break;
            }

            // Pequeno intervalo antes de começar outro evento
            yield return new WaitForSeconds(5f);
        }
    }

    private FireTree GetRandomAvailableTree()
    {
        if (trees == null || trees.Length == 0)
            return null;

        // Tenta algumas vezes encontrar uma árvore que não esteja queimando
        for (int i = 0; i < 20; i++)
        {
            FireTree tree = trees[Random.Range(0, trees.Length)];

            if (!tree.IsBurning)
                return tree;
        }

        return null;
    }

    private void Defeat()
    {
        Debug.Log("O jogador não conseguiu apagar o incêndio!");

        if (fireMusic != null)
            fireMusic.Stop();

        if (defeatScreen != null)
            defeatScreen.SetActive(true);

        // Desativa todos os scripts do Player
        if (player != null)
        {
            MonoBehaviour[] playerScripts = player.GetComponentsInChildren<MonoBehaviour>();

            foreach (MonoBehaviour script in playerScripts)
            {
                script.enabled = false;
            }
        }

        // Libera o mouse para a tela de derrota
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        Time.timeScale = 0f;
    }
}