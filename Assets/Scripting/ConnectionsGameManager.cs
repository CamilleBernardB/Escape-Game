using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class ConnectionsGameManager : MonoBehaviour
{
    [SerializeField] private Transform gridContainer;
    [SerializeField] private WordCell cellPrefab;
    [SerializeField] private Button submitButton;
    [SerializeField] private Transform solvedCategoriesContainer;
    [SerializeField] private CategoryBlock categoryBlockPrefab;
    private List<WordData> allWords;
    private readonly List<WordCell> selectedCells = new();

    void UpdateSubmitButton()
    {
        submitButton.interactable = (selectedCells.Count == 4);
    }

    public void OnSubmit()
    {
        if (selectedCells.Count != 4)
            return;

        string cat = selectedCells[0].Data.categoryId;

        bool ok = true;
        for (int i = 1; i < selectedCells.Count; i++)
        {
            if (selectedCells[i].Data.categoryId != cat)
            {
                ok = false;
                break;
            }
        }

        if (ok)
        {
            Debug.Log("✅ Correct group: " + cat);

            // créer la ligne de mots
            string wordsLine = "";
            for (int i = 0; i < selectedCells.Count; i++)
            {
                wordsLine += selectedCells[i].Data.word;
                if (i < selectedCells.Count - 1) wordsLine += ", ";
            }

            // spawn le bloc catégorie
            CategoryBlock block = Instantiate(categoryBlockPrefab, solvedCategoriesContainer);
            block.Init(cat, wordsLine);


            // supprimer les 4 cells de la grille
            foreach (var cell in selectedCells)
                Destroy(cell.gameObject);

            selectedCells.Clear();
        }
        else
        {
            Debug.Log("❌ Wrong group");
        }

    }

    void Start()
    {
        CreateWordData();
        SpawnGrid();
        submitButton.onClick.AddListener(OnSubmit);
        UpdateSubmitButton();

    }

    void CreateWordData()
    {
        allWords = new List<WordData>
        {
            new WordData("EXPERT", "experience"),
            new WordData("INTERMEDIATE", "experience"),
            new WordData("NOVICE", "experience"),
            new WordData("PROFICIENT", "experience"),

            new WordData("ARMCHAIR", "furniture"),
            new WordData("BOOKCASE", "furniture"),
            new WordData("CONSOLE", "furniture"),
            new WordData("FOOTSTOOL", "furniture"),

            new WordData("CEFTAZIDIME", "coins"),
            new WordData("HEADQUARTER", "coins"),
            new WordData("MONEYPENNY", "coins"),
            new WordData("PUMPERNICKEL", "coins"),

            new WordData("AGREEMENT", "promise"),
            new WordData("COMPACT", "promise"),
            new WordData("HANDSHAKE", "promise"),
            new WordData("UNDERSTANDING", "promise"),
        };
    }

    void SpawnGrid()
    {
        Shuffle(allWords);

        foreach (var word in allWords)
        {
            WordCell cell = Instantiate(cellPrefab, gridContainer);
            cell.Init(word, this); // ✅ le 2e paramètre manquait
        }
    }



    public void TryToggleCell(WordCell cell)
    {
        if (cell.IsSelected)
        {
            cell.SetSelected(false);
            selectedCells.Remove(cell);
            return;
        }

        if (selectedCells.Count >= 4)
            return;

        cell.SetSelected(true);
        selectedCells.Add(cell);
        UpdateSubmitButton();
    }

    void Shuffle(List<WordData> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            int j = Random.Range(i, list.Count);
            (list[i], list[j]) = (list[j], list[i]);
        }
    }
}
