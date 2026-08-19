using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CutsceneManager : MonoBehaviour
{
    [Header("Animação")]
    [SerializeField] private Animator animator;
    [SerializeField] private string animationStateName = "Cutscene";
    [SerializeField] private float animationDuration = 5f;

    [Header("Tela Preta")]
    [SerializeField] private Image blackScreen;

    [Header("Quadrinho")]
    [SerializeField] private GameObject comicImage;
    [SerializeField] private CanvasGroup comicCanvasGroup;

    [Header("Narração")]
    [SerializeField] private AudioSource narration;

    [Header("Próxima Cena")]
    [SerializeField] private string nextSceneName;

    [Header("Fades")]
    [SerializeField] private float fadeDuration = 1.5f;
    [SerializeField] private float comicFadeDuration = 1.5f;

    private void Start()
    {
        // Começa completamente preto
        SetBlackScreen(1f);

        // Quadrinho começa desligado
        if (comicImage != null)
            comicImage.SetActive(false);

        StartCoroutine(CutsceneSequence());
    }

    private IEnumerator CutsceneSequence()
    {
        // =========================================
        // 1. FADE OUT INICIAL
        // =========================================

        yield return StartCoroutine(
            FadeBlack(1f, 0f)
        );

        // =========================================
        // 2. COMEÇA A ANIMAÇÃO
        // =========================================

        if (animator != null)
        {
            animator.Play(animationStateName);

            yield return new WaitForSeconds(animationDuration);
        }

        // =========================================
        // 3. FADE PARA PRETO
        // =========================================

        yield return StartCoroutine(
            FadeBlack(0f, 1f)
        );

        // =========================================
        // 4. MOSTRA O QUADRINHO
        // =========================================

        if (comicImage != null)
            comicImage.SetActive(true);

        if (comicCanvasGroup != null)
            comicCanvasGroup.alpha = 1f;

        // =========================================
        // 5. COMEÇA A NARRAÇÃO
        // =========================================

        if (narration != null && narration.clip != null)
        {
            narration.Play();

            yield return new WaitWhile(
                () => narration.isPlaying
            );
        }

        // =========================================
        // 6. FADE-OUT DO QUADRINHO
        // =========================================

        if (comicCanvasGroup != null)
        {
            yield return StartCoroutine(
                FadeComic(1f, 0f)
            );
        }

        // =========================================
        // 7. DESATIVA O QUADRINHO
        // =========================================

        if (comicImage != null)
            comicImage.SetActive(false);

        // =========================================
        // 8. MUDA DE CENA
        // =========================================

        if (!string.IsNullOrEmpty(nextSceneName))
        {
            SceneManager.LoadScene(nextSceneName);
        }
    }

    private IEnumerator FadeBlack(float startAlpha, float endAlpha)
    {
        float timer = 0f;

        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;

            float t = timer / fadeDuration;

            SetBlackScreen(
                Mathf.Lerp(startAlpha, endAlpha, t)
            );

            yield return null;
        }

        SetBlackScreen(endAlpha);
    }

    private IEnumerator FadeComic(float startAlpha, float endAlpha)
    {
        float timer = 0f;

        while (timer < comicFadeDuration)
        {
            timer += Time.deltaTime;

            float t = timer / comicFadeDuration;

            comicCanvasGroup.alpha =
                Mathf.Lerp(startAlpha, endAlpha, t);

            yield return null;
        }

        comicCanvasGroup.alpha = endAlpha;
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