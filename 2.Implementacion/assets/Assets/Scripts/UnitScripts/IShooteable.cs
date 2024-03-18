using System;

namespace UnitScripts
{
    /// <summary>
    /// Interfaz que define el comportamiento de un objeto que puede ser impactado por un proyectil.
    /// </summary>
    public interface IShooteable
    {
        public event Action<PlayerType, float> OnHitReceived;
        
        void OnHit(PlayerType source, float damage);
    }
}