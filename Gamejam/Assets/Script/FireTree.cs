using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireTree : MonoBehaviour
{
    [Header("Fogo")]
    [SerializeField] private GameObject fireEffect;
    [Header("Som")]
    [SerializeField] private AudioSource fireSound;
    [Header("Extinção")]
    [SerializeField] private float extinguishTime = 3f;
    [Header("Trigger")]
    [SerializeField] private Collider fireTriggerCollider; // arraste o empty filho aqui no Inspector

    public bool IsBurning { get; private set; }
    private Coroutine extinguishCoroutine;
    private Collider myCollider;

    public static readonly List<Collider> BurningColliders = new List<Collider>();

    private void Awake()
    {
        // Usa o collider arrastado no Inspector; se não arrastou, tenta achar automaticamente nos filhos
        myCollider = fireTriggerCollider != null ? fireTriggerCollider : GetComponentInChildren<Collider>();

        if (myCollider == null)
            Debug.LogWarning(gameObject.name + ": nenhum Collider encontrado para o FireTree!");
    }

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

        if (myCollider != null && !BurningColliders.Contains(myCollider))
            BurningColliders.Add(myCollider);

        Debug.Log(gameObject.name + " começou a pegar fogo! Collider registrado: " + (myCollider != null));
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
        IsBurning = false;

        if (myCollider != null)
            BurningColliders.Remove(myCollider);

        float timer = 0f;
        float initialVolume = 0f;
        if (fireSound != null)
            initialVolume = fireSound.volume;

        while (timer < extinguishTime)
        {
            timer += Time.deltaTime;
            float progress = timer / extinguishTime;
            if (fireSound != null)
                fireSound.volume = Mathf.Lerp(initialVolume, 0f, progress);
            yield return null;
        }

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

    private void OnDestroy()
    {
        if (myCollider != null)
            BurningColliders.Remove(myCollider);
    }
}