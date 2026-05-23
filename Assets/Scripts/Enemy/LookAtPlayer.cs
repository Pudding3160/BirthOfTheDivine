using UnityEngine;

public class LookAtPlayer : MonoBehaviour
{
    public Transform player;

    private bool lockRotation;
    private Vector2 lockedDirection;

    void Start()
    {
        if (player == null)
        {
            GameObject playerObj =
                GameObject.FindGameObjectWithTag("Player");

            if (playerObj != null)
            {
                player = playerObj.transform;
            }
        }
    }

    void Update()
    {
        if (player == null) return;

        Vector2 direction;

        if (lockRotation)
        {
            direction = lockedDirection;
        }
        else
        {
            direction = player.position - transform.position;
        }

        float angle =
            Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        transform.rotation =
            Quaternion.Euler(0f, 0f, angle + 90f);
    }

    public void LockLookDirection(Vector2 dir)
    {
        lockRotation = true;
        lockedDirection = dir.normalized;
    }

    public void UnlockLookDirection()
    {
        lockRotation = false;
    }
}