using UnityEngine;

public class CaixaAgua : MonoBehaviour
{
    private ArmaAgua armaDoJogador;

    [Header("Mensagem na tela")]
    [SerializeField] private GameObject mensagemRecarregar;

    private void OnTriggerEnter(Collider other)
    {
        ArmaAgua arma = other.GetComponentInChildren<ArmaAgua>();

        if (arma != null)
        {
            armaDoJogador = arma;

            // Mostra a mensagem
            mensagemRecarregar.SetActive(true);

            Debug.Log("Entrou na caixa d'água");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        ArmaAgua arma = other.GetComponentInChildren<ArmaAgua>();

        if (arma != null)
        {
            armaDoJogador = null;

            // Esconde a mensagem
            mensagemRecarregar.SetActive(false);

            Debug.Log("Saiu da caixa d'água");
        }
    }

    void Update()
    {
        if (armaDoJogador != null && Input.GetKey(KeyCode.R))
        {
            armaDoJogador.RecarregarAgua();
        }
    }
}