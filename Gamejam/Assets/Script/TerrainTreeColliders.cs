using UnityEngine;
using System.Collections.Generic;

public class TerrainTreeColliders : MonoBehaviour
{
    [Header("Configuração")]
    public Terrain terrain;

    [Tooltip("Prefab da árvore usada pelo Terrain.")]
    public GameObject treePrefab;

    [Tooltip("Altura do collider.")]
    public float colliderHeight = 6f;

    [Tooltip("Raio do collider do tronco.")]
    public float colliderRadius = 0.5f;

    [Tooltip("Altura do collider em relação ao chão.")]
    public float colliderCenterY = 3f;

    private GameObject colliderParent;

    void Start()
    {
        GenerateColliders();
    }

    [ContextMenu("Gerar Colliders")]
    public void GenerateColliders()
    {
        if (terrain == null)
            terrain = GetComponent<Terrain>();

        if (terrain == null)
        {
            Debug.LogError("Nenhum Terrain encontrado!");
            return;
        }

        if (treePrefab == null)
        {
            Debug.LogError("Arraste o prefab da árvore para Tree Prefab!");
            return;
        }

        // Apaga colliders antigos
        if (colliderParent != null)
            Destroy(colliderParent);

        colliderParent = new GameObject("Terrain Tree Colliders");
        colliderParent.transform.SetParent(transform);

        TerrainData terrainData = terrain.terrainData;
        TreeInstance[] trees = terrainData.treeInstances;

        int count = 0;

        foreach (TreeInstance tree in trees)
        {
            // Só cria collider para o prefab escolhido
            if (tree.prototypeIndex >= terrainData.treePrototypes.Length)
                continue;

            GameObject prototype =
                terrainData.treePrototypes[tree.prototypeIndex].prefab;

            if (prototype != treePrefab)
                continue;

            // Posição normalizada do Terrain -> posição mundial
            Vector3 worldPosition =
                Vector3.Scale(tree.position, terrainData.size)
                + terrain.transform.position;

            GameObject colliderObject =
                new GameObject("Tree Collider");

            colliderObject.transform.SetParent(colliderParent.transform);

            colliderObject.transform.position =
                worldPosition + Vector3.up * colliderCenterY;

            CapsuleCollider capsule =
                colliderObject.AddComponent<CapsuleCollider>();

            capsule.height = colliderHeight;
            capsule.radius = colliderRadius;
            capsule.direction = 1;

            count++;
        }

        Debug.Log("Colliders de árvores criados: " + count);
    }
}