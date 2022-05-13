using UnityEngine;
using UnityEngine.AI;

public class GameController : MonoBehaviour
{
    public static GameController Instance { get; protected set; }
    public Transform camPoint;
    public NavMeshSurface navMesh;

    void Awake()
    {
        Instance = this;
        UpdateNaveMesh();
    }

    public void UpdateNaveMesh()
    {
        navMesh.UpdateNavMesh(navMesh.navMeshData);
    }
}