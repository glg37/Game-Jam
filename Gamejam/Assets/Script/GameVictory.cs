using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class GameVictory : MonoBehaviour
{
    [Header("Tempo")]
    [SerializeField] private float segundosPorHora = 60f;

    [Header("Relógio")]
    [SerializeField] private int horaInicial = 17;
    [SerializeField] private int horaFinal = 22;
    [SerializeField] private GameObject textoRelogio;

    [Header("Fade do Relógio")]
    [SerializeField] private float tempoParaSumirRelogio = 10f;
    [SerializeField] private float duracaoFadeRelogio = 2f;

    [Header("Vitória")]
    [SerializeField] private GameObject textoVitoria;
    [SerializeField] private float tempoParaSumirVitoria = 10f;
    [SerializeField] private float duracaoFadeVitoria = 2f;

    [Header("Incêndios")]
    [SerializeField] private FireEventManager fireEventManager;

    [Header("Collider da Cabana")]
    [SerializeField] private Collider colliderCabana;

    [Header("Fade da Tela")]
    [SerializeField] private Image telaPreta;
    [SerializeField] private float tempoFade = 2f;

    [Header("Cena Final")]
    [SerializeField] private string nomeCenaFinal;

    private int horaAtual;
    private float contador;

    private bool venceu = false;
    private bool entrandoNaCabana = false;

    private CanvasGroup grupoRelogio;
    private CanvasGroup grupoVitoria;
    private CanvasGroup grupoTelaPreta;

    private TMP_Text textoRelogioTMP;

    private void Start()
    {
        horaAtual = horaInicial;

        // RELÓGIO
        if (textoRelogio != null)
        {
            textoRelogioTMP =
                textoRelogio.GetComponentInChildren<TMP_Text>();

            grupoRelogio =
                textoRelogio.GetComponent<CanvasGroup>();

            if (grupoRelogio == null)
            {
                grupoRelogio =
                    textoRelogio.AddComponent<CanvasGroup>();
            }

            grupoRelogio.alpha = 1f;
            textoRelogio.SetActive(true);
        }

        // TEXTO DE VITÓRIA
        if (textoVitoria != null)
        {
            textoVitoria.SetActive(false);

            grupoVitoria =
                textoVitoria.GetComponent<CanvasGroup>();

            if (grupoVitoria == null)
            {
                grupoVitoria =
                    textoVitoria.AddComponent<CanvasGroup>();
            }

            grupoVitoria.alpha = 1f;
        }

        // TELA PRETA
        if (telaPreta != null)
        {
            grupoTelaPreta =
                telaPreta.GetComponent<CanvasGroup>();

            if (grupoTelaPreta == null)
            {
                grupoTelaPreta =
                    telaPreta.gameObject.AddComponent<CanvasGroup>();
            }

            grupoTelaPreta.alpha = 0f;

            telaPreta.gameObject.SetActive(true);

            Color cor = telaPreta.color;
            cor.r = 0f;
            cor.g = 0f;
            cor.b = 0f;
            cor.a = 1f;
            telaPreta.color = cor;

            telaPreta.transform.SetAsLastSibling();
        }

        // COLLIDER DA CABANA
        if (colliderCabana != null)
            colliderCabana.enabled = false;

        AtualizarRelogio();
    }

    private void Update()
    {
        if (venceu)
            return;

        contador += Time.deltaTime;

        if (contador >= segundosPorHora)
        {
            contador = 0f;
            horaAtual++;

            AtualizarRelogio();

            Debug.Log("Hora atual: " + horaAtual + ":00");

            if (horaAtual >= horaFinal)
            {
                Vitoria();
            }
        }
    }

    private void AtualizarRelogio()
    {
        if (textoRelogioTMP == null)
            return;

        textoRelogioTMP.text =
            horaAtual.ToString("00") + ":00";
    }

    private void Vitoria()
    {
        venceu = true;

        AtualizarRelogio();

        Debug.Log("O jogador sobreviveu até às 22:00!");

        // PARA NOVOS INCÊNDIOS E MÚSICAS
        if (fireEventManager != null)
            fireEventManager.PararIncendios();

        // TEXTO DE VITÓRIA
        if (textoVitoria != null)
        {
            textoVitoria.SetActive(true);

            if (grupoVitoria != null)
                grupoVitoria.alpha = 1f;
        }

        // LIBERA A CABANA
        if (colliderCabana != null)
            colliderCabana.enabled = true;

        StartCoroutine(FadeRelogio());
        StartCoroutine(FadeTextoVitoria());
    }

    private IEnumerator FadeRelogio()
    {
        yield return new WaitForSeconds(
            tempoParaSumirRelogio
        );

        if (grupoRelogio == null)
            yield break;

        float tempo = 0f;

        while (tempo < duracaoFadeRelogio)
        {
            tempo += Time.deltaTime;

            float progresso =
                tempo / duracaoFadeRelogio;

            grupoRelogio.alpha =
                Mathf.Lerp(1f, 0f, progresso);

            yield return null;
        }

        grupoRelogio.alpha = 0f;
        textoRelogio.SetActive(false);
    }

    private IEnumerator FadeTextoVitoria()
    {
        yield return new WaitForSeconds(
            tempoParaSumirVitoria
        );

        if (grupoVitoria == null)
            yield break;

        float tempo = 0f;

        while (tempo < duracaoFadeVitoria)
        {
            tempo += Time.deltaTime;

            float progresso =
                tempo / duracaoFadeVitoria;

            grupoVitoria.alpha =
                Mathf.Lerp(1f, 0f, progresso);

            yield return null;
        }

        grupoVitoria.alpha = 0f;
    }

    public void PlayerEnteredCabin()
    {
        if (!venceu || entrandoNaCabana)
            return;

        entrandoNaCabana = true;

        Debug.Log("Jogador entrou na cabana!");

        StartCoroutine(FinalizarJogo());
    }

    private IEnumerator FinalizarJogo()
    {
        Debug.Log("Iniciando fade para preto...");

        if (telaPreta != null)
        {
            telaPreta.gameObject.SetActive(true);
            telaPreta.transform.SetAsLastSibling();
        }

        float tempo = 0f;

        while (tempo < tempoFade)
        {
            tempo += Time.unscaledDeltaTime;

            float progresso =
                Mathf.Clamp01(tempo / tempoFade);

            if (grupoTelaPreta != null)
                grupoTelaPreta.alpha = progresso;

            yield return null;
        }

        if (grupoTelaPreta != null)
            grupoTelaPreta.alpha = 1f;

        Debug.Log("Fade completo! Carregando cena final...");

        yield return new WaitForSecondsRealtime(0.1f);

        Time.timeScale = 1f;

        SceneManager.LoadScene(nomeCenaFinal);
    }
}