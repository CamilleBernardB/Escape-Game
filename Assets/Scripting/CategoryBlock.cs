using TMPro;
using UnityEngine;

public class CategoryBlock : MonoBehaviour
{
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private TMP_Text wordsText;

    public void Init(string categoryName, string wordsLine)
    {
        titleText.text = categoryName.ToUpper();
        wordsText.text = wordsLine;
    }
}
