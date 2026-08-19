using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CutsceneManager : MonoBehaviour
{
    [Header("Animação")]
    [SerializeField] private Animator animator;
    [SerializeField] private string animationStateName = "Cutscene";

    [Header("Tela Preta")]
    [SerializeField] private Image blackScreen;

    [Header("Quadrinho")]
    [SerializeField] private GameObject comicImage;

    [Header("Narração")]
    [SerializeField] private AudioSource narration;

    [Header("Próxima Cena")]
    [SerializeField] private string nextSceneName;

    [Header("Fades")]
    [SerializeField] private float fadeDuration = 1.5f;

    private void Start()
    {
        // Começa completamente preto
        SetBlackScreen(1f);

        // Quadrinho começa escondido
        if (comicImage != null)
            comicImage.SetActive(false);

        StartCoroutine(CutsceneSequence());
    }

    private IEnumerator CutsceneSequence()
    {
        Debug.Log("Cutscene começou.");

        // 1. Fade inicial: preto -> transparente
        Debug.Log("Iniciando fade inicial...");
        yield return StartCoroutine(FadeBlack(1f, 0f));

        // 2. Começa a animação
        if (animator != null)
        {
            Debug.Log("Iniciando animação...");

            animator.Play(animationStateName);

            yield return StartCoroutine(WaitForAnimation());

            Debug.Log("ANIMAÇÃO TERMINOU!");
        }

        // 3. Fade para preto
        Debug.Log("INICIANDO FADE PARA PRETO!");

        yield return StartCoroutine(FadeBlack(0f, 1f));

        Debug.Log("FADE PARA PRETO TERMINOU!");

        // 4. Mostra o quadrinho
        if (comicImage != null)
        {
            comicImage.SetActive(true);
            Debug.Log("Quadrinho ativado.");
        }

        // 5. Começa narração
        if (narration != null && narration.clip != null)
        {
            Debug.Log("Começando narração...");

            narration.Play();

            yield return new WaitWhile(
                () => narration.isPlaying
            );

            Debug.Log("Narração terminou.");
        }

        // 6. Esconde quadrinho
        if (comicImage != null)
            comicImage.SetActive(false);

        // 7. Fade final
        Debug.Log("Iniciando fade final...");

        yield return StartCoroutine(FadeBlack(0f, 1f));

        Debug.Log("Fade final terminou.");

        // 8. Próxima cena
        if (!string.IsNullOrEmpty(nextSceneName))
        {
            SceneManager.LoadScene(nextSceneName);
        }
    }

    private IEnumerator WaitForAnimation()
    {
        // Espera o Animator entrar no estado
        yield return null;

        AnimatorStateInfo stateInfo =
            animator.GetCurrentAnimatorStateInfo(0);

        // Espera até o estado correto começar
        while (!stateInfo.IsName(animationStateName))
        {
            yield return null;

            stateInfo =
                animator.GetCurrentAnimatorStateInfo(0);
        }

        // Espera a animação terminar
        while (stateInfo.normalizedTime < 1f)
        {
            yield return null;

            stateInfo =
                animator.GetCurrentAnimatorStateInfo(0);
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

    private void SetBlackScreen(float alpha)
    {
        if (blackScreen == null)
            return;

        Color color = blackScreen.color;
        color.a = alpha;
        blackScreen.color = color;
    }
}