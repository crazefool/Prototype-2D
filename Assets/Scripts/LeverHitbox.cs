using UnityEngine;

public class LeverHitbox : MonoBehaviour
{
    private LeverTrigger lever;

    void Awake()
    {
        lever = GetComponent<LeverTrigger>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // More robust: detect PlayerAttack on parent too
        PlayerAttack sword = other.GetComponent<PlayerAttack>() 
                             ?? other.GetComponentInParent<PlayerAttack>();

        if (sword != null && lever != null)
        {
            lever.ActivateLever();
        }
    }
}
