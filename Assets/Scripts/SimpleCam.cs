using UnityEngine;

public class CameraFollowSimple : MonoBehaviour
{
    public Transform target;
    public Vector3 offset = new Vector3(0, 0, -10f);

    void LateUpdate()
    {
        if (target == null)
            return;

        transform.position = target.position + offset;
    }
}
