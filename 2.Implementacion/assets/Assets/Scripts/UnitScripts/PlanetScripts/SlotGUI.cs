using UnityEngine;

namespace UnitScripts.PlanetScripts
{
    /// <summary>
    /// Clase para la interfaz gráfica de un slot
    /// Gestiona la interacción con el slot
    /// </summary>
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

        /// <summary>
        /// Si el slot es de población, muestra el panel de naves
        /// Si el slot no tiene nada, muestra el panel de compra de slots
        /// </summary>
        public void SlotClicked()
        {
            if (_slot && _slot.SlotFarm.SlotType != SlotType.PopulationFarm) return;
            
            if (!_slot)
            {
                slotPanel.SetActive(true);
                shipsPanel.SetActive(false);
            }
            else if (_slot.SlotFarm.SlotType == SlotType.PopulationFarm)
            {
                shipsPanel.SetActive(true);
                slotPanel.SetActive(false);
            }
        }

        public void Unload()
        {
            Destroy(_slotGameObject);
        }
    }
}