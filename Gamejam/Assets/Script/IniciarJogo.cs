using UnityEngine;
using UnityEngine.SceneManagement;

public class IniciarJogo : MonoBehaviour
{
    public void MudarCena(string SampleScene)
    {
        SceneManager.LoadScene(SampleScene);
    }
}