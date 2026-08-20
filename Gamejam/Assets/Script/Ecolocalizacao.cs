using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Ecolocalizacao : MonoBehaviour
{
    [Header("Ecolocalização")]
    [SerializeField] private float raioInicial = 20f;
    [SerializeField] private float duracao = 5f;
    [SerializeField] private float tempoRecarga = 1f;

    [Header("Objetos normais")]
    [SerializeField] private LayerMask objetosDetectaveis;

    [Header("Terrains")]
    [SerializeField] private Terrain[] terrains;

    [Header("Cores das árvores do Terrain")]
    [Tooltip("Cada elemento corresponde ao tipo de árvore na ordem dos Tree Prototypes.")]
    [SerializeField]
    private Color[] coresArvores =
    {
        Color.red
    };

    [Header("Som da bengala")]
    [SerializeField] private AudioSource somBengala;

    [Header("Outline")]
    [SerializeField] private float larguraOutline = 5f;

    private bool podeUsar = true;

    private List<Outline> outlinesAtivos = new List<Outline>();


    private List<GameObject> arvoresTemporarias =
        new List<GameObject>();

    private void Update()
    {
        if (Input.GetMouseButtonDown(1) && podeUsar)
        {
            StartCoroutine(AtivarEcolocalizacao());
        }
    }

    private IEnumerator AtivarEcolocalizacao()
    {
        podeUsar = false;

        if (somBengala != null)
        {
            somBengala.Play();
        }

        float tempo = 0f;

        while (tempo < duracao)
        {
            tempo += Time.deltaTime;

            float progresso = tempo / duracao;

            float raioAtual = Mathf.Lerp(
                raioInicial,
                0f,
                progresso
            );

        
            DetectarObjetos(raioAtual);

            
            DetectarArvoresTerrain(raioAtual);

            yield return null;
        }

        DesativarTodosOutlines();
        DestruirArvoresTemporarias();

        yield return new WaitForSeconds(tempoRecarga);

        podeUsar = true;
    }

    

    private void DetectarObjetos(float raio)
    {
        Collider[] objetosEncontrados =
            Physics.OverlapSphere(
                transform.position,
                raio,
                objetosDetectaveis,
                QueryTriggerInteraction.Collide
            );

        HashSet<Outline> outlinesEncontrados =
            new HashSet<Outline>();

        foreach (Collider objeto in objetosEncontrados)
        {
            Outline outline =
                objeto.GetComponent<Outline>();

            if (outline == null)
            {
                outline =
                    objeto.GetComponentInParent<Outline>();
            }

            if (outline == null)
            {
                continue;
            }

            outlinesEncontrados.Add(outline);

            if (!outlinesAtivos.Contains(outline))
            {
                AtivarOutline(outline);
                outlinesAtivos.Add(outline);
            }
        }

        for (int i = outlinesAtivos.Count - 1; i >= 0; i--)
        {
            Outline outline = outlinesAtivos[i];

            if (outline == null)
            {
                outlinesAtivos.RemoveAt(i);
                continue;
            }

            if (!outlinesEncontrados.Contains(outline))
            {
                outline.enabled = false;
                outlinesAtivos.RemoveAt(i);
            }
        }
    }

    private void AtivarOutline(Outline outline)
    {
        outline.OutlineMode = Outline.Mode.OutlineAll;
        outline.OutlineWidth = larguraOutline;
        outline.enabled = true;
    }

    

    private void DetectarArvoresTerrain(float raio)
    {
        if (terrains == null)
            return;

        foreach (Terrain terrain in terrains)
        {
            if (terrain == null)
                continue;

            TerrainData data = terrain.terrainData;

            if (data == null)
                continue;

            TreeInstance[] arvores = data.treeInstances;

            for (int i = 0; i < arvores.Length; i++)
            {
                TreeInstance arvore = arvores[i];

               
              
                Vector3 posicaoMundo =
                    terrain.transform.TransformPoint(
                        Vector3.Scale(
                            arvore.position,
                            data.size
                        )
                    );

                float distancia =
                    Vector3.Distance(
                        transform.position,
                        posicaoMundo
                    );

               
                if (distancia <= raio)
                {
                    CriarOutlineArvore(
                        terrain,
                        arvore,
                        i,
                        posicaoMundo
                    );
                }
            }
        }
    }

    private void CriarOutlineArvore(
        Terrain terrain,
        TreeInstance arvore,
        int indice,
        Vector3 posicao
    )
    {
        TerrainData data = terrain.terrainData;

        if (arvore.prototypeIndex < 0 ||
            arvore.prototypeIndex >= data.treePrototypes.Length)
        {
            return;
        }

        GameObject prefab =
            data.treePrototypes[
                arvore.prototypeIndex
            ].prefab;

        if (prefab == null)
            return;

        
        string nomeArvore =
            "ECO_TREE_" +
            terrain.GetInstanceID() +
            "_" +
            indice;

        GameObject existente =
            GameObject.Find(nomeArvore);

        if (existente != null)
            return;

        GameObject arvoreTemporaria =
            Instantiate(
                prefab,
                posicao,
                Quaternion.Euler(
                    0f,
                    arvore.rotation *
                    Mathf.Rad2Deg,
                    0f
                )
            );

        arvoreTemporaria.name = nomeArvore;

        
        arvoreTemporaria.transform.localScale =
            new Vector3(
                arvore.widthScale,
                arvore.heightScale,
                arvore.widthScale
            );

        
        Outline outline =
            arvoreTemporaria.GetComponent<Outline>();

        if (outline == null)
        {
            outline =
                arvoreTemporaria.AddComponent<Outline>();
        }

        outline.OutlineMode =
            Outline.Mode.OutlineAll;

        outline.OutlineWidth =
            larguraOutline;

        outline.OutlineColor =
            PegarCorArvore(
                arvore.prototypeIndex
            );

        outline.enabled = true;

        arvoresTemporarias.Add(
            arvoreTemporaria
        );
    }

    private Color PegarCorArvore(int prototypeIndex)
    {
        if (coresArvores == null ||
            coresArvores.Length == 0)
        {
            return Color.white;
        }

        if (prototypeIndex >= 0 &&
            prototypeIndex < coresArvores.Length)
        {
            return coresArvores[prototypeIndex];
        }

        return Color.white;
    }

  

    private void DesativarTodosOutlines()
    {
        foreach (Outline outline in outlinesAtivos)
        {
            if (outline != null)
            {
                outline.enabled = false;
            }
        }

        outlinesAtivos.Clear();
    }

    private void DestruirArvoresTemporarias()
    {
        foreach (GameObject arvore in arvoresTemporarias)
        {
            if (arvore != null)
            {
                Destroy(arvore);
            }
        }

        arvoresTemporarias.Clear();
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.white;

        Gizmos.DrawWireSphere(
            transform.position,
            raioInicial
        );
    }
}