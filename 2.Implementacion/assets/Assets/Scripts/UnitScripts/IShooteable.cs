using System;
using UnityEngine;

namespace UnitScripts
{
    public interface IShooteable
    {
        public event Action<PlayerType, float> OnHitReceived;
        
        void OnHit(PlayerType source, float damage);
    }
}