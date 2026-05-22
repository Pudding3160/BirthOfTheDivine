using System;
using Unity.VisualScripting;

namespace Events
{
    public class UpgradeEvents
    {
        public event Action UnlockUpgrades;
        protected virtual void OnUnlockUpgrades()
        {
            UnlockUpgrades?.Invoke();
        }
    }
}