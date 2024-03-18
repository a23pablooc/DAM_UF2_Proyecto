using System.Linq;
using UnityEngine;

namespace UnitScripts
{
    public class UnitSelectionBox : MonoBehaviour
    {
        private Camera _camera;

        [SerializeField] private RectTransform boxVisual;

        private Rect _selectionBox;

        private Vector2 _startPosition;
        private Vector2 _endPosition;

        private void Start()
        {
            _camera = Camera.main;
            _startPosition = Vector2.zero;
            _endPosition = Vector2.zero;
            DrawVisual();
        }

        private void Update()
        {
            // Si el juego está pausado o se selecciona un planeta y se muestra su panel, no se puede seleccionar
            // Se hace para que el rectangulo de selección no moleste al interactuar con la UI
            if (UnitSelectionManager.Instance.PlanetSelected || Time.timeScale == 0) return;

            // Al pulsar el botón izquierdo del ratón
            if (Input.GetMouseButtonDown(0))
            {
                _startPosition = Input.mousePosition;
                _selectionBox = new Rect();
            }

            // Al mantener pulsado el botón izquierdo del ratón
            if (Input.GetMouseButton(0))
            {
                if (boxVisual.rect.width > 0 || boxVisual.rect.height > 0)
                {
                    UnitSelectionManager.Instance.DeselectAll();
                    SelectUnits();
                }

                _endPosition = Input.mousePosition;
                DrawVisual();
                DrawSelection();
            }

            // Al soltar el botón izquierdo del ratón
            if (Input.GetMouseButtonUp(0))
            {
                SelectUnits();

                _startPosition = Vector2.zero;
                _endPosition = Vector2.zero;
                DrawVisual();
            }
        }

        private void DrawVisual()
        {
            // Calculate the starting and ending positions of the selection box.
            var boxStart = _startPosition;
            var boxEnd = _endPosition;

            // Calculate the center of the selection box.
            var boxCenter = (boxStart + boxEnd) / 2;

            // Set the position of the visual selection box based on its center.
            boxVisual.position = boxCenter;

            // Calculate the size of the selection box in both width and height.
            var boxSize = new Vector2(Mathf.Abs(boxStart.x - boxEnd.x), Mathf.Abs(boxStart.y - boxEnd.y));

            // Set the size of the visual selection box based on its calculated size.
            boxVisual.sizeDelta = boxSize;
        }

        private void DrawSelection()
        {
            if (Input.mousePosition.x < _startPosition.x)
            {
                _selectionBox.xMin = Input.mousePosition.x;
                _selectionBox.xMax = _startPosition.x;
            }
            else
            {
                _selectionBox.xMin = _startPosition.x;
                _selectionBox.xMax = Input.mousePosition.x;
            }

            if (Input.mousePosition.y < _startPosition.y)
            {
                _selectionBox.yMin = Input.mousePosition.y;
                _selectionBox.yMax = _startPosition.y;
            }
            else
            {
                _selectionBox.yMin = _startPosition.y;
                _selectionBox.yMax = Input.mousePosition.y;
            }
        }

        private void SelectUnits()
        {
            foreach (var unit in UnitSelectionManager.Instance.AllSelectableUnits.Where(unit =>
                         _selectionBox.Contains(_camera.WorldToScreenPoint(unit.transform.position))))
            {
                UnitSelectionManager.Instance.DragSelect(unit);
            }
        }
    }
}