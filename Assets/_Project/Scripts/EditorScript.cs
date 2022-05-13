using UnityEngine;

[ExecuteInEditMode]
public class EditorScript : MonoBehaviour
{
    public bool isTriggered;

    void Update()
    {
        if (isTriggered) return;
        // Do Something
        isTriggered = true;
    }
}