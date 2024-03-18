using System;
using TMPro;
using UnitScripts.ShipScripts;
using UnityEngine;

namespace UnitScripts.PlanetScripts
{
    public class PlanetPanel : MonoBehaviour
    {
        [SerializeField] private GameObject emptySlotPrefab;
        [SerializeField] private GameObject creditSlotPrefab;
        [SerializeField] private GameObject metalSlotPrefab;
        [SerializeField] private GameObject energySlotPrefab;
        [SerializeField] private GameObject populationSlotPrefab;

        [SerializeField] private SlotGUI[] slots;

        [Header("FastShip Cost Texts")] [SerializeField]
        private TextMeshProUGUI[] resourceTexts = new TextMeshProUGUI[4];

        [Header("NormalShip Cost Texts")] [SerializeField]
        private TextMeshProUGUI[] resourceTexts2 = new TextMeshProUGUI[4];

        [Header("BomberShip Cost Texts")] [SerializeField]
        private TextMeshProUGUI[] resourceTexts3 = new TextMeshProUGUI[4];


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

        public void Load(PlanetUnit planetUnit)
        {
            _planetUnit = planetUnit;

            ReloadSlots();

            TextMeshProUGUI[][] texts = { resourceTexts, resourceTexts2, resourceTexts3 };
            for (var i = 0; i < texts.Length; i++)
            {
                var costs = ShipCosts.GetCosts(i switch
                {
                    0 => ShipType.FastShip,
                    1 => ShipType.NormalShip,
                    2 => ShipType.BomberShip,
                    _ => throw new ArgumentOutOfRangeException()
                });
                texts[i][0].text = costs[ResourceType.Credits].ToString();
                texts[i][1].text = costs[ResourceType.Metal].ToString();
                texts[i][2].text = costs[ResourceType.Energy].ToString();
                texts[i][3].text = costs[ResourceType.Population].ToString();
            }

            TextMeshProUGUI[][] texts2 = { resourceTexts4, resourceTexts5, resourceTexts6, resourceTexts7 };
            for (var i = 0; i < texts2.Length; i++)
            {
                var costs = SlotCosts.GetCosts(i switch
                {
                    0 => SlotType.CreditFarm,
                    1 => SlotType.MetalFarm,
                    2 => SlotType.EnergyFarm,
                    3 => SlotType.PopulationFarm,
                    _ => throw new ArgumentOutOfRangeException()
                });
                texts2[i][0].text = costs[ResourceType.Credits].ToString();
                texts2[i][1].text = costs[ResourceType.Metal].ToString();
                texts2[i][2].text = costs[ResourceType.Energy].ToString();
            }
        }

        public void BuyShip(int shipTypeIdx)
        {
            try
            {
                var shipType = ShipTypeExtension.GetShipType(shipTypeIdx);
                GameManager.Instance.SpawnShip(shipType,
                    _planetUnit.transform.position + Vector3.right * 50,
                    _planetUnit.Owner);
            }
            catch (Exception e)
            {
                Debug.Log(e);
            }
        }

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

        private void OnDisable()
        {
            foreach (var slot in slots)
            {
                slot.Unload();
            }

            shipsPanel.SetActive(false);
            slotPanel.SetActive(false);
        }
    }
}