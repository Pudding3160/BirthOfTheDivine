using Events;
using Managers;
using TMPro;
using UnityEngine;

public class FinalAltar : MonoBehaviour
{
    [SerializeField] private int bloodRequirement;
    private Collider2D nextLevelTrigger;
    public TMP_Text success;
    public TMP_Text fail;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        if (ResourceManager.Instance.blood == 0) return;
        GameEventManager.Instance.resourceEvents.OnOfferBlood(bloodRequirement);
        if (ResourceManager.Instance.currentOfferedBlood != ResourceManager.Instance.requiredBlood)
        {
            fail.gameObject.SetActive(true);
            GameEventManager.Instance.miscEvents.OnActivateSheep();
            return;
        }
        success.gameObject.SetActive(true);
        GameEventManager.Instance.resourceEvents.OnUnlockNextLevel();
    }
}