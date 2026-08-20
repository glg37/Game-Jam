using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class FadeSceneTransition : MonoBehaviour
{
    [Header("Tela Preta")]
    [SerializeField] private Image blackScreen;

    [Header("Música")]
    [SerializeField] private AudioSource music;

    [Header("Configuração")]
    [SerializeField] private float fadeDuration = 1.5f;
    [SerializeField] private string nextSceneName;

    private float volumeInicial;

    private void Start()
    {
        if (blackScreen != null)
        {
            Color color = blackScreen.color;
            color.a = 0f;
            blackScreen.color = color;
        }

        if (music != null)
        {
            volumeInicial = music.volume;
        }
    }

    public void StartTransition()
    {
        StartCoroutine(FadeAndLoadScene());
    }

    private IEnumerator FadeAndLoadScene()
    {
        float timer = 0f;

        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;

            float t = timer / fadeDuration;

            // Fade da tela
            if (blackScreen != null)
            {
                Color color = blackScreen.color;
                color.a = Mathf.Lerp(0f, 1f, t);
                blackScreen.color = color;
            }

            // Fade da música
            if (music != null)
            {
                music.volume = Mathf.Lerp(volumeInicial, 0f, t);
            }

            yield return null;
        }

        // Garante que tudo terminou completamente
        if (blackScreen != null)
        {
            Color color = blackScreen.color;
            color.a = 1f;
            blackScreen.color = color;
        }

        if (music != null)
        {
            music.volume = 0f;
        }

        SceneManager.LoadScene(nextSceneName);
    }
}