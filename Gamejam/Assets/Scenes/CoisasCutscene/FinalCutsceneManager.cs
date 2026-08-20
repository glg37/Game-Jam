using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class FinalCutsceneManager : MonoBehaviour
{
    [Header("Tela Preta")]
    [SerializeField] private Image blackScreen;

    [Header("Quadrinho")]
    [SerializeField] private GameObject comicImage;
    [SerializeField] private CanvasGroup comicCanvasGroup;

    [Header("Entrada do Quadrinho")]
    [SerializeField] private float tempoAntesDoQuadrinho = 1f;
    [SerializeField] private float comicFadeInDuration = 1.5f;

    [Header("Narração do Quadrinho")]
    [SerializeField] private AudioSource narration;

    [Header("Fade do Quadrinho")]
    [SerializeField] private float comicFadeOutDuration = 1.5f;

    [Header("Tela de Vitória")]
    [SerializeField] private GameObject victoryPanel;
    [SerializeField] private CanvasGroup victoryCanvasGroup;
    [SerializeField] private float victoryFadeDuration = 1.5f;
    [SerializeField] private float tempoNaTelaDeVitoria = 10f;

    [Header("Som de Vitória")]
    [SerializeField] private AudioSource victorySound;

    [Header("Frase Final")]
    [SerializeField] private TextMeshProUGUI fraseFinal;
    [SerializeField]
    private string textoFrase =
        "OBRIGADO POR JOGAR!";

    [SerializeField] private float velocidadeDigitacao = 0.05f;
    [SerializeField] private float tempoAposFrase = 10f;
    [SerializeField] private float fadeOutFraseDuration = 2f;

    [Header("Final")]
    [SerializeField] private string menuSceneName = "Menu";

    private void Start()
    {
        SetBlackScreen(1f);

        if (blackScreen != null)
            blackScreen.transform.SetAsFirstSibling();

        if (comicImage != null)
            comicImage.SetActive(false);

        if (comicCanvasGroup != null)
            comicCanvasGroup.alpha = 0f;

        if (victoryPanel != null)
            victoryPanel.SetActive(false);

        if (victoryCanvasGroup != null)
            victoryCanvasGroup.alpha = 0f;

        if (fraseFinal != null)
        {
            fraseFinal.text = "";
            fraseFinal.gameObject.SetActive(false);
        }

        if (victorySound != null)
        {
            victorySound.playOnAwake = false;
            victorySound.Stop();
        }

        StartCoroutine(CutsceneSequence());
    }

    private IEnumerator CutsceneSequence()
    {
        yield return new WaitForSeconds(
            tempoAntesDoQuadrinho
        );

        if (comicImage != null)
            comicImage.SetActive(true);

        if (comicCanvasGroup != null)
        {
            comicCanvasGroup.alpha = 0f;

            yield return StartCoroutine(
                FadeComic(
                    0f,
                    1f,
                    comicFadeInDuration
                )
            );
        }

        if (narration != null &&
            narration.clip != null)
        {
            narration.Play();

            yield return new WaitForSeconds(
                narration.clip.length
            );

            narration.Stop();
        }

        if (comicCanvasGroup != null)
        {
            yield return StartCoroutine(
                FadeComic(
                    1f,
                    0f,
                    comicFadeOutDuration
                )
            );
        }

        if (comicImage != null)
            comicImage.SetActive(false);

        if (victoryPanel != null)
            victoryPanel.SetActive(true);

        if (victorySound != null)
            victorySound.Play();

        if (victoryCanvasGroup != null)
        {
            victoryCanvasGroup.alpha = 0f;

            yield return StartCoroutine(
                FadeVictory(
                    0f,
                    1f
                )
            );
        }

        yield return new WaitForSeconds(
            tempoNaTelaDeVitoria
        );

        if (victoryCanvasGroup != null)
        {
            yield return StartCoroutine(
                FadeVictory(
                    1f,
                    0f,
                    victoryFadeDuration
                )
            );
        }

        if (victoryPanel != null)
            victoryPanel.SetActive(false);

        if (fraseFinal != null)
        {
            fraseFinal.gameObject.SetActive(true);
            fraseFinal.alpha = 1f;

            yield return StartCoroutine(
                DigitarTexto()
            );
        }

        yield return new WaitForSeconds(
            tempoAposFrase
        );

        if (fraseFinal != null)
        {
            yield return StartCoroutine(
                FadeOutFrase()
            );

            fraseFinal.gameObject.SetActive(false);
        }

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        SceneManager.LoadScene(menuSceneName);
    }

    private IEnumerator DigitarTexto()
    {
        fraseFinal.text = "";

        foreach (char letra in textoFrase)
        {
            fraseFinal.text += letra;

            yield return new WaitForSeconds(
                velocidadeDigitacao
            );
        }
    }

    private IEnumerator FadeOutFrase()
    {
        float timer = 0f;
        float alphaInicial = fraseFinal.alpha;

        while (timer < fadeOutFraseDuration)
        {
            timer += Time.deltaTime;

            float t = Mathf.Clamp01(
                timer / fadeOutFraseDuration
            );

            fraseFinal.alpha = Mathf.Lerp(
                alphaInicial,
                0f,
                t
            );

            yield return null;
        }

        fraseFinal.alpha = 0f;
    }

    private IEnumerator FadeComic(
        float startAlpha,
        float endAlpha,
        float duration)
    {
        float timer = 0f;

        while (timer < duration)
        {
            timer += Time.deltaTime;

            float t = Mathf.Clamp01(
                timer / duration
            );

            comicCanvasGroup.alpha =
                Mathf.Lerp(
                    startAlpha,
                    endAlpha,
                    t
                );

            yield return null;
        }

        comicCanvasGroup.alpha = endAlpha;
    }

    private IEnumerator FadeVictory(
        float startAlpha,
        float endAlpha)
    {
        yield return StartCoroutine(
            FadeVictory(
                startAlpha,
                endAlpha,
                victoryFadeDuration
            )
        );
    }

    private IEnumerator FadeVictory(
        float startAlpha,
        float endAlpha,
        float duration)
    {
        float timer = 0f;

        while (timer < duration)
        {
            timer += Time.deltaTime;

            float t = Mathf.Clamp01(
                timer / duration
            );

            if (victoryCanvasGroup != null)
            {
                victoryCanvasGroup.alpha =
                    Mathf.Lerp(
                        startAlpha,
                        endAlpha,
                        t
                    );
            }

            yield return null;
        }

        if (victoryCanvasGroup != null)
            victoryCanvasGroup.alpha = endAlpha;
    }

    private void SetBlackScreen(float alpha)
    {
        if (blackScreen == null)
            return;

        Color color = blackScreen.color;

        color.r = 0f;
        color.g = 0f;
        color.b = 0f;
        color.a = alpha;

        blackScreen.color = color;
    }
}