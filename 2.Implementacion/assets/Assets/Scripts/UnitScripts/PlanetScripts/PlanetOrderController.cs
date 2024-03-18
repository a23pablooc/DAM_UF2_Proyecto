using System;
using UnityEngine;

namespace UnitScripts.PlanetScripts
{
    public class PlanetOrderController : MonoBehaviour
    {
        [SerializeField] private PlanetUnit planetUnit;
        private GameObject _planetPanel;

        private void Awake()
        {
            _planetPanel = GameObject.FindGameObjectWithTag("UICanvas").transform.Find("PlanetPanel").gameObject;
        }

        private void OnEnable()
        {
            ShowPanel();
        }

        private void OnDisable()
        {
            HidePanel();
        }

        private void ShowPanel()
        {
            _planetPanel.SetActive(true);
            _planetPanel.GetComponent<PlanetPanel>().Load(planetUnit);
        }

        private void HidePanel()
        {
            _planetPanel.SetActive(false);
        }
    }
}