using UnityEngine;

public class CamSetup : MonoBehaviour
{
    [SerializeField] private float cameraSize = 10f;

    void Awake()
    {
        Camera cam = GetComponent<Camera>();
        cam.orthographicSize = cameraSize;
    }
}
