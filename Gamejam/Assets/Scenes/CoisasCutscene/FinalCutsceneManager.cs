using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

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

    [Header("Som de Vitória")]
    [SerializeField] private AudioSource victorySound;

    [Header("Voltar ao Menu")]
    [SerializeField] private float tempoNaTelaDeVitoria = 10f;
    [SerializeField] private float fadeOutFinalDuration = 2f;
    [SerializeField] private string menuSceneName = "Menu";

    private void Start()
    {
        // ==========================================
        // TELA PRETA
        // ==========================================

        SetBlackScreen(1f);

        if (blackScreen != null)
            blackScreen.transform.SetAsFirstSibling();

        // ==========================================
        // QUADRINHO
        // ==========================================

        if (comicImage != null)
            comicImage.SetActive(false);

        if (comicCanvasGroup != null)
            comicCanvasGroup.alpha = 0f;

        // ==========================================
        // PAINEL DE VITÓRIA
        // ==========================================

        if (victoryPanel != null)
            victoryPanel.SetActive(false);

        if (victoryCanvasGroup != null)
            victoryCanvasGroup.alpha = 0f;

        // ==========================================
        // SOM DE VITÓRIA
        // ==========================================

        if (victorySound != null)
        {
            victorySound.playOnAwake = false;
            victorySound.Stop();
        }

        StartCoroutine(CutsceneSequence());
    }

    private IEnumerator CutsceneSequence()
    {
        // ==========================================
        // ESPERA ANTES DO QUADRINHO
        // ==========================================

        yield return new WaitForSeconds(
            tempoAntesDoQuadrinho
        );

        // ==========================================
        // MOSTRA QUADRINHO
        // ==========================================

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

        // ==========================================
        // NARRAÇÃO
        // ==========================================

        if (narration != null &&
            narration.clip != null)
        {
            narration.Play();

            yield return new WaitForSeconds(
                narration.clip.length
            );

            narration.Stop();
        }

        // ==========================================
        // FADE-OUT DO QUADRINHO
        // ==========================================

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

        // ==========================================
        // PAINEL DE VITÓRIA
        // ==========================================

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

        // ==========================================
        // ESPERA NA TELA DE VITÓRIA
        // ==========================================

        yield return new WaitForSeconds(
            tempoNaTelaDeVitoria
        );

        // ==========================================
        // FADE-OUT FINAL
        // ==========================================

        yield return StartCoroutine(
            FadeVictory(
                1f,
                0f,
                fadeOutFinalDuration
            )
        );

        // ==========================================
        // VOLTA AO MENU
        // ==========================================

        SceneManager.LoadScene(menuSceneName);
    }

    // ==========================================
    // FADE DO QUADRINHO
    // ==========================================

    private IEnumerator FadeComic(
        float startAlpha,
        float endAlpha,
        float duration)
    {
        float timer = 0f;

        while (timer < duration)
        {
            timer += Time.deltaTime;

            float t =
                Mathf.Clamp01(
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

    // ==========================================
    // FADE DA VITÓRIA
    // ==========================================

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

            float t =
                Mathf.Clamp01(
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

    // ==========================================
    // TELA PRETA
    // ==========================================

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