using System;
using System.Collections;
using UnityEngine;

namespace UnitScripts.PlanetScripts
{
    /// <summary>
    /// Clase que representa un slot de un planeta
    /// Cada slot produce recursos de un tipo para un jugador
    /// </summary>
    public class Slot : MonoBehaviour
    {
        private PlayerType _owner;
        public SlotFarm SlotFarm { get; private set; }
        private bool _isRunning;

        public void Init(PlayerType owner, SlotFarm slotFarm)
        {
            _owner = owner;
            SlotFarm = slotFarm;

            if (slotFarm.SlotType == SlotType.PopulationFarm) return;

            _isRunning = true;
            StartCoroutine(Run());
        }

        private IEnumerator Run()
        {
            while (_isRunning)
            {
                yield return new WaitForSeconds(SlotFarm.ProductionRate);

                switch (SlotFarm.SlotType)
                {
                    case SlotType.CreditFarm:
                    case SlotType.MetalFarm:
                    case SlotType.EnergyFarm:
                        GameManager.Instance.CollectResource(_owner, this);
                        break;
                    case SlotType.PopulationFarm:
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        public void Stop()
        {
            _isRunning = false;
        }
    }
}