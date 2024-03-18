using System;
using UnityEngine;

namespace UnitScripts.PlanetScripts
{
    public class PlanetUnit : Unit
    {
        [SerializeField] private string neutralPlanetsLayer;
        [SerializeField] private string playerPlanetsLayer;
        [SerializeField] private string iaPlanetsLayer;

        [SerializeField] private int populationCapacity;
        public int PopulationCapacity => populationCapacity;
        [SerializeField] private int creditReward;

        private const int MaxSlots = 5;
        public Slot[] Slots { get; private set; }

        public void Init(PlayerType owner)
        {
            base.Init(owner, creditReward);
            GameManager.Instance.RegisterPlanet(this);

            var layer = owner switch
            {
                PlayerType.Neutral => neutralPlanetsLayer,
                PlayerType.Player => playerPlanetsLayer,
                PlayerType.IA => iaPlanetsLayer,
                _ => throw new ArgumentOutOfRangeException()
            };
            gameObject.layer = LayerMask.NameToLayer(layer);

            ResetSlots();
        }

        public void SetOwner(PlayerType owner)
        {
            Init(owner);
        }

        public void AddSlot(SlotFarm slotFarm)
        {
            for (var i = 0; i < Slots.Length; i++)
            {
                if (Slots[i] != null) continue;

                Slots[i] = gameObject.AddComponent<Slot>();
                Slots[i].Init(Owner, slotFarm);
                return;
            }

            throw new Exception("No more slots available");
        }

        private void ResetSlots()
        {
            if (Slots != null)
            {
                foreach (var slot in Slots)
                {
                    if (slot == null) continue;

                    slot.Stop();
                }
            }

            Slots = new Slot[MaxSlots];
        }
    }
}