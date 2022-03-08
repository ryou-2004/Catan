using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Toggle))]
public class ToggleValueChangeColor : MonoBehaviour
{
    [SerializeField]
    private Color onColor;

    [SerializeField]
    private Color offColor;

    private Toggle toggle;

    void Start()
    {
        toggle = GetComponent<Toggle>();
        toggle.onValueChanged.AddListener(OnToggleValueChanged);
        OnToggleValueChanged(toggle.isOn);
    }


    private void OnToggleValueChanged(bool isOn)
    {
        if (isOn)
        {
            //colorsは構造体のため、値型であるnormalColorを直接変えることはできないので一旦別変数に確認
            var newColors = toggle.colors;
            newColors.normalColor = new Color(onColor.r, onColor.g, onColor.b, onColor.a);
            newColors.highlightedColor = new Color(onColor.r, onColor.g, onColor.b, onColor.a);
            newColors.selectedColor = new Color(onColor.r, onColor.g, onColor.b, onColor.a);
            toggle.colors = newColors;

        }
        else
        {
            var newColors = toggle.colors;
            newColors.normalColor = new Color(offColor.r, offColor.g, offColor.b, offColor.a);
            newColors.highlightedColor = new Color(offColor.r, offColor.g, offColor.b, offColor.a);
            newColors.selectedColor = new Color(offColor.r, offColor.g, offColor.b, offColor.a);
            toggle.colors = newColors;
        }
    }
}