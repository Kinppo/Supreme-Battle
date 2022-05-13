using UnityEngine;

public class BloodSplatter : MonoBehaviour
{
    void Start()
    {
        Destroy(gameObject, 5f);
    }
}