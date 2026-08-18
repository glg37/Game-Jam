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
    public float consumoPorSegundo = 25f;

    [Header("Recarga")]
    public float recargaPorSegundo = 30f;

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
            jatoAgua.SetActive(false);
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
        if (jatoAgua != null)
        {
            jatoAgua.SetActive(true);

            if (particula != null && !particula.isPlaying)
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
        if (jatoAgua != null)
        {
            if (particula != null)
            {
                particula.Stop();
            }

            jatoAgua.SetActive(false);
        }
    }

    
    public void RecarregarAgua()
    {
        aguaAtual += recargaPorSegundo * Time.deltaTime;

        if (aguaAtual > aguaMaxima)
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

            Debug.Log("Água: " + aguaAtual + " | Barra: " + barraAgua.value);
        }
        else
        {
            Debug.LogError("A BarraAgua não foi atribuída!");
        }
    }
}