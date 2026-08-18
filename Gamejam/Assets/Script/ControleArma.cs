using UnityEngine;

public class ControleArma : MonoBehaviour
{
    public KeyCode teclaArma = KeyCode.E;

    [Header("Arma")]
    public GameObject armaAgua;

    private bool armaAtiva = false;

    void Start()
    {
        if (armaAgua != null)
            armaAgua.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(teclaArma))
        {
            AlternarArma();
        }
    }

    void AlternarArma()
    {
        armaAtiva = !armaAtiva;

        if (armaAgua != null)
            armaAgua.SetActive(armaAtiva);
    }
}