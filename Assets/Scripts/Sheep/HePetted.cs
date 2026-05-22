using UnityEngine;

public class Interactable : MonoBehaviour
{
    [SerializeField] private KeyCode interactKey = KeyCode.E;
    private bool playerInRange = false;
    private ParticleSystem particle;
    private void Update()
    {
        if (playerInRange && Input.GetKeyDown(interactKey))
        {
            Interact();
        }
    }

    private void Interact()
    {
        GetComponent<ParticleThingo>().SpawnParticle();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerInRange = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerInRange = false;
        }
    }
}
