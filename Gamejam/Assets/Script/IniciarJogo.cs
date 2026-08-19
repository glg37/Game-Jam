using UnityEngine;
using UnityEngine.SceneManagement;

public class IniciarJogo : MonoBehaviour
{
    public GameObject painelInicial;
    public void MudarCena(string NomeDaCena)
    {
        SceneManager.LoadScene(NomeDaCena);
    }
    public void OnApplicationQuit()
    {
        Application.Quit();
    }
   public void AbrirPainel(GameObject painel)
    {
        painel.SetActive(true);
    }
    public void FecharPainel(GameObject painel)
    {
        painel.SetActive(false);
    }

    public void ReiniciarJogo()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        Time.timeScale = 1f;
    }
}
