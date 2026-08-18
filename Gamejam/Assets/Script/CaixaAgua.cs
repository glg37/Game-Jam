using UnityEngine;

public class CaixaAgua : MonoBehaviour
{
    private ArmaAgua armaDoJogador;

    private void OnTriggerEnter(Collider other)
    {
        ArmaAgua arma = other.GetComponentInChildren<ArmaAgua>();

        if (arma != null)
        {
            armaDoJogador = arma;
            Debug.Log("Entrou na caixa d'água");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        ArmaAgua arma = other.GetComponentInChildren<ArmaAgua>();

        if (arma != null)
        {
            armaDoJogador = null;
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