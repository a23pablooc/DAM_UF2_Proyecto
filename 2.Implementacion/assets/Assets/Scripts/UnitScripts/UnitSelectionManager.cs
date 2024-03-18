using System.Collections.Generic;
using UnitScripts.PlanetScripts;
using UnitScripts.ShipScripts;
using UnityEngine;
using UnityEngine.AI;

namespace UnitScripts
{
    public class UnitSelectionManager : MonoBehaviour
    {
        public static UnitSelectionManager Instance { get; private set; }

        public List<GameObject> AllSelectableUnits { get; } = new();
        private readonly List<GameObject> _selectedUnits = new();

        [SerializeField] private LayerMask playerShipsLayer;
        [SerializeField] private LayerMask playerPlanetsLayer;
        [SerializeField] private LayerMask groundLayer;

        public bool PlanetSelected { get; private set; }

        private Camera _camera;

        private const float ShipDistance = 5f;
        private const int RingSizeFactor = 8;

        private void Awake()
        {
            if (Instance && Instance != this)
                Destroy(gameObject);
            else
                Instance = this;
        }

        private void Start()
        {
            _camera = Camera.main;
        }

        private void Update()
        {
            var ray1 = _camera.ScreenPointToRay(Input.mousePosition);
            var ray2 = _camera.ScreenPointToRay(Input.mousePosition);
            var isOverUI = UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject();

            if (isOverUI) return;

            if (Input.GetMouseButtonDown(0))
            {
                HandleLeftClick(ray1, ray2);
            }
            else if (Input.GetMouseButtonDown(1))
            {
                HandleRightClick(ray1);
            }
        }

        private void HandleLeftClick(Ray ray1, Ray ray2)
        {
            PlanetSelected = false;
            if (Physics.Raycast(ray1, out var hit1, Mathf.Infinity, playerPlanetsLayer))
            {
                PlanetSelected = true;
                var planet = hit1.collider.gameObject;
                SelectByClick(planet);
            }
            else if (Physics.Raycast(ray2, out var hit2, Mathf.Infinity, playerShipsLayer))
            {
                var unit = hit2.collider.gameObject;
                if (Input.GetKey(KeyCode.LeftControl))
                    MultiSelect(unit);
                else
                    SelectByClick(unit);
            }
            else if (!Input.GetKey(KeyCode.LeftControl))
            {
                DeselectAll();
            }
        }

        private void HandleRightClick(Ray ray)
        {
            if (!Physics.Raycast(ray, out var hit, Mathf.Infinity, groundLayer)) return;

            var position = hit.point;
            var positions = GeneratePositions(position, _selectedUnits.Count, ShipDistance, RingSizeFactor);
            for (var i = 0; i < _selectedUnits.Count; i++)
            {
                if (!_selectedUnits[i].TryGetComponent<ShipOrderController>(out var shipOrderController)) continue;

                shipOrderController.SetDestination(positions[i]);
            }
        }

        private Vector3[] GeneratePositions(Vector3 center, int totalPositions, float distance, int ringSizeFactor)
        {
            var positions = new List<Vector3>();

            if (NavMesh.SamplePosition(center, out _, 0.1f, NavMesh.AllAreas))
            {
                positions.Add(center);
            }

            for (var ringIndex = 1; positions.Count < totalPositions; ringIndex++)
            {
                var ringPositions = ringSizeFactor * ringIndex;
                var angleStep = 360f / ringPositions;

                for (var j = 0; j < ringPositions && positions.Count < totalPositions; j++)
                {
                    var angle = j * angleStep * Mathf.Deg2Rad;
                    var pos = center + new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)) * (distance * ringIndex);

                    if (NavMesh.SamplePosition(pos, out _, 0.1f, NavMesh.AllAreas))
                    {
                        positions.Add(pos);
                    }
                }
            }

            return positions.ToArray();
        }

        public void RegisterUnit(GameObject unit)
        {
            if (AllSelectableUnits.Contains(unit)) return;

            AllSelectableUnits.Add(unit);
        }

        public void UnregisterUnit(GameObject unit)
        {
            AllSelectableUnits.Remove(unit);
            _selectedUnits.Remove(unit);
        }

        private void SelectByClick(GameObject unit)
        {
            DeselectAll();

            _selectedUnits.Add(unit);
            SelectUnit(unit, true);
        }

        private void MultiSelect(GameObject unit)
        {
            var isSelected = !_selectedUnits.Contains(unit);

            if (isSelected)
                _selectedUnits.Add(unit);
            else
                _selectedUnits.Remove(unit);

            SelectUnit(unit, isSelected);
        }

        public void DragSelect(GameObject unit)
        {
            if (_selectedUnits.Contains(unit)) return;

            _selectedUnits.Add(unit);
            SelectUnit(unit, true);
        }

        public void DeselectAll()
        {
            foreach (var unit in _selectedUnits)
            {
                SelectUnit(unit, false);
            }

            _selectedUnits.Clear();
        }

        private void SelectUnit(GameObject unit, bool isSelected)
        {
            TriggerSelectionIndicator(unit, isSelected);
            EnableOrderController(unit, isSelected);
        }

        private void EnableOrderController(GameObject unit, bool enable)
        {
            if (unit.TryGetComponent<ShipOrderController>(out var shipOrderController))
            {
                shipOrderController.enabled = enable;
                PlanetSelected = false;
            }
            else if (unit.TryGetComponent<PlanetOrderController>(out var planetOrderController))
            {
                planetOrderController.enabled = enable;
                PlanetSelected = enable;
            }
        }

        private static void TriggerSelectionIndicator(GameObject unit, bool enable)
        {
            if (unit.transform.childCount == 0 || unit.TryGetComponent<PlanetOrderController>(out var _)) return;

            unit.transform.GetChild(0).gameObject.SetActive(enable);
        }
    }
}