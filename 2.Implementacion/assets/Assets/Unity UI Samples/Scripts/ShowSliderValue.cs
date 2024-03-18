using UnityEngine;
using UnityEngine.UI;

namespace Unity_UI_Samples.Scripts
{
    [RequireComponent(typeof(Text))]
    public class ShowSliderValue : MonoBehaviour
    {
        public void UpdateLabel(float value)
        {
            var lbl = GetComponent<Text>();
            if (lbl != null)
                lbl.text = Mathf.RoundToInt(value * 100) + "%";
        }
    }
}