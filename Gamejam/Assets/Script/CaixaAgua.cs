using UnityEngine;

public class CaixaAgua : MonoBehaviour
{
    [Header("Interação")]
    [SerializeField] private float distanciaInteracao = 3f;
    [SerializeField] private Camera cameraJogador;

    [Header("Mensagem na tela")]
    [SerializeField] private GameObject mensagemRecarregar;

    private ArmaAgua armaDoJogador;

    private void Start()
    {
        mensagemRecarregar.SetActive(false);

        armaDoJogador = FindFirstObjectByType<ArmaAgua>();
    }

    private void Update()
    {
        VerificarOlhar();

        if (armaDoJogador != null && Input.GetKey(KeyCode.R))
        {
            armaDoJogador.RecarregarAgua();
        }
    }

    private void VerificarOlhar()
    {
        Ray ray = new Ray(
            cameraJogador.transform.position,
            cameraJogador.transform.forward
        );

        if (Physics.Raycast(ray, out RaycastHit hit, distanciaInteracao))
        {
            if (hit.collider.transform.IsChildOf(transform) ||
                hit.collider.gameObject == gameObject)
            {
                mensagemRecarregar.SetActive(true);
                return;
            }
        }

        mensagemRecarregar.SetActive(false);
    }
}