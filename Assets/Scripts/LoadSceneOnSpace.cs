using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadSceneOnSpace : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            string currentScene = SceneManager.GetActiveScene().name;

            if (currentScene == "Story Scene")
            {
                SceneManager.LoadScene("Ole Scene");
            }
            else if (currentScene == "End Scene")
            {
                SceneManager.LoadScene("MainMenu");
            }
        }
    }
}