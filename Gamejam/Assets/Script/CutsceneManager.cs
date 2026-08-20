using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class CutsceneManager : MonoBehaviour
{
    [Header("Animação")]
    [SerializeField] private Animator animator;
    [SerializeField] private string animationStateName = "Cutscene";
    [SerializeField] private float animationDuration = 5f;

    [Header("Som da Animação")]
    [SerializeField] private AudioSource animationSound;

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

    [Header("Pular Cutscene")]
    [SerializeField] private TextMeshProUGUI skipText;
    [SerializeField] private float skipFadeDuration = 1f;

    [Header("Próxima Cena")]
    [SerializeField] private string nextSceneName;

    [Header("Fades da Tela")]
    [SerializeField] private float fadeInDuration = 3f;
    [SerializeField] private float fadeToBlackDuration = 1.5f;

    [Header("Fade do Quadrinho")]
    [SerializeField] private float comicFadeDuration = 1.5f;

    private bool cutsceneFinished = false;
    private bool isSkipping = false;

    private void Start()
    {
        SetBlackScreen(1f);

        if (comicImage != null)
            comicImage.SetActive(false);

        if (controlsText != null)
            controlsText.SetActive(false);

        if (controlsCanvasGroup != null)
            controlsCanvasGroup.alpha = 0f;

        if (skipText != null)
            skipText.alpha = 1f;

        if (animationSound != null)
        {
            animationSound.playOnAwake = false;
            animationSound.Stop();
            animationSound.Play();
        }

        StartCoroutine(CutsceneSequence());
    }

    private void Update()
    {
        if (cutsceneFinished || isSkipping)
            return;

        if (Input.GetKeyDown(KeyCode.Space))
            StartCoroutine(SkipCutscene());
    }

    private IEnumerator CutsceneSequence()
    {
        yield return StartCoroutine(
            FadeBlack(1f, 0f, fadeInDuration)
        );

        if (isSkipping)
            yield break;

        if (animator != null)
            animator.Play(animationStateName);

        yield return new WaitForSeconds(
            animationDuration
        );

        if (isSkipping)
            yield break;

        yield return StartCoroutine(
            FadeAnimationAndBlack()
        );

        if (comicImage != null)
            comicImage.SetActive(true);

        if (comicCanvasGroup != null)
            comicCanvasGroup.alpha = 1f;

        if (narration != null &&
            narration.clip != null)
        {
            narration.Play();

            yield return new WaitForSeconds(
                narration.clip.length
            );

            narration.Stop();
        }

        if (isSkipping)
            yield break;

        if (comicCanvasGroup != null)
        {
            yield return StartCoroutine(
                FadeComicAndNarration()
            );
        }

        if (comicImage != null)
            comicImage.SetActive(false);

        if (controlsText != null)
            controlsText.SetActive(true);

        if (controlsNarration != null &&
            controlsNarration.clip != null)
        {
            controlsNarration.Play();
        }

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

        if (isSkipping)
            yield break;

        if (controlsNarration != null &&
            controlsNarration.clip != null)
        {
            yield return new WaitForSeconds(
                controlsNarration.clip.length
            );

            controlsNarration.Stop();
        }

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

        LoadNextScene();
    }

    private IEnumerator FadeAnimationAndBlack()
    {
        float timer = 0f;

        float soundStartVolume =
            animationSound != null
                ? animationSound.volume
                : 0f;

        while (timer < fadeToBlackDuration)
        {
            timer += Time.deltaTime;

            float t = Mathf.Clamp01(
                timer / fadeToBlackDuration
            );

            SetBlackScreen(
                Mathf.Lerp(
                    0f,
                    1f,
                    t
                )
            );

            if (animationSound != null)
            {
                animationSound.volume =
                    Mathf.Lerp(
                        soundStartVolume,
                        0f,
                        t
                    );
            }

            yield return null;
        }

        SetBlackScreen(1f);

        if (animationSound != null)
        {
            animationSound.Stop();
            animationSound.volume = soundStartVolume;
        }
    }

    private IEnumerator FadeComicAndNarration()
    {
        float timer = 0f;

        float narrationStartVolume =
            narration != null
                ? narration.volume
                : 0f;

        while (timer < comicFadeDuration)
        {
            timer += Time.deltaTime;

            float t = Mathf.Clamp01(
                timer / comicFadeDuration
            );

            if (comicCanvasGroup != null)
            {
                comicCanvasGroup.alpha =
                    Mathf.Lerp(
                        1f,
                        0f,
                        t
                    );
            }

            if (narration != null)
            {
                narration.volume =
                    Mathf.Lerp(
                        narrationStartVolume,
                        0f,
                        t
                    );
            }

            yield return null;
        }

        if (comicCanvasGroup != null)
            comicCanvasGroup.alpha = 0f;

        if (narration != null)
        {
            narration.Stop();
            narration.volume = narrationStartVolume;
        }
    }

    private IEnumerator SkipCutscene()
    {
        isSkipping = true;

        if (animationSound != null &&
            animationSound.isPlaying)
        {
            yield return StartCoroutine(
                FadeAudio(
                    animationSound,
                    skipFadeDuration
                )
            );
        }

        if (narration != null &&
            narration.isPlaying)
        {
            yield return StartCoroutine(
                FadeAudio(
                    narration,
                    skipFadeDuration
                )
            );
        }

        if (controlsNarration != null &&
            controlsNarration.isPlaying)
        {
            yield return StartCoroutine(
                FadeAudio(
                    controlsNarration,
                    skipFadeDuration
                )
            );
        }

        if (comicCanvasGroup != null &&
            comicImage != null &&
            comicImage.activeSelf)
        {
            yield return StartCoroutine(
                FadeComic(
                    1f,
                    0f,
                    skipFadeDuration
                )
            );
        }

        if (comicImage != null)
            comicImage.SetActive(false);

        if (controlsCanvasGroup != null &&
            controlsText != null &&
            controlsText.activeSelf)
        {
            yield return StartCoroutine(
                FadeControls(
                    controlsCanvasGroup.alpha,
                    0f,
                    skipFadeDuration
                )
            );
        }

        if (controlsText != null)
            controlsText.SetActive(false);

        if (skipText != null)
        {
            float timer = 0f;
            float startAlpha = skipText.alpha;

            while (timer < skipFadeDuration)
            {
                timer += Time.deltaTime;

                float t = Mathf.Clamp01(
                    timer / skipFadeDuration
                );

                skipText.alpha =
                    Mathf.Lerp(
                        startAlpha,
                        0f,
                        t
                    );

                yield return null;
            }

            skipText.alpha = 0f;
        }

        LoadNextScene();
    }

    private IEnumerator FadeAudio(
        AudioSource audio,
        float duration)
    {
        float timer = 0f;
        float startVolume = audio.volume;

        while (timer < duration)
        {
            timer += Time.deltaTime;

            float t = Mathf.Clamp01(
                timer / duration
            );

            audio.volume =
                Mathf.Lerp(
                    startVolume,
                    0f,
                    t
                );

            yield return null;
        }

        audio.Stop();
        audio.volume = startVolume;
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

            float t = Mathf.Clamp01(
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

            if (comicCanvasGroup != null)
            {
                comicCanvasGroup.alpha =
                    Mathf.Lerp(
                        startAlpha,
                        endAlpha,
                        t
                    );
            }

            yield return null;
        }

        if (comicCanvasGroup != null)
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

            float t = Mathf.Clamp01(
                timer / duration
            );

            if (controlsCanvasGroup != null)
            {
                controlsCanvasGroup.alpha =
                    Mathf.Lerp(
                        startAlpha,
                        endAlpha,
                        t
                    );
            }

            yield return null;
        }

        if (controlsCanvasGroup != null)
            controlsCanvasGroup.alpha = endAlpha;
    }

    private void LoadNextScene()
    {
        if (cutsceneFinished)
            return;

        cutsceneFinished = true;

        if (!string.IsNullOrEmpty(nextSceneName))
            SceneManager.LoadScene(nextSceneName);
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