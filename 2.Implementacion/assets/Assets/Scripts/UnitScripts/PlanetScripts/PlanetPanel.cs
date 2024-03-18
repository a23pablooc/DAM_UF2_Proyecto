using System;
using TMPro;
using UnitScripts.ShipScripts;
using UnityEngine;

namespace UnitScripts.PlanetScripts
{
    /// <summary>
    /// Controlador del panel de slots de los planetas
    /// Se encarga de mostrar los slots de un planeta y de comprar naves y slots
    /// </summary>
    public class PlanetPanel : MonoBehaviour
    {
        [SerializeField] private GameObject emptySlotPrefab;
        [SerializeField] private GameObject creditSlotPrefab;
        [SerializeField] private GameObject metalSlotPrefab;
        [SerializeField] private GameObject energySlotPrefab;
        [SerializeField] private GameObject populationSlotPrefab;

        [SerializeField] private SlotGUI[] slots;

        /// <summary>
        /// Campos de texto de los costos de los distintos tipos de naves
        /// </summary>
        [Header("FastShip Cost Texts")] [SerializeField]
        private TextMeshProUGUI[] resourceTexts = new TextMeshProUGUI[4];

        [Header("NormalShip Cost Texts")] [SerializeField]
        private TextMeshProUGUI[] resourceTexts2 = new TextMeshProUGUI[4];

        [Header("BomberShip Cost Texts")] [SerializeField]
        private TextMeshProUGUI[] resourceTexts3 = new TextMeshProUGUI[4];

        /// <summary>
        /// Campos de texto de los costos de los distintos tipos de granjas/fábricas/
        /// </summary>
        [Header("CreditsFarm Cost Texts")] [SerializeField]
        private TextMeshProUGUI[] resourceTexts4 = new TextMeshProUGUI[3];

        [Header("MetalFarm Cost Texts")] [SerializeField]
        private TextMeshProUGUI[] resourceTexts5 = new TextMeshProUGUI[3];

        [Header("EnergyFarm Cost Texts")] [SerializeField]
        private TextMeshProUGUI[] resourceTexts6 = new TextMeshProUGUI[3];

        [Header("PopulationFarm Cost Texts")] [SerializeField]
        private TextMeshProUGUI[] resourceTexts7 = new TextMeshProUGUI[3];

        [SerializeField] private GameObject shipsPanel;
        [SerializeField] private GameObject slotPanel;

        private PlanetUnit _planetUnit;

        /// <summary>
        /// Carga el panel con los slots del planeta y los costos de las naves y granjas
        /// </summary>
        /// <param name="planetUnit">Unidad de planeta de la que se muestran los slots</param>
        public void Load(PlanetUnit planetUnit)
        {
            _planetUnit = planetUnit;

            ReloadSlots();

            TextMeshProUGUI[][] texts = { resourceTexts, resourceTexts2, resourceTexts3 };
            for (var i = 0; i < texts.Length; i++)
            {
                var costs = ShipCosts.GetCosts(ShipTypeExtension.GetShipType(i));
                texts[i][0].text = costs[ResourceType.Credits].ToString();
                texts[i][1].text = costs[ResourceType.Metal].ToString();
                texts[i][2].text = costs[ResourceType.Energy].ToString();
                texts[i][3].text = costs[ResourceType.Population].ToString();
            }

            TextMeshProUGUI[][] texts2 = { resourceTexts4, resourceTexts5, resourceTexts6, resourceTexts7 };
            for (var i = 0; i < texts2.Length; i++)
            {
                var costs = SlotCosts.GetCosts(SlotTypeExtension.GetSlotType(i));
                texts2[i][0].text = costs[ResourceType.Credits].ToString();
                texts2[i][1].text = costs[ResourceType.Metal].ToString();
                texts2[i][2].text = costs[ResourceType.Energy].ToString();
            }
        }

        /// <summary>
        /// Compra una nave, si no se puede comprar y se lanza una excepción se captura y se muestra un mensaje de error en consola
        /// </summary>
        /// <param name="shipTypeIdx">Tipo de nave a comprar</param>
        /// <see cref="SlotType"/>
        /// <seealso cref="SlotTypeExtension"/>
        public void BuyShip(int shipTypeIdx)
        {
            try
            {
                var shipType = ShipTypeExtension.GetShipType(shipTypeIdx);
                GameManager.Instance.SpawnShip(shipType, _planetUnit.transform.position + Vector3.right * 50,
                    _planetUnit.Owner);
            }
            catch (Exception e)
            {
                Debug.Log(e);
            }
        }

        /// <summary>
        /// Compra un slot, si no se puede comprar y se lanza una excepción se captura y se muestra un mensaje de error en consola
        /// </summary>
        /// <param name="slotTypeIdx">Tipo de slot a comprar</param>
        /// <see cref="SlotType"/>
        /// <seealso cref="SlotTypeExtension"/>
        public void BuySlot(int slotTypeIdx)
        {
            try
            {
                var slotType = SlotTypeExtension.GetSlotType(slotTypeIdx);
                GameManager.Instance.BuySlot(_planetUnit, slotType);
                slotPanel.SetActive(false);
                ReloadSlots();
            }
            catch (Exception e)
            {
                Debug.Log(e);
            }
        }

        /// <summary>
        /// Recarga los slots del planeta para mostrar el que se acaba de comprar
        /// </summary>
        private void ReloadSlots()
        {
            for (var i = 0; i < _planetUnit.Slots.Length; i++)
            {
                GameObject prefab;

                if (_planetUnit.Slots[i] == null)
                {
                    prefab = emptySlotPrefab;
                }
                else
                {
                    prefab = _planetUnit.Slots[i].SlotFarm.SlotType switch
                    {
                        SlotType.CreditFarm => creditSlotPrefab,
                        SlotType.MetalFarm => metalSlotPrefab,
                        SlotType.EnergyFarm => energySlotPrefab,
                        SlotType.PopulationFarm => populationSlotPrefab,
                        _ => emptySlotPrefab
                    };
                }

                slots[i].Load(prefab, _planetUnit.Slots[i]);
            }
        }

        /// <summary>
        /// Al cerrar el panel se descargan los slots
        /// </summary>
        private void OnDisable()
        {
            foreach (var slotGUI in slots)
            {
                slotGUI.Unload();
            }

            shipsPanel.SetActive(false);
            slotPanel.SetActive(false);
        }
    }
}