using UnityEngine;

public class SlimeAI : BaseEnemyAI
{
    void Update()
    {
        if (PlayerInRange())
        {
            MoveTowardsPlayer();
        }
        else
        {
            // Idle behavior (optional)
            // You can add animations or small idle movement later
        }
    }
}
