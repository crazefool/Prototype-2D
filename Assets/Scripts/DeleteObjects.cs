using UnityEngine;

public class DeleteObjects : MonoBehaviour
{
    public GameObject[] objectsToDelete;

    private int currentIndex = 0;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (currentIndex < objectsToDelete.Length)
            {
                Destroy(objectsToDelete[currentIndex]);
                currentIndex++;
            }
        }
    }
}
