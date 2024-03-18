using System;
using UnityEngine;

namespace UnitScripts.PlanetScripts
{
    /// <summary>
    /// Clase que representa una unidad de planeta
    /// </summary>
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

        /// <summary>
        /// Inicializa la unidad de planeta, registra el planeta en el GameManager, establece la capa del planeta y resetea los slots
        /// </summary>
        /// <param name="owner">Jugador</param>
        /// <exception cref="ArgumentOutOfRangeException">Excepción si el jugador no es válido</exception>
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

        /// <summary>
        /// Cambia el dueño del planeta
        /// </summary>
        /// <param name="owner"></param>
        public void SetOwner(PlayerType owner)
        {
            Init(owner);
        }

        /// <summary>
        /// Añade un slot al planeta
        /// </summary>
        /// <param name="slotFarm">Tipo de slot</param>
        /// <exception cref="Exception">Se lanza si no quedan espacios para slots disponibles</exception>
        public void AddSlot(SlotFarm slotFarm)
        {
            for (var i = 0; i < Slots.Length; i++)
            {
                if (!Slots[i]) continue;

                Slots[i] = gameObject.AddComponent<Slot>();
                Slots[i].Init(Owner, slotFarm);
                return;
            }

            throw new Exception("No more slots available");
        }

        /// <summary>
        /// Al destruirse el planeta y cambiar de jugador los slots dejan de producir y se resetean
        /// </summary>
        private void ResetSlots()
        {
            if (Slots != null)
            {
                foreach (var slot in Slots)
                {
                    if (!slot) continue;

                    slot.Stop();
                    Destroy(slot);
                }
            }

            Slots = new Slot[MaxSlots];
        }
    }
}