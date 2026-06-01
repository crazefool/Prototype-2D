using UnityEngine;
using UnityEngine.SceneManagement;

public class DeleteAndLoadScene : MonoBehaviour
{
    public GameObject[] objectsToDelete;

    private int currentIndex = 0;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            // Delete all objects except the last one
            if (currentIndex < objectsToDelete.Length - 1)
            {
                Destroy(objectsToDelete[currentIndex]);
                currentIndex++;
            }
            else
            {
                SceneManager.LoadScene("Ole Scene");
            }
        }
    }
}