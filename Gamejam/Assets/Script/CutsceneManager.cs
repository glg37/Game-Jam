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

    [Header("Narração do Quadrinho")]
    [SerializeField] private AudioSource narration;

    [Header("Controles")]
    [SerializeField] private GameObject controlsText;
    [SerializeField] private CanvasGroup controlsCanvasGroup;
    [SerializeField] private AudioSource controlsNarration;

    [Header("Fade dos Controles")]
    [SerializeField] private float controlsFadeDuration = 1.5f;
    [SerializeField] private float controlsFadeOutDuration = 1.5f;

    [Header("Próxima Cena")]
    [SerializeField] private string nextSceneName;

    [Header("Fades da Tela")]
    [SerializeField] private float fadeInDuration = 3f;
    [SerializeField] private float fadeToBlackDuration = 1.5f;

    [Header("Fade do Quadrinho")]
    [SerializeField] private float comicFadeDuration = 1.5f;

    private void Start()
    {
        SetBlackScreen(1f);

        if (comicImage != null)
            comicImage.SetActive(false);

        if (controlsText != null)
            controlsText.SetActive(false);

        if (controlsCanvasGroup != null)
            controlsCanvasGroup.alpha = 0f;

        StartCoroutine(CutsceneSequence());
    }

    private IEnumerator CutsceneSequence()
    {
        // ==========================================
        // FADE-IN INICIAL
        // ==========================================

        yield return StartCoroutine(
            FadeBlack(1f, 0f, fadeInDuration)
        );

        // ==========================================
        // ANIMAÇÃO
        // ==========================================

        if (animator != null)
        {
            animator.Play(animationStateName);

            yield return new WaitForSeconds(animationDuration);
        }

        // ==========================================
        // TELA PRETA ANTES DO QUADRINHO
        // ==========================================

        yield return StartCoroutine(
            FadeBlack(0f, 1f, fadeToBlackDuration)
        );

        // ==========================================
        // MOSTRA QUADRINHO
        // ==========================================

        if (comicImage != null)
            comicImage.SetActive(true);

        if (comicCanvasGroup != null)
            comicCanvasGroup.alpha = 1f;

        // ==========================================
        // NARRAÇÃO DO QUADRINHO
        // ==========================================

        if (narration != null && narration.clip != null)
        {
            narration.Play();

            // Espera a duração REAL do áudio
            yield return new WaitForSeconds(
                narration.clip.length
            );

            // Garante que o áudio terminou
            narration.Stop();
        }

        // ==========================================
        // AGORA SIM: FADE DO QUADRINHO
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
        // CONTROLES
        // ==========================================

        if (controlsText != null)
            controlsText.SetActive(true);

        // Começa a narração dos controles
        if (controlsNarration != null &&
            controlsNarration.clip != null)
        {
            controlsNarration.Play();
        }

        // Fade-in dos controles
        if (controlsCanvasGroup != null)
        {
            yield return StartCoroutine(
                FadeControls(
                    0f,
                    1f,
                    controlsFadeDuration
                )
            );
        }

        // ==========================================
        // ESPERA NARRAÇÃO DOS CONTROLES
        // ==========================================

        if (controlsNarration != null &&
            controlsNarration.clip != null)
        {
            yield return new WaitForSeconds(
                controlsNarration.clip.length
            );

            controlsNarration.Stop();
        }

        // ==========================================
        // FADE-OUT DOS CONTROLES
        // ==========================================

        if (controlsCanvasGroup != null)
        {
            yield return StartCoroutine(
                FadeControls(
                    1f,
                    0f,
                    controlsFadeOutDuration
                )
            );
        }

        if (controlsText != null)
            controlsText.SetActive(false);

        // ==========================================
        // PRÓXIMA CENA
        // ==========================================

        if (!string.IsNullOrEmpty(nextSceneName))
        {
            SceneManager.LoadScene(nextSceneName);
        }
    }

    private IEnumerator FadeBlack(
        float startAlpha,
        float endAlpha,
        float duration)
    {
        float timer = 0f;

        while (timer < duration)
        {
            timer += Time.deltaTime;

            float t = timer / duration;

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

    private IEnumerator FadeComic(
        float startAlpha,
        float endAlpha)
    {
        float timer = 0f;

        while (timer < comicFadeDuration)
        {
            timer += Time.deltaTime;

            float t =
                timer / comicFadeDuration;

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

    private IEnumerator FadeControls(
        float startAlpha,
        float endAlpha,
        float duration)
    {
        float timer = 0f;

        while (timer < duration)
        {
            timer += Time.deltaTime;

            float t =
                timer / duration;

            controlsCanvasGroup.alpha =
                Mathf.Lerp(
                    startAlpha,
                    endAlpha,
                    t
                );

            yield return null;
        }

        controlsCanvasGroup.alpha = endAlpha;
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