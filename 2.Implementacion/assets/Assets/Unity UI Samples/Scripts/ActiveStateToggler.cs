using UnityEngine;

namespace Unity_UI_Samples.Scripts
{
    public class ActiveStateToggler : MonoBehaviour
    {
        public void ToggleActive()
        {
            gameObject.SetActive(!gameObject.activeSelf);
        }
    }
}