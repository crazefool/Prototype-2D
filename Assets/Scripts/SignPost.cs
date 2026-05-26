using UnityEngine;

public class SignPost : MonoBehaviour
{
    [Header("Text to Show")]
    [SerializeField] private GameObject textObject;  // Assign the TMP text object here

    private void Awake()
    {
        if (textObject != null)
            textObject.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (textObject != null)
                textObject.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (textObject != null)
                textObject.SetActive(false);
        }
    }
}
