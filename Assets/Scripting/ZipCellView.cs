using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CellView : MonoBehaviour
{
    [SerializeField] private Image background;
    [SerializeField] private TextMeshProUGUI label;
    [SerializeField] private Button button;

    public void Init(System.Action onClick)
    {
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() => onClick());
    }

    public void SetNumber(int number)
    {
        label.text = number > 0 ? number.ToString() : "";
    }

    public void SetLit(bool lit)
    {
        background.color = lit ? Color.yellow : Color.gray;
    }
}
