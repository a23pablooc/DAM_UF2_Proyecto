using UnityEngine;

namespace UnitScripts.PlanetScripts
{
    public class SlotGUI : MonoBehaviour
    {
        [SerializeField] private GameObject shipsPanel;
        [SerializeField] private GameObject slotPanel;
        
        private GameObject _slotGameObject;
        private Slot _slot;

        public void Load(GameObject prefab, Slot slot)
        {
            _slotGameObject = Instantiate(prefab, Vector3.zero, Quaternion.identity);
            _slotGameObject.transform.SetParent(transform);
            _slotGameObject.transform.localPosition = Vector3.zero;
            _slotGameObject.transform.localScale = Vector3.one;

            _slot = slot;
        }

        public void SlotClicked()
        {
            if (_slot && _slot.SlotFarm.SlotType != SlotType.PopulationFarm) return;
            
            if (!_slot)
            {
                slotPanel.SetActive(true);
            }
            else if (_slot.SlotFarm.SlotType == SlotType.PopulationFarm)
            {
                shipsPanel.SetActive(true);
            }
        }

        public void Unload()
        {
            Destroy(_slotGameObject);
        }
    }
}