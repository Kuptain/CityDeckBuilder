using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshSurface))]
public class NavMeshRuntimeBaker : MonoBehaviour
{
    public static NavMeshRuntimeBaker Instance { get; private set; }

    private NavMeshSurface surface;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        surface = GetComponent<NavMeshSurface>();
    }

    public void BakeNavMesh()
    {
        surface.BuildNavMesh();
    }
}
