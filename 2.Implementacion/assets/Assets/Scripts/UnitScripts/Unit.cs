using System;
using UnityEngine;

namespace UnitScripts
{
    /// <summary>
    /// Clase genérica para todas las unidades del juego.
    /// </summary>
    public abstract class Unit : MonoBehaviour, IShooteable
    {
        public int CreditReward { get; private set; }
        public PlayerType Owner { get; private set; } = PlayerType.Neutral;

        public event Action<PlayerType, float> OnHitReceived;

        protected void Init(PlayerType owner, int reward)
        {
            Owner = owner;
            CreditReward = reward;
        }

        public void OnHit(PlayerType source, float damage)
        {
            OnHitReceived?.Invoke(source, damage);
        }
    }
}