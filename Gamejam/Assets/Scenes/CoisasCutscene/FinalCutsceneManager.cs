using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class FinalCutsceneManager : MonoBehaviour
{
    [Header("Tela Preta")]
    [SerializeField] private Image blackScreen;
    [SerializeField] private float fadeInDuration = 2f;

    [Header("Quadrinho")]
    [SerializeField] private GameObject comicImage;
    [SerializeField] private CanvasGroup comicCanvasGroup;

    [Header("Narração do Quadrinho")]
    [SerializeField] private AudioSource narration;

    [Header("Fade do Quadrinho")]
    [SerializeField] private float comicFadeDuration = 1.5f;

    [Header("Tela de Vitória")]
    [SerializeField] private GameObject victoryPanel;
    [SerializeField] private CanvasGroup victoryCanvasGroup;

    [SerializeField] private float victoryFadeDuration = 1.5f;

    [Header("Botão")]
    [SerializeField] private Button menuButton;

    [Header("Menu")]
    [SerializeField] private string menuSceneName = "Menu";

    private void Start()
    {
        // Tela começa preta
        SetBlackScreen(1f);

        // Quadrinho começa desligado
        if (comicImage != null)
            comicImage.SetActive(false);

        // Painel de vitória começa desligado
        if (victoryPanel != null)
            victoryPanel.SetActive(false);

        if (victoryCanvasGroup != null)
            victoryCanvasGroup.alpha = 0f;

        // Configura botão
        if (menuButton != null)
        {
            menuButton.onClick.AddListener(VoltarAoMenu);
        }

        StartCoroutine(CutsceneSequence());
    }

    private IEnumerator CutsceneSequence()
    {
        // ==========================================
        // FADE-IN DA TELA PRETA
        // ==========================================

        yield return StartCoroutine(
            FadeBlack(1f, 0f, fadeInDuration)
        );

        // ==========================================
        // MOSTRA QUADRINHO
        // ==========================================

        if (comicImage != null)
            comicImage.SetActive(true);

        if (comicCanvasGroup != null)
            comicCanvasGroup.alpha = 1f;

        // ==========================================
        // NARRAÇÃO
        // ==========================================

        if (narration != null &&
            narration.clip != null)
        {
            narration.Play();

            // Espera o áudio inteiro terminar
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
                FadeComic(1f, 0f)
            );
        }

        if (comicImage != null)
            comicImage.SetActive(false);

        // ==========================================
        // PAINEL DE VITÓRIA
        // ==========================================

        if (victoryPanel != null)
            victoryPanel.SetActive(true);

        if (victoryCanvasGroup != null)
        {
            victoryCanvasGroup.alpha = 0f;

            yield return StartCoroutine(
                FadeVictory(0f, 1f)
            );
        }
    }

    // ==========================================
    // FADE DA TELA
    // ==========================================

    private IEnumerator FadeBlack(
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

            SetBlackScreen(
                Mathf.Lerp(
                    startAlpha,
                    endAlpha,
                    t
                )
            );

            yield return null;
        }

        SetBlackScreen(endAlpha);
    }

    // ==========================================
    // FADE DO QUADRINHO
    // ==========================================

    private IEnumerator FadeComic(
        float startAlpha,
        float endAlpha)
    {
        float timer = 0f;

        while (timer < comicFadeDuration)
        {
            timer += Time.deltaTime;

            float t =
                Mathf.Clamp01(
                    timer / comicFadeDuration
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
        float timer = 0f;

        while (timer < victoryFadeDuration)
        {
            timer += Time.deltaTime;

            float t =
                Mathf.Clamp01(
                    timer / victoryFadeDuration
                );

            victoryCanvasGroup.alpha =
                Mathf.Lerp(
                    startAlpha,
                    endAlpha,
                    t
                );

            yield return null;
        }

        victoryCanvasGroup.alpha = endAlpha;
    }

    // ==========================================
    // BOTÃO
    // ==========================================

    private void VoltarAoMenu()
    {
        if (narration != null)
            narration.Stop();

        SceneManager.LoadScene(menuSceneName);
    }

    private void SetBlackScreen(float alpha)
    {
        if (blackScreen == null)
            return;

        Color color = blackScreen.color;
        color.a = alpha;
        blackScreen.color = color;
    }
}