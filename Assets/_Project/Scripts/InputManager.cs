using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance { get; protected set; }
    public Camera cam;
    public Fence fencePrefab;
    public LayerMask handleMask;
    public LayerMask fenceMask;
    public LayerMask handleFenceMask;
    public Ease closeFenceEase;
    public List<Color32> fenceColors = new List<Color32>();
    private Fence fence;

    void Awake()
    {
        Instance = this;
    }

    void Update()
    {
        if (GameManager.gameState is not (GameState.Play or GameState.Build)) return;
        CheckClickDownEvent();
        CheckClickHoldEvent();
        CheckClickUpEvent();
    }

    private void CheckClickDownEvent()
    {
        if (!Input.GetMouseButtonDown(0)) return;

        RaycastHit hit;
        var ray = cam.ScreenPointToRay(Input.mousePosition);

        if (!Physics.Raycast(ray, out hit, Mathf.Infinity, handleFenceMask)) return;

        if (hit.transform.gameObject.layer == 9)
        {
            var pos = hit.transform.position;
            fence = Instantiate(fencePrefab, new Vector3(pos.x, 0.5f, pos.z), Quaternion.identity);
            StartCoroutine(fence.SetUpTransparency(fence.ReduceMaterialOpacity(fenceColors[0])));
            GameManager.gameState = GameState.Build;
        }
        else
            hit.transform.DOScaleZ(0, 0.4f).SetEase(closeFenceEase)
                .OnComplete(() =>
                {
                    Destroy(hit.transform.gameObject);
                    GameController.Instance.UpdateNaveMesh();

                    if (!GameManager.Instance.selectedLevel.hasTutorial) return;
                    if (GameManager.Instance.tutorialIndex is 1 or 3)
                        GameManager.Instance.LoadTutorial();
                });
    }

    private void CheckClickHoldEvent()
    {
        if (fence == null || !Input.GetMouseButton(0)) return;

        RaycastHit hit;
        var ray = cam.ScreenPointToRay(Input.mousePosition);
        if (!Physics.Raycast(ray, out hit, Mathf.Infinity)) return;

        StartCoroutine(hit.transform.gameObject.layer == 9 && !fence.CheckColliding() && fence.CheckSize()
            ? fence.SetUpTransparency(fence.ReduceMaterialOpacity(fenceColors[1]))
            : fence.SetUpTransparency(fence.ReduceMaterialOpacity(fenceColors[0])));

        fence.RotateFence(new Vector3(hit.point.x, 0.5f, hit.point.z));
    }

    private void CheckClickUpEvent()
    {
        if (!Input.GetMouseButtonUp(0)) return;

        RaycastHit hit;
        var ray = cam.ScreenPointToRay(Input.mousePosition);

        if (fence == null) return;

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, handleMask) && !fence.CheckColliding() && fence.CheckSize())
        {
            StartCoroutine(fence.SetUpTransparency(fenceColors[1]));
            var pos = hit.transform.position;
            fence.RotateFence(new Vector3(pos.x, 0.5f, pos.z));
            fence.gameObject.layer = 11;
            fence.coll.size += new Vector3(0, 0, -0.9f);
            GameController.Instance.UpdateNaveMesh();

            if (GameManager.Instance.selectedLevel.hasTutorial && GameManager.Instance.tutorialIndex == 2)
                GameManager.Instance.LoadTutorial();
        }
        else
        {
            fence.isDestroyed = true;
            Destroy(fence.gameObject);
        }

        GameManager.gameState = GameState.Play;
        fence = null;
    }
}