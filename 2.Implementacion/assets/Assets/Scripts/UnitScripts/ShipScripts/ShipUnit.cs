using System;
using UnityEngine;

namespace UnitScripts.ShipScripts
{
    /// <summary>
    /// Clase que representa una unidad de nave
    /// </summary>
    public class ShipUnit : Unit
    {
        [SerializeField] private string playerLayer;
        [SerializeField] private string iaLayer;

        [SerializeField] private ShipType shipType;

        [SerializeField] private int creditReward;

        public ShipType ShipType => shipType;

        /// <summary>
        /// Inicializa la unidad de nave, la registra en el GameManager y en el UnitSelectionManager si es del jugador y establece la capa de la nave
        /// </summary>
        /// <param name="owner">Jugador</param>
        /// <exception cref="Exception">Excepción si el jugador no es válido</exception>
        public void Init(PlayerType owner)
        {
            base.Init(owner, creditReward);

            switch (owner)
            {
                case PlayerType.Player:
                    var go = gameObject;
                    UnitSelectionManager.Instance.RegisterUnit(go);
                    go.layer = LayerMask.NameToLayer(playerLayer);
                    break;
                case PlayerType.IA:
                    gameObject.layer = LayerMask.NameToLayer(iaLayer);
                    break;
                case PlayerType.Neutral:
                default:
                    throw new Exception($"Invalid ship owner: {owner}");
            }

            GameManager.Instance.RegisterShip(this);
        }

        /// <summary>
        /// Al destruir la nave, la elimina del UnitSelectionManager y la elimina del GameManager
        /// </summary>
        private void OnDestroy()
        {
            UnitSelectionManager.Instance.UnregisterUnit(gameObject);
            GameManager.Instance.ShipDestroyed(gameObject);
        }
    }
}