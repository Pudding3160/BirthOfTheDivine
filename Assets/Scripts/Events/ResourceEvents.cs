using System;
using UnityEngine;

namespace Events
{
    public class ResourceEvents
    {
        #region Reward

        public event Action<int> RewardBlood;
        public void OnRewardBlood(int amount)
        {
            RewardBlood?.Invoke(amount);
        }

        public event Action<int> RewardBones;
        public void OnRewardBones(int amount)
        {
            RewardBones?.Invoke(amount);
        }

        #endregion

        #region Lose

        public event Action<int> TakeBlood;
        public void OnTakeBlood(int amount)
        {
            TakeBlood?.Invoke(amount);
        }
        
        public event Action<int> TakeBones;
        public void OnTakeBones(int amount)
        {
            TakeBones?.Invoke(amount);
        }
        
        public event Action<int> OfferBlood;
        public virtual void OnOfferBlood(int obj)
        {
            OfferBlood?.Invoke(obj);
        }

        #endregion

        #region RetrieveFromOtherClasses

        public event Func<int> GetBlood;
        public int OnGetBlood()
        {
            return GetBlood?.Invoke()?? 0;
        }
        
        public event Func<int> GetBones;
        public int OnGetBones()
        {
            return GetBones?.Invoke() ?? 0;
        }

        #endregion

        public event Action UnlockNextLevel;
        public virtual void OnUnlockNextLevel()
        {
            UnlockNextLevel?.Invoke();
        }
    }
}
