using UnityEngine;

public class HeGo : MonoBehaviour
{
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private float moveSpeed = 12f;
    [SerializeField] private float moveDuration = 0.5f;

    private void Start()
    {
        if (rb == null)
            rb = GetComponent<Rigidbody2D>();

        StartCoroutine(HeDecide());
    }

    private System.Collections.IEnumerator HeDecide()
    {
        while (true)
        {
            HeHalt();

            float waitTime = Random.Range(1f, 4f);
            yield return new WaitForSeconds(waitTime);

            Vector2 dir = new Vector2(
                Random.Range(-1f, 1f),
                Random.Range(-1f, 1f)
            ).normalized;

            // Smooth movement instead of instant snap
            StartCoroutine(HeMove(dir));

            yield return new WaitForSeconds(moveDuration);

            HeHalt();
        }
    }

    private System.Collections.IEnumerator HeMove(Vector2 targetDir)
    {
        Vector2 startVel = rb.linearVelocity;
        Vector2 targetVel = targetDir * moveSpeed;

        float t = 0f;
        float duration = 0.3f;

        while (t < duration)
        {
            t += Time.deltaTime;
            rb.linearVelocity = Vector2.Lerp(startVel, targetVel, t / duration);
            yield return null;
        }

        rb.linearVelocity = targetVel;
    }

    private void HeHalt()
    {
        rb.linearVelocity = Vector2.zero;
    }
}
