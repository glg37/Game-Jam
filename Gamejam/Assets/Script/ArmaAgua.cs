using UnityEngine;
using UnityEngine.UI;

public class ArmaAgua : MonoBehaviour
{
    [Header("Disparo")]
    public GameObject jatoAgua;
    private ParticleSystem particula;

    [Header("Água")]
    public float aguaMaxima = 100f;
    public float aguaAtual = 100f;
    public float consumoPorSegundo = 20f;

    [Header("Recarga")]
    public float recargaPorSegundo = 70f;

    [Header("UI")]
    public Slider barraAgua;

    void Start()
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

            // Deixa o objeto ativo para que as partículas
            // possam continuar existindo depois que o disparo parar.
            jatoAgua.SetActive(true);

            if (particula != null)
            {
                particula.Stop(
                    true,
                    ParticleSystemStopBehavior.StopEmittingAndClear
                );
            }
        }
    }

    void Update()
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

    void AtirarAgua()
    {
        if (particula != null)
        {
            if (!particula.isPlaying)
            {
                particula.Play();
            }
        }

        aguaAtual -= consumoPorSegundo * Time.deltaTime;

        if (aguaAtual <= 0)
        {
            aguaAtual = 0;
            PararAgua();
        }
    }

    void PararAgua()
    {
        if (particula != null)
        {
            // Para SOMENTE a emissão de novas partículas.
            // As partículas que já estão no ar continuam.
            particula.Stop(
                true,
                ParticleSystemStopBehavior.StopEmitting
            );
        }
    }

    public void RecarregarAgua()
    {
        aguaAtual += recargaPorSegundo * Time.deltaTime;

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