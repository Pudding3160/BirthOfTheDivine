using System;
using Unity.VisualScripting;

namespace Events
{
    public class MiscEvents
    {
        public event Action AddHealthComponentToSheep;

        public virtual void OnAddHealthComponentToSheep()
        {
            AddHealthComponentToSheep?.Invoke();
        }
        
        public event Action ActivateSheep;

        public virtual void OnActivateSheep()
        {
            ActivateSheep?.Invoke();
        }
    }
}