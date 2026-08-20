using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireTree : MonoBehaviour
{
    [Header("Fogo")]
    [SerializeField] private GameObject fireEffect;

    [Header("Fumaça")]
    [SerializeField] private GameObject smokeEffect;

    [Header("Som")]
    [SerializeField] private AudioSource fireSound;

    [Header("Extinção")]
    [SerializeField] private float tempoParaApagar = 6f;
    [SerializeField] private float tempoParaSumir = 1.5f;

    [Header("Água")]
    [SerializeField] private float tempoSemAguaParaParar = 0.15f;

    [Header("Trigger")]
    [SerializeField] private Collider fireTriggerCollider;

    public bool IsBurning { get; private set; }

    private Coroutine extinguishCoroutine;
    private Collider myCollider;

    private float progressoExtincao = 0f;
    private float ultimoAcertoAgua = -999f;

    private ParticleSystem[] particulasFogo;
    private float[] emissaoOriginal;

    public static readonly List<Collider> BurningColliders = new List<Collider>();

    private void Awake()
    {
        myCollider = fireTriggerCollider != null
            ? fireTriggerCollider
            : GetComponentInChildren<Collider>();

        if (myCollider == null)
        {
            Debug.LogWarning(
                gameObject.name +
                ": nenhum Collider encontrado para o FireTree!"
            );
        }

       
        if (fireEffect != null)
        {
            particulasFogo =
                fireEffect.GetComponentsInChildren<ParticleSystem>();

            emissaoOriginal =
                new float[particulasFogo.Length];

            for (int i = 0; i < particulasFogo.Length; i++)
            {
                var emission = particulasFogo[i].emission;

                emissaoOriginal[i] =
                    emission.rateOverTimeMultiplier;
            }
        }
    }

    private void Start()
    {
        if (fireEffect != null)
            fireEffect.SetActive(false);

        if (smokeEffect != null)
            smokeEffect.SetActive(false);

        IsBurning = false;
        progressoExtincao = 0f;
    }

    private void Update()
    {
        if (!IsBurning)
            return;

        
        bool recebendoAgua =
            Time.time - ultimoAcertoAgua <= tempoSemAguaParaParar;

        if (recebendoAgua)
        {
            
            progressoExtincao +=
                Time.deltaTime / tempoParaApagar;

            progressoExtincao =
                Mathf.Clamp01(progressoExtincao);

            AtualizarFogo();

            
            if (progressoExtincao >= 1f)
            {
                ExtinguishFire();
            }
        }
    }

    public void StartFire()
    {
        if (IsBurning)
            return;

        if (extinguishCoroutine != null)
        {
            StopCoroutine(extinguishCoroutine);
            extinguishCoroutine = null;
        }

        IsBurning = true;

        progressoExtincao = 0f;
        ultimoAcertoAgua = -999f;

        if (fireEffect != null)
            fireEffect.SetActive(true);

        if (smokeEffect != null)
            smokeEffect.SetActive(true);

        
        RestaurarFogo();

        if (fireSound != null)
        {
            fireSound.volume = 1f;
            fireSound.Play();
        }

        if (myCollider != null &&
            !BurningColliders.Contains(myCollider))
        {
            BurningColliders.Add(myCollider);
        }

        Debug.Log(
            gameObject.name +
            " começou a pegar fogo!"
        );
    }

  
    public void AguaAtingindo()
    {
        if (!IsBurning)
            return;

        
        ultimoAcertoAgua = Time.time;
    }

    private void AtualizarFogo()
    {
        float progresso =
            progressoExtincao;

       
        if (particulasFogo != null)
        {
            for (int i = 0; i < particulasFogo.Length; i++)
            {
                var emission =
                    particulasFogo[i].emission;

                emission.rateOverTimeMultiplier =
                    Mathf.Lerp(
                        emissaoOriginal[i],
                        0f,
                        progresso
                    );
            }
        }

      
        if (fireSound != null)
        {
            fireSound.volume =
                Mathf.Lerp(
                    1f,
                    0f,
                    progresso
                );
        }
    }

    public void ExtinguishFire()
    {
        if (!IsBurning)
            return;

        if (extinguishCoroutine != null)
            StopCoroutine(extinguishCoroutine);

        extinguishCoroutine =
            StartCoroutine(ExtinguishRoutine());
    }

    private IEnumerator ExtinguishRoutine()
    {
        float timer = 0f;

        float volumeInicial = fireSound != null
            ? fireSound.volume
            : 0f;

        while (timer < tempoParaSumir)
        {
            timer += Time.deltaTime;

            float progresso =
                timer / tempoParaSumir;

            if (fireSound != null)
            {
                fireSound.volume = Mathf.Lerp(
                    volumeInicial,
                    0f,
                    progresso
                );
            }

            yield return null;
        }

        
        IsBurning = false;

        if (myCollider != null)
            BurningColliders.Remove(myCollider);

        if (fireEffect != null)
            fireEffect.SetActive(false);

        if (smokeEffect != null)
            smokeEffect.SetActive(false);

        if (fireSound != null)
        {
            fireSound.Stop();
            fireSound.volume = 1f;
        }

        progressoExtincao = 0f;

        Debug.Log(gameObject.name + " foi apagada!");

        extinguishCoroutine = null;
    }

    private void RestaurarFogo()
    {
        if (particulasFogo == null)
            return;

        for (int i = 0; i < particulasFogo.Length; i++)
        {
            var emission =
                particulasFogo[i].emission;

            emission.rateOverTimeMultiplier =
                emissaoOriginal[i];
        }
    }

    private void OnDestroy()
    {
        if (myCollider != null)
            BurningColliders.Remove(myCollider);
    }
}