using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WordCell : MonoBehaviour
{
    [SerializeField] private TMP_Text wordText;

    public WordData Data { get; private set; }
    public bool IsSelected { get; private set; }

    private ConnectionsGameManager manager;
    private Image background;

    void Awake()
    {
        background = GetComponent<Image>(); // le fond du bouton
    }

    public void Init(WordData data, ConnectionsGameManager manager)
    {
        Data = data;
        this.manager = manager;

        wordText.text = data.word;
        SetSelected(false);
    }

    // On l'appellera depuis le Button OnClick (étape 4.2)
    public void ToggleSelect()
    {
        manager.TryToggleCell(this);
    }

    public void SetSelected(bool selected)
    {
        IsSelected = selected;
        if (background != null)
            background.color = selected ? new Color(0.5f, 0.5f, 0.5f) : Color.white;
    }
}
