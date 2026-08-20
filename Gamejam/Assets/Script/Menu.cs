using UnityEngine;

public class Menu : MonoBehaviour
{
    [Header("Menu")]
    public GameObject pausePanel;
    public MonoBehaviour playerCamera;

    [Header("Som do Menu")]
    public AudioSource menuAudio;

    bool paused;

    void Start()
    {
        pausePanel.SetActive(false);

        // Garante que o som do menu comece desligado
        if (menuAudio != null)
            menuAudio.Stop();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            paused = !paused;

            pausePanel.SetActive(paused);

            Time.timeScale = paused ? 0 : 1;

            Cursor.lockState = paused ? CursorLockMode.None : CursorLockMode.Locked;
            Cursor.visible = paused;

            playerCamera.enabled = !paused;

            if (paused)
            {
                // Pausa todos os sons do jogo
                AudioListener.pause = true;

                // Toca o som do menu
                if (menuAudio != null)
                {
                    menuAudio.ignoreListenerPause = true;
                    menuAudio.Play();
                }
            }
            else
            {
                // Para o som do menu
                if (menuAudio != null)
                    menuAudio.Stop();

                // Retoma todos os sons exatamente de onde estavam
                AudioListener.pause = false;
            }
        }
    }
}