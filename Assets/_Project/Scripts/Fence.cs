using System;
using System.Collections;
using UnityEngine;

public class Fence : MonoBehaviour
{
    public new Renderer renderer;
    public BoxCollider coll;
    public float maxSize = 1.56f;
    [HideInInspector] public bool isInitiated;
    private float sizeZ = 4.53f;
    private float scaleZ;
    private Vector3 pointB;
    private RaycastHit hit;

    private void Start()
    {
        //if (coll != null) sizeZ = coll.size.z;
    }

    private void Update()
    {
        if (isInitiated)
        {
            isInitiated = false;
            Invoke(nameof(UpdateMaterial), 0.1f);
        }
    }
    
    private void UpdateMaterial()
    {
        renderer.material.color = InputManager.Instance.fenceColors[1];

    }
    
    public void RotateFence(Vector3 point)
    {
        var pointA = transform.position;
        pointB = point;
        pointB.y = pointA.y;

        var separation = pointB - pointA;
        var rot = Quaternion.LookRotation(separation);
        transform.rotation = rot;

        scaleZ = Vector3.Distance(pointB, pointA);
        if (scaleZ > 0 && sizeZ > 0 && CheckSize())
            transform.localScale = new Vector3(1, 1, scaleZ / sizeZ);
    }

    public bool CheckSize()
    {
        return (scaleZ / sizeZ) < maxSize;
    }

    public Color32 ReduceMaterialOpacity(Color color)
    {
        color.a = 0.4f;
        return color;
    }

    public IEnumerator SetUpTransparency(Color32 color)
    {
        var currentTime = 0f;
        var cycleTime = 0.1f;

        while (currentTime < cycleTime)
        {
            currentTime += Time.deltaTime;
            var t = currentTime / cycleTime;

            if (renderer == null) break;
            var currentColor = Color.Lerp(renderer.material.color, color, t);
            renderer.material.color = currentColor;
            yield return null;
        }
    }

    public bool CheckColliding()
    {
        if (!Physics.Linecast(transform.position, pointB, out hit, InputManager.Instance.fenceMask) &&
            !Physics.Linecast(coll.bounds.center + new Vector3(-1f, 0, -1f),
                coll.bounds.center + new Vector3(1f, 0, 1f), out hit, InputManager.Instance.fenceMask)) return false;

        return Vector3.Distance(hit.transform.position, transform.position) > 0;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        Gizmos.DrawLine(coll.bounds.center + new Vector3(-1f, 0, -1f),
            coll.bounds.center + new Vector3(1f, 0, 1f));
        //Gizmos.color = Color.blue;
        //Gizmos.DrawLine(transform.position, pointB);
    }
}