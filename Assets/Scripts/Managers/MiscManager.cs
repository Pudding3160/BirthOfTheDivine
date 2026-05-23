using System;
using System.Collections.Generic;
using Components;
using Events;
using UnityEngine;

namespace Managers
{
    public class MiscManager : MonoBehaviour
    {
        [SerializeField]
        private List<GameObject> Sheep;

        private void Start()
        {
            Sheep = new List<GameObject>(GameObject.FindGameObjectsWithTag("Sheep"));
            // GameEventManager.Instance.miscEvents.ActivateSheep += AddHealthComponentToSheep;
        }

        private void AddHealthComponentToSheep()
        {
            foreach (var t in Sheep)
            {
                var missingBlood = ResourceManager.Instance.requiredBlood -
                                   ResourceManager.Instance.currentOfferedBlood;
                GameEventManager.Instance.miscEvents.OnActivateSheep();
                t.TryGetComponent(out SheepHealthComponent sheepController);
                if (!sheepController) return;
                sheepController.blood = (int)Mathf.Ceil(missingBlood / 5) + 1;
            }
        }
    }
}