using UnityEngine;

public class CaixaAgua : MonoBehaviour
{
    private bool jogadorDentro = false;
    private ArmaAgua arma;

    void OnTriggerEnter(Collider other)
    {
        ArmaAgua armaEncontrada = other.GetComponentInChildren<ArmaAgua>();

        if (armaEncontrada != null)
        {
            jogadorDentro = true;
            arma = armaEncontrada;
        }
    }

    void OnTriggerExit(Collider other)
    {
        ArmaAgua armaEncontrada = other.GetComponentInChildren<ArmaAgua>();

        if (armaEncontrada != null)
        {
            jogadorDentro = false;
            arma = null;
        }
    }

    void Update()
    {
        if (jogadorDentro && Input.GetKeyDown(KeyCode.R))
        {
            arma.RecarregarAgua();
        }
    }
}