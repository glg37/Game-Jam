using UnityEngine;

public class CaixaAgua : MonoBehaviour
{
    [Header("Interação")]
    [SerializeField] private float distanciaInteracao = 3f;
    [SerializeField] private Camera cameraJogador;

    [Header("Mensagem na tela")]
    [SerializeField] private GameObject mensagemRecarregar;

    [Header("Som da Recarga")]
    [SerializeField] private AudioSource somRecarga;

    private ArmaAgua armaDoJogador;

    private bool podeRecarregar = false;

    private void Start()
    {
        mensagemRecarregar.SetActive(false);

        armaDoJogador = FindFirstObjectByType<ArmaAgua>();

        if (somRecarga != null)
            somRecarga.Stop();
    }

    private void Update()
    {
        VerificarOlhar();

        if (podeRecarregar &&
            armaDoJogador != null &&
            Input.GetKey(KeyCode.R))
        {
            armaDoJogador.RecarregarAgua();

            // Começa o som enquanto segura R
            if (somRecarga != null && !somRecarga.isPlaying)
            {
                somRecarga.Play();
            }
        }
        else
        {
            // Para quando soltar R ou olhar para outro lugar
            if (somRecarga != null && somRecarga.isPlaying)
            {
                somRecarga.Stop();
            }
        }
    }

    private void VerificarOlhar()
    {
        podeRecarregar = false;

        Ray ray = new Ray(
            cameraJogador.transform.position,
            cameraJogador.transform.forward
        );

        if (Physics.Raycast(
            ray,
            out RaycastHit hit,
            distanciaInteracao))
        {
            if (hit.collider.transform.IsChildOf(transform) ||
                hit.collider.gameObject == gameObject)
            {
                podeRecarregar = true;
                mensagemRecarregar.SetActive(true);
                return;
            }
        }

        mensagemRecarregar.SetActive(false);
    }
}