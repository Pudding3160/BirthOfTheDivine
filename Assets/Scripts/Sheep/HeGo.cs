using UnityEngine;

public class HeGo : MonoBehaviour
{
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private float moveSpeed = 12f;
    [SerializeField] private float moveDuration = 0.5f;

    private void Start()
    {
        if (!rb) rb = GetComponent<Rigidbody2D>();

        StartCoroutine(HeDecide());
    }

    private System.Collections.IEnumerator HeDecide()
    {
        while (true)
        {
            HeHalt();

            var waitTime = Random.Range(1f, 4f);
            yield return new WaitForSeconds(waitTime);

            var dir = new Vector2(
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
        var startVel = rb.linearVelocity;
        var targetVel = targetDir * moveSpeed;

        var t = 0f;
        const float duration = 0.3f;

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
