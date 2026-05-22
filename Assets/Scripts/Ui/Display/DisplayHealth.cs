
using Components.HealthComponent;
using Events;
using UnityEngine;
using Slider = UnityEngine.UI.Slider;

namespace Ui.Display
{
    public class DisplayHealth : DisplayStat
    {
        private Slider healthSlider;

        private void Awake()
        {
            healthSlider = GetComponent<Slider>();
        }

        private void Start()
        {
            UpdateSlider();
            GameEventManager.Instance.uiEvents.UpdateHealth += GetDisplayValue;
        }

        protected override void GetDisplayValue(int amount)
        {
            base.GetDisplayValue(amount);
            healthSlider.value = amount;
            UpdateSlider();
        }

        private void UpdateSlider()
        {
            var player = GameObject.FindGameObjectWithTag("Player").GetComponent<HealthComponent>();
            healthSlider.maxValue = player.GetMaxHealth();
            healthSlider.value = player.GetCurrentHealth();
        }
    }
}