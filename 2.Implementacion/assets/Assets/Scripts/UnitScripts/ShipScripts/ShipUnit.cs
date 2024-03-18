using System;
using UnityEngine;

namespace UnitScripts.ShipScripts
{
    public class ShipUnit : Unit
    {
        [SerializeField] private string playerLayer;
        [SerializeField] private string iaLayer;

        [SerializeField] private ShipType shipType;

        [SerializeField] private int creditReward;

        public ShipType ShipType => shipType;

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

        private void OnDestroy()
        {
            UnitSelectionManager.Instance.UnregisterUnit(gameObject);
            GameManager.Instance.ShipDestroyed(gameObject);
        }
    }
}