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

    [Header("Músicas")]
    [SerializeField] private AudioSource calmMusic;
    [SerializeField] private AudioSource fireMusic;

    [SerializeField] private float calmVolume = 0.5f;
    [SerializeField] private float fireVolume = 0.7f;
    [SerializeField] private float fadeDuration = 2f;

    [Header("Derrota")]
    [SerializeField] private GameObject defeatScreen;

    [SerializeField] private GameObject player;

    private FireTree currentTree;
    private Coroutine fireRoutine;

    private List<FireTree> treesAlreadyUsed = new List<FireTree>();

    private Coroutine musicFadeRoutine;

    private void Start()
    {
        if (defeatScreen != null)
            defeatScreen.SetActive(false);

        // Música calma começa tocando
        if (calmMusic != null)
        {
            calmMusic.volume = calmVolume;
            calmMusic.loop = true;
            calmMusic.Play();
        }

        // Música de incêndio começa desligada
        if (fireMusic != null)
        {
            fireMusic.volume = 0f;
            fireMusic.loop = true;
            fireMusic.Stop();
        }

        fireRoutine = StartCoroutine(FireCycle());
    }

    private IEnumerator FireCycle()
    {
        while (true)
        {
            float waitTime = Random.Range(
                minTimeUntilFire,
                maxTimeUntilFire
            );

            yield return new WaitForSeconds(waitTime);

            currentTree = GetRandomUnusedTree();

            if (currentTree == null)
            {
                Debug.Log("Todas as árvores já pegaram fogo!");
                yield break;
            }

            treesAlreadyUsed.Add(currentTree);

            currentTree.StartFire();

            Debug.Log(
                "Nova árvore pegando fogo: " +
                currentTree.gameObject.name
            );

            // Troca para música frenética
            StartMusicTransition(true);

            float timer = timeToExtinguish;

            while (timer > 0f)
            {
                if (!currentTree.IsBurning)
                {
                    // Volta para música calma
                    StartMusicTransition(false);

                    break;
                }

                timer -= Time.deltaTime;

                yield return null;
            }

            if (currentTree != null &&
                currentTree.IsBurning)
            {
                Defeat();

                yield break;
            }

            yield return new WaitForSeconds(5f);
        }
    }

    private void StartMusicTransition(bool fire)
    {
        if (musicFadeRoutine != null)
            StopCoroutine(musicFadeRoutine);

        musicFadeRoutine = StartCoroutine(
            CrossFadeMusic(fire)
        );
    }

    private IEnumerator CrossFadeMusic(bool fire)
    {
        float startCalmVolume =
            calmMusic != null ? calmMusic.volume : 0f;

        float startFireVolume =
            fireMusic != null ? fireMusic.volume : 0f;

        if (fire && fireMusic != null &&
            !fireMusic.isPlaying)
        {
            fireMusic.volume = 0f;
            fireMusic.Play();
        }

        float timer = 0f;

        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;

            float t = timer / fadeDuration;

            if (calmMusic != null)
            {
                float targetCalm =
                    fire ? 0f : calmVolume;

                calmMusic.volume =
                    Mathf.Lerp(
                        startCalmVolume,
                        targetCalm,
                        t
                    );
            }

            if (fireMusic != null)
            {
                float targetFire =
                    fire ? fireVolume : 0f;

                fireMusic.volume =
                    Mathf.Lerp(
                        startFireVolume,
                        targetFire,
                        t
                    );
            }

            yield return null;
        }

        if (calmMusic != null)
        {
            calmMusic.volume =
                fire ? 0f : calmVolume;
        }

        if (fireMusic != null)
        {
            fireMusic.volume =
                fire ? fireVolume : 0f;

            if (!fire)
                fireMusic.Stop();
        }
    }

    private FireTree GetRandomUnusedTree()
    {
        if (trees == null || trees.Length == 0)
            return null;

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

        if (availableTrees.Count == 0)
            return null;

        int index =
            Random.Range(0, availableTrees.Count);

        return availableTrees[index];
    }

    private void Defeat()
    {
        Debug.Log(
            "O jogador não conseguiu apagar o incêndio!"
        );

        if (musicFadeRoutine != null)
            StopCoroutine(musicFadeRoutine);

        if (calmMusic != null)
            calmMusic.Stop();

        if (fireMusic != null)
            fireMusic.Stop();

        if (defeatScreen != null)
            defeatScreen.SetActive(true);

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