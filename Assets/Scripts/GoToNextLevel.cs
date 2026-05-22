using System;
using Events;
using UnityEngine;

public class GoToNextLevel : MonoBehaviour
{
    [SerializeField] private int bloodRequirement;
    private bool hasOfferedBlood;
    private Collider2D nextLevelTrigger;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (hasOfferedBlood) return;
        if (!other.CompareTag("Player")) return;
        GameEventManager.Instance.resourceEvents.OnOfferBlood(bloodRequirement);
        hasOfferedBlood = true;
    }
}
