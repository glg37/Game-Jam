using System.Collections;
using UnityEngine;

public class FireTree : MonoBehaviour
{
    [Header("Fogo")]
    [SerializeField] private GameObject fireEffect;

    [Header("Som")]
    [SerializeField] private AudioSource fireSound;

    [Header("Extinção")]
    [SerializeField] private float extinguishTime = 3f;

    public bool IsBurning { get; private set; }

    private Coroutine extinguishCoroutine;

    private void Start()
    {
        if (fireEffect != null)
            fireEffect.SetActive(false);

        IsBurning = false;
    }

    public void StartFire()
    {
        if (IsBurning)
            return;

        IsBurning = true;

        if (fireEffect != null)
            fireEffect.SetActive(true);

        if (fireSound != null)
            fireSound.Play();

        Debug.Log(gameObject.name + " começou a pegar fogo!");
    }

    public void ExtinguishFire()
    {
        if (!IsBurning)
            return;

        if (extinguishCoroutine != null)
            StopCoroutine(extinguishCoroutine);

        extinguishCoroutine = StartCoroutine(ExtinguishRoutine());
    }

    private IEnumerator ExtinguishRoutine()
    {
        // Impede que o jogador tente apagar várias vezes
        IsBurning = false;

        float timer = 0f;

        // Diminui o volume do fogo gradualmente
        float initialVolume = 0f;

        if (fireSound != null)
            initialVolume = fireSound.volume;

        while (timer < extinguishTime)
        {
            timer += Time.deltaTime;

            float progress = timer / extinguishTime;

            // Diminui o volume do som
            if (fireSound != null)
                fireSound.volume = Mathf.Lerp(initialVolume, 0f, progress);

            yield return null;
        }

        // Desliga o fogo no final
        if (fireEffect != null)
            fireEffect.SetActive(false);

        if (fireSound != null)
        {
            fireSound.Stop();
            fireSound.volume = initialVolume;
        }

        Debug.Log(gameObject.name + " foi apagada!");

        extinguishCoroutine = null;
    }
}