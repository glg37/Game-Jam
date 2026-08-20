using UnityEngine;
using UnityEngine.UI;

public class ArmaAgua : MonoBehaviour
{
    [Header("Disparo")]
    public GameObject jatoAgua;
    private ParticleSystem particula;

    [Header("Som do Disparo")]
    [SerializeField] private AudioSource somAgua;
    [SerializeField] private AudioSource somParadaAgua;

    [Header("Água")]
    public float aguaMaxima = 100f;
    public float aguaAtual = 100f;
    public float consumoPorSegundo = 20f;

    [Header("Recarga")]
    public float recargaPorSegundo = 70f;

    [Header("UI")]
    public Slider barraAgua;

    private void Start()
    {
        aguaAtual = aguaMaxima;

        if (barraAgua != null)
        {
            barraAgua.minValue = 0;
            barraAgua.maxValue = aguaMaxima;
            barraAgua.value = aguaAtual;
        }

        if (jatoAgua != null)
        {
            particula = jatoAgua.GetComponent<ParticleSystem>();

            jatoAgua.SetActive(true);

            if (particula != null)
            {
                particula.Stop(
                    true,
                    ParticleSystemStopBehavior.StopEmittingAndClear
                );
            }
        }

        if (somAgua != null)
        {
            somAgua.loop = true;
            somAgua.playOnAwake = false;
            somAgua.Stop();
        }

        if (somParadaAgua != null)
        {
            somParadaAgua.loop = false;
            somParadaAgua.playOnAwake = false;
            somParadaAgua.Stop();
        }
    }

    private void Update()
    {
        if (Input.GetMouseButton(0) && aguaAtual > 0)
        {
            AtirarAgua();
        }
        else
        {
            PararAgua();
        }

        AtualizarBarra();
    }

    private void AtirarAgua()
    {
        if (particula != null)
        {
            if (!particula.isPlaying)
            {
                particula.Play();
            }
        }

        // Som contínuo do disparo
        if (somAgua != null && !somAgua.isPlaying)
        {
            somAgua.Play();
        }

        aguaAtual -=
            consumoPorSegundo * Time.deltaTime;

        if (aguaAtual <= 0)
        {
            aguaAtual = 0;
            PararAgua();
        }
    }

    private void PararAgua()
    {
        bool estavaAtirando =
            somAgua != null && somAgua.isPlaying;

        if (particula != null)
        {
            particula.Stop(
                true,
                ParticleSystemStopBehavior.StopEmitting
            );
        }

        if (estavaAtirando)
        {
            // Começa o som de parada primeiro
            if (somParadaAgua != null)
            {
                somParadaAgua.Stop();
                somParadaAgua.Play();
            }

            // Para o som contínuo imediatamente depois
            if (somAgua != null)
            {
                somAgua.Stop();
            }
        }
    }

    public void RecarregarAgua()
    {
        aguaAtual +=
            recargaPorSegundo * Time.deltaTime;

        if (aguaAtual >= aguaMaxima)
        {
            aguaAtual = aguaMaxima;
        }

        AtualizarBarra();
    }

    public void AtualizarBarra()
    {
        if (barraAgua != null)
        {
            barraAgua.value = aguaAtual;
        }
    }
}